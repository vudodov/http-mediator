using System;
using System.Buffers;
using System.Net;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
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
        private const string requestPathIdentifier = "mediator-request";

        private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = false
        };

        public RequestsMiddleware(RequestDelegate next, IRequestRegistry registry, ILoggerFactory loggerFactory)
        {
            _next = next;
            _registry = registry;
            _logger = loggerFactory.CreateLogger<RequestsMiddleware>();
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var (middlewareIdentifier, requestName) = httpContext.Request.Path.DecomposePath();

            switch (middlewareIdentifier)
            {
                case requestPathIdentifier when HttpMethods.IsPost(httpContext.Request.Method) &&
                                                !string.IsNullOrWhiteSpace(requestName):
                    var requestId = Guid.NewGuid();
                    var requestResponse = await ExecuteRequest(httpContext, requestName, requestId);
                    SetHttpResponse(httpContext, requestResponse, requestId);
                    break;
                case requestPathIdentifier when HttpMethods.IsGet(httpContext.Request.Method) &&
                                                string.IsNullOrWhiteSpace(requestName):
                    //TODO: return all possible requests
                    break;
                default:
                    await _next.Invoke(httpContext);
                    break;
            }
        }

        private async Task<object> ExecuteRequest(HttpContext httpContext, string requestName, Guid requestId)
        {
            if (_registry.TryGetValue(requestName, out var requestTypeInformation))
            {
                var (requestType, requestTypeHandler) = requestTypeInformation;
                var cancellationToken = httpContext.RequestAborted;
                var request = await httpContext.Request.BodyReader.DeserializeBodyAsync(
                    requestType,
                    _jsonSerializerOptions,
                    cancellationToken);

                return await requestTypeHandler.HandleRequest(request, requestId,
                    httpContext.RequestServices, cancellationToken);
            }

            throw new NullReferenceException($"Mediator request or request handler {requestName} was not found");
        }

        private void SetHttpResponse(HttpContext httpContext, object requestResponse, Guid requestId)
        {
            httpContext.Response.StatusCode = (int) HttpStatusCode.OK;
            httpContext.Response.ContentType = MediaTypeNames.Application.Json;
            httpContext.Response.BodyWriter.Write(
                JsonSerializer.SerializeToUtf8Bytes(new {requestId, result = requestResponse}, _jsonSerializerOptions));
            httpContext.Response.BodyWriter.Complete();
        }
    }
}