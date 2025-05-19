// "-----------------------------------------------------------------------
//  <copyright file="LanguageRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Data.Entities.Languages;
using CrawlDataWebNews.Infrastructure.AppDbContext;
using CrawlDataWebNews.Infrastructure.Repositories._BaseRepo;

namespace CrawlDataWebNews.Infrastructure.Repositories.LanguageRepo
{
    public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Task<Language> GetLanguageTranslationByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
