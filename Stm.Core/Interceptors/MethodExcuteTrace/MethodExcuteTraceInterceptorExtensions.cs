
using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using Stm.Core.Aop;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Interceptors
{
    public static class MethodExcuteTraceInterceptorExtensions
    {
        public static IServiceCollection AddMethodExcuteTraceInterceptor(
            this IServiceCollection serviceCollection, 
            Action<MethodExcuteTraceInterceptorOptions> options)
        {
            //serviceCollection.AddTransient<Castle.DynamicProxy.IInterceptor, MethodExcuteTraceInterceptor>();
            serviceCollection.AddTransient<Aop.IInterceptor, MethodExcuteTraceInterceptor>();
            serviceCollection.Configure<MethodExcuteTraceInterceptorOptions>(options);


            return serviceCollection;
        }
    }
}
