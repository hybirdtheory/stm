using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Domain.Generic
{
    public interface IConfigClient
    {
        T GetData<T> ( string key );

    }
}
