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

        #region Message
        public static string SessionLoginEx = "Session login expired!";
        public static string SessionLogin = "Session login!";
        public static string LoginIn = "Login Successfully!";
        public static string Logout = "Login out!";
        public static string LoginFailed = "Login failed!";
        public static string LogoutFailed = "Logout failed!";
        public static string MissInformation = "Missing information!";
        public static string InvalidToken = "Invalid token. Please login again.";
        public static string UserNotActive = "User not active!";
        public static string UserNotFound = "User not found!";
        public static string UserOrPwdIncorrect = "User or password incorrect!";
        #endregion
        public static string Exception = "Server error, please call to manager!";
    }
}
