using ExampleAPI.DataAccess;
using ExampleAPI.Domain.Entities;
using ExampleAPI.Middleware;
using ExampleAPI.Services.ExampleService;
using Microsoft.OpenApi.Models;

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddMemoryCache();

    builder.Services.AddSingleton<IRepository<ExampleEntity>, Repository<ExampleEntity>>();
    builder.Services.AddScoped<IExampleService, ExampleService>();

    //builder.Services.AddTransient<ApiResponseMiddleware>(); 

    // Add Swagger services

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Example API", Version = "v1" });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Example API v1");
        });
    }

    app.UseMiddleware<ApiResponseMiddleware>();

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception)
{
    //TODO: Log the exception
	throw;
}
