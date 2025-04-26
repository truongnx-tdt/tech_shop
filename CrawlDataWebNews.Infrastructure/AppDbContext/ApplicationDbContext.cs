// "-----------------------------------------------------------------------
//  <copyright file="ApplicationDbContext.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Data.Entities.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CrawlDataWebNews.Infrastructure.AppDbContext
{
    public class ApplicationDbContext : DbContext
    {
        private readonly ILogger<ApplicationDbContext> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ILogger<ApplicationDbContext> logger, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        DbSet<User> Users { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region config entity for auth table

            modelBuilder.Entity<User>()
           .HasIndex(u => u.Email)
           .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAudit();
            return await base.SaveChangesAsync(cancellationToken);
        }
        public override int SaveChanges()
        {
            ApplyAudit();
            return base.SaveChanges();
        }
        private void ApplyAudit()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            var now = DateTimeOffset.UtcNow;
            var user = _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";

            foreach (var entry in entries)
            {
                var entity = entry.Entity;
                var type = entity.GetType();

                var idProp = type.GetProperty("Id");
                if (entry.State == EntityState.Added && idProp != null && idProp.PropertyType == typeof(Guid))
                {
                    var idValue = (Guid)idProp.GetValue(entity)!;
                    if (idValue == Guid.Empty)
                    {
                        idProp.SetValue(entity, Guid.NewGuid());
                    }
                }

                var createAtProp = type.GetProperty("CreateAt");
                if (entry.State == EntityState.Added && createAtProp != null && createAtProp.PropertyType == typeof(DateTimeOffset))
                {
                    createAtProp.SetValue(entity, now);
                }

                var createUserProp = type.GetProperty("CreateUser");
                if (entry.State == EntityState.Added && createUserProp != null && createUserProp.PropertyType == typeof(string))
                {
                    createUserProp.SetValue(entity, user);
                }

                var updateAtProp = type.GetProperty("UpdateAt");
                if (updateAtProp != null && updateAtProp.PropertyType == typeof(DateTimeOffset))
                {
                    updateAtProp.SetValue(entity, now);
                }

                var updateUserProp = type.GetProperty("UpdateUser");
                if (updateUserProp != null && updateUserProp.PropertyType == typeof(string))
                {
                    updateUserProp.SetValue(entity, user);
                }
            }
        }
    }

    public class ClientInfoHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<ClientInfoHelper> _logger;
        public ClientInfoHelper(IHttpContextAccessor httpContextAccessor, ILogger<ClientInfoHelper> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }
        /// <summary>
        /// Return Client IP by device
        /// </summary>
        /// <returns></returns>
        public string GetClientIp()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null) return "Unknown";

                var ip = context.Request.Headers["X-Forwarded-For"].FirstOrDefault()
                         ?? context.Connection.RemoteIpAddress?.ToString();

                return string.IsNullOrWhiteSpace(ip) ? "Unknown" : ip;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get client IP");
                return "Unknown";
            }
        }
        /// <summary>
        /// Return information about the user's machine device
        /// </summary>
        /// <returns></returns>
        public string GetUserAgent()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                return context?.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user agent");
                return "Unknown";
            }
        }
        //public string? GetUserId()
        //{
        //    try
        //    {
        //        var context = _httpContextAccessor.HttpContext;
        //        if (context == null || context.User == null || !context.User.Identity.IsAuthenticated)
        //            return null;

        //        // Lấy Claim "sub" hoặc "user_id" hoặc "nameidentifier" tùy theo cách bạn set khi đăng nhập
        //        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier) // Thường là "sub" hoặc "nameidentifier"
        //                         ?? context.User.FindFirst("sub")
        //                         ?? context.User.FindFirst("user_id");

        //        return userIdClaim?.Value;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Failed to get user ID from claims");
        //        return null;
        //    }
        //}
    }
}