using Microsoft.Extensions.DependencyInjection;
using Stm.Core.Security;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.AspNetCore
{
    public static class ClaimsPrincipalCookiePersistorExtensions
    {

        public static IServiceCollection AddClaimsPrincipalCookiePersistor ( 
            this IServiceCollection serviceCollection, Action<ClaimsPrincipalCookiePersistorOptions> options )
        {
            serviceCollection.AddScoped<IStmPrincipalPersistor, ClaimsPrincipalCookiePersistor>();

            serviceCollection.Configure( options );

            return serviceCollection;
        }
    }
}
