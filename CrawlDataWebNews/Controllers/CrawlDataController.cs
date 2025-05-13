// "-----------------------------------------------------------------------
//  <copyright file="CrawlDataController.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using CrawlDataWebNews.Application.Services.Interfaces;
using CrawlDataWebNews.Data.Common;
using CrawlDataWebNews.Manufacture;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CrawlDataWebNews.Controllers
{
    [ApiController]
    public class CrawlDataController : ControllerBase
    {
        [HttpGet("/api/version")]
        public IActionResult Version()
        {
            return Ok(new Version(0, 0, 2));
        }
        //[HttpGet(Routes.GetByCtg)]
        //public async Task<IActionResult> GetByCtg([Required]string linkWeb, [Required]string extension)
        //{
        //    if (!extension.StartsWith(".") || !linkWeb.StartsWith("http"))
        //    {
        //        return BadRequest(new
        //        {
        //            Data = "400",
        //            Message = "Nhập đầy đủ giá trị"
        //        });
        //    }
        //    var rs = await _getDataService.GetByCtg(linkWeb, extension);
        //    return Ok(rs);
        //}
    }
}
