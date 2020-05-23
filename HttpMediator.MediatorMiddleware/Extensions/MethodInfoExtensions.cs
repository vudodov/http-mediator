using System;
using System.Reflection;
using System.Threading.Tasks;

namespace HttpMediator.MediatorMiddleware
{
    internal static class MethodInfoExtensions
    {
        internal static async Task<object> InvokeAndReturnAsync(this MethodInfo methodInfo, object obj,
            params object[]? parameters)
        {
            dynamic awaitable = methodInfo.Invoke(obj, parameters) ??
                                throw new NullReferenceException(
                                    $"{methodInfo.DeclaringType?.Name}.{methodInfo.Name} should return Task, but found null");
            await awaitable;
            return awaitable.GetAwaiter().GetResult();
        }

        internal static async Task InvokeAsync(this MethodInfo methodInfo, object obj, params object[]? parameters)
        {
            dynamic awaitable = methodInfo.Invoke(obj, parameters) ??
                                throw new NullReferenceException(
                                    $"{methodInfo.DeclaringType?.Name}.{methodInfo.Name} should return Task, but found null");
            await awaitable;
        }
    }
}