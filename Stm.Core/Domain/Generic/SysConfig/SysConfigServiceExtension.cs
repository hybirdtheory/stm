using Microsoft.Extensions.DependencyInjection;
using Stm.Core;
using Stm.Core.Db;
using System;
using Stm.Core.Domain.Generic;
using System.Collections.Generic;
using System.Text;
using Stm.Core.Consul;

namespace Stm.Core.Domain.Generic
{
    public static class SysConfigServiceExtension
    {
        /// <summary>
        /// 用于本地直连数据库
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="repositoryOptions"></param>
        public static void AddDbbasedConfigService( this IServiceCollection serviceCollection,Action<RepositoryOptions<SysConfigService>> repositoryOptions )
        {
            serviceCollection.AddScoped<ISysConfigService, SysConfigService>();
            serviceCollection.AddScoped<ISysConfigClient, SysConfigClient>();



            RepositoryOptions<SysConfigService> options = new RepositoryOptions<SysConfigService>();
            repositoryOptions( options );

            serviceCollection.Add( new ServiceDescriptor( typeof( RepositoryOptions<SysConfigService> ), options ) );
        }

        /// <summary>
        /// 用于server
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="repositoryOptions"></param>
        public static void AddSysConfigServiceServer ( this IServiceCollection serviceCollection, Action<RepositoryOptions<SysConfigService>> repositoryOptions )
        {
            serviceCollection.AddScoped<ISysConfigService, SysConfigService>();



            RepositoryOptions<SysConfigService> options = new RepositoryOptions<SysConfigService>();
            repositoryOptions( options );

            serviceCollection.Add( new ServiceDescriptor( typeof( RepositoryOptions<SysConfigService> ), options ) );
        }

        /// <summary>
        /// 用于通过api获取config
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddSysConfigServiceHttpClient ( this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ISysConfigClient, SysConfigClient>();

            serviceCollection.AddHttpService<ISysConfigService>( "http://{lb:sysconfigservice}/{service}/{action}" );

        }

        /// <summary>
        /// 使用Consul配置中心
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddConsulSysConfigService ( this IServiceCollection serviceCollection )
        {
            serviceCollection.AddScoped<ISysConfigClient, SysConfigClient>();

            serviceCollection.AddSingleton<ISysConfigService, ConsulConfigService>();

        }
    }
}
