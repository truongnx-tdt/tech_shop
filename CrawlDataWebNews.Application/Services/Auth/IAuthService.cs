// "-----------------------------------------------------------------------
//  <copyright file="IAuthService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Application.Services.Interfaces;
using CrawlDataWebNews.Data.DTO;
using CrawlDataWebNews.Data.Request;
using CrawlDataWebNews.Data.Response;

namespace CrawlDataWebNews.Application.Services.Auth
{
    public interface IAuthService : IBaseService
    {
        Task<(int, string)> Registeration(RegistrationRequest model);
        Task<LoginResponse> Login(LoginRequest model);
        Task<TokenModel> TokenRefresh(TokenModel model);
        Task<(bool,string)> LogoutAsync(string username, string sessionId);
        Task<(bool,string)> LogoutAllAsync(string username);
    }
}
