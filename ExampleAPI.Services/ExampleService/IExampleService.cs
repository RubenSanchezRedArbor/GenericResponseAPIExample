using ExampleAPI.Domain.Entities;

namespace ExampleAPI.Services.ExampleService
{
    public interface IExampleService
    {
        Task<List<ExampleEntity>> GetExampleListAsync();
        Task<ExampleEntity> GetExampleByIdAsync(int id);
        Task<int> CreateOrUpdateExampleAsync(ExampleEntity model);
        Task<bool> DeleteExample(int id);
    }
}
