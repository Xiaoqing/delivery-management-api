namespace DeliveryManagement.Api.Infrastructure
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public class UnhandledExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<UnhandledExceptionHandlerMiddleware> _logger;

        public UnhandledExceptionHandlerMiddleware(RequestDelegate next, ILogger<UnhandledExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Uncaught exception in {httpContext.Request.Method} {httpContext.Request.Path}");
                await HandleExceptionAsync(httpContext, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync("Internal Server Error");
        }
    }

    public static class UnhandledExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseUnhandledExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<UnhandledExceptionHandlerMiddleware>();
        }
    }
}