namespace DeliveryManagement.Api.Infrastructure
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;

    public sealed class CorrelationIdLoggingMiddleware
    {
        private const string CorrelationIdHeaderName = "X-Correlation-Id";

        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdLoggingMiddleware> _logger;

        public CorrelationIdLoggingMiddleware(RequestDelegate next, ILogger<CorrelationIdLoggingMiddleware> logger)
        {
            this._next = next;
            _logger = logger;
        }

        public Task Invoke(HttpContext context)
        {
            string correlationId = null;

            // Try to get the correlationId from header; if it doesn't exist, create a new one
            if (context.Request.Headers.TryGetValue(CorrelationIdHeaderName, out var correlationIdValues))
            {
                correlationId = correlationIdValues.FirstOrDefault();
            }

            if (string.IsNullOrWhiteSpace(correlationId))
            {
                correlationId = Guid.NewGuid().ToString();
            }

            using (_logger.BeginScope("{CorrelationId}", correlationId))
            {
                return this._next(context);
            }
        }
    }

    public static class CorrelationIdLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseCorrelationIdLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CorrelationIdLoggingMiddleware>();
        }
    }
}