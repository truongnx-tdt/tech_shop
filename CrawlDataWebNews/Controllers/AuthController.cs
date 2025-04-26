// "-----------------------------------------------------------------------
//  <copyright file="AuthController.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Manufacture.CommonConst;
using Microsoft.AspNetCore.Mvc;
using CrawlDataWebNews.Data.Request;
using CrawlDataWebNews.Application.Services.Auth;
using CrawlDataWebNews.Data.Response;

namespace CrawlDataWebNews.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }
        [HttpPost(RouteConst.Login)]
        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest request)
        {
            var rs = await _authService.Login(request);
            if (rs.IsLogin)
            {
                return new ApiResponse<LoginResponse>() { Status = ResponseStatusCode.Success, Data = rs, Message = ResponseStatusName.Success };
            }
            return new ApiResponse<LoginResponse>() { Status = ResponseStatusCode.UnSuccess, Message = "Incorrect Account!" };
        }

        [HttpPost(RouteConst.Register)]
        public async Task<ApiResponse<object>> Register(RegistrationRequest request)
        {
            var rs = await _authService.Registeration(request);
            return new ApiResponse<object>() { Status = rs.Item1, Message = rs.Item2 };
        }
    }
}
