// "-----------------------------------------------------------------------
//  <copyright file="ILanguageRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Data.Entities.Languages;
using CrawlDataWebNews.Infrastructure.Repositories._BaseRepo;

namespace CrawlDataWebNews.Infrastructure.Repositories.LanguageRepo
{
    public interface ILanguageRepository : IGenericRepository<Language>
    {
        Task<Language> GetLanguageTranslationByIdAsync(int id);
    }
}
