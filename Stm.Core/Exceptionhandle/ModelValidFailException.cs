using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 模型验证失败
    /// </summary>
    public class ModelValidFailException:BaseException
    {
        public string Field { get; set; }
        public ModelValidFailException(string field, string message) : base(message)
        {
            Code = StandradErrorCodes.ModelValidFail;
            Field = field;
        }
    }
}
