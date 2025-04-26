// "-----------------------------------------------------------------------
//  <copyright file="IUnitOfWork.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using CrawlDataWebNews.Infrastructure.Repositories.RefreshTokenRepo;
using CrawlDataWebNews.Infrastructure.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrawlDataWebNews.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        public IUserRepository UserRepository { get; }
        public IRefreshTokenRepository RefreshToken { get; }
        Task<bool> CommitAsync();
        IExecutionStrategy CreateExecutionStrategy();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task BulkDeleteAsync<T>(ICollection<T> datas) where T : class;
    }
}
