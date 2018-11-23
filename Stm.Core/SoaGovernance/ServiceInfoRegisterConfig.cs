using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Stm.Core.SoaGovernance
{
    /// <summary>
    /// 服务注册配置信息
    /// </summary>
    public class ServiceInfoRegisterConfig
    {
        public string ServiceCenterHost { get; set; }
        public int ServiceCenterPort { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        
        public string CheckUrl { get; set; }
        public int CheckInterval { get; set; }

        /// <summary>
        /// 附加信息
        /// 格式*-*,以-分割键值
        /// </summary>
        public string[] Tags { get; set; }

        public string[] Services { get; set; }

        public string GetTagValue(string key )
        {
            if (Tags == null || string.IsNullOrWhiteSpace( key )) return null;

            var kv = Tags.FirstOrDefault( t => t.Split( ':' )[0] == key );

            return kv.Split( ':' )[1];
        }
    }



}
