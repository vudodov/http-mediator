using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace HttpMediator.Infrastructure.Requests
{
    public sealed class RequestRegistry : IRequestRegistry
    {
        private readonly IImmutableDictionary<string, (Type requestType, Type requestHandlerType)> _mapping;

        public RequestRegistry(): this(new[] {Assembly.GetCallingAssembly()})
        {
        }

        public RequestRegistry(IEnumerable<Assembly> assemblies)
        {
            _mapping = Scan(assemblies);
        }

        public IEnumerator<(string requestName, Type requestType, Type requestTypeHandler)> GetEnumerator() =>
            _mapping.Select(map => (
                    requestName: map.Key,
                    requestType: map.Value.requestType,
                    requestHandlerType: map.Value.requestHandlerType))
                .GetEnumerator();

        public bool TryGetValue(string requestName, out (Type requestType, Type requestTypeHandler) map) =>
            _mapping.TryGetValue(requestName, out map);

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static IImmutableDictionary<string, (Type requestType, Type notificationHandlerTypes)> Scan(
            IEnumerable<Assembly> assemblies)
        {
            if (!assemblies.Any())
                throw new ArgumentException(
                    "Notification registry requires at least one assembly to scan for commands");

            var requestToHandlerMapping = new Dictionary<Type, Type>();

            foreach (var assembly in assemblies)
            {
                var discoveredRequestTypes = assembly.GetRequestTypes();

                foreach (var requestType in discoveredRequestTypes)
                {
                    if (requestToHandlerMapping.ContainsKey(requestType))
                        throw new InvalidOperationException(
                            $"Request mapping for {requestType.Name} already registered. Make sure you have single request of type {requestType.Name}");

                    try
                    {
                        var requestHandlerType = assembly.GetRequestHandlerTypeFor(requestType);
                        requestToHandlerMapping[requestType] = requestHandlerType;
                    }
                    catch (InvalidOperationException)
                    {
                        throw new InvalidOperationException(
                            $"{requestType.Name} request should have one and only one request handler.");
                    }
                }
            }

            return requestToHandlerMapping.ToImmutableDictionary(
                map => map.Key.Name.ToKebabCase(),
                map => (requestType: map.Key, requestTypeHandler: map.Value)
            );
        }
    }
}