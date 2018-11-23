using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 错误处理输出展示
    /// </summary>
    public class ExceptionExporter: IExceptionExporter
    {
        /// <summary>
        /// 错误转换器
        /// </summary>
        readonly Dictionary<Type, Func<Exception, ExceptionViewInfo>> _exceptionTransform = new Dictionary<Type, Func<Exception, ExceptionViewInfo>>();

        /// <summary>
        /// baseexception 转换器
        /// </summary>
        readonly Dictionary<int, Func<BaseException, ExceptionViewInfo>> _baseexceptionTransform = new Dictionary<int, Func<BaseException, ExceptionViewInfo>>();

        /// <summary>
        /// 未知异常处理器
        /// </summary>
        Func<Exception, ExceptionViewInfo> _unkownExceptionTransform = exp => new ExceptionViewInfo { ErrorCode = StandradErrorCodes.UnkonwError, Message = "服务器异常" };

        public ExceptionExporter(IOptions<ExceptionExporterOptions> options)
        {
            if (options != null)
            {
                foreach(var transform in options.Value.ExceptionTransform)
                {
                    _exceptionTransform.Add(transform.Key,transform.Value);
                }

                foreach(var transform in options.Value.BaseexceptionTransform)
                {
                    _baseexceptionTransform.Add(transform.Key, transform.Value);
                }

                _unkownExceptionTransform = options.Value.UnkownExceptionTransform;
            }
        }


        public ExceptionExporter Register(Type type, Func<Exception, ExceptionViewInfo> transform) 
        {
            _exceptionTransform.Add(type, transform);

            return this;
        }
        public ExceptionExporter RegisterStatusCode(int statuscode,Func<BaseException, ExceptionViewInfo> transform)
        {
            _baseexceptionTransform.Add(statuscode, transform);

            return this;
        }
        public ExceptionExporter RegisterStatusCode(int statuscode)
        {
            _baseexceptionTransform.Add(statuscode, baseexp=>new ExceptionViewInfo { ErrorCode=baseexp.Code, Message=baseexp.Message });

            return this;
        }

        public ExceptionExporter SetUnkownExceptionTransform(Func<Exception, ExceptionViewInfo> transform)
        {
            _unkownExceptionTransform = transform;

            return this;
        }

        public ExceptionViewInfo GetViewInfo(Exception e)
        {

            ///错误转换
            Exception current = e;
            do
            {
                foreach (var transform in _exceptionTransform)
                {
                    if (transform.Key.IsAssignableFrom(current.GetType()))
                    {
                        return transform.Value(current);
                    }
                }
                current = current.InnerException;
            } while (current != null);


            //向下找到BaseException,未找到就返回默认错误消息
            BaseException baseException = null;
            current = e;
            do
            {
                var exp = current as BaseException;
                if (exp == null)
                {
                    current = current.InnerException;
                }
                else
                {
                    baseException = exp;
                    break;
                }
            } while (current != null);

            if (baseException != null)
            {
                if (_baseexceptionTransform.ContainsKey(baseException.Code))
                {
                    var transform = _baseexceptionTransform[baseException.Code];
                    if (transform != null)
                    {
                        return transform(baseException);
                    }
                }
            }

            //如果baseexception的Code==1,认为是可以直接展示给用户的信息
            if(baseException!=null&&baseException.Code== StandradErrorCodes.NormalError)
            {
                return new ExceptionViewInfo { ErrorCode = StandradErrorCodes.NormalError, Message = baseException.Message };
            }

            //未知异常处理
            return _unkownExceptionTransform(e);
        }
    }
}
