using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stm.Core.Aop
{
    public class ExecutingMethodDescriptor
    {
        public MethodInfo Method { get; set; }

        public List<ExecutingParameterDescriptor> Parameters { get; set; }

        public ExecutingMethodDescriptor ()
        {
            Parameters = new List<ExecutingParameterDescriptor>();
        }
    }
}
