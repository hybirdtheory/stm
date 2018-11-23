using System;
using System.Collections.Generic;
using System.Linq;
using System.DirectoryServices;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using System.Net.Http;

namespace Stm.IISiteAliveKeeper
{
    public class Keeper : ServiceControl, ServiceSuspend, ServiceShutdown
    {
        private System.Timers.Timer _timer;
        public Keeper ()
        {

            _timer = new System.Timers.Timer( 1000 * 60 );

            _timer.Elapsed += ( sender, eventArgs ) =>
            {
                visitReq();
            };

        }

        private void visitReq ()
        {
            List<Task> tasks = new List<Task>();
            DirectoryEntry rootfolder = new DirectoryEntry( "IIS://localhost/W3SVC" );
            foreach (DirectoryEntry child in rootfolder.Children)
            {
                if (child.SchemaClassName == "IIsWebServer")
                {
                    var protocol = "http";
                    var props = new string[3];
                    var sslBinding = child.Properties["SecureBindings"].Value?.ToString();
                    if (!string.IsNullOrWhiteSpace( sslBinding ))
                    {
                        props = sslBinding.Split( ':' );
                        protocol = "https";
                    }
                    else
                    {
                        var httpBindings = child.Properties["ServerBindings"].Value.ToString();
                        props= httpBindings.Split( ':' );
                    }

                    var host = props[2];
                    if (string.IsNullOrWhiteSpace( host ))
                    {
                        host = props[0];
                        if (string.IsNullOrWhiteSpace( host ))
                        {
                            host = "127.0.0.1";
                        }
                    }

                    var url = protocol + "://"+ host+":" + props[1];


                    var reqTask = Task.Factory.StartNew( async () =>
                    {
                        var rsp = await new HttpClient().GetAsync( url );

                        var rspCode = rsp.StatusCode;

                        Console.WriteLine( rspCode+"  "+ url );
                    } );

                    tasks.Add( reqTask );

                }
            }

            Task.WaitAll( tasks.ToArray() );
        }

        public bool Continue ( HostControl hostControl )
        {
            return true;
        }

        public bool Pause ( HostControl hostControl )
        {
            return true;
        }

        public void Shutdown ( HostControl hostControl )
        {
        }

        public bool Start ( HostControl hostControl )
        {
            _timer.Start();
            visitReq();
            return true;
        }

        public bool Stop ( HostControl hostControl )
        {
            _timer.Stop();
            return true;
        }
    }
}
