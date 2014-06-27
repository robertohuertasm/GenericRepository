using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

namespace GenericRepository
{
    public class Repository<T, TId, TC> :  
        IDisposable, 
        IRepository<T,TId, TC> 
        where T: BaseModel<TId>
        where TC : DbContext, new() 
        
    {

    	#region [properties]

        private ObjectContext ObjectContext
        {
            get
            {
                return ((IObjectContextAdapter)Extra).ObjectContext;
            }
        }

        #endregion

        #region [constructor]

        public Repository()
        {
            Extra = new TC();
        }

        #endregion

        #region [interface]

        public TC Extra { get; set; } 

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
            var result = all.FirstOrDefault(x => x.Id.Equals(id));
            return result;
        }

        public T Get(TId id, params Expression<Func<T, object>>[] includePropertiesExpressions)
        {
            var all = Get(includePropertiesExpressions);
            var result = all.FirstOrDefault(x => x.Id.Equals(id));
            return result;
        }

        public IQueryable<T> Get()
        {
            return Extra.Set<T>();
        }

        public IQueryable<T> Get(params Expression<Func<T, object>>[] includePropertiesExpressions)
        {
            var all = Get();
            return includePropertiesExpressions.Aggregate(all, (current, expression) => current.Include(expression));
        }

        public IQueryable<T> Find(Expression<Func<T, bool>> predicate)
        {
            var all = Get();
            var query = all.Where(predicate);
            return query;
        } 

        public void AddGraph(T item)
        {
            Extra.Set<T>().Add(item);
        }

        public void DetectChanges()
        {
            ObjectContext.DetectChanges();
        }

        public void Save()
        {
            Extra.SaveChanges();
        }

        #endregion


        #region [private]

        private DbEntityEntry GetDbEntityEntry(T entity)
        {
            DbEntityEntry dbEntityEntry = Extra.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {

                Extra.Set<T>().Attach(entity);
            }

            return dbEntityEntry;
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
                    Extra.Dispose();
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

