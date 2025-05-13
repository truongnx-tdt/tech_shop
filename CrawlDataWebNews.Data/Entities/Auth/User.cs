// "-----------------------------------------------------------------------
//  <copyright file="User.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using CrawlDataWebNews.Data.Entities.Abtractions;
using CrawlDataWebNews.Manufacture.CommonConst;

namespace CrawlDataWebNews.Data.Entities.Auth
{
    public class User : AuditEntityBase<Guid>
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;
        public string? Picture { get; set; } 
        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(255)]
        public string? FullName { get; set; }

        public bool IsActive { get; set; } = true;

        [MaxLength(50)]
        public string Role { get; set; } = "user";
        public string? GoogleId { get; set; }
        public string LoginProvider { get; set; } = StringConst.LoginProviderDefault;
        public DateTimeOffset? LastLogin { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
