using System;
using System.Threading;
using System.Threading.Tasks;

namespace HttpMediator.Infrastructure.Requests
{
    public interface IRequestHandler<in TRequest, TRequestResult>
        where TRequest : IRequest
    {
        Task<TRequestResult> HandleAsync(TRequest request, Guid requestId, CancellationToken cancellationToken);
    }
}