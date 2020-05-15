using System;
using System.Collections.Generic;

namespace HttpMediator.Infrastructure.Notifications
{
    public interface INotificationRegistry 
        : IEnumerable<(string notificationName, Type notificationType, IEnumerable<Type> notificationHandlerTypes)>
    {
        bool TryGetValue(string notificationName,
            out (Type notificationType, IEnumerable<Type> notificationHandlerTypes) map);
    }
}