using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HttpMediator.Infrastructure.Notifications
{
    public sealed class NotificationRegistry : INotificationRegistry
    {
        private readonly
            IImmutableDictionary<string, (Type notificationType, IEnumerable<Type> notificationHandlerTypes)> _mapping;

        public NotificationRegistry() : this(new[] {Assembly.GetCallingAssembly()})
        {
        }

        public NotificationRegistry(IEnumerable<Assembly> assemblies)
        {
            _mapping = Scan(assemblies);
        }

        public bool TryGetHandlersMapping(string notificationName,
            out (Type notificationType, IEnumerable<Type> notificationTypeHanlers) map) =>
            _mapping.TryGetValue(notificationName, out map);

        public IEnumerator<(string notificationName, Type notificationType, IEnumerable<Type> notificationTypeHandlers)>
            GetEnumerator() =>
            _mapping.Select(map =>
            (
                notificationName: map.Key,
                notificationType: map.Value.notificationType,
                notificationHanlerTypes: map.Value.notificationHandlerTypes
            )).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static IImmutableDictionary<string, (Type notificationType, IEnumerable<Type> notificationHandlerTypes)>
            Scan(
                IEnumerable<Assembly> assemblies)
        {
            if (!assemblies.Any())
                throw new ArgumentException(
                    "Notification registry requires at least one assembly to scan for commands");

            var notificationToHandlersMapping = new Dictionary<Type, HashSet<Type>>();

            foreach (var assembly in assemblies)
            {
                var discoveredNotificationTypes = assembly.GetNotificationTypes();

                foreach (var notificationType in discoveredNotificationTypes)
                {
                    var discoveredNotificationHandlerTypes = assembly.GetNotificationHandlerTypesFor(notificationType);

                    if (discoveredNotificationHandlerTypes.Any())
                    {
                        if (notificationToHandlersMapping.ContainsKey(notificationType))
                            notificationToHandlersMapping[notificationType]
                                .UnionWith(discoveredNotificationHandlerTypes);
                        else
                            notificationToHandlersMapping[notificationType] =
                                new HashSet<Type>(discoveredNotificationHandlerTypes);
                    }
                }
            }

            return notificationToHandlersMapping.ToImmutableDictionary(
                map => map.Key.Name.ToKebabCase(),
                map => (
                    notificationType: map.Key,
                    notificationTypeHandlers: map.Value.AsEnumerable()));
        }
    }
}