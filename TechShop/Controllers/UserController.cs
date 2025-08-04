// "-----------------------------------------------------------------------
//  <copyright file="UserController.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechShop.Application.Services.UserS;
using TechShop.Data.Response;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;
        public UserController(ILogger<UserController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }
        [Authorize]
        [HttpGet]
        [Route(RouteConst.GetDevicesLoggingIn)] 
        public async Task<ActionResult<ApiResponse<object>>> GetDevicesLogin()
        {
            try
            {
                var response = await _userService.GetDevicesLoginAsync();
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting devices login.");
                return StatusCode(500, new ApiResponse<object>
                {
                    Status = ResponseStatusCode.Error,
                    Message = StringConst.Exception,
                });
            }
        }

        [Authorize]
        [HttpGet]
        [Route(RouteConst.GetDeviceInfo)]
        public Task<ActionResult<ApiResponse<object>>> GetDeviceInfo()
        {
            var devices = new List<object>
            {
                new { DeviceName = "Laptop", LastLogin = DateTime.UtcNow.AddDays(-1) },
                new { DeviceName = "Mobile Phone", LastLogin = DateTime.UtcNow.AddDays(-2) }
            };
            return Task.FromResult<ActionResult<ApiResponse<object>>>(Ok(new ApiResponse<object>
            {
                Status = 200,
                Message = "Success",
                Data = devices
            }));
        }
    }
}
