using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 用于展示异常信息
    /// </summary>
    public class ExceptionViewInfo:Dictionary<string,object>
    {
        public string Message { get; set; }

        public int ErrorCode { get; set; }

    }
}
