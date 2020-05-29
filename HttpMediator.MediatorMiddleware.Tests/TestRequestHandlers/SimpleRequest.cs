using System;
using System.Threading;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Requests;

namespace HttpMediator.MediatorMiddleware.Tests.TestRequestHandlers
{
    internal class SimpleRequest : IRequest
    {
        
    }

    internal class SimpleRequestHandler : IRequestHandler<SimpleRequest, string>
    {
        public Task<string> HandleAsync(SimpleRequest request, Guid requestId, CancellationToken cancellationToken) => 
            Task.FromResult("simple request result");
    }
}