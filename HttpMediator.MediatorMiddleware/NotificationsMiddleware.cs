using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Notifications;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HttpMediator.MediatorMiddleware
{
    public class NotificationsMiddleware
    {
        private readonly INotificationRegistry _registry;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private const string notificationPathIdentifier = "mediator-notifications";

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = false
        };

        public NotificationsMiddleware(RequestDelegate next, INotificationRegistry registry,
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _registry = registry;
            _logger = loggerFactory.CreateLogger<NotificationsMiddleware>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var (middlewareIdentifier, notificationName) = httpContext.Request.Path.DecomposePath();

            switch (middlewareIdentifier)
            {
                case notificationPathIdentifier when HttpMethods.IsPost(httpContext.Request.Method) &&
                                           !string.IsNullOrWhiteSpace(notificationName):
                    await ExecuteNotifications(httpContext, notificationName);
                    break;
                case notificationPathIdentifier when HttpMethods.IsGet(httpContext.Request.Method) &&
                                           string.IsNullOrWhiteSpace(notificationName):
                    //TODO: return all possible notifications
                    break;
                default:
                    await _next.Invoke(httpContext);
                    break;
            }
        }

        private async Task ExecuteNotifications(HttpContext httpContext, string notificationName)
        {
            if (_registry.TryGetValue(notificationName, out var handlersTypeInformation))
            {
                var notificationBatchId = Guid.NewGuid();
                var cancellationToken = httpContext.RequestAborted;
                var notification = await httpContext.Request.BodyReader.DeserializeBodyAsync(
                    handlersTypeInformation.notificationType,
                    _jsonSerializerOptions,
                    cancellationToken);

                Task.WaitAll(
                    handlersTypeInformation.notificationHandlerTypes
                        .Select(notificationHandlerType =>
                            notificationHandlerType.HandleNotification(
                                notification, 
                                notificationBatchId,
                                httpContext.RequestServices, 
                                cancellationToken))
                        .ToArray(), 
                    cancellationToken);
            }
        }
    }
}