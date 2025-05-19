// "-----------------------------------------------------------------------
//  <copyright file="SystemConfigController.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CrawlDataWebNews.Controllers
{
    [ApiController]
    public class SystemConfigController : ControllerBase
    {
        [HttpGet]
        [Route("api/system-config")]
        public IActionResult GetSystemConfig()
        {
            var systemConfig = new
            {
                Version = "1.0.0",
                BuildDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            };

            return Ok(systemConfig);
        }
    }
}
