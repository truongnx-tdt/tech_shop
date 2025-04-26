// "-----------------------------------------------------------------------
//  <copyright file="Program.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System;
using System.IO.Compression;
using System.Text;
using CrawlDataWebNews.Application.Services;
using CrawlDataWebNews.Application.Services.Auth;
using CrawlDataWebNews.Application.Services.Interfaces;
using CrawlDataWebNews.Infrastructure.AppDbContext;
using CrawlDataWebNews.Infrastructure.UnitOfWork;
using CrawlDataWebNews.Manufacture;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;

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
    builder.Services.AddScoped<IGetDataService, GetDataService>();
    builder.Services.AddScoped<IAuthService, AuthService>();
    builder.Services.AddControllers();

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    #region configure connect to db
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseNpgsql(AppSettings.ConnectionString, cfg =>
        {
            cfg.MigrationsAssembly("CrawlDataWebNews.Infrastructure");
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

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        if (!context.Database.CanConnect())
        {
            throw new Exception("Cannot connect to the database.");
        }
        //await context.Database.MigrateAsync();
        //var dbInit = services.GetRequiredService<DbInit>();
        //dbInit.Seed().Wait();
    }

    app.UseResponseCompression();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.DefaultModelsExpandDepth(-1);
        });
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();

    app.UseCors(MyAllowSpecificOrigins);

    app.UseAuthentication();
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