using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HttpMediator.Infrastructure.Notifications;
using HttpMediator.Infrastructure.Requests;

namespace HttpMediator.Infrastructure
{
    internal static class AssemblyExtensions
    {
        internal static IEnumerable<Type> GetNotificationTypes(this Assembly assembly) =>
            assembly.GetTypes()
                .Where(type => type.IsClass &&
                               !type.IsAbstract &&
                               typeof(INotification).IsAssignableFrom(type));

        internal static IEnumerable<Type>
            GetNotificationHandlerTypesFor(this Assembly assembly, Type notificationType) =>
            assembly.GetTypes()
                .Where(type => type.IsNotificationHandlerFor(notificationType));

        internal static IEnumerable<Type> GetRequestTypes(this Assembly assembly) =>
            assembly.GetTypes()
                .Where(type => type.IsClass &&
                               !type.IsAbstract &&
                               typeof(IRequest).IsAssignableFrom(type));

        internal static Type GetRequestHandlerTypeFor(this Assembly assembly, Type requestType) =>
            assembly.GetTypes()
                .Single(type => type.IsRequestHandlerFor(requestType));
    }
}