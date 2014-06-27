using System;
using System.Linq;
using System.Linq.Expressions;

namespace GenericRepository
{
    public interface IRepository<T, in TId, TExtra> : IDisposable
        where T : BaseModel<TId> 
    {
        TExtra Extra { get;  set; }
        void Create(T item);
        void Update(T item);
        void Delete(T item);
        T Get(TId id);
        T Get(TId id, params Expression<Func<T, object>>[] includePropertiesExpressions);
        IQueryable<T> Get();
        IQueryable<T> Get(params Expression<Func<T, object>>[] includePropertiesExpressions);
        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        void AddGraph(T item);
        void DetectChanges();
        void Save();
    }
}
