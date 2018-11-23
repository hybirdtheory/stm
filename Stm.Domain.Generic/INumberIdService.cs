using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Domain.Generic
{
    public interface INumberIdService
    {
        long GetId (string key);
    }
}
