// "-----------------------------------------------------------------------
//  <copyright file="ILanguageRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Entities.Languages;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.LanguageRepo
{
    public interface ILanguageRepository : IGenericRepository<Language>
    {
        Task<Language> GetLanguageTranslationByIdAsync(int id);
    }
}
