using System;
using System.IO.Pipelines;
using System.Text.Json;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Notifications;
using HttpMediator.Infrastructure.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HttpMediator.Middleware
{
    public class MiddlewareNotifications
    {
        private readonly INotificationRegistry _registry;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const string notificationPath = "mediator-notifications";

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = false
        };
        
        public MiddlewareNotifications(RequestDelegate next, INotificationRegistry registry, ILoggerFactory loggerFactory)
        {
            _next = next;
            _registry = registry;
            _logger = loggerFactory.CreateLogger<MiddlewareNotifications>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var (middlewareIdentifier, notificationName) = DecomposePath(httpContext.Request.Path.Value);

            switch (middlewareIdentifier)
            {
                case notificationPath when HttpMethods.IsPost(httpContext.Request.Method) && !string.IsNullOrWhiteSpace(notificationName):
                    await ExecuteNotifications(httpContext, notificationName);
                    break;
                case notificationPath when HttpMethods.IsGet(httpContext.Request.Method) && string.IsNullOrWhiteSpace(notificationName):
                    break;
                default:
                    await _next.Invoke(httpContext);
                    break;
            }
        }

        private async Task ExecuteNotifications(HttpContext httpContext, string notificationName)
        {
            var notificationGuid = Guid.NewGuid();

            if (_registry.TryGetValue(notificationName, out var handlersTypeInformation))
            {
                httpContext.Request.
            }
        }

        private static (string middlewareIdentifier, string notificationName) DecomposePath(string path)
        {
            var pathParts = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return (pathParts[0], pathParts[1]);
        }
        
        
        private static async Task ExecuteNotificationHandler()
    }
}