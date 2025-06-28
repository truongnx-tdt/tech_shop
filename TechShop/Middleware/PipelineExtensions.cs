// "-----------------------------------------------------------------------
//  <copyright file="PipelineExtensions.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.Net;
using Microsoft.EntityFrameworkCore;
using TechShop.Data.Response;
using TechShop.Infrastructure.AppDbContext;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Middleware
{
    public static class PipelineExtensions
    {
        public static async Task ConfigurePipelineAsync(this WebApplication app)
        {
            var MyAllowSpecificOrigins = StringConst.MyAllowSpecificOrigins;

            // Initialize database
            await app.InitializeDatabaseAsync();

            // Configure compression
            app.UseResponseCompression();

            // Configure development environment
            app.ConfigureDevelopmentEnvironment();

            // Configure request pipeline
            app.ConfigureRequestPipeline(MyAllowSpecificOrigins);
        }

        private static async Task InitializeDatabaseAsync(this WebApplication app)
        {
            // create singleton instance of DbInit to seed data and ensure database is created
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
            //var dbInit = services.GetRequiredService<DbInit>();
            //dbInit.Seed().Wait();
        }

        private static void ConfigureDevelopmentEnvironment(this WebApplication app)
        {
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
        }

        private static void ConfigureRequestPipeline(this WebApplication app, string corsPolicy)
        {
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors(corsPolicy);

            app.UseAuthentication();
            // validate session by user claims and session id
            //app.UseMiddleware<SessionValidationMiddleware>();

            // Configure error handling middleware
            app.ConfigureErrorHandling();

            app.UseAuthorization();
            app.MapControllers();
        }

        private static void ConfigureErrorHandling(this WebApplication app)
        {
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
        }
    }
}
