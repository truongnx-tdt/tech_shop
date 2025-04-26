// "-----------------------------------------------------------------------
//  <copyright file="ApiResponse.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

namespace CrawlDataWebNews.Data.Response
{
    public class ApiResponse<T>
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
        public object? Error { get; set; }
    }
}
