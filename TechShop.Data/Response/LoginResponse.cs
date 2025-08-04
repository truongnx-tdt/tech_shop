// "-----------------------------------------------------------------------
//  <copyright file="LoginResponse.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using TechShop.Manufacture.Enums;

namespace TechShop.Data.Response
{
    public class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
