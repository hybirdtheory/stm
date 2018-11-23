
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Stm.Core.Interceptors;
using Stm.Core.Aop;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuthorizeInterceptorExtensions
    {
        public static IServiceCollection AddAuthorizeInterceptor(
            this IServiceCollection serviceCollection, 
            Action<AuthorizeInterceptorOptions> options)
        {
            //serviceCollection.AddScoped<IInterceptor,AuthorizeInterceptor>();
            serviceCollection.AddScoped<IInterceptor, AuthorizeInterceptor>();
            serviceCollection.Configure<AuthorizeInterceptorOptions>(options);


            return serviceCollection;
        }
    }
}
