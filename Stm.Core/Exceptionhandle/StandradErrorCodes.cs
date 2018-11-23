using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 基础错误码集
    /// 0:无错误
    /// 1-99:常见异常
    /// 100-199:微服务相关异常
    /// 
    /// </summary>
    public static class StandradErrorCodes
    {
        /// <summary>
        /// 无错误
        /// </summary>
        public const int NoError = 0;

        /// <summary>
        /// 常规业务异常，可直接展示给最终用户
        /// </summary>
        public const int NormalError = 1;

        /// <summary>
        /// 未知错误
        /// </summary>
        public const int UnkonwError = 99;

        #region  常见异常

        /// <summary>
        /// 权限不足
        /// </summary>
        public const int PermissionDenied = 12;

        /// <summary>
        /// 需要登陆
        /// </summary>
        public const int NeedLogin = 13;

        /// <summary>
        /// 模型验证失败
        /// </summary>
        public const int ModelValidFail = 15;

        /// <summary>
        /// 没找到支持的服务
        /// </summary>
        public const int NoServiceFound = 21;

        /// <summary>
        /// 服务不可达
        /// </summary>
        public const int ServiceUnreachable = 22;

        /// <summary>
        /// 服务出错
        /// </summary>
        public const int ServiceError = 23;

        /// <summary>
        /// 服务超时
        /// </summary>
        public const int ServiceTimeout = 24;

        /// <summary>
        /// 服务结果格式错误
        /// </summary>
        public const int ServiceResultFormatError = 25;
        #endregion

    }
}
