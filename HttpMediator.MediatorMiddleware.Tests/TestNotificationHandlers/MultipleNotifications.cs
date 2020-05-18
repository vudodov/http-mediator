using System;
using System.Threading;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Notifications;

namespace HttpMediator.MediatorMiddleware.Tests.TestNotificationHandlers
{
    public class MultipleNotifications : INotification
    {
        
    }

    public class MultipleNotificationHandler_1 : INotificationHandler<MultipleNotifications>
    {
        public static short ExecutionCounter { get; set; }
        
        public Task HandleAsync(MultipleNotifications notification, Guid notificationBatchId, CancellationToken cancellationToken)
        {
            ExecutionCounter++;
            
            return Task.CompletedTask;
        }
    }
        
    public class MultipleNotificationHandler_2 : INotificationHandler<MultipleNotifications>
    {
        public static short ExecutionCounter { get; set; }
        
        public Task HandleAsync(MultipleNotifications notification, Guid notificationBatchId, CancellationToken cancellationToken)
        {
            ExecutionCounter++;
            
            return Task.CompletedTask;
        }
    }
}