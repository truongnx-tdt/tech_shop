// "-----------------------------------------------------------------------
//  <copyright file="RegistrationRequest.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.ComponentModel.DataAnnotations;

namespace CrawlDataWebNews.Data.Request
{
    public class RegistrationRequest
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = null!;

        [MaxLength(255)]
        public string? FullName { get; set; }
    }
}
