using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    public class EmptyUserIpAccessor : IUserIpAccessor
    {
        public string GetIp ()
        {
            return "";
        }
    }
}
