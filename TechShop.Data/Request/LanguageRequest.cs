// "-----------------------------------------------------------------------
//  <copyright file="LanguageRequest.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;

namespace TechShop.Data.Request
{
    public class LanguageRequest
    {
        [Required(ErrorMessage ="_key_req")]
        public string Key { get; set; }
        [Required(ErrorMessage = "_name_req")]
        public string Name { get; set; }
        public string Flag { get; set; } 
        public bool IsActive { get; set; }
    }
}
