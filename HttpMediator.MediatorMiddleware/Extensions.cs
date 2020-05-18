using System.Reflection;
using HttpMediator.Infrastructure.Notifications;
using HttpMediator.Infrastructure.Requests;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace HttpMediator.MediatorMiddleware
{
    public static class Extensions
    {
        public static IApplicationBuilder UseHttpMediatorNotifications(this IApplicationBuilder builder)
            => builder.UseMiddleware<NotificationsMiddleware>();
        
        public static IApplicationBuilder UseHttpMediatorRequests(this IApplicationBuilder builder)
            => builder.UseMiddleware<RequestsMiddleware>();



        public static void AddHttpMediator(this IServiceCollection serviceCollection, params Assembly[] assemblies)
        {
            assemblies = assemblies.Length == 0 ? new[] {Assembly.GetCallingAssembly()} : assemblies;

            serviceCollection.AddSingleton<INotificationRegistry>(new NotificationRegistry(assemblies));
            serviceCollection.AddSingleton<IRequestRegistry>(new RequestRegistry(assemblies));
        }
    }
}