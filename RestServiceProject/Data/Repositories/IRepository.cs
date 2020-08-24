using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestServiceProject.Data.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> FindById(Guid id);
        Task<IEnumerable<TEntity>> FindAll();
        Task Add(TEntity entity);
        void Remove(TEntity entity);
        void Update(TEntity entity);
        Task<bool> Contains(Expression<Func<TEntity, bool>> predicate);
        Task<int> Count(Expression<Func<TEntity, bool>> predicate);
    }
}
