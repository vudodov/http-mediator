[![Nuget](https://img.shields.io/nuget/v/http-mediator)](https://www.nuget.org/packages/http-mediator/)

# HTTP Mediator
Simple, easy-to-hook-up mediator implementation.

The http-mediator allows you to dispatch HTTP requests directly to the handlers.

## Inspiration
Not all tasks fit well in the RESTful API architecture.

Quite often Controllers will be used as an intermediate layer to map the HTTP Request 
into some sort of message-object that will be eventually dispatched by some sort of mediator implementation to the final message handler.

This middleware removes the gap between HTTP Request and handler and dispatches deserialized HTTP messages directly into the handler.

No controllers, no extra libraries required.

## Usage

Plug in this middleware into your project like so

```csharp
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddHttpMediator();
    ...
}

...

public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
{
    ...
    app.UseHttpMediatorRequests();      // To use HTTP Mediator Requests
    app.UseHttpMediatorNotifications(); // To use HTTP Mediator Notifications
    ...
}
```

And that's it, you are ready to go.

### How to shoot your first message?
Add some notifications and requests and they will be automatically available by follwing addresses `http://<host>:<port>/mediator-notification/<notification-name-kebab-case>` and `http://<host>:<port>/mediator-request/<request-name-kebab-case>` correspondigly.
If your message or request has some payload, add it to the HTTP Request Body as you would do for any other HTTP Request.
The expected HTTP Method is `POST`.
A notification name or request name convention is a class name in the kebab case notation.

### What will happen next?
Right after you make an HTTP POST request to one of the addresses mentioned above, the HTTP Mediator Middleware will build a notification or a request message object, and deserialize HTTP Request body payload into the newly created message. After that handler class for the message will be created (concerning all Dependency Injection configuration) and the handler will be executed with the deserialized message.

## Basics

Currently, HTTP Mediator dispatches two types of messages

 - Request/Response messages despatched to a **single** handler and provide a manageable response.
 - Notification dispatched to **multiple** handlers.
 
### Request/Response

Let's create a simple Request `Ping` that should be responded with `"Pong"`.

We will start with the Request class

```csharp
public class Ping : IRequest { }
```

Very straight-forward, no payload. Now let's move to the Request Handler, it will look as follows

```csharp
public class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> HandleAsync(Ping request, Guid requestId, CancellationToken cancellationToken)
    {
        return Task.FromResult("Pong");
    }
}
```
That's it, now to test it out, we need to do the HTTP `POST` request to the `http://<host>:<port>/mediator-request/ping` and we will get the response `HTTP OK/200 '"Pong"'`

Try it yourself and check out [Ping class](HttpMediator.Playground.WebApplication/Requests/Ping.cs) example in the [Playground project](HttpMediator.Playground.WebApplication) and/or run through the whole scenario with [Playground Tests](HttpMediator.Playground.Tests) which will simulate HTTP Request/Response.

### Notification

Now let's try to create a notification with multiple handlers. 

Let's say we are doing some IoT and we want to read a temperature measurement from the device, log it, and update the current temperature indicator.

We'll start the same way as we did with the Request, but now we will create the Notification

```csharp
public class TemperatureMeasuredInCelsius : INotification
{
    public float Temperature { get; set; }
}
```
Let's start with logging temperature. The handler will look as following

```csharp
public class LogTemperatureRecord : INotificationHandler<TemperatureMeasuredInCelsius>
{
    public Task HandleAsync(TemperatureMeasuredInCelsius notification, Guid notificationId,
        CancellationToken cancellationToken)
    {
        InMemoryDatabase.Database["temperature_log"].Add(notification.Temperature);
        return Task.CompletedTask;
    }
}
```
Once a new temperature is received we add it to the `"temperature_log"` list.

The next step is to update the current temperature indication. Let's say we want to have our temperature displayed in a human-friendly way.

```csharp
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
```
Now we are capturing temperature in both numeric and human-friendly ways.
That's the second handler which is handling the same notification as `LogTemperatureRecord` does.

What will happen, is once we will do the HTTP `POST` notification to the `http://<host>:<port>/mediator-notification/temperature-measured-in-celsius` with JSON body payload let's say `{ "temperature": 25 }` it will be automatically deserialized and delivered to both notification handlers and once it will happen, we will get `HTTP OK/200`

Try it yourself and check out [TemperatureMeasured class](HttpMediator.Playground.WebApplication/Notifications/TempratureMeasured.cs) example in the [Playground project](HttpMediator.Playground.WebApplication) and/or actually run through the whole scenario with [Playground Tests](HttpMediator.Playground.Tests) which will simulate HTTP Request/Response.

### Correlation Ids

Both request and notification handlers will receive `Guid requestId` or `Guid notificationId` correspondingly. These ids will be generated by the middleware and passed to the handlers as well as added to the response payload.

