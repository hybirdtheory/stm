using Stm.Core.Security;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Stm.Core.Security
{
    /// <summary>
    /// 鉴权器
    /// </summary>
    public interface IAuthenticator
    {
        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        StmPrincipal GetCurrentUser ();


    }
}
