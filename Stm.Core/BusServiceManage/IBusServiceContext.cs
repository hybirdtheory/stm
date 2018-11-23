
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stm.Core
{
    public interface IBusServiceContext
    {
        string Id { get; }

        IServiceInterceptor[] GetInterceptorsOfMethod ( MethodInfo method );
    }
}
