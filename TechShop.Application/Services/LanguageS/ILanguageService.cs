// "-----------------------------------------------------------------------
//  <copyright file="ILanguageService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Application.Services.Interfaces;
using TechShop.Data.Entities.Languages;
using TechShop.Data.Request;
using TechShop.Data.Response;

namespace TechShop.Application.Services.LanguageS
{
    public interface ILanguageService : IBaseService
    {
        #region Admin set
        public Task<List<Language>> GetLanguages();
        public Task<bool> UpdateLanguage(List<LanguageRequest> request);
        public Task<string> AddLanguage(List<LanguageRequest> request);
        #endregion
        public List<LanguageResponse> LanguageResponses();
    }
}
