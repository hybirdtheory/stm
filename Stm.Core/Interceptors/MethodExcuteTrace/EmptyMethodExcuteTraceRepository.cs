using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Stm.Core.Interceptors
{
    public class EmptyMethodExcuteTraceRepository : IMethodExcuteTraceRepository
    {
        public void SaveMethodExcuteTraceInfo(MethodExcuteTraceInfo traceInfo)
        {

            Debug.WriteLine(JsonConvert.SerializeObject(traceInfo));
        }
    }
}
