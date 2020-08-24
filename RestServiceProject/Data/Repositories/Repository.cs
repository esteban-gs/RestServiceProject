using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestServiceProject.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext _context;

        public Repository(DbContext context)
        {
            _context = context;
        }

        public async Task Add(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
        }


        public async Task<bool> Contains(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().CountAsync(predicate) > 0;
        }

        public async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            return await _context.Set<TEntity>().Where(predicate).CountAsync();
        }

        public async Task<IEnumerable<TEntity>> FindAll()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public async Task<TEntity> FindById(Guid id)
        {
            var user = await _context.Set<TEntity>().FindAsync(id);
            if (user == null) return null;
            return user;
        }

        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity entity)
        {
            _context.Set<TEntity>().Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }
    }
}
