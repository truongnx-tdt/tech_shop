// "-----------------------------------------------------------------------
//  <copyright file="UserRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Data.Entities.Auth;
using CrawlDataWebNews.Infrastructure.AppDbContext;
using CrawlDataWebNews.Infrastructure.Repositories._BaseRepo;

namespace CrawlDataWebNews.Infrastructure.Repositories.UserRepo
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
