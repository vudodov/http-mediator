using System;
using System.Threading;
using System.Threading.Tasks;

namespace HttpMediator.Infrastructure.Notifications
{
    public interface INotificationHandler<in TNotification>
        where TNotification : INotification
    {
        Task HandleAsync(TNotification notification, Guid notificationId, CancellationToken cancellationToken);
    }
}