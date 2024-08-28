using ExampleAPI.Common;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;

namespace ExampleAPI.Middleware
{

    public class ApiResponseMiddleware : ControllerBase
    {
        private readonly RequestDelegate _next;

        public ApiResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using (var memoryStream = new MemoryStream())
            {
                context.Response.Body = memoryStream;

                try
                {
                    await _next(context);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();

                    if (context.Response.StatusCode >= StatusCodes.Status200OK && context.Response.StatusCode <= Constants.StatusCodeOkMax)
                    {
                        var genericResponse = GetResponseObject(true, Messages.OK, JsonSerializer.Deserialize<object>(responseBody));

                        // Restablecer el flujo original antes de escribir la respuesta
                        context.Response.Body = originalBodyStream;

                        await WriteResponseAsync(context, JsonSerializer.Serialize(genericResponse));
                    }
                    else
                    {
                        context.Response.Body = originalBodyStream;

                        await WriteOriginalResponseAsync(context, responseBody);
                    }
                }
                catch (NotFoundException exN)
                {
                    context.Response.StatusCode = StatusCodes.Status404NotFound;

                    var genericResponse = GetResponseObject(false, exN.Message, null);

                    context.Response.Body = originalBodyStream;

                    await WriteResponseAsync(context, JsonSerializer.Serialize(genericResponse));
                }
                catch (BusinessException exb)
                {
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;

                    var genericResponse = GetResponseObject(false, exb.Message, null);

                    context.Response.Body = originalBodyStream;

                    await WriteResponseAsync(context, JsonSerializer.Serialize(genericResponse));
                }
                catch (Exception ex)
                {
                    //TODO: Loggear el error

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    context.Response.Body = originalBodyStream;

                    var genericResponse = GetResponseObject(false, Messages.UnexpectedError, null);

                    await WriteResponseAsync(context, JsonSerializer.Serialize(genericResponse));
                }
                finally
                {
                    // Restablecer el flujo de respuesta original al finalizar
                    context.Response.Body = originalBodyStream;
                }
            }
        }


        private async Task WriteResponseAsync(HttpContext context, string jsonResponse)
        {
            context.Response.ContentType = "application/json";

            context.Response.ContentLength = Encoding.UTF8.GetByteCount(jsonResponse);

            await context.Response.WriteAsync(jsonResponse);
        }

        private async Task WriteOriginalResponseAsync(HttpContext context, string responseBody)
        {
            await context.Response.WriteAsync(responseBody);
        }

        private ApiGenericResponse<object> GetResponseObject(bool isSuccess, string message, object? data)
        {
            return new ApiGenericResponse<object>
            {
                IsSuccess = isSuccess,
                Message = message,
                Data = data
            };
        }
    }
}
