using System;
using System.Threading;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Requests;

namespace HttpMediator.MediatorMiddleware.Tests.TestRequestHandlers
{
    internal class SimpleDataRequest : IRequest
    {
        public string Data { get; set; }
    }

    internal class SimpleDataRequestHandler : IRequestHandler<SimpleDataRequest, string>
    {
        public Task<string> HandleAsync(SimpleDataRequest request, Guid requestId, CancellationToken cancellationToken) => 
            Task.FromResult(request.Data);
    }
}