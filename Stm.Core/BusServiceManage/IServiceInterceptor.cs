using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core
{
    public interface IServiceInterceptor: IInterceptor
    {
        /// <summary>
        /// 排序级
        /// 正序方式
        /// </summary>
        int Order { get; set; }

        //Task Invoke ( AspectContext context, AspectDelegate next );
    }
}
