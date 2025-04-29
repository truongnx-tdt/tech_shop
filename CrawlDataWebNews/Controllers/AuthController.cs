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
using CrawlDataWebNews.Data.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CrawlDataWebNews.Infrastructure.UnitOfWork;

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
        /// <summary>
        /// Login api: Login for system
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(RouteConst.Login)]
        public async Task<ApiResponse<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<LoginResponse>() { Status = 400, Message = ResponseStatusName.UnSuccess, Error = ModelState };
            }
            var rs = await _authService.Login(request);
            if (rs.IsLogin)
            {
                return new ApiResponse<LoginResponse>() { Status = ResponseStatusCode.Success, Data = rs, Message = ResponseStatusName.Success };
            }
            return new ApiResponse<LoginResponse>() { Status = ResponseStatusCode.UnSuccess, Message = "Incorrect Account!" };
        }
        /// <summary>
        /// Register API
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(RouteConst.Register)]
        public async Task<ApiResponse<object>> Register([FromBody] RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<object>() { Status = 400, Message = ResponseStatusName.UnSuccess, Error = ModelState };
            }
            var rs = await _authService.Registeration(request);
            return new ApiResponse<object>() { Status = rs.Item1, Message = rs.Item2 };
        }
        /// <summary>
        /// Refresh API: refresh new token 
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <returns></returns>
        [HttpPost(RouteConst.TokenRefresh)]
        public async Task<ApiResponse<TokenModel>> Refresh(TokenModel tokenModel)
        {
            try
            {
                var rs = await _authService.TokenRefresh(tokenModel);
                return new ApiResponse<TokenModel>() { Status = ResponseStatusCode.Success, Data = rs, Message = ResponseStatusName.Success };
            }
            catch (Exception ex)
            {
                return new ApiResponse<TokenModel>() { Status = ResponseStatusCode.UnSuccess, Message = ex.Message };
            }
        }
        /// <summary>
        /// Logout: logout on 1 device
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost(RouteConst.Logout)]
        public async Task<ApiResponse<object>> Logout()
        {
            try
            {
                var username = User.Identity?.Name;
                var sessionId = User.Claims.FirstOrDefault(c => c.Type == StringConst.ClaimSessionId)?.Value;

                var rs = await _authService.LogoutAsync(username, sessionId);
                return new ApiResponse<object>()
                {
                    Status = rs.Item1 ? ResponseStatusCode.Success : ResponseStatusCode.UnSuccess,
                    Message = rs.Item2
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<object>()
                {
                    Status = ResponseStatusCode.UnSuccess,
                    Message = ex.Message
                };
            }

        }
        /// <summary>
        /// LogoutAll: logout on all devices
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost(RouteConst.LogoutAll)]
        public async Task<ApiResponse<object>> LogoutAll()
        {
            var username = User.Identity?.Name;
            var rs = await _authService.LogoutAllAsync(username);
            return new ApiResponse<object>()
            {
                Status = ResponseStatusCode.Success,
                Message = rs.Item2
            };
        }
    }
}
