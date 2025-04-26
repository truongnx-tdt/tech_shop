// "-----------------------------------------------------------------------
//  <copyright file="LoginRequest.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using CrawlDataWebNews.Data.Common;

namespace CrawlDataWebNews.Data.Request
{
    public class LoginRequest
    {
        [Required]
        public string Account { get; set; } = null!;
        [Required]
        [StrongPassword]
        public string Password { get; set; } = null!;
        public string? Capcha { get; set; }
    }
}
