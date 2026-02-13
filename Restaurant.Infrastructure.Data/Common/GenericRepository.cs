using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Data;
using Restaurant.Infrastructure.EF;
using Restaurant.Core.Contracts.Repository.Common;

namespace Restaurant.Infrastructure.Data.Common
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly DemoContext Context;
        public GenericRepository(DemoContext Context)
        {
            this.Context = Context;
        }

        public T Add(T entity)
        {
            Context.Set<T>().Add(entity);
            Context.Entry(entity).State = EntityState.Added;
            Context.SaveChanges();
            return entity;

        }
        public async Task<T> AddAsync(T entity)
        {
            //await Context.Set<T>().AddAsync(entity);
            //Context.Entry(entity).State = EntityState.Added;
            ////Context.SaveChangesAsync();
            //return entity;
            try
            {
                await Context.Set<T>().AddAsync(entity);
                await Context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

            }
            return entity;
        }

        public IEnumerable<T> GetAll()
        {
            //var res=Context.Set<T>().Reverse();

            var data = Context.Set<T>().ToList();
            try
            {
                var f = data.OrderByDescending(GetIdPropertyValue);
                return f;
            }
            catch
            {
                return data;
            }

        }
        private long GetIdPropertyValue(T item)
        {
            // Assuming the Id property name is "Id", you can replace it with the actual property name if different.
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty != null)
            {
                var idValue = idProperty.GetValue(item);
                if (idValue is int idInt)
                {
                    return Convert.ToInt64(idInt);
                }
                else if (idValue is long idLong)
                {
                    return idLong;
                }
            }
            else
            {
                return 0;
            }

            // Return a default value (e.g., 0) or throw an exception if the "Id" property is not found or is not an integer.
            throw new InvalidOperationException($"Unable to retrieve the Id property value for entity {typeof(T).Name}.");
        }

        public T GetById(int id)
        {
            return Context.Set<T>().Find(id);

        }

        //public PagedList<T> GetPaged(PaginationParams Parameters)
        //{
        //    var sortedData = Context.Set<T>().OrderByDescending(GetIdPropertyValue);
        //    return PagedList<T>.ToPagedList(
        //        sortedData,
        //        Parameters.PageNumber,
        //        Parameters.PageSize);
        //}

        public void Remove(T entity)
        {
            Context.Set<T>().Remove(entity);
            Context.Entry(entity).State = EntityState.Deleted;
            Context.SaveChanges();
        }

        public void Update(T entity)
        {
            Context.Set<T>().Update(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();

        }


        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await Context.Set<T>().ToListAsync();
        }
        public async Task<T> GetByIdAsync(int id)
        {
            return await Context.Set<T>().FindAsync(id);
        }

        public int AddRange(List<T> entity)
        {
            Context.Set<T>().AddRange(entity);
            // Context.Entry(entity).State = EntityState.Modified;
            return entity.Count;

        }
        public int UpdateRange(List<T> entity)
        {
            Context.Set<T>().UpdateRange(entity);
            // Context.Entry(entity).State = EntityState.Modified;
            return entity.Count;

        }
        public void RemoveRange(List<T> entity)
        {
            Context.Set<T>().RemoveRange(entity);
            // Context.Entry(entity).State = EntityState.Modified;


        }
        public async Task<T?> GetLastPrimaryKeyAsync()
        {
            var entityType = Context.Model.FindEntityType(typeof(T));
            if (entityType == null)
                throw new InvalidOperationException($"Entity type {typeof(T).Name} not found in the model.");

            var primaryKey = entityType.FindPrimaryKey()?.Properties.FirstOrDefault();
            if (primaryKey == null)
                throw new InvalidOperationException($"No primary key found for {typeof(T).Name}");

            string primaryKeyName = primaryKey.Name;

            return await Context.Set<T>()
                .OrderByDescending(e => Microsoft.EntityFrameworkCore.EF.Property<object>(e, primaryKeyName))
                .FirstOrDefaultAsync();

        }

        public IQueryable<T> GetFiltered(List<Expression<Func<T, bool>>> expressions)
        {
            var query = Context.Set<T>().AsQueryable();

            if (expressions != null)
            {
                foreach (var expression in expressions)
                {
                    // Check if the expression is for a nullable boolean or non-nullable boolean
                    if (expression.Body.Type == typeof(bool?))
                    {
                        // Adjust the expression for nullable boolean
                        var adjustedExpression = AdjustForNullableBoolean(expression);
                        query = query.Where(adjustedExpression).AsNoTracking();
                    }
                    else if (expression.Body.Type == typeof(bool))
                    {
                        // Use the expression directly for non-nullable boolean
                        query = query.Where(expression).AsNoTracking();
                    }
                    else
                    {
                        // Handle other types (e.g., string, int, etc.) as usual
                        query = query.Where(expression).AsNoTracking();
                    }
                }
            }

            return query;
        }

        private Expression<Func<T, bool>> AdjustForNullableBoolean<T>(Expression<Func<T, bool>> property)
        {
            // Adjust expression to handle nullable boolean
            var coalesceExpression = Expression.Coalesce(property.Body, Expression.Constant(false));
            var equalExpression = Expression.Equal(coalesceExpression, Expression.Constant(true));

            return Expression.Lambda<Func<T, bool>>(equalExpression, property.Parameters);
        }


        // در GenericRepository.cs - متد GetByConditionAsync
        public virtual async Task<List<T>> GetByConditionAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includeProperties)
        {
            // استفاده از Context.Set<T>() به جای DbSet
            IQueryable<T> query = Context.Set<T>();

            // Include کردن navigation properties
            if (includeProperties != null && includeProperties.Length > 0)
            {
                foreach (var includeProperty in includeProperties)
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.Where(predicate).ToListAsync();
        }
    }

}