using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    public class NameAttribute:Attribute
    {
        public string Value { get; set; }

        public NameAttribute(string value )
        {
            Value = value;
        }
    }
}
