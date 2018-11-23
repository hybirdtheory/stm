using Stm.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    public interface INumberIdService
    {
        long GetId (string key);
    }
}
