// "-----------------------------------------------------------------------
//  <copyright file="UserController.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechShop.Data.Response;

namespace TechShop.Controllers
{
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }
        [Authorize]
        [HttpGet]
        [Route("/api/devices-login")] 
        public Task<ActionResult<ApiResponse<object>>> GetDevicesLogin()
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
