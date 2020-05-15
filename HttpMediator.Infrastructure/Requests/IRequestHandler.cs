using System.Threading;
using System.Threading.Tasks;

namespace HttpMediator.Infrastructure.Requests
{
    public interface IRequestHandler<in TRequest, TRequestResult>
        where TRequest : IRequest<TRequestResult>
    {
        Task<TRequestResult> HandleAsync(TRequest request, CancellationToken cancellationToken);
    }
}