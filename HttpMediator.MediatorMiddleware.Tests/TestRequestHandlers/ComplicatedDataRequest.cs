using System;
using System.Threading;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Requests;

namespace HttpMediator.MediatorMiddleware.Tests.TestRequestHandlers
{
    internal class ComplicatedDataRequest : IRequest
    {
        public string StringData { get; set; }
        public int IntData { get; set; }
        public bool BoolData { get; set; }
        public ComplicatedDataObject ObjectData { get; set; }
    }

    public class ComplicatedDataObject
    {
        public int IntProp { get; set; }
        public string StringProp { get; set; }
    }
    
    public class ComplicatedDataResponse
    {
        public string StringData { get; set; }
        public int IntData { get; set; }
        public bool BoolData { get; set; }
        public ComplicatedDataObject ObjectData { get; set; }
    }
    
    internal class ComplicatedDataRequestHandler : IRequestHandler<ComplicatedDataRequest, ComplicatedDataResponse>
    {
        public Task<ComplicatedDataResponse> HandleAsync(ComplicatedDataRequest request, Guid requestId, CancellationToken cancellationToken) => 
            Task.FromResult(new ComplicatedDataResponse
            {
                StringData = request.StringData,
                IntData = request.IntData,
                BoolData = request.BoolData,
                ObjectData = request.ObjectData
            });
    }
}