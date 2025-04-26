// "-----------------------------------------------------------------------
//  <copyright file="UnitOfWork.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using CrawlDataWebNews.Infrastructure.AppDbContext;
using CrawlDataWebNews.Infrastructure.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore.Storage;

namespace CrawlDataWebNews.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private UserRepository _userRepository;
        public UnitOfWork(ApplicationDbContext context) => _context = context;
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<bool> CommitAsync()
        {
            var cm = await _context.SaveChangesAsync().ConfigureAwait(false);
            return cm != 0;
        }
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }

        /// <summary>
        /// free memory cache and close connection
        /// </summary>
        private bool disposed = false;

        public IUserRepository UserRepository
        {
            get { return _userRepository ?? (_userRepository = new UserRepository(_context)); }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                this.disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
