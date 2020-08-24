using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestServiceProject.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
        Task<int> Complete();
    }

    public class UnitOfWork : IUnitOfWork
    {

        private readonly ApplicationDBContext _context;
        private Hashtable _repositories;

        public UnitOfWork(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task<int> Complete()
        {
            return await _context.SaveChangesAsync();
        }


        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            if (_repositories == null)
                _repositories = new Hashtable();

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(Repository<>);

                var repositoryInstance =
                    Activator.CreateInstance(repositoryType
                        .MakeGenericType(typeof(TEntity)), _context);

                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<TEntity>)_repositories[type];
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
