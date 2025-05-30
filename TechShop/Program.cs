// "-----------------------------------------------------------------------
//  <copyright file="Program.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.IO.Compression;
using System.Net;
using System.Text;
using TechShop.Application.Services.Auth;
using TechShop.Application.Services.Token;
using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.UnitOfWork;
using TechShop.Manufacture;
using TechShop.Manufacture.CommonConst;
using TechShop.Middleware;
using TechShop.Data.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using TechShop.Application.Services.LanguageS;
using TechShop.Data.Mapper;
using TechShop.Data.Entities.Languages;
using TechShop.Data.Request;
using TechShop.Data.Mapper.LanguageMapper;
using Microsoft.AspNetCore.Mvc;

// Early init of NLog to allow startup and exception logging, before host is built
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");
try
{
    var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddResponseCompression(options =>
    {
        options.EnableForHttps = true;
        options.Providers.Add<BrotliCompressionProvider>();
        options.Providers.Add<GzipCompressionProvider>();
    });

    builder.Services.Configure<BrotliCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.Fastest;
    });

    builder.Services.Configure<GzipCompressionProviderOptions>(options =>
    {
        options.Level = CompressionLevel.SmallestSize;
    });

    // Add services to the container.
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddScoped<ClientInfoHelper>();
    builder.Services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddScoped<ITokenService, TokenService>();
    builder.Services.AddScoped<ILanguageService, LanguageService>();
    builder.Services.AddScoped<ILanguageTranslationService, LanguageTranslationSerivce>();
    // mapping config
    builder.Services.AddScoped<IMapper<Language, LanguageRequest>, LanguageMapper>();

    builder.Services.AddControllers().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
    builder.Services.AddHostedService<TokenCleanupService>();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    #region configure connect to db
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(AppSettings.ConnectionString, cfg =>
        {
            cfg.MigrationsAssembly("TechShop.Infrastructure");
            cfg.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
        });
    });
    #endregion

    #region auth
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = AppSettings.ValidAudience,
            ValidIssuer = AppSettings.ValidIssuer,
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.SecretKey))
        };
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(MyAllowSpecificOrigins, policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
    });
    #endregion

    builder.Services.Configure<ApiBehaviorOptions>(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new
                {
                    Field = e.Key,
                    Error = e.Value.Errors.First().ErrorMessage
                }).ToList();

            var apiResponse = new ApiResponse<object>
            {
                Status = 400,
                Error = errors
            };

            return new BadRequestObjectResult(apiResponse);
        };
    });


    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    #region add miniprofiler
    // Add services to the container.
    builder.Services.AddMemoryCache(); // add the memory cache to the service collection
                                       // Add miniprofilter : write speed time query sql
                                       // More more about config miniprofiler at https://ilovedotnet.org/blogs/profiling-webapi-with-mini-profiler/
    builder.Services.AddMiniProfiler(options =>
    {
        options.RouteBasePath = "/profiler"; // /profiler/results-index
    }).AddEntityFramework();
    #endregion
    var app = builder.Build();

    // create singleton instance of DbInit to seed data and ensure database is created
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        if (!context.Database.CanConnect())
        {
            throw new Exception("Cannot connect to the database.");
        }
        await context.Database.MigrateAsync();
        //var dbInit = services.GetRequiredService<DbInit>();
        //dbInit.Seed().Wait();
    }

    app.UseResponseCompression();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseMiniProfiler();
        app.UseSwaggerUI(options =>
        {
            options.DefaultModelsExpandDepth(-1); // remove models from swagger UI
        });
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseCors(MyAllowSpecificOrigins);

    app.UseAuthentication();
    // validate session by user claims and session id
    app.UseMiddleware<SessionValidationMiddleware>();
    app.Use(async (context, next) =>
    {
        await next();

        if (context.Response.StatusCode == (int)HttpStatusCode.NotFound && context.GetEndpoint() == null)
        {
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>()
            {
                Status = (int)HttpStatusCode.NotFound,
                Message = ResponseStatusName.NotFound,
            });
        }

        if (context.Response.StatusCode == (int)HttpStatusCode.InternalServerError)
        {
            await context.Response.WriteAsJsonAsync(new ApiResponse<object>()
            {
                Status = (int)HttpStatusCode.InternalServerError,
                Message = ResponseStatusName.InternalServerError,
            });
        }
    });
    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    NLog.LogManager.Shutdown();
}