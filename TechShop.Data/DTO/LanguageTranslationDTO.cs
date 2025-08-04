// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslationDTO.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using TechShop.Manufacture.CommonConst;

namespace TechShop.Data.DTO
{
    public class LanguageTranslationDTO
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "_invalid_language_code")]
        public required string LanguageCode { get; set; }
        [Required(ErrorMessage = "_invalid_key")]
        public required string Key { get; set; }
        public string? Value { get; set; }
        public string? Module { get; set; }
    }
}
