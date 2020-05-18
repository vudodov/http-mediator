using System.Text.Json;
using HttpMediator.Infrastructure.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HttpMediator.MediatorMiddleware
{
    public class RequestsMiddleware
    {
        private readonly IRequestRegistry _registry;
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = false
        };
        
        public RequestsMiddleware(RequestDelegate next, IRequestRegistry registry, 
            ILoggerFactory loggerFactory)
        {
            _next = next;
            _registry = registry;
            _logger = loggerFactory.CreateLogger<RequestsMiddleware>();
        }
    }
}