// "-----------------------------------------------------------------------
//  <copyright file="RefreshTokenRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Infrastructure.AppDbContext;
using CrawlDataWebNews.Infrastructure.Repositories._BaseRepo;
using CrawlDataWebNews.Infrastructure.Repositories.RefreshTokenRepo;

namespace CrawlDataWebNews.Infrastructure.Repositories.RefreshToken
{
    public class RefreshTokenRepository : GenericRepository<CrawlDataWebNews.Data.Entities.Auth.RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
