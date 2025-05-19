// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslation.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"


using System.ComponentModel.DataAnnotations;
using CrawlDataWebNews.Data.Entities.Abtractions;

namespace CrawlDataWebNews.Data.Entities.Languages
{
    public class LanguageTranslation : EntityBase<int>
    {
        [Required]
        public string LanguageCode { get; set; } = null!;
        [Required]
        public string Key { get; set; } = null!;
        public string? Value { get; set; }
        [Required]
        public Language Language { get; set; } = null!;
    }
}
