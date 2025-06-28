// "-----------------------------------------------------------------------
//  <copyright file="AppSettings.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.Extensions.Configuration;

namespace TechShop.Manufacture
{
    public class AppSettings
    {
        private static IConfiguration configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true).AddEnvironmentVariables().Build();
        /// <summary>
        /// ConnectionString: string connect to database
        /// </summary>
        public static string ConnectionString
        {
            get { return configuration["ConnectionStrings:DefaultConnection"] ?? ""; }
        }
        #region JWT
        public static string ValidAudience => configuration["JWT:ValidAudience"] ?? "";
        public static string ValidIssuer => configuration["JWT:ValidIssuer"] ?? "";
        public static int TokenExpiryTiemInHours => (int)Convert.ToInt64(configuration["JWT:TokenExpiryTiemInHours"]);
        public static int TokenExpiryTiemInMinutes => (int)Convert.ToInt64(configuration["JWT:TokenExpiryTiemInMinutes"]);
        public static string SecretKey => configuration["JWT:SecretKey"] ?? "";
        #endregion
        public static string Salt => configuration["Salt"] ?? "";
        public static int RefreshTokenExperyTimeInDay => (int)Convert.ToInt64(configuration["RefreshToken:ExpiryTimeInDay"]);
        public static int TimeToClearTokenByHours => (int)Convert.ToInt64(configuration["RefreshToken:TimeToClearTokenByHours"]);

        public static string GoogleUserInfoUrl => configuration["Authorization:Google:UserInfoUrl"]!;

        public static string GoogleClientId => configuration["Authorization:Google:ClientId"]!;
    }
}
