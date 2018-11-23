using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    /// <summary>
    /// 权限不足错误
    /// </summary>
    public class PermissionDeniedException:BaseException
    {
        public PermissionDeniedException(string message) : base(message)
        {
            Code = StandradErrorCodes.PermissionDenied;
        }

        public PermissionDeniedException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public PermissionDeniedException(string message, int code) : base(message,code)
        {
        }

        public PermissionDeniedException(string message, int code, Exception innerException) : base(message, code, innerException)
        {
        }
    }
}
