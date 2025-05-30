// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslationDTO.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;

namespace TechShop.Data.DTO
{
    public class LanguageTranslationDTO
    {
        [Required]
        public string LanguageCode { get; set; }
        [Required]
        public string Key { get; set; }
        public string? Value { get; set; }
        public string? Module { get; set; }
    }
}
