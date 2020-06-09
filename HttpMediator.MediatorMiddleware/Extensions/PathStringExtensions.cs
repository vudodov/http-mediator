using System;
using Microsoft.AspNetCore.Http;

namespace HttpMediator.MediatorMiddleware
{
    public static class PathStringExtensions
    {
        public static (string middlewareIdentifier, string targetName) DecomposePath(this PathString pathString)
        {
            var pathParts = pathString.Value.Split('/', StringSplitOptions.RemoveEmptyEntries);
            return pathParts.Length == 2
                ? (pathParts[0], pathParts[1])
                : (string.Empty, string.Empty);
        }
    }
}