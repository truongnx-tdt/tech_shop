// "-----------------------------------------------------------------------
//  <copyright file="LanguageRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Data.Entities.Languages;
using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.LanguageRepo
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
