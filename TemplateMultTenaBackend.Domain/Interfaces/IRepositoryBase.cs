using System.Linq.Expressions;

namespace TemplateMultTenaBackend.Domain.Interfaces
{
    public interface IRepositoryBase<T>
    {
        IQueryable<T> FindAll(bool trackChanged);

        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges);

        void Add(T entity);

        void Update(T entity);

        void Remove(T entity);
    }
}
