using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace HttpMediator.Playground.Tests
{
    public class Requests
        : IClassFixture<WebApplicationFactory<WebApplication.Startup>>
    {
        private readonly HttpClient _client;
        
        public Requests(WebApplicationFactory<WebApplication.Startup> factory)
        {
            _client = factory.CreateClient();
        }
        
        [Fact]
        public async Task Requests_should_return_result()
        {
            var response = await _client.PostAsync("/mediator-request/ping", null);

            var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync()).RootElement;
            
            json.GetProperty("requestId").GetGuid().Should().NotBeEmpty();
            json.GetProperty("result").GetString().Should().Be("Pong");
        }
    }
}