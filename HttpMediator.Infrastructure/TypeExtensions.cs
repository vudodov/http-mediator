using System;
using System.Linq;
using HttpMediator.Infrastructure.Notifications;

namespace HttpMediator.Infrastructure
{
    internal static class TypeExtensions
    {
        internal static bool IsNotificationHandlerFor(this Type handlerType, Type notificationType) =>
            handlerType.GetInterfaces()
                .Any(@interface =>
                    @interface.IsGenericType
                    && @interface.GetGenericTypeDefinition() == typeof(INotificationHandler<>)
                    && @interface.GetGenericArguments().Single().IsAssignableFrom(notificationType));
    }
}