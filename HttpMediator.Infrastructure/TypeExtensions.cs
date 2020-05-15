using System;
using System.Linq;
using HttpMediator.Infrastructure.Notifications;
using HttpMediator.Infrastructure.Requests;

namespace HttpMediator.Infrastructure
{
    internal static class TypeExtensions
    {
        internal static bool IsNotificationHandlerFor(this Type handlerType, Type notificationType) =>
            handlerType.GetInterfaces()
                .Any(@interface =>
                    @interface.IsGenericType
                    && @interface.GetGenericTypeDefinition().IsAssignableFrom(typeof(INotificationHandler<>))
                    && @interface.GetGenericArguments().Single().IsAssignableFrom(notificationType));

        internal static bool IsRequestHandlerFor(this Type handlerType, Type requestType) =>
            handlerType.GetInterfaces()
                .Any(@interface =>
                    @interface.IsGenericType
                    && @interface.GetGenericTypeDefinition().IsAssignableFrom(typeof(IRequestHandler<,>))
                    && @interface.GetGenericArguments().First().IsAssignableFrom(requestType));
    }
}