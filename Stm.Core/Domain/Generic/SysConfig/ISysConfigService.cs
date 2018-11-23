using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stm.Core.Domain.Generic
{
    public interface ISysConfigService
    {
        Task PutAsync ( string key, object value );

        Task DeleteAsync ( string key );

        Task<SysConfigInfo> GetConfigAsync ( string key );

        Task<List<SysConfigInfo>> GetConfigsAsync ( List<string> keys );
    }
}
