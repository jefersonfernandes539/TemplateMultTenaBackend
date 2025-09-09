using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TemplateMultTenaBackend.Domain.Interfaces;
using TemplateMultTenaBackend.Infrastructure.Persistence;

namespace TemplateMultTenaBackend.Infrastructure.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected ApplicationDbContext ApplicationDbContext;

        public RepositoryBase(ApplicationDbContext applicationDbContext)
        {
            ApplicationDbContext = applicationDbContext;
        }

        public IQueryable<T> FindAll(bool trackChanges)
        {
            return trackChanges ? ApplicationDbContext.Set<T>() : ApplicationDbContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges)
        {
            return trackChanges ? ApplicationDbContext.Set<T>().Where(expression) : ApplicationDbContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Add(T entity) => ApplicationDbContext.Set<T>().Add(entity);

        public void Update(T entity) => ApplicationDbContext.Set<T>().Update(entity);

        public void Remove(T entity) => ApplicationDbContext.Set<T>().Remove(entity);
    }
}
