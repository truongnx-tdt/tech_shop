// "-----------------------------------------------------------------------
//  <copyright file="IAuthService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Application.Services.Interfaces;
using TechShop.Data.DTO;
using TechShop.Data.Request;
using TechShop.Data.Response;
using Microsoft.AspNetCore.Mvc;

namespace TechShop.Application.Services.Auth
{
    public interface IAuthService : IBaseService
    {
        Task<(int, string)> Registeration(RegistrationRequest model);
        Task<LoginResponse> Login(LoginRequest model);
        Task<TokenModel> TokenRefresh(TokenModel model);
        Task<(bool,string)> LogoutAsync(string username, string sessionId);
        Task<(bool,string)> LogoutAllAsync(string username);
        Task<LoginResponse> GoogleLogin([FromBody] string idToken);
    }
}
