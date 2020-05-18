using System;
using System.Threading;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Notifications;

namespace HttpMediator.MediatorMiddleware.Tests.TestNotificationHandlers
{
    public class SingleNotification : INotification
    {
        
    }

    public class SingleNotificationHandler : INotificationHandler<SingleNotification>
    {
        public static short ExecutionCounter { get; set; }
        
        public Task HandleAsync(SingleNotification notification, Guid notificationBatchId, CancellationToken cancellationToken)
        {
            ExecutionCounter++;
            return Task.CompletedTask;
        }
    }
}