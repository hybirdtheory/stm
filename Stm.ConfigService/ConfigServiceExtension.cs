using Microsoft.Extensions.DependencyInjection;
using Stm.Core;
using Stm.Core.Db;
using System;
using Stm.Core.Domain.Generic;
using System.Collections.Generic;
using System.Text;
using Stm.Core.Consul;

namespace Stm.ConfigService
{
    public static class ConfigServiceExtension
    {
        /// <summary>
        /// 用于本地直连数据库
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="repositoryOptions"></param>
        public static void AddLocalConfigService( this IServiceCollection serviceCollection,Action<RepositoryOptions<ConfigService>> repositoryOptions )
        {
            serviceCollection.AddScoped<IConfigService, ConfigService>();
            serviceCollection.AddScoped<IConfigClient, ConfigClient>();



            RepositoryOptions<ConfigService> options = new RepositoryOptions<ConfigService>();
            repositoryOptions( options );

            serviceCollection.Add( new ServiceDescriptor( typeof( RepositoryOptions<ConfigService> ), options ) );
        }

        /// <summary>
        /// 用于server
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="repositoryOptions"></param>
        public static void AddConfigServiceServer ( this IServiceCollection serviceCollection, Action<RepositoryOptions<ConfigService>> repositoryOptions )
        {
            serviceCollection.AddScoped<IConfigService, ConfigService>();



            RepositoryOptions<ConfigService> options = new RepositoryOptions<ConfigService>();
            repositoryOptions( options );

            serviceCollection.Add( new ServiceDescriptor( typeof( RepositoryOptions<ConfigService> ), options ) );
        }

        /// <summary>
        /// 用于通过api获取config
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddHttpConfigService ( this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IConfigClient, ConfigClient>();

            serviceCollection.AddHttpService<IConfigService>( "http://{lb:configservice}/{service}/{action}" );

        }

        /// <summary>
        /// 使用Consul配置中心
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddConsulConfigService ( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddScoped<IConfigClient, ConfigClient>();

            serviceCollection.AddSingleton<IConfigService, ConsulConfigService>();

        }
    }
}
