﻿// "-----------------------------------------------------------------------
//  <copyright file="LanguageTranslation.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.LanguageRepo
{
    public class LanguageTranslationRepository : GenericRepository<Data.Entities.Languages.LanguageTranslation>, ILanguageTranslationRepository
    {
        public LanguageTranslationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
