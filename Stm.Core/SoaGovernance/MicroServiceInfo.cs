using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.SoaGovernance
{
    /// <summary>
    /// 微服务描述信息
    /// </summary>
    public class MicroServiceInfo
    {
        public string Host { get; set; }

        public int Port { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 服务id，应唯一
        /// </summary>
        public string Id { get; set; }

        public List<String> Tags { get; set; }

        /// <summary>
        /// 存活检测轮询地址
        /// </summary>
        public string CheckUrl { get; set; }

        /// <summary>
        /// 存活检测轮询间隔（秒）
        /// </summary>
        public int CheckInterval { get; set; }
    }
}
