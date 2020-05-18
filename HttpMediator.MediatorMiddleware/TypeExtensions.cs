using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HttpMediator.MediatorMiddleware
{
    internal static class TypeExtensions
    {
        private const string HandleMethodName = "HandleAsync";

        internal static Task HandleNotification(this Type notificationHandlerType, object? notification,
            Guid notificationBatchId, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            object notificationHandlerInstance =
                ActivatorUtilities.CreateInstance(serviceProvider, notificationHandlerType);
            MethodInfo? handleAsyncMethod = notificationHandlerType.GetMethod(HandleMethodName);

            if (handleAsyncMethod == null)
                throw new MissingMethodException(nameof(notificationHandlerType), HandleMethodName);

            var handlingTask = handleAsyncMethod.Invoke(notificationHandlerInstance,
                           new[] {notification, notificationBatchId, cancellationToken})
                       ?? throw new NullReferenceException(
                           $"{nameof(notificationHandlerType)}.{HandleMethodName} should return Task, but found null");

            return (Task) handlingTask;
        }
    }
}