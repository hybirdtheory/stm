using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Interceptors
{
    public interface IMethodExcuteTraceRepository
    {
        void SaveMethodExcuteTraceInfo(MethodExcuteTraceInfo traceInfo);
    }
}
