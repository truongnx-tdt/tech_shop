// "-----------------------------------------------------------------------
//  <copyright file="BaseService.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Application.Services.Interfaces;
using CrawlDataWebNews.Infrastructure.UnitOfWork;

namespace CrawlDataWebNews.Application.Services
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
