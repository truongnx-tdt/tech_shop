// "-----------------------------------------------------------------------
//  <copyright file="IRefreshTokenRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Infrastructure.Repositories._BaseRepo;
using CrawlDataWebNews.Data.Entities.Auth;

namespace CrawlDataWebNews.Infrastructure.Repositories.RefreshTokenRepo
{
    public interface IRefreshTokenRepository : IGenericRepository<CrawlDataWebNews.Data.Entities.Auth.RefreshToken>
    {
    }
}
