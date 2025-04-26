// "-----------------------------------------------------------------------
//  <copyright file="LoginRequest.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

namespace CrawlDataWebNews.Data.Request
{
    public class LoginRequest
    {
        public string Account { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string? Capcha { get; set; }
    }
}
