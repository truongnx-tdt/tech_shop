// "-----------------------------------------------------------------------
//  <copyright file="ILanguageTranslation.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechShop.Infrastructure.Repositories._BaseRepo;

namespace TechShop.Infrastructure.Repositories.LanguageRepo
{
    public interface ILanguageTranslationRepository : IGenericRepository<Data.Entities.Languages.LanguageTranslation>
    {
    }
}
