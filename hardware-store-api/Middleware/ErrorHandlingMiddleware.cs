using hardware_store_api.Exceptions;
using hardware_store_api.Models;
using System.Text.Json;

namespace hardware_store_api.Middleware
{
    public class ErrorHandlingMiddleware : IMiddleware
    {
        //private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger)
        {
            //_next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int code = 500;
            //string data = "Internal server error.";
            string data = ex.Message;

            if (ex is HttpStatusException httpStatusException)
            {
                code = (int)httpStatusException.Status;
                data = JsonSerializer.Serialize(new APIResponse(code, httpStatusException.Message));
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = code;
            return context.Response.WriteAsync(ex.Message);
        }


    }
}
