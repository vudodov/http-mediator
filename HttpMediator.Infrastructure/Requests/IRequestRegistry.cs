using System;
using System.Collections.Generic;

namespace HttpMediator.Infrastructure.Requests
{
    public interface IRequestRegistry
        : IEnumerable<(string requestName, Type requestType, Type requestTypeHandler)>
    {
        bool TryGetValue(string requestName, out (Type requestType, Type requestTypeHandler) map);
    }
}