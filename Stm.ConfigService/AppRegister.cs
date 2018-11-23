using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.ConfigService
{
    public static class AppRegister
    {
        public static IApplicationBuilder UseConfigService ( 
            this IApplicationBuilder app, 
            IApplicationLifetime lifetime,List<String> observeKeys =null)
        {
            if (observeKeys != null)
            {
                foreach(var key in observeKeys)
                {
                    ConfigClient.Observe( key );
                }
            }

            ConfigClient.Start( app.ApplicationServices );


            lifetime.ApplicationStopping.Register( () =>
            {
                ConfigClient.Stop();
            } );

            return app;
        }
    }
}
