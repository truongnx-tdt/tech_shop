// "-----------------------------------------------------------------------
//  <copyright file="ILanguageTranslationService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.AspNetCore.Http;
using TechShop.Application.Services.Interfaces;
using TechShop.Data.DTO;
using TechShop.Data.Entities.Languages;

namespace TechShop.Application.Services.LanguageS
{
    public interface ILanguageTranslationService : IBaseService
    {
        Task<List<LanguageTranslation>> GetAsync(string languageCode, string module);
        Task<bool> AddMultiAsync(List<LanguageTranslationDTO> request);
        Task<bool> EditMultiAsync(List<LanguageTranslationDTO> request);
        Task<bool> DeleteByLCode(string languageCode);
        Task<bool> DeleteByIds(List<int> id);
        Task<bool> Import(IFormFile file);
    }
}
