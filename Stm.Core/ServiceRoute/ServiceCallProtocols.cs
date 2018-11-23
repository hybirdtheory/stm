using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    public class ServiceCallProtocols
    {
        /// <summary>
        /// 本地调用
        /// </summary>
        public const string LOCAL = "local";

        /// <summary>
        /// http调用
        /// </summary>
        public const string HTTP = "http";

        /// <summary>
        /// https调用
        /// </summary>
        public const string SSL = "ssl";

        /// <summary>
        /// grpc调用
        /// </summary>
        public const string GRPC = "grpc";

    }
}
