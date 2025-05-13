// "-----------------------------------------------------------------------
//  <copyright file="ResponseStatus.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.Net;

namespace CrawlDataWebNews.Manufacture.CommonConst
{
    public static class ResponseStatusCode
    {
        public const int Success = (int)HttpStatusCode.OK;
        public const int UnSuccess = (int)HttpStatusCode.BadRequest;
        public const int Exists = 02;
        public const int NotFound = 404;
        public const int Error = 500;
        public const int Unauthorized = 401;
    }

    public static class ResponseStatusName
    {
        public const string Success = "Successfully!";
        public const string UnSuccess = "UnSuccessfully!";
        public const string Exists = "Data already exists!";
        public const string NotFound = "Data not found!";
        public const string Error = "Error!";
        public const string Unauthorized = "Unauthorized!";
        public const string InternalServerError = "Internal Server Error!";
    }
}
