// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslation.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Infrastructure.AppDbContext;
using CrawlDataWebNews.Infrastructure.Repositories._BaseRepo;

namespace CrawlDataWebNews.Infrastructure.Repositories.LanguageRepo
{
    public class LanguageTranslation : GenericRepository<Data.Entities.Languages.LanguageTranslation>, ILanguageTranslationRepository
    {
        public LanguageTranslation(ApplicationDbContext context) : base(context)
        {
        }
    }
}
