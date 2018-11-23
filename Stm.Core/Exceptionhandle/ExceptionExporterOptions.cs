using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    public class ExceptionExporterOptions
    {
        internal Dictionary<Type, Func<Exception, ExceptionViewInfo>> ExceptionTransform = new Dictionary<Type, Func<Exception, ExceptionViewInfo>>();

        /// <summary>
        /// baseexception 转换器
        /// </summary>
        internal Dictionary<int, Func<BaseException, ExceptionViewInfo>> BaseexceptionTransform = new Dictionary<int, Func<BaseException, ExceptionViewInfo>>();

        /// <summary>
        /// 未知异常处理器
        /// </summary>
        internal Func<Exception, ExceptionViewInfo> UnkownExceptionTransform = exp => new ExceptionViewInfo { ErrorCode = StandradErrorCodes.UnkonwError, Message = "服务器异常" };

        public ExceptionExporterOptions Register(Type type, Func<Exception, ExceptionViewInfo> transform)
        {
            ExceptionTransform.Add(type, transform);

            return this;
        }
        public ExceptionExporterOptions RegisterStatusCode(int statuscode, Func<BaseException, ExceptionViewInfo> transform)
        {
            BaseexceptionTransform.Add(statuscode, transform);

            return this;
        }
        public ExceptionExporterOptions RegisterStatusCode(int statuscode)
        {
            BaseexceptionTransform.Add(statuscode, baseexp => new ExceptionViewInfo { ErrorCode = baseexp.Code, Message = baseexp.Message });

            return this;
        }

        public ExceptionExporterOptions SetUnkownExceptionTransform(Func<Exception, ExceptionViewInfo> transform)
        {
            UnkownExceptionTransform = transform;

            return this;
        }
    }
}
