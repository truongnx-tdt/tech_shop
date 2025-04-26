// "-----------------------------------------------------------------------
//  <copyright file="Helper.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
namespace CrawlDataWebNews.Manufacture
{
    public class Helper
    {
        public static bool Compare2String(string target1, string target2)
        {
            return target1.Trim().ToLower().Equals(target2.Trim().ToLower());
        }
    }
}
