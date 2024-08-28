using System.Linq.Expressions;

namespace ExampleAPI.DataAccess
{
    public interface IRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? whereCondition = null);
        Task CreateOrUpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
