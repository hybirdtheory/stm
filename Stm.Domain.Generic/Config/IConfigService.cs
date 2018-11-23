using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stm.Domain.Generic
{
    public interface IConfigService
    {
        Task PutAsync ( string key, object value );

        Task DeleteAsync ( string key );

        Task<ConfigInfo> GetConfigAsync ( string key );

        Task<List<ConfigInfo>> GetConfigsAsync ( List<string> keys );
    }
}
