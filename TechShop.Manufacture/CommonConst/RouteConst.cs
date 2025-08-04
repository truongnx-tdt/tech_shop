// "-----------------------------------------------------------------------
//  <copyright file="RouteConst.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

namespace TechShop.Manufacture.CommonConst
{
    public static class RouteConst
    {
        public const string Login = "/api/login";
        public const string LoginGG = "/api/auth/google-login";
        public const string Register = "/api/register";
        public const string TokenRefresh = "/api/token-refresh";
        public const string Logout = "/api/logout";
        public const string LogoutAll = "/api/logout-all";

        public const string GetUserInfo = "/api/get-user-info";

        #region system config api
        public const string LanguageResponses = "/api/languages";
        public const string AdminGetLanguages = "/api/admin/languages";
        public const string AdminUpdateLanguage = "/api/admin/update-language";
        public const string AdminAddLanguage = "/api/admin/add-language";
        public const string Config = "/api/version";
        public const string GetLanguageTranslations = "/api/translations";
        public const string AddLanguageTranslations = "/api/admin/add-translations";
        public const string UpdateLanguageTranslations = "/api/admin/update-translations";
        public const string GetDevicesLoggingIn = "/api/devices-login";
        public const string GetDeviceInfo = "/api/device-info";
        #endregion
    }
}
