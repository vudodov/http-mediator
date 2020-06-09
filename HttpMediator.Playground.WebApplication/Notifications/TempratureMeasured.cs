using System;
using System.Threading;
using System.Threading.Tasks;
using HttpMediator.Infrastructure.Notifications;
using Microsoft.Extensions.Logging;

namespace HttpMediator.Playground.WebApp.Notifications
{
    public class TemperatureMeasuredInCelsius : INotification
    {
        public float Temperature { get; set; }
    }

    public class UpdateCurrentTemperatureState : INotificationHandler<TemperatureMeasuredInCelsius>
    {
        public Task HandleAsync(TemperatureMeasuredInCelsius notification, Guid notificationId,
            CancellationToken cancellationToken)
        {
            InMemoryDatabase.Database["human_friendly"] =
                GetHumanFriendlyTemperature(notification.Temperature);

            InMemoryDatabase.Database["temperature"] = notification.Temperature;

            return Task.CompletedTask;
        }

        private static string GetHumanFriendlyTemperature(float temperature) =>
            temperature switch
            {
                var t when t < -20 => "Freezing",
                var t when t < -10 => "Bracing",
                var t when t < 0 => "Chilly",
                var t when t < 5 => "Cool",
                var t when t < 10 => "Mild",
                var t when t < 20 => "Warm",
                var t when t < 25 => "Balmy",
                var t when t < 30 => "Hot",
                var t when t < 35 => "Sweltering",
                _ => "Scorching"
            };
    }

    public class LogTemperatureRecord : INotificationHandler<TemperatureMeasuredInCelsius>
    {
        public Task HandleAsync(TemperatureMeasuredInCelsius notification, Guid notificationId,
            CancellationToken cancellationToken)
        {
            InMemoryDatabase.Database["temperature_log"].Add(notification.Temperature);
            return Task.CompletedTask;
        }
    }
}