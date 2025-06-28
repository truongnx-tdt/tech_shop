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
        public UserInfoLogin? UserInfoLogin { get; set; }
    }

    public class UserInfoLogin
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public UserRole Role { get; set; } 
    }
}
