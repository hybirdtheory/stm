using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Stm.IISiteAliveKeeper
{
    public class Program
    {
        public static void Main ()
        {
            HostFactory.Run( x =>                                 //1
            {
                x.Service<Keeper>();
                x.RunAsLocalSystem();                            //6

                x.SetDescription( "IIS 站点保活" );        //7
                x.SetDisplayName( "IISiteAliveKeeper" );                       //8
                x.SetServiceName( "IISiteAliveKeeper" );                       //9
            } );                                                  //10
        }
    }
}
