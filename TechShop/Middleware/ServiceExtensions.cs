// "-----------------------------------------------------------------------
//  <copyright file="ServiceExtensions.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.IO.Compression;
using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NLog.Web;
using TechShop.Application.Services.Auth;
using TechShop.Application.Services.LanguageS;
using TechShop.Application.Services.Token;
using TechShop.Data.DTO;
using TechShop.Data.Entities.Languages;
using TechShop.Data.Mapper;
using TechShop.Data.Mapper.LanguageMapper;
using TechShop.Data.Request;
using TechShop.Data.Response;
using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.UnitOfWork;
using TechShop.Manufacture;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Middleware
{
    public static class ServiceExtensions
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            // Configure compression
            builder.ConfigureCompression();

            // Configure core services
            builder.ConfigureCoreServices();

            // Configure database
            builder.ConfigureDatabase();

            // Configure authentication
            builder.ConfigureAuthentication();

            // Configure CORS
            builder.ConfigureCors();

            // Configure API behavior
            builder.ConfigureApiBehavior();

            // Configure Swagger
            builder.ConfigureSwagger();

            // Configure file upload
            builder.ConfigureFileUpload();

            // Configure profiler
            builder.ConfigureProfiler();

            // Configure logging
            builder.ConfigureLogging();

            // Configure Rate Limiter
            builder.ConfigureRateLimiter();
        }

        private static void ConfigureCompression(this WebApplicationBuilder builder)
        {
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
        }

        private static void ConfigureCoreServices(this WebApplicationBuilder builder)
        {
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
            builder.Services.AddScoped<IMapper<LanguageTranslation, LanguageTranslationDTO>, LanguageTranslationMapper>();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DictionaryKeyPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
            });

            builder.Services.AddHostedService<TokenCleanupService>();
        }

        private static void ConfigureDatabase(this WebApplicationBuilder builder)
        {
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(AppSettings.ConnectionString, cfg =>
                {
                    cfg.MigrationsAssembly("TechShop.Infrastructure");
                    cfg.EnableRetryOnFailure(maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(30), errorCodesToAdd: null);
                });
            });
        }

        private static void ConfigureAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
                options.TokenValidationParameters = new TokenValidationParameters()
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
        }

        private static void ConfigureCors(this WebApplicationBuilder builder)
        {
            var MyAllowSpecificOrigins = StringConst.MyAllowSpecificOrigins;

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins, policy =>
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });
        }

        private static void ConfigureApiBehavior(this WebApplicationBuilder builder)
        {
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
        }

        private static void ConfigureSwagger(this WebApplicationBuilder builder)
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        private static void ConfigureFileUpload(this WebApplicationBuilder builder)
        {
            // Configure file upload limits
            builder.Services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100MB
            });
        }

        private static void ConfigureProfiler(this WebApplicationBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddMiniProfiler(options =>
            {
                options.RouteBasePath = "/profiler"; // /profiler/results-index
            }).AddEntityFramework();
        }

        private static void ConfigureLogging(this WebApplicationBuilder builder)
        {
            // NLog: Setup NLog for Dependency injection
            builder.Logging.ClearProviders();
            builder.Host.UseNLog();
        }

        private static void ConfigureRateLimiter(this WebApplicationBuilder builder)
        {
            // Configure rate limiting
            builder.Services.AddRateLimiter(options =>
            {
                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(
                   httpContext => RateLimitPartition.GetFixedWindowLimiter(
                       partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                       factory: partition => new FixedWindowRateLimiterOptions
                       {
                           AutoReplenishment = true,
                           PermitLimit = 20,
                           QueueLimit = 0,
                           Window = TimeSpan.FromMinutes(1)
                       }
                   )
                );
            });
        }
    }
}
