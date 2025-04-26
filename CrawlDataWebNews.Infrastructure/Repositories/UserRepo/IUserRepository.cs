// "-----------------------------------------------------------------------
//  <copyright file="IUserRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Data.Entities.Auth;
using CrawlDataWebNews.Infrastructure.Repositories._BaseRepo;

namespace CrawlDataWebNews.Infrastructure.Repositories.UserRepo
{
    public interface IUserRepository : IGenericRepository<User>
    {

    }
}
