// "-----------------------------------------------------------------------
//  <copyright file="ResponseStatus.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.Net;

namespace TechShop.Manufacture.CommonConst
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
        public const string Success = "_success";
        public const string UnSuccess = "_unsuccess";
        public const string Exists = "_data_exists";
        public const string NotFound = "_data_not_found";
        public const string Error = "_error";
        public const string Unauthorized = "_unauthorized";
        public const string InternalServerError = "_internal_sv";
        public const string UpdateDone = "_update_done";
        public const string UpdateFailed = "_update_failed";
        public const string DataRequired = "_data_required";
    }
}
