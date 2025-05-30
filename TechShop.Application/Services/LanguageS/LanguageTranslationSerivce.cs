// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslationSerivce.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using Microsoft.AspNetCore.Http;
using TechShop.Data.Entities.Languages;
using TechShop.Infrastructure.UnitOfWork;

namespace TechShop.Application.Services.LanguageS
{
    public class LanguageTranslationSerivce : BaseService, ILanguageTranslationService
    {
        public LanguageTranslationSerivce(IUnitOfWork unitOfWork) : base(unitOfWork)
        {

        }

        public Task<bool> AddMultiAsync(List<LanguageTranslation> request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByIds(List<int> id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteByLCode(string languageCode)
        {
            throw new NotImplementedException();
        }

        public Task<bool> EditMultiAsync(List<LanguageTranslation> request)
        {
            throw new NotImplementedException();
        }

        public async Task<List<LanguageTranslation>> GetAsync(string languageCode, string module)
        {
            var rs = await UnitOfWork.LanguageTranslation.FindAllAsync(x => x.LanguageCode == languageCode && (string.IsNullOrWhiteSpace(module) || x.Module == module));
            return rs.ToList();
        }

        public Task<bool> Import(IFormFile file)
        {
            throw new NotImplementedException();
        }
    }
}
