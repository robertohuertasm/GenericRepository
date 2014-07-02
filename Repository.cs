using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace GenericRepository
{
    public class Repository<T, TId> :  
        IRepository<T,TId, TC> 
        where T: BaseModel<TId>
        where TId : IComparable
        
    {

        #region [properties]

        private ObjectContext ObjectContext
        {
            get
            {
                return ((IObjectContextAdapter)Context).ObjectContext;
            }
        }

        #endregion

        #region [constructor]

        public Repository()
        {
            Context = new DbContext(); //your EF context here
        }

        #endregion

        #region [interface]

        protected DbContext Context { get; set; } 

        public void Create(T item)
        {
            GetDbEntityEntry(item).State = EntityState.Added;
        }

        public void Update(T item)
        {
            GetDbEntityEntry(item).State = EntityState.Modified;
        }

        public void Delete(T item)
        {
            GetDbEntityEntry(item).State = EntityState.Deleted;
        }

        public T Get(TId id)
        {
            var all = Get();
            var filtered = Filter(all, x => x.Id, id);
            var result = filtered.FirstOrDefault();
            return result;
        }

        public T Get(TId id, params Expression<Func<T, object>>[] includePropertiesExpressions)
        {
            var all = Get(includePropertiesExpressions);
            var filtered = Filter(all, x => x.Id, id);
            var result = filtered.FirstOrDefault();
            return result;
        }

        public IQueryable<T> Get()
        {
            return Context.Set<T>();
        }

        public IQueryable<T> Get(params Expression<Func<T, object>>[] includePropertiesExpressions)
        {
            var all = Get();
            foreach (var expression in includePropertiesExpressions)
            {
                all = all.Include(expression);
            }
            return all;
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            var all = Get();
            var query = all.Where(predicate);
            return query;
        } 

        public void AddGraph(T item)
        {
            Context.Set<T>().Add(item);
        }

        public void DetectChanges()
        {
            ObjectContext.DetectChanges();
        }

        public void Save()
        {
            Context.SaveChanges();
        }

        #endregion


        #region [private]

        private DbEntityEntry GetDbEntityEntry(T entity)
        {
            DbEntityEntry dbEntityEntry = Context.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {

                Context.Set<T>().Attach(entity);
            }

            return dbEntityEntry;
        }

        private IQueryable<T> Filter<TProperty>(IQueryable<T> dbSet,
            Expression<Func<T, TProperty>> property, TProperty value)
            where TProperty : IComparable
        {

            var memberExpression = property.Body as MemberExpression;
            if (memberExpression == null || !(memberExpression.Member is PropertyInfo))
            {

                throw new ArgumentException("Property expected", "property");
            }

            var left = property.Body;
            Expression right = Expression.Constant(value, typeof(TProperty));
            Expression searchExpression = Expression.Equal(left, right);
            var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, new[] { property.Parameters.Single() });

            return dbSet.Where(lambda);
        }

        #endregion

        #region [Dispose]

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

    }
}

