using ExampleAPI.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace ExampleAPI.DataAccess
{
    public class Repository<T> : IRepository<T> where T : class
    {
        #region Properties

        protected readonly IMemoryCache _cache;

        #endregion

        #region Constructor

        public Repository(IMemoryCache cache)
        {
            _cache = cache;
        }

        #endregion

        #region Public Methods
        public async Task CreateOrUpdateAsync(T entity)
        {
            var data = await GetOrSetRepository();
            data.Add(entity);
            await UpdateCache(data);
        }

        public async Task DeleteAsync(T entity)
        {
            var data = await GetOrSetRepository();
            data.Remove(entity);
            await UpdateCache(data);

        }

        public async Task<List<T>> GetAllAsync(Expression<Func<T, bool>>? whereCondition = null)
        {
            var data = await GetOrSetRepository();

            IQueryable<T> query = data.AsQueryable();

            if (whereCondition != null)
            {
                query = query.Where(whereCondition);
            }

            return await Task.FromResult(query.ToList());

        }

        #endregion

        #region Private Methods
        private async Task<List<T>> GetOrSetRepository()
        {
            var data = await _cache.GetOrCreateAsync("defaultCacheKey", async entry =>
            {
                entry.SlidingExpiration = TimeSpan.FromHours(1);
                return await GenerateData();
            });

            return data as List<T>;
        }

        private async Task<List<ExampleEntity>> GenerateData()
        {
            var exampleData = new List<ExampleEntity>();

            for (int i = 1; i <= 10; i++)
            {
                var exampleRecord = new ExampleEntity
                {
                    Id = i,
                    Name = $"ExampleName{i}",
                    LastName = $"ExampleLastName {i}",
                    Age = i * 10
                };
                exampleData.Add(exampleRecord);
            }

            return await Task.FromResult(exampleData);
        }

        private async Task UpdateCache(List<T> dataList)
        {
            _cache.Remove("defaultCacheKey");
            await Task.Run(() => _cache.Set("defaultCacheKey", dataList, TimeSpan.FromHours(1)));
        }

        #endregion
    }
}
