using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 异常基础类，所有的业务异常都必须继承此类
    /// </summary>
    public class BaseException: Exception
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; } = StandradErrorCodes.NormalError;

        /// <summary>
        /// 附加信息
        /// </summary>
        public String TagInfo { get; set; }

        public BaseException(string message) : base(message)
        {
        }


        public BaseException(string message,Exception innerException) : base(message, innerException)
        {

        }

        public BaseException(string message, int code) : base(message)
        {
            Code = code;
        }

        public BaseException(string message,int code, Exception innerException) : base(message, innerException)
        {
            Code = code;
        }


        public BaseException Tag(string tag )
        {
            this.TagInfo = tag;
            
            return this;
        }
    }
}
