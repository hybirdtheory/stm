using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 获取用户Ip
    /// </summary>
    public interface IUserIpAccessor
    {
        String GetIp ();
    }
}
