using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Stm.Core.Interceptors
{
    public class MethodExcuteTraceLogger : IMethodExcuteTraceRepository
    {
        private ILogger<MethodExcuteTraceLogger> _logger;

        public MethodExcuteTraceLogger ( ILogger<MethodExcuteTraceLogger> logger )
        {
            _logger = logger;
        }

        public void SaveMethodExcuteTraceInfo(MethodExcuteTraceInfo traceInfo)
        {
            _logger.LogInformation(JsonConvert.SerializeObject(traceInfo));
        }
    }
}
