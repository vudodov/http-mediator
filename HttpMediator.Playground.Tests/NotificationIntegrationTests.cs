using System;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HttpMediator.Playground.Tests
{
    public class NotificationIntegrationTests
        : IClassFixture<WebApplicationFactory<WebApplication.Startup>>
    {
        private readonly HttpClient _client;

        public NotificationIntegrationTests(WebApplicationFactory<WebApplication.Startup> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Notifications_should_be_delivered_to_multiple_handlers()
        {
            var postResponse = await _client.PostAsync(
                "/mediator-notification/temperature-measured-in-celsius",
                new StringContent(@"{""temperature"": 20}",
                    Encoding.UTF8, MediaTypeNames.Application.Json));

            postResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            var postJson = JsonDocument.Parse(await postResponse.Content.ReadAsStringAsync()).RootElement;
            postJson.GetProperty("notificationId").GetGuid().Should().NotBeEmpty();

            var getCurrentTemperature = await _client.GetAsync("currenttemperature");
            var currentTemperature = await getCurrentTemperature.Content.ReadAsStringAsync();
            currentTemperature.Should().Be("Balmy");
            
            var getTemperatureLog = await _client.GetAsync("temperaturelog");
            var temperatureLog = await getTemperatureLog.Content.ReadAsStringAsync();
            temperatureLog.Should().Be("20");

        }
    }
}