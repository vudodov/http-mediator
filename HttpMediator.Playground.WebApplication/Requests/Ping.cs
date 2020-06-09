using System;
using System.Threading;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Requests;

namespace HttpMediator.Playground.WebApp.Requests
{
    public class Ping : IRequest { }
    
    public class PingHandler : IRequestHandler<Ping, string>
    {
        public Task<string> HandleAsync(Ping request, Guid requestId, CancellationToken cancellationToken)
        {
            return Task.FromResult("Pong");
        }
    }
}