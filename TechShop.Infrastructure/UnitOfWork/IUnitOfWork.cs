// "-----------------------------------------------------------------------
//  <copyright file="IUnitOfWork.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using TechShop.Infrastructure.Repositories.LanguageRepo;
using TechShop.Infrastructure.Repositories.RefreshTokenRepo;
using TechShop.Infrastructure.Repositories.UserRepo;
using Microsoft.EntityFrameworkCore.Storage;
using TechShop.Infrastructure.Repositories.AddressRepo;

namespace TechShop.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork
    {
        #region register Repo
        public IUserRepository UserRepository { get; }
        public IRefreshTokenRepository RefreshToken { get; }
        public ILanguageRepository Language { get; }
        public ILanguageTranslationRepository LanguageTranslation { get; }
        public IAddressRepository AddressRepository { get; }
        #endregion
        Task<bool> SaveChangesAsync();
        IExecutionStrategy CreateExecutionStrategy();
        Task<TResult> ExecuteWithStrategyAsync<TResult>(Func<Task<TResult>> operation);
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task BulkDeleteAsync<T>(ICollection<T> datas) where T : class;
        Task BulkInsertAsync<T>(ICollection<T> datas) where T : class;
        Task BulkUpdateAsync<T>(ICollection<T> datas) where T : class;
    }
}
