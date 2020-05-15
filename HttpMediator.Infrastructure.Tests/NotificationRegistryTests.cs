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
            var isMappingFound = _registry.TryGetValue("single-test-notification", out var mapping);
            
            isMappingFound.Should().BeTrue();
            mapping.notificationType.Should().Be(typeof(SingleTestNotification));
            mapping.notificationHandlerTypes.Should().HaveCount(1);
            mapping.notificationHandlerTypes.Single().Should().Be(typeof(SingleTestNotificationHandler));
        }

        [Fact]
        public void When_multiple_notifications_registered_it_should_return()
        {
            var isMappingFound = _registry.TryGetValue("multiple-test-notification", out var mapping);
            
            isMappingFound.Should().BeTrue();
            mapping.notificationType.Should().Be(typeof(MultipleTestNotification));
            mapping.notificationHandlerTypes.Should().HaveCount(2);
            mapping.notificationHandlerTypes.ElementAt(0).Should().Be(typeof(MultipleTestNotificationHandler1));
            mapping.notificationHandlerTypes.ElementAt(1).Should().Be(typeof(MultipleTestNotificationHandler2));
        }

        [Fact]
        public void When_no_notifications_registered_it_should_return()
        {
            var isMappingFound = _registry.TryGetValue("no-test-notification", out var mapping);
            
            isMappingFound.Should().BeFalse();
        }
    }
}