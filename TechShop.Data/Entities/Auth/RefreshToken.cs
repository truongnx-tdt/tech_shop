// "-----------------------------------------------------------------------
//  <copyright file="RefreshToken.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using TechShop.Data.Entities.Abtractions;

namespace TechShop.Data.Entities.Auth
{
    public class RefreshToken : AuditEntityBase<Guid>
    {
        [Required]
        public string Token { get; set; } = null!;
        [Required]
        public DateTimeOffset ExpiredAt { get; set; }
        [MaxLength(255)]
        public string? DeviceInfo { get; set; }
        public string? SessionId { get; set; }
        [MaxLength(255)]
        public string? IpAddress { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public User User { get; set; }
    }
}
