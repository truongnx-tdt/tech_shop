// "-----------------------------------------------------------------------
//  <copyright file="TokenModel.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.ComponentModel.DataAnnotations;

namespace CrawlDataWebNews.Data.DTO
{
    public class TokenModel
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;

        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
