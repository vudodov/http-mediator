using System;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace HttpMediator.MediatorMiddleware.Tests
{
    internal static class StringExtensions
    {
        internal static async Task<HttpContext> InvokeNotificationMiddleware(this string notificationName,
            Func<HttpContext, Task> invokeAsync, string requestJson = null) =>
            await ConstructContextAndInvoke("mediator-notification", notificationName, requestJson, invokeAsync);

        internal static async Task<HttpContext> InvokeRequestMiddleware(this string notificationName,
            Func<HttpContext, Task> invokeAsync, string requestJson = null) =>
            await ConstructContextAndInvoke("mediator-request", notificationName, requestJson, invokeAsync);

        private static async Task<HttpContext> ConstructContextAndInvoke(this string path1,
            string path2, string requestJson, Func<HttpContext, Task> invokeAsync)
        {
            var bodyRequestStream = new MemoryStream();

            if (!string.IsNullOrWhiteSpace(requestJson))
            {
                await bodyRequestStream.WriteAsync(Encoding.UTF8.GetBytes(requestJson));
                bodyRequestStream.Seek(0, SeekOrigin.Begin);
            }

            var bodyResponseStream = new MemoryStream();
            var httpContext = new DefaultHttpContext(new FeatureCollection
            {
                [typeof(IHttpResponseBodyFeature)] = new StreamResponseBodyFeature(bodyResponseStream),
                [typeof(IHttpResponseFeature)] = new HttpResponseFeature(),
                [typeof(IHttpRequestFeature)] = new HttpRequestFeature
                {
                    Body = bodyRequestStream,
                    Path = $"/{path1}/{path2}",
                    Method = HttpMethods.Post
                }
            });

            httpContext.Request.ContentType = MediaTypeNames.Application.Json;

            await invokeAsync(httpContext);

            bodyResponseStream.Position = 0;

            return httpContext;
        }
    }
}