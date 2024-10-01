using ExampleAPI.Domain.Entities;
using ExampleAPI.Services.ExampleService;
using Microsoft.AspNetCore.Mvc;

namespace ExampleAPI.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class ExampleController : ControllerBase
    {

        private readonly ILogger<ExampleController> _logger;
        private readonly IExampleService _exampleService;

        public ExampleController(ILogger<ExampleController> logger, IExampleService exampleService)
        {
            _logger = logger;
            _exampleService = exampleService;
        }

        #region Metodos

        [HttpGet]
        public async Task<ActionResult> GetList()
        {
            return Ok(await _exampleService.GetExampleListAsync());
        }

        [HttpGet]
        public async Task<ActionResult> GetById(int id)
        {
            return Ok(await _exampleService.GetExampleByIdAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult> Create(ExampleEntity model)
        {
            return Ok(await _exampleService.CreateOrUpdateExampleAsync(model));
        }

        [HttpPut]
        public async Task<ActionResult> Update(ExampleEntity model)
        {
            return Ok(await _exampleService.CreateOrUpdateExampleAsync(model));
        }

        [HttpDelete]
        public async Task<ActionResult> Delete(int id)
        {
            return Ok(await _exampleService.DeleteExample(id));
        }

        [HttpGet]
        public async Task<ActionResult> GetText()
        {
            return Ok("Esto es una prueba");
        }

        #endregion Metodos
    }
}
