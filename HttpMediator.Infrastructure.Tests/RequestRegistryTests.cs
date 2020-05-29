using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HttpMediator.Infrastructure.Requests;
using Xunit;

namespace HttpMediator.Infrastructure.Tests
{
    public class RequestRegistryTests
    {
        private class TestRequest : IRequest
        {
            
        }
        
        private class TestRequestHandler : IRequestHandler<TestRequest, string>
        {
            public Task<string> HandleAsync(TestRequest request, Guid requestId, CancellationToken cancellationToken) =>
                Task.FromResult("Done");
        }

        private readonly RequestRegistry _registry;
        
        public RequestRegistryTests()
        {
            _registry = new RequestRegistry();
        }

        [Fact]
        public void When_single_request_registered_it_should_return()
        {
            var isMappingFound = _registry.TryGetValue("test-request", out var mapping);
            
            isMappingFound.Should().BeTrue();
            mapping.requestType.Should().Be(typeof(TestRequest));
            mapping.requestTypeHandler.Should().Be(typeof(TestRequestHandler));
        }
    }
}