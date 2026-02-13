namespace Restaurant.Core.Contracts.Repository.Common
{
    public interface IGenericRepository<T> where T : class
    {
        T GetById(int id);
        IEnumerable<T> GetAll();

        T Add(T entity);
        void Remove(T entity);
        void Update(T entity);
        int AddRange(List<T> entity);
        int UpdateRange(List<T> entity);
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        //PagedList<T> GetPaged(PaginationParams Parameters);
        void RemoveRange(List<T> entity);
        IQueryable<T> GetFiltered(List<System.Linq.Expressions.Expression<Func<T, bool>>> expressions);
        Task<T?> GetLastPrimaryKeyAsync();
        Task<List<T>> GetByConditionAsync(System.Linq.Expressions.Expression<Func<T, bool>> predicate, params System.Linq.Expressions.Expression<Func<T, object>>[] includeProperties);
    }
}
