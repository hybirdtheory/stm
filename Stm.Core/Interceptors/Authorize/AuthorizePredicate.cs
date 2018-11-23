using Stm.Core.Security;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace Stm.Core.Interceptors
{
    /// <summary>
    /// 权限验证谓词，判断一个用户是否满足条件
    /// </summary>
    /// <param name="user">当前用户</param>
    /// <param name="method">当前执行的方法</param>
    /// <returns></returns>
    public delegate bool AuthorizePredicate( StmPrincipal user,MethodInfo method );

    
}
