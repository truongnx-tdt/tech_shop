// "-----------------------------------------------------------------------
//  <copyright file="BaseService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Application.Services.Interfaces;
using TechShop.Infrastructure.UnitOfWork;

namespace TechShop.Application.Services
{
    public class BaseService : IBaseService
    {
        protected IUnitOfWork UnitOfWork { get; set; }
        protected BaseService(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }
    }
}
