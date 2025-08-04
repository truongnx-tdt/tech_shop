// "-----------------------------------------------------------------------
//  <copyright file="StrongPasswordAttribute.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Data.Common
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var password = value as string;

            if (string.IsNullOrWhiteSpace(password))
                return false;
            var regex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            return regex.IsMatch(password);
        }

        public override string FormatErrorMessage(string name)
        {
            return StringConst.PasswordRequired;
        }
    }
}
