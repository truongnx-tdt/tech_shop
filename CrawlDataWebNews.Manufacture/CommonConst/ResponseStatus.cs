// "-----------------------------------------------------------------------
//  <copyright file="ResponseStatus.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
namespace CrawlDataWebNews.Manufacture.CommonConst
{
    public static class ResponseStatusCode
    {
        public const int Success = 00;
        public const int UnSuccess = 01;
        public const int Exists = 02;
    }

    public static class ResponseStatusName
    {
        public const string Success = "Successfully!";
        public const string UnSuccess = "UnSuccessfully!";
        public const string Exists = "Data already exists!";
    }
}
