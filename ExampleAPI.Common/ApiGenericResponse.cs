
namespace ExampleAPI.Common
{
    public class ApiGenericResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
