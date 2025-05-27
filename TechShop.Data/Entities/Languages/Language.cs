// "-----------------------------------------------------------------------
//  <copyright file="Language.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System.ComponentModel.DataAnnotations;
using TechShop.Data.Entities.Abtractions;

namespace TechShop.Data.Entities.Languages
{
    public class Language : EntityBase<string>
    {
        [Required]
        public string Name { get; set; }
        public string Flag { get; set; } // URL to the flag image
        public bool IsActive { get; set; } = true;
        public ICollection<LanguageTranslation> Translations { get; set; }
    }
}
