// "-----------------------------------------------------------------------
//  <copyright file="LoginResponse.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
namespace CrawlDataWebNews.Data.Response
{
    public class LoginResponse
    {
        public bool IsLogin { get; set; } = false;
        public string? AccessToken { get; set; }
        public string? RefeshToken { get; set; }
    }
}
