using Microsoft.EntityFrameworkCore;
using Stm.Core.Db;
using Stm.Core.Utils;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Stm.Core.Domain.Generic;

namespace Stm.Core.Domain.Generic
{
    public class SysConfigService : ISysConfigService
    {
        private DbContext _dbContext;

        /// <summary>
        /// 生效延迟 分钟
        /// </summary>
        private static int EFFECTIVEDELAY = 3;

        public SysConfigService ( IDbContextFactory dbContextFactory, RepositoryOptions<SysConfigService> options )
        {
            _dbContext = dbContextFactory.GetDbContext( options.DbName );
        }

        public async Task DeleteAsync ( string key )
        {

            var configInfo = await _dbContext.FindAsync<SysConfigInfo>( key );

            if (configInfo != null)
            {
                configInfo.IsDeleted = true;
                configInfo.Version += 1;
                configInfo.EffectiveDt = DateTime.Now.AddMinutes( EFFECTIVEDELAY );
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<SysConfigInfo> GetConfigAsync ( string key)
        {
            var configInfo = await _dbContext.FindAsync<SysConfigInfo>( key );

            return configInfo;
        }

        public async Task PutAsync ( string key, object value )
        {
            var configInfo = await _dbContext.FindAsync<SysConfigInfo>( key );

            if (configInfo == null)
            {
                configInfo = new SysConfigInfo { Key = key };
                configInfo.Value = Object2String( value );
                configInfo.LastValue = Object2String( value );
            }
            else
            {
                configInfo.Value = Object2String( value );
                configInfo.LastValue = configInfo.Value;
            }
            configInfo.IsDeleted = false;
            configInfo.Version += 1;
            configInfo.EffectiveDt = DateTime.Now.AddMinutes( EFFECTIVEDELAY );
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<SysConfigInfo>> GetConfigsAsync ( List<string> keys )
        {
            if (keys == null || !keys.Any()) return new List<SysConfigInfo>();

            var configs = await _dbContext.Set<SysConfigInfo>().Where( t => keys.Contains( t.Key ) ).ToListAsync();

            return configs;
        }

        private string Object2String(object value )
        {
            if (value == null) return "";
            if(value.GetType()==typeof(string)||
               value.GetType() == typeof( long ) ||
               value.GetType() == typeof( int ) ||
               value.GetType() == typeof( float ) ||
               value.GetType() == typeof( double ) ||
               value.GetType() == typeof( byte ) ||
               value.GetType() == typeof( DateTime ) ||
               value.GetType() == typeof( Guid ) ||
               value.GetType() == typeof( short ) ||
               value.GetType() == typeof( decimal ))
            {
                return value.ToString();
            }

            return JsonUtil.ToJson( value );
        }

    }
}
