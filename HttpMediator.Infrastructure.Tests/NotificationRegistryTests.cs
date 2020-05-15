using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using HttpMediator.Infrastructure.Notifications;
using Xunit;

namespace HttpMediator.Infrastructure.Tests
{
    public class NotificationRegistryTests
    {
        private class SingleTestNotification : INotification
        {
        }

        private class MultipleTestNotification : INotification
        {
        }

        private class NoTestNotification : INotification
        {
        }

        private class SingleTestNotificationHandler : INotificationHandler<SingleTestNotification>
        {
            public Task HandleAsync(SingleTestNotification notification, CancellationToken cancellationToken) =>
                Task.CompletedTask;
        }

        private class MultipleTestNotificationHandler1 : INotificationHandler<MultipleTestNotification>
        {
            public Task HandleAsync(MultipleTestNotification notification, CancellationToken cancellationToken) =>
                Task.CompletedTask;
        }
        
        private class MultipleTestNotificationHandler2 : INotificationHandler<MultipleTestNotification>
        {
            public Task HandleAsync(MultipleTestNotification notification, CancellationToken cancellationToken) =>
                Task.CompletedTask;
        }

        private readonly NotificationRegistry _registry;
        
        public NotificationRegistryTests()
        {
            _registry = new NotificationRegistry();
        }
        
        [Fact]
        public void When_single_notification_registered_it_should_return()
        {
            var isMappingFound = _registry.TryGetHandlersMapping("single-test-notification", out var mapping);
            
            isMappingFound.Should().BeTrue();
            mapping.notificationType.Should().Be(typeof(SingleTestNotification));
            mapping.notificationTypeHanlers.Should().HaveCount(1);
            mapping.notificationTypeHanlers.Single().Should().Be(typeof(SingleTestNotificationHandler));
        }

        [Fact]
        public void When_multiple_notifications_registered_it_should_return()
        {
            var isMappingFound = _registry.TryGetHandlersMapping("multiple-test-notification", out var mapping);
            
            isMappingFound.Should().BeTrue();
            mapping.notificationType.Should().Be(typeof(MultipleTestNotification));
            mapping.notificationTypeHanlers.Should().HaveCount(2);
            mapping.notificationTypeHanlers.ElementAt(0).Should().Be(typeof(MultipleTestNotificationHandler1));
            mapping.notificationTypeHanlers.ElementAt(1).Should().Be(typeof(MultipleTestNotificationHandler2));
        }

        [Fact]
        public void When_no_notifications_registered_it_should_return()
        {
            var isMappingFound = _registry.TryGetHandlersMapping("no-test-notification", out var mapping);
            
            isMappingFound.Should().BeFalse();
        }
    }
}