using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace HttpMediator.MediatorMiddleware
{
    internal static class TypeExtensions
    {
        internal static Task HandleNotification(this Type notificationHandlerType, object? notification,
            Guid notificationBatchId, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            object notificationHandlerInstance =
                ActivatorUtilities.CreateInstance(serviceProvider, notificationHandlerType);
            MethodInfo handleAsyncMethod = notificationHandlerType.GetHandleAsyncMethod();

            return handleAsyncMethod.InvokeAsync(
                notificationHandlerInstance,
                notification!,
                notificationBatchId,
                cancellationToken);
        }

        internal static async Task<object> HandleRequest(this Type requestHandlerType, object? request, Guid requestId,
            IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            object requestHandlerInstance = ActivatorUtilities.CreateInstance(serviceProvider, requestHandlerType);
            MethodInfo handleAsyncMethod = requestHandlerType.GetHandleAsyncMethod();


            return await handleAsyncMethod.InvokeAndReturnAsync(
                requestHandlerInstance,
                request!,
                requestId,
                cancellationToken);
        }

        private static MethodInfo GetHandleAsyncMethod(this Type handlerType) =>
            handlerType.GetMethod("HandleAsync") ??
            throw new MissingMethodException(nameof(handlerType), "HandleAsync");
    }
}