using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.AspNetCore
{
    public class ClaimsPrincipalCookiePersistorOptions
    {
        /// <summary>
        /// 存储的cookie/session/key 的名称
        /// </summary>
        public string KeyName { get; set; } = "stm_idtt";

        /// <summary>
        /// 加密key
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 过期分钟数
        /// </summary>
        public int ExpireMinutes { get; set; } = 60 * 24;
    }
}
