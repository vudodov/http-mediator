using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using HttpMediator.Infrastructure.Notifications;
using HttpMediator.MediatorMiddleware.Tests.TestNotificationHandlers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

#pragma warning disable 1998

namespace HttpMediator.MediatorMiddleware.Tests
{
    public class When_handling_notifications
    {
        [Fact]
        public async Task It_should_handle_single_notification()
        {
            const string notificationName = "single-notification";
            var registryMock = new Mock<INotificationRegistry>();
            var notificationType = typeof(SingleNotification);
            var notificationHandlerTypes = new List<Type> {typeof(SingleNotificationHandler)}.AsEnumerable();
            var outMapping = (notificationType, notificationHandlerTypes);
            registryMock.Setup(registry => registry.TryGetValue(notificationName, out outMapping)).Returns(true);
            
            var middleware = new NotificationsMiddleware(async _ => { }, registryMock.Object, Mock.Of<ILoggerFactory>());
            
            var httpContext  = await notificationName.ConstructContextAndInvoke(async ctx =>
            {
                SingleNotificationHandler.ExecutionCounter = 0;
                await middleware.InvokeAsync(ctx);
            });

            httpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            SingleNotificationHandler.ExecutionCounter.Should().Be(1);
        }
        
        [Fact]
        public async Task It_should_handle_multiple_notifications()
        {
            const string notificationName = "multiple-notifications";
            var registryMock = new Mock<INotificationRegistry>();
            var notificationType = typeof(MultipleNotifications);
            var notificationHandlerTypes = new List<Type>
            {
                typeof(MultipleNotificationHandler_1), 
                typeof(MultipleNotificationHandler_2)
            }.AsEnumerable();
            var outMapping = (notificationType, notificationHandlerTypes);
            registryMock.Setup(registry => registry.TryGetValue(notificationName, out outMapping)).Returns(true);
            
            var middleware = new NotificationsMiddleware(async _ => { }, registryMock.Object, Mock.Of<ILoggerFactory>());
            
            var httpContext  = await notificationName.ConstructContextAndInvoke(async ctx =>
            {
                MultipleNotificationHandler_1.ExecutionCounter = 0;
                MultipleNotificationHandler_2.ExecutionCounter = 0;
                await middleware.InvokeAsync(ctx);
            });

            httpContext.Response.StatusCode.Should().Be((int) HttpStatusCode.OK);
            MultipleNotificationHandler_1.ExecutionCounter.Should().Be(1);
            MultipleNotificationHandler_2.ExecutionCounter.Should().Be(1);
        }
        
        [Fact]
        public async Task It_should_pass_notification_data_to_the_handler()
        {
            
        }
        
        [Fact]
        public async Task It_should_not_fail_when_notification_does_not_have_any_data()
        {
            
        }
    }
}