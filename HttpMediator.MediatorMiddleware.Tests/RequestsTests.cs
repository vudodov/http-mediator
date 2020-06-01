using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using HttpMediator.Infrastructure.Requests;
using HttpMediator.MediatorMiddleware.Tests.TestRequestHandlers;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

#pragma warning disable 1998

namespace HttpMediator.MediatorMiddleware.Tests
{
    public class When_handling_request
    {
        [Fact]
        public async Task It_should_handle_single_request()
        {
            const string requestName = "simple-request";
            var registryMock = new Mock<IRequestRegistry>();
            var requestType = typeof(SimpleRequest);
            var requestHandlerType = typeof(SimpleRequestHandler);
            
            var outMapping = (requestType, requestHandlerType);
            registryMock.Setup(registry => registry.TryGetValue(requestName, out outMapping)).Returns(true);

            var middleware = new RequestsMiddleware(async _ => { }, registryMock.Object, new NullLoggerFactory());
            
            var httpContext  = await requestName.InvokeRequestMiddleware(async ctx =>
            {
                await middleware.InvokeAsync(ctx);
            });

            httpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            
            var bodyContent = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            
            var jsonBody = JsonDocument.Parse(bodyContent).RootElement;
            jsonBody
                .GetProperty("requestId").GetGuid()
                .Should().NotBeEmpty();

            jsonBody
                .GetProperty("result").GetString()
                .Should().Be("simple request result");
        }

        [Fact]
        public async Task It_should_respect_request_data()
        {
            const string requestName = "simple-data-request";
            var registryMock = new Mock<IRequestRegistry>();
            var requestType = typeof(SimpleDataRequest);
            var requestHandlerType = typeof(SimpleDataRequestHandler);
            
            var outMapping = (requestType, requestHandlerType);
            registryMock.Setup(registry => registry.TryGetValue(requestName, out outMapping)).Returns(true);

            var middleware = new RequestsMiddleware(async _ => { }, registryMock.Object, new NullLoggerFactory());
            
            var httpContext  = await requestName.InvokeRequestMiddleware(async ctx =>
            {
                await middleware.InvokeAsync(ctx);
            }, @"{""data"": ""test data for simple data request"" }");

            httpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            
            var bodyContent = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            
            var jsonBody = JsonDocument.Parse(bodyContent).RootElement;
            jsonBody
                .GetProperty("requestId").GetGuid()
                .Should().NotBeEmpty();

            jsonBody
                .GetProperty("result").GetString()
                .Should().Be("test data for simple data request");
        }

        [Fact]
        public async Task It_should_return_complicated_objects()
        {
            const string requestName = "complicated-data-request";
            var registryMock = new Mock<IRequestRegistry>();
            var requestType = typeof(ComplicatedDataRequest);
            var requestHandlerType = typeof(ComplicatedDataRequestHandler);
            
            var outMapping = (requestType, requestHandlerType);
            registryMock.Setup(registry => registry.TryGetValue(requestName, out outMapping)).Returns(true);

            var middleware = new RequestsMiddleware(async _ => { }, registryMock.Object, new NullLoggerFactory());
            
            var httpContext  = await requestName.InvokeRequestMiddleware(async ctx =>
            {
                await middleware.InvokeAsync(ctx);
            }, @"{
                ""stringData"": ""string data"",
                ""intData"": 12,
                ""boolData"": true,
                ""objectData"": {""stringProp"": ""prop1"", ""intProp"": 2}
                }");

            httpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            
            var bodyContent = await new StreamReader(httpContext.Response.Body).ReadToEndAsync();
            
            var jsonBody = JsonDocument.Parse(bodyContent).RootElement;
            jsonBody
                .GetProperty("requestId").GetGuid()
                .Should().NotBeEmpty();

            var resultJson = jsonBody
                .GetProperty("result");
            
            resultJson
                .GetProperty("stringData").GetString()
                .Should().Be("string data");
            
            resultJson
                .GetProperty("intData").GetInt32()
                .Should().Be(12);
            
            resultJson
                .GetProperty("boolData").GetBoolean()
                .Should().BeTrue();
            
            resultJson
                .GetProperty("objectData")
                .GetProperty("stringProp").GetString()
                .Should().Be("prop1");
            
            resultJson
                .GetProperty("objectData")
                .GetProperty("intProp").GetInt32()
                .Should().Be(2);
        }
    }
}