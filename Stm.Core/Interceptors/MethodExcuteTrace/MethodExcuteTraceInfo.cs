using System;

namespace Stm.Core.Interceptors
{
    /// <summary>
    /// 方法调用跟踪信息
    /// </summary>
    public class MethodExcuteTraceInfo
    {
        public string InterfaceName { get; set; }

        public string ServiceName { get; set; }

        public string MethodName { get; set; }

        public DateTime CallDt { get; set; }

        public DateTime CompleteDt { get; set; }

        public bool IsError { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorStackTrace { get; set; }

        public string ParameterInfo { get; set; }

        public string ResultData { get; set; }
    }
}
