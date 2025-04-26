// "-----------------------------------------------------------------------
//  <copyright file="Helper.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.Text.RegularExpressions;

namespace CrawlDataWebNews.Manufacture
{
    public class Helper
    {
        public static bool Compare2String(string target1, string target2)
        {
            return target1.Trim().ToLower().Equals(target2.Trim().ToLower());
        }
        public static bool ValidatePassword(string pwd)
        {
            // Validate strong password
            Regex validateGuidRegex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            return validateGuidRegex.IsMatch(pwd);
        }
    }
}
