using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    public static class ExceptionExporterExtensions
    {
        public static IServiceCollection AddExceptionExporter(this IServiceCollection serviceCollection, Action<ExceptionExporterOptions> options)
        {
            serviceCollection.AddSingleton<IExceptionExporter, ExceptionExporter>();

            serviceCollection.Configure(options);

            return serviceCollection;
        }
    }
}
