﻿// "-----------------------------------------------------------------------
//  <copyright file="ApplicationDbContext.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Entities.Auth;
using TechShop.Data.Entities.Languages;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechShop.Data.Entities;
using System.Security.Claims;

namespace TechShop.Infrastructure.AppDbContext
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

        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<LanguageTranslation> LanguageTranslations { get; set; }
        public DbSet<Address> Addresses { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            #region config entity for auth table

            modelBuilder.Entity<User>()
           .HasIndex(u => new { u.Email, u.LoginProvider })
           .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Language>()
                .HasIndex(l => l.Id)
                .IsUnique();

            modelBuilder.Entity<LanguageTranslation>()
                .HasIndex(lt => new { lt.LanguageCode, lt.Key })
                .IsUnique();

            modelBuilder.Entity<LanguageTranslation>()
                .HasOne(lt => lt.Language)
                .WithMany(l => l.Translations)
                .HasForeignKey(lt => lt.LanguageCode)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region config entity for address table
            modelBuilder.Entity<Address>()
                .HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
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
        public string? GetUserID()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null || context.User == null || !context.User.Identity.IsAuthenticated)
                    return null;

                // Lấy Claim "sub" hoặc "user_id" hoặc "nameidentifier" tùy theo cách bạn set khi đăng nhập
                var userIdClaim = context.User.FindFirst("UserID");

                return userIdClaim?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user ID from claims");
                return null;
            }
        }

        public string? GetUserName()
        {
            try
            {
                var context = _httpContextAccessor.HttpContext;
                if (context == null || context.User == null || !context.User.Identity.IsAuthenticated)
                    return null;

                // get from ClaimTypes.Name
                var userNameClaim = context.User.FindFirst(ClaimTypes.Name);
                return userNameClaim?.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user ID from claims");
                return null;
            }
        }
    }
}