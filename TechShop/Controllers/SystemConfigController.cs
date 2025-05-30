// "-----------------------------------------------------------------------
//  <copyright file="SystemConfigController.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechShop.Application.Services.LanguageS;
using TechShop.Data.Entities.Languages;
using TechShop.Data.Request;
using TechShop.Data.Response;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Controllers
{
    [ApiController]
    public class SystemConfigController : ControllerBase
    {
        private readonly ILanguageService _languageService;
        private readonly ILanguageTranslationService _languageTranslationService;
        public SystemConfigController(ILanguageService languageService, ILanguageTranslationService languageTranslationService)
        {
            _languageService = languageService;
            _languageTranslationService = languageTranslationService;
        }
        [HttpGet]
        [Route(RouteConst.Config)]
        public IActionResult GetSystemConfig()
        {
            var systemConfig = new
            {
                Version = new Version(0, 0, 1),
                BuildDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
            };

            return Ok(systemConfig);
        }

        #region Language
        [HttpGet]
        [Route(RouteConst.LanguageResponses)]
        public async Task<ActionResult<ApiResponse<List<LanguageResponse>>>> LanguageResponses()
        {
            var rs = await _languageService.LanguageResponses();
            return Ok(new ApiResponse<List<LanguageResponse>>()
            {
                Status = ResponseStatusCode.Success,
                Data = rs,
            });
        }
        [Authorize]
        [HttpGet]
        [Route(RouteConst.AdminGetLanguages)]
        public async Task<ActionResult<ApiResponse<List<Language>>>> AdminGetLanguages()
        {
            var rs = await _languageService.GetLanguages();
            return Ok(new ApiResponse<List<Language>>()
            {
                Status = ResponseStatusCode.Success,
                Data = rs,
            });
        }

        [Authorize]
        [HttpPut]
        [Route(RouteConst.AdminUpdateLanguage)]
        public async Task<ActionResult<ApiResponse<object>>> AdminUpdateLanguage(LanguageRequest request)
        {
            var rs = await _languageService.UpdateLanguage(request);
            if (rs)
            {
                return Ok(new ApiResponse<List<LanguageResponse>>()
                {
                    Status = ResponseStatusCode.Success,
                    Message = ResponseStatusName.UpdateDone
                });
            }
            return BadRequest(new ApiResponse<List<LanguageResponse>>()
            {
                Status = ResponseStatusCode.Success,
                Message = ResponseStatusName.UpdateFailed,
            });
        }

        [Authorize]
        [HttpPost]
        [Route(RouteConst.AdminAddLanguage)]
        public async Task<ActionResult<ApiResponse<object>>> AdminAddLanguage(LanguageRequest request)
        {
            var rs = await _languageService.AddLanguage(request);

            return Ok(new ApiResponse<List<LanguageResponse>>()
            {
                Status = ResponseStatusCode.Success,
                Message = rs
            });
        }
        #endregion

        #region Language translation
        [HttpGet]
        [Route(RouteConst.GetLanguageTranslations)]
        public async Task<ActionResult<ApiResponse<object>>> GetLanguageLocalize([Required] string lang, string? module)
        {
            var rs = await _languageTranslationService.GetAsync(lang, module).ConfigureAwait(false);
            return Ok(new ApiResponse<List<LanguageTranslation>>()
            {
                Status = ResponseStatusCode.Success,
                Data = rs
            });
        }

        [HttpPost]
        [Route(RouteConst.AddLanguageTranslations)]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> AddLanguageTranslations(List<LanguageTranslation> request)
        {
            if (request == null || !request.Any())
            {
                return BadRequest(new ApiResponse<object>()
                {
                    Status = ResponseStatusCode.UnSuccess,
                    Message = ResponseStatusName.DataRequired
                });
            }
            var rs = await _languageTranslationService.AddMultiAsync(request).ConfigureAwait(false);
            if (rs)
            {
                return Ok(new ApiResponse<object>()
                {
                    Status = ResponseStatusCode.Success,
                    Message = StringConst.AddDone
                });
            }
            return BadRequest(new ApiResponse<object>()
            {
                Status = ResponseStatusCode.UnSuccess,
                Message = StringConst.AddFailed
            });
        }
        #endregion
    }
}
