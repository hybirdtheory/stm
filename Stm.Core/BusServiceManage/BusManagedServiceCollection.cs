using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 业务服务注册器
    /// </summary>
    public class BusManagedServiceCollection
    {
        private static Castle.DynamicProxy.ProxyGenerator proxyGenerator = new Castle.DynamicProxy.ProxyGenerator();

        private IServiceCollection _services;

        public BusManagedServiceCollection () {
            _services = new ServiceCollection();
        }

        public void RegisterTo( IServiceCollection services )
        {
            foreach(var service in _services)
            {
                services.Add( service );
            }
        }


        public BusManagedServiceCollection AddLocalTransient ( Type serviceType, Type implementationType )
        {
            _services.AddTransient( implementationType );

            ServiceDescriptor serviceDescriptor = new ServiceDescriptor(
                  serviceType,
                  srvProvider =>
                    new LocalServiceAdapter( serviceType, serviceProvider => serviceProvider.GetService( implementationType ) )
                        .GetProxyObject( proxyGenerator )( srvProvider ),
                  ServiceLifetime.Transient );

            _services.Add( serviceDescriptor );

            return this;
        }

        public BusManagedServiceCollection AddLocalTransient<TService, TImplementation> ()
            where TService : class
            where TImplementation : class, TService
        {
            return AddLocalTransient(typeof(TService),typeof( TImplementation ) );
        }


        public BusManagedServiceCollection AddLocalScoped ( Type serviceType, Type implementationType )
        {
            _services.AddScoped( implementationType );

            ServiceDescriptor serviceDescriptor = new ServiceDescriptor(
                  serviceType,
                  srvProvider =>
                    new LocalServiceAdapter( serviceType, serviceProvider => serviceProvider.GetService( implementationType ) )
                        .GetProxyObject( proxyGenerator )( srvProvider ),
                  ServiceLifetime.Scoped );

            _services.Add( serviceDescriptor );

            return this;
        }
        public BusManagedServiceCollection AddLocalScoped<TService, TImplementation> ()
            where TService : class
            where TImplementation : class, TService
        {
            return AddLocalScoped( typeof( TService ), typeof( TImplementation ) );
        }


        public BusManagedServiceCollection AddLocalSingleton ( Type serviceType, Type implementationType )
        {
            _services.AddSingleton( implementationType );

            ServiceDescriptor serviceDescriptor = new ServiceDescriptor(
                  serviceType,
                  srvProvider =>
                    new LocalServiceAdapter( serviceType, serviceProvider => serviceProvider.GetService( implementationType ) )
                        .GetProxyObject( proxyGenerator )( srvProvider ),
                  ServiceLifetime.Singleton );

            _services.Add( serviceDescriptor );

            return this;
        }
        public BusManagedServiceCollection AddLocalSingleton<TService, TImplementation> ()
            where TService : class
            where TImplementation : class, TService
        {
            return AddLocalSingleton( typeof( TService ), typeof( TImplementation ) );
        }

    }
}
