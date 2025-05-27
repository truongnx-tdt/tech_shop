// "-----------------------------------------------------------------------
//  <copyright file="SystemConfigController.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace TechShop.Controllers
{
    [ApiController]
    public class SystemConfigController : ControllerBase
    {
        [HttpGet]
        [Route("/api/system-config")]
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
        [HttpGet]
        [Route("/api/get-languages")]
        public IActionResult GetLanguages()
        {
            var languages = new[]
            {
                new { Code = "en", Name = "English" },
                new { Code = "vi", Name = "Tiếng Việt" },
                new { Code = "fr", Name = "Français" },
                new { Code = "es", Name = "Español" }
            };
            return Ok(languages);
        }
        [HttpGet]
        [Route("/api/translations")]
        public IActionResult GetLanguageLocalize([Required] string lang, string module)
        {
            if (ModelState.IsValid)
            {
                var languages = new[] {
                    new
                    {
                        Code = "en",
                        Translations = new Dictionary<string, string>
                        {
                            { "Hello", "Hello" },
                            { "Goodbye", "Goodbye" }
                        }
                    },
                    new
                    {
                        Code = "vi",
                        Translations = new Dictionary<string, string>
                        {
                            { "Hello", "Xin chào" },
                            { "Goodbye", "Tạm biệt" }
                        }
                    },
                    new
                    {
                        Code = "fr",
                        Translations = new Dictionary<string, string>
                        {
                            { "Hello", "Bonjour" },
                            { "Goodbye", "Au revoir" }
                        }
                    }
                };
                return Ok(lang);
            }
            return BadRequest(ModelState);
        }

    }
}
