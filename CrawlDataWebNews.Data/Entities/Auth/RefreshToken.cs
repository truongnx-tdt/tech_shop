// "-----------------------------------------------------------------------
//  <copyright file="RefreshToken.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using CrawlDataWebNews.Data.Entities.Abtractions;

namespace CrawlDataWebNews.Data.Entities.Auth
{
    public class RefreshToken : AuditEntityBase<Guid>
    {
        [Required]
        public string Token { get; set; } = null!;
        [Required]
        public DateTimeOffset Expires { get; set; }
        public DateTimeOffset? Revoked { get; set; }
        [MaxLength(255)]
        public string? DeviceInfo { get; set; } 
        [MaxLength(255)]
        public string? IpAddress { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
        public bool IsActive => Revoked == null && !IsExpired;
    }
}
