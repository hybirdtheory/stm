using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Stm.Core.Db;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core.Domain.Generic
{
    /// <summary>
    /// 基于数据库的鉴权服务
    /// </summary>
    public class DbBasedAuthService : IAuthService
    {
        private DbContext _dbContext;

        public DbBasedAuthService ( IDbContextFactory dbContextFactory, RepositoryOptions<DbBasedAuthService> options )
        {
            _dbContext = dbContextFactory.GetDbContext( options.DbName );
        }

        /// <summary>
        /// 申请操作token
        /// </summary>
        /// <param name="resourceDescriptor">欲操作资源描述</param>
        /// <param name="regToken">登记客户端token</param>
        /// <returns></returns>
        public async Task<string> RegisterAsync ( ResourceDescriptor resourceDescriptor, string regToken )
        {
            ResourceGrantInfo resourceGrantInfo = new ResourceGrantInfo
            {
                Token = Guid.NewGuid().ToString( "N" ),
                CreateDt = DateTime.Now,
                ExpireDt = resourceDescriptor.GetExpireDt(),
                ResourceDescriptor = resourceDescriptor.ToString(),
                UseTimes = 0
            };

            _dbContext.Set<ResourceGrantInfo>().Add( resourceGrantInfo );

            await _dbContext.SaveChangesAsync();

            return resourceGrantInfo.Token;
        }

        /// <summary>
        /// 检测token是否可以对资源进行操作
        /// </summary>
        /// <param name="token"></param>
        /// <param name="resourceName">资源名称</param>
        /// <param name="action">操作</param>
        /// <returns></returns>
        public async Task<bool> IsValidAsync ( string token, string resourceName, string action )
        {
            var resourceGrantInfo = await _dbContext.Set<ResourceGrantInfo>().FirstOrDefaultAsync( t => t.Token == token );

            if (resourceGrantInfo == null || resourceGrantInfo.ExpireDt < DateTime.Now) return false;

            var resourceDescriptor = ResourceDescriptor.FromString( resourceGrantInfo.ResourceDescriptor );

            if (resourceDescriptor == null || resourceDescriptor.GetExpireDt() < DateTime.Now) return false;

            return resourceDescriptor.IsValid( resourceName, action );
        }
    }
}
