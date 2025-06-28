// "-----------------------------------------------------------------------
//  <copyright file="StringConst.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
namespace TechShop.Manufacture.CommonConst
{
    public class StringConst
    {
        #region Claim
        public static string ClaimSessionId = "session_id";
        public static string LoginProviderDefault = "WebApp";
        public static string LoginProviderGG = "Google";
        public static string ClaimRole = "Roles";
        public static string ClaimUserId = "UserID";
        #endregion

        public const string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        #region Message
        public static string SessionLoginEx = "_session_expired";
        public static string SessionLogin = "_session_login";
        public static string LoginIn = "_login_success";
        public static string Logout = "_logout";
        public static string LoginFailed = "_login_failed";
        public static string LogoutFailed = "_logout_failed";
        public static string MissInformation = "_miss_info";
        public static string InvalidToken = "_valid_token";
        public static string UserNotActive = "_user_not_active";
        public static string UserNotFound = "_user_not_found";
        public static string UserOrPwdIncorrect = "_username_pwd_incorrect";
        public static string Exception = "_call_to_manager";
        public static string AddDone = "_add_done";
        public static string AddFailed = "_add_failed";
        #endregion
    }
}
