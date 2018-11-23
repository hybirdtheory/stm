using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    public static class ServiceAdapterExtensions
    {

        private static Castle.DynamicProxy.ProxyGenerator proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();

        public static void AddLocalService<TInterface,TImpl>(
            this IServiceCollection serviceCollection, 
            ServiceLifetime serviceLifetime )
            where TImpl: class,TInterface
        {

            if(serviceLifetime== ServiceLifetime.Scoped)
            {
                serviceCollection.AddScoped<TImpl>();
            }
            else if(serviceLifetime== ServiceLifetime.Singleton)
            {
                serviceCollection.AddSingleton<TImpl>();
            }
            else
            {
                serviceCollection.AddTransient<TImpl>();
            }

            ServiceDescriptor serviceDescriptor = new ServiceDescriptor(
                   typeof( TInterface ),
                   srvProvider =>
                   new LocalServiceAdapter<TInterface>( serviceProvider => serviceProvider.GetService<TImpl>() ).GetProxyObject( proxyGenerator )(srvProvider),
                   ServiceLifetime.Scoped );

            serviceCollection.Add( serviceDescriptor );
        }

        public static void AddHttpService<TInterface> ( this IServiceCollection serviceCollection, string url )
        {

            ServiceDescriptor serviceDescriptor = new ServiceDescriptor(
                   typeof( TInterface ),
                   srvProvider =>
                   new HttpServiceAdapter<TInterface>( url ).GetProxyObject( proxyGenerator )( srvProvider ),
                   ServiceLifetime.Scoped );

            serviceCollection.Add( serviceDescriptor );
        }
    }
}
