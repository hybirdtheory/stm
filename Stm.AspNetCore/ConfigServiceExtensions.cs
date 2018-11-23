using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Stm.Core.Domain.Generic;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.AspNetCore
{
    public static class ConfigServiceExtensions
    {
        public static IApplicationBuilder UseSysConfigService ( 
            this IApplicationBuilder app, 
            IApplicationLifetime lifetime,List<String> observeKeys =null)
        {
            if (observeKeys != null)
            {
                foreach(var key in observeKeys)
                {
                    SysConfigClient.Observe( key );
                }
            }

            SysConfigClient.Start( app.ApplicationServices );


            lifetime.ApplicationStopping.Register( () =>
            {
                SysConfigClient.Stop();
            } );

            return app;
        }
    }
}
