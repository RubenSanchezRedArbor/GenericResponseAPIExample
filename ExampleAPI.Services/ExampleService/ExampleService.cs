using ExampleAPI.Common;
using ExampleAPI.DataAccess;
using ExampleAPI.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace ExampleAPI.Services.ExampleService
{
    public class ExampleService : IExampleService
    {
        #region Properties

        protected readonly IMemoryCache _cache;
        protected readonly IRepository<ExampleEntity> _repository;

        #endregion

        #region Constructor

        public ExampleService(IMemoryCache cache, IRepository<ExampleEntity> repository)
        {
            _cache = cache;
            _repository = repository;
        }

        #endregion

        #region Methods

        public async Task<List<ExampleEntity>> GetExampleListAsync()
        {
            return await _repository.GetAllAsync();
        }
        public async Task<ExampleEntity> GetExampleByIdAsync(int id)
        {
            var exampleEntity = (await _repository.GetAllAsync(x => x.Id == id)).FirstOrDefault();

            if (exampleEntity == null)
            {
                throw new NoContentException(Messages.NoRecordFound);
            }

            return exampleEntity;
        }
        public async Task<int> CreateOrUpdateExampleAsync(ExampleEntity model)
        {

            DoValidations(model);

            if (model.Id <= 0)
            {
                model.Id = (await _repository.GetAllAsync()).LastOrDefault().Id + 1;
                await _repository.CreateOrUpdateAsync(model);
            }
            else
            {
                var existingRecord = (await _repository.GetAllAsync(x => x.Id == model.Id)).FirstOrDefault();
                if (existingRecord != null)
                {
                    existingRecord.Name = model.Name;
                    existingRecord.LastName = model.LastName;
                    existingRecord.Age = model.Age;
                }
                else
                {
                    throw new BusinessException(Messages.NoRecordFound);
                }
                await DeleteExample(model.Id);
                await _repository.CreateOrUpdateAsync(model);
            }

            return model.Id;

        }
        public async Task<bool> DeleteExample(int id)
        {
            var existingRecord = (await _repository.GetAllAsync(x => x.Id == id)).FirstOrDefault();

            if (existingRecord == null)
            {
                throw new BusinessException(Messages.NoRecordFound);
            }

            await _repository.DeleteAsync(existingRecord);

            return true;
        }

        private void DoValidations(ExampleEntity model)
        {
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new BusinessException($"Name {Messages.RequiredField}");
            }
            if (string.IsNullOrWhiteSpace(model.LastName))
            {
                throw new BusinessException($"LastName {Messages.RequiredField}");
            }
            if (model.Age <= 0)
            {
                throw new BusinessException($"Age {Messages.RequiredField}");
            }
        }

        #endregion
    }
}
