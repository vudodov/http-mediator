using System;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace HttpMediator.MediatorMiddleware.Tests
{
    public static class StringExtensions
    {
        public static async Task<HttpContext> ConstructContextAndInvoke(this string notificationName, Func<HttpContext, Task> invokeAsync)
        {
            var bodyRequestStream = new MemoryStream();
            var bodyResponseStream = new MemoryStream();
            var httpContext = new DefaultHttpContext(new FeatureCollection
            {
                [typeof(IHttpResponseBodyFeature)] = new StreamResponseBodyFeature(bodyResponseStream),
                [typeof(IHttpResponseFeature)] = new HttpResponseFeature(),
                [typeof(IHttpRequestFeature)] = new HttpRequestFeature
                {
                    Path = $"/mediator-notifications/{notificationName}",
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