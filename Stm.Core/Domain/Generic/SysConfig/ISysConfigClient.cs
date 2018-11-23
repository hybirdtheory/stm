using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core.Domain.Generic
{
    public interface ISysConfigClient
    {
        T GetData<T> ( string key );

    }
}
