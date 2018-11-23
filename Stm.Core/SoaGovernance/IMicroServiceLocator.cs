using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.SoaGovernance
{
    /// <summary>
    /// 微服务查找器
    /// </summary>
    public interface IMicroServiceLocator
    {
        /// <summary>
        /// 查找微服务
        /// </summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        MicroServiceInfo FindService ( string serviceName );


        List<MicroServiceInfo> FindServices ( string serviceName );
    }
}
