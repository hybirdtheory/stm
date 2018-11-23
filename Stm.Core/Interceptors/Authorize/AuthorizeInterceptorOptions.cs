using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Interceptors
{
    /// <summary>
    /// 鉴权拦截器配置参数
    /// </summary>
    public class AuthorizeInterceptorOptions
    {
        /// <summary>
        /// 拦截器优先级
        /// </summary>
        //public int Order { get; private set; }

        /// <summary>
        /// 验证项
        /// </summary>
        public List<AuthorizeItem> Items { get; private set; }

        public AuthorizeInterceptorOptions()
        {
            Items = new List<AuthorizeItem>();
        }


        //public AuthorizeInterceptorOptions SetOrder(int order)
        //{
        //    Order = order;

        //    return this;
        //}

        /// <summary>
        /// 添加验证谓词
        /// </summary>
        /// <param name="predicates"></param>
        /// <returns></returns>
        public AuthorizeInterceptorOptions AddAuthorizeItem(params AuthorizeItem[] authorizes)
        {
            Items.AddRange(authorizes);

            return this;
        }
    }
}
