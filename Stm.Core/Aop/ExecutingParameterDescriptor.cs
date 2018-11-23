using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Aop
{
    public class ExecutingParameterDescriptor
    {
        public String Name { get; set; }

        public Type ParamterType { get; set; }

        public Object Value { get; set; }
    }
}
