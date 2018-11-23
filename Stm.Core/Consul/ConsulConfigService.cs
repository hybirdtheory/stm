using Consul;
using Microsoft.Extensions.Options;
using Stm.Core.Domain.Generic;
using Stm.Core.SoaGovernance;
using Stm.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core.Consul
{
    public class ConsulConfigService : ISysConfigService
    {
        ServiceInfoRegisterConfig _config = null;
        ConsulClient _consulClient = null;
        /// <summary>
        /// 生效延迟 分钟
        /// </summary>
        private static int EFFECTIVEDELAY = 3;

        public ConsulConfigService ( IOptions<ServiceInfoRegisterConfig> consulCfg )
        {
            _config = consulCfg.Value;
            _consulClient = new ConsulClient(x =>x.Address = new Uri( $"http://{_config.ServiceCenterHost}:{_config.ServiceCenterPort}" ) );
        }


        public async Task DeleteAsync ( string key )
        {
            await _consulClient.KV.Delete( key );
        }

        public async Task<SysConfigInfo> GetConfigAsync ( string key )
        {
            var queryResult = await _consulClient.KV.Get(key );


            if(queryResult.StatusCode!= System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            var data = queryResult.Response.Value;

            var str = System.Text.Encoding.UTF8.GetString( data );
            try
            {
                return JsonUtil.ToModel<SysConfigInfo>( str );
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<SysConfigInfo>> GetConfigsAsync ( List<string> keys )
        {
            if (keys == null || !keys.Any()) return new List<SysConfigInfo>();

            List<SysConfigInfo> result = new List<SysConfigInfo>();

            foreach(var key in keys)
            {
                var config = await GetConfigAsync( key );
                if (config != null)
                {
                    result.Add( config );
                }
            }

            return result;
        }

        public async Task PutAsync ( string key, object value )
        {
            SysConfigInfo configInfo = await GetConfigAsync( key );
            if (configInfo == null)
            {
                configInfo = new SysConfigInfo();
                configInfo.Key = key;
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

            KVPair kVPair = new KVPair( key );
            kVPair.Value = System.Text.Encoding.UTF8.GetBytes( JsonUtil.ToJson( configInfo ) );

            await _consulClient.KV.Put( kVPair );
        }

        private string Object2String ( object value )
        {
            if (value == null) return "";
            if (value.GetType() == typeof( string ) ||
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
