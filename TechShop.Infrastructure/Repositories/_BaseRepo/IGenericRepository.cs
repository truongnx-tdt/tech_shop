// "-----------------------------------------------------------------------
//  <copyright file="IGenericRepository.cs" author=TDT>
//      Copyright (c) TDT. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------"
using System.Linq.Expressions;

namespace TechShop.Infrastructure.Repositories._BaseRepo
{
    public interface IGenericRepository<T> where T : class
    {
        T Add(T t);
        Task<T> AddAsyn(T t);
        bool Any();
        Task<bool> AnyAsync();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        int Count();
        Task<int> CountAsync();
        void Delete(T entity);
        Task DeleteAsyn(T entity);
#pragma warning disable S2953 // Methods named "Dispose" should implement "IDisposable.Dispose"
        void Dispose();
#pragma warning restore S2953 // Methods named "Dispose" should implement "IDisposable.Dispose"
        T Find(Expression<Func<T, bool>> match);
        T FirstOrDefault(Expression<Func<T, bool>> match);
        ICollection<T> FindAll(Expression<Func<T, bool>> match);
        Task<ICollection<T>> FindAllAsync(Expression<Func<T, bool>> match);
        Task<T> FindAsync(Expression<Func<T, bool>> match);
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> match);
        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);
        Task<ICollection<T>> FindByAsyn(Expression<Func<T, bool>> predicate);
        T Get(object id);
        IQueryable<T> GetAll();
        Task<IEnumerable<T>> GetAllAsyn();
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        Task<T> GetAsync(params object[] id);
        T Update(T t, object key);
        Task<T> UpdateAsyn(T t, object key);
    }
}
