// "-----------------------------------------------------------------------
//  <copyright file="RegistrationRequest.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.ComponentModel.DataAnnotations;
using TechShop.Data.Common;

namespace TechShop.Data.Request
{
    public class RegistrationRequest
    {
        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress(ErrorMessage = "_valid_email")]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        [StrongPassword]
        public string Password { get; set; } = null!;

        [MaxLength(255)]
        public string? FullName { get; set; }
        public string Details { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}
