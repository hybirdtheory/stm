using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    public interface IExceptionExporter
    {
        ExceptionViewInfo GetViewInfo(Exception e);
    }
}
