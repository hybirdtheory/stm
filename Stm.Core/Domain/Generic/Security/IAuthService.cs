using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core.Domain.Generic
{
    public interface IAuthService
    {
        /// <summary>
        /// 授权登记
        /// </summary>
        /// <param name="resourceDescriptor">欲操作资源描述</param>
        /// <param name="regToken">登记客户端token</param>
        /// <returns>令牌</returns>
        Task<string> RegisterAsync ( ResourceDescriptor resourceDescriptor,string regToken );

        /// <summary>
        /// 检测token是否可以对资源进行操作
        /// </summary>
        /// <param name="token"></param>
        /// <param name="resourceName">资源名称</param>
        /// <param name="action">操作</param>
        /// <returns></returns>
        Task<bool> IsValidAsync ( string token,string resourceName, string action );
    }
}
