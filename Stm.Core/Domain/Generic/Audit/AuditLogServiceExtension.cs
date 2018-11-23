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
    public static class AuditLogServiceExtension
    {
        /// <summary>
        /// 用于本地直连数据库
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="repositoryOptions"></param>
        public static void AddDbbasedAuditLogService ( this IServiceCollection serviceCollection,Action<RepositoryOptions<DbBasedAuditLogService>> repositoryOptions )
        {
            serviceCollection.AddTransient<IAuditLogService, DbBasedAuditLogService>();


            RepositoryOptions<DbBasedAuditLogService> options = new RepositoryOptions<DbBasedAuditLogService>();
            repositoryOptions( options );

            serviceCollection.Add( new ServiceDescriptor( typeof( RepositoryOptions<DbBasedAuditLogService> ), options ) );
        }

        /// <summary>
        /// 用于server
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="repositoryOptions"></param>
        public static void AddAuditLogServiceServer ( this IServiceCollection serviceCollection, Action<RepositoryOptions<DbBasedAuditLogService>> repositoryOptions )
        {
            serviceCollection.AddTransient<IAuditLogService, DbBasedAuditLogService>();


            RepositoryOptions<DbBasedAuditLogService> options = new RepositoryOptions<DbBasedAuditLogService>();
            repositoryOptions( options );

            serviceCollection.Add( new ServiceDescriptor( typeof( RepositoryOptions<DbBasedAuditLogService> ), options ) );
        }

        /// <summary>
        /// 用于通过api获取config
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void AddAuditLogServiceHttpClient ( this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHttpService<ISysConfigService>( "http://{lb:auditlogservice}/{service}/{action}" );

        }

    }
}
