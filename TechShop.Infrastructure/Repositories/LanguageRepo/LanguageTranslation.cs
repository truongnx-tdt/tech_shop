// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslation.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.LanguageRepo
{
    public class LanguageTranslation : GenericRepository<Data.Entities.Languages.LanguageTranslation>, ILanguageTranslationRepository
    {
        public LanguageTranslation(ApplicationDbContext context) : base(context)
        {
        }
    }
}
