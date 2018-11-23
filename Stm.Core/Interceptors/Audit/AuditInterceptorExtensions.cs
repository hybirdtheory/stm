
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Stm.Core.Interceptors;
using Stm.Core.Aop;
using Stm.Core.Interceptors.Audit;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class AuditInterceptorExtensions
    {
        public static IServiceCollection AddAuditInterceptor(
            this IServiceCollection serviceCollection, 
            Action<AuditInterceptorOptions> options)
        {
            //serviceCollection.AddScoped<IInterceptor,AuthorizeInterceptor>();
            serviceCollection.AddTransient<IInterceptor, AuditInterceptor>();
            serviceCollection.Configure<AuditInterceptorOptions>(options);


            return serviceCollection;
        }
    }
}
