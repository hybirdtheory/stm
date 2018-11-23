using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    /// <summary>
    /// 存储库配置项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RepositoryOptions<T>
    {
        /// <summary>
        /// 对应的dbcontext名称
        /// </summary>
        public string DbName { get; set; }

        public RepositoryOptions()
        {

        }


    }
}
