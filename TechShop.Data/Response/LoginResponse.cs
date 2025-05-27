// "-----------------------------------------------------------------------
//  <copyright file="LoginResponse.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
namespace TechShop.Data.Response
{
    public class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefeshToken { get; set; }
        public string? Message { get; set; }
    }
}
