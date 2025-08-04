// "-----------------------------------------------------------------------
//  <copyright file="UnitOfWork.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"

using TechShop.Infrastructure.AppDbContext;
using TechShop.Infrastructure.Repositories.LanguageRepo;
using TechShop.Infrastructure.Repositories.RefreshTokenRepo;
using TechShop.Infrastructure.Repositories.UserRepo;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using TechShop.Infrastructure.Repositories.AddressRepo;

namespace TechShop.Infrastructure.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly ApplicationDbContext _context;
        private UserRepository _userRepository;
        private RefreshTokenRepository _refreshTokenRepository;
        private LanguageRepository _languageRepository;
        private LanguageTranslationRepository _languageTranslationRepository;
        private AddressRepository _addressRepository;
        public UnitOfWork(ApplicationDbContext context) => _context = context;
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public async Task<bool> SaveChangesAsync()
        {
            var cm = await _context.SaveChangesAsync().ConfigureAwait(false);
            return cm != 0;
        }
        public IExecutionStrategy CreateExecutionStrategy()
        {
            return _context.Database.CreateExecutionStrategy();
        }

        public async Task<TResult> ExecuteWithStrategyAsync<TResult>(Func<Task<TResult>> operation)
        {
            var strategy = CreateExecutionStrategy();
            return await strategy.ExecuteAsync(operation);
        }

        /// <summary>
        /// free memory cache and close connection
        /// </summary>
        private bool disposed = false;

        public IUserRepository UserRepository
        {
            get { return _userRepository ?? (_userRepository = new UserRepository(_context)); }
        }

        public IRefreshTokenRepository RefreshToken
        {
            get { return _refreshTokenRepository ?? (_refreshTokenRepository = new RefreshTokenRepository(_context)); }
        }

        public ILanguageRepository Language
        {
            get { return _languageRepository ?? (_languageRepository = new LanguageRepository(_context)); }
        }

        public ILanguageTranslationRepository LanguageTranslation
        {
            get { return _languageTranslationRepository ?? (_languageTranslationRepository = new LanguageTranslationRepository(_context)); }
        }

        public IAddressRepository AddressRepository => _addressRepository ?? (_addressRepository = new AddressRepository(_context));

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task BulkDeleteAsync<T>(ICollection<T> datas) where T : class
        {
            await _context.BulkDeleteAsync(datas).ConfigureAwait(false);
        }
        public async Task BulkInsertAsync<T>(ICollection<T> datas) where T : class
        {
            await _context.BulkInsertAsync(datas).ConfigureAwait(false);
        }
        public async Task BulkUpdateAsync<T>(ICollection<T> datas) where T : class
        {
            await _context.BulkUpdateAsync(datas).ConfigureAwait(false);
        }
    }
}
