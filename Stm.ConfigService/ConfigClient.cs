
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using Stm.Core.Domain.Generic;

namespace Stm.ConfigService
{
    public class ConfigClient : IConfigClient
    {

        private static ConcurrentDictionary<string, ConfigInfo> _configs;

        private static List<string> _observeKeys;

        private static object _lockObj;

        private static string _status = "stop";

        public static int REQUEST_INTERVAL = 30000;

        static ConfigClient ()
        {
            _configs = new ConcurrentDictionary<string, ConfigInfo>();
            _observeKeys = new List<string>();
            _lockObj = new object();
            _status = "stop";
        }

        internal static async Task OnTickAsync ( IServiceProvider serviceProvider)
        {
            if (_status == "stop") return;

            var configService= serviceProvider.GetService<IConfigService>();

            string[] observeKeys;

            lock (_lockObj)
            {
                observeKeys = _observeKeys.ToArray();
            }

           
            if (_status == "stop") return;

            var newconfigs = await configService.GetConfigsAsync( observeKeys.ToList() );
            if (newconfigs == null||!newconfigs.Any()) return;

            foreach (var key in observeKeys)
            {
                var newconfig = newconfigs.FirstOrDefault( t => t.Key == key );

                if (newconfig == null) throw new Exception( $"get config {key} fail" );

                newconfig.Key = key;

                ConfigInfo configInfo;

                if (_configs.TryGetValue( key, out configInfo ))
                {
                    if (configInfo.GetHashKey() != newconfig.GetHashKey())
                    {
                        _configs.TryUpdate( key, newconfig, configInfo );
                    }
                }
                else
                {
                    _configs.TryAdd( key, newconfig );
                }

            }
        }

        internal static  void Start ( IServiceProvider serviceProvider )
        {
            if (_status == "stop")
            {
                _status = "starting";
            }

            new Thread( async () =>
            {
                while (_status!= "stop")
                {
                    try
                    {
                        using (var scope = serviceProvider.CreateScope())
                        {
                            await OnTickAsync( scope.ServiceProvider );
                        }
                        if(_status== "starting")
                        {
                            _status = "running";
                        }
                    }
                    catch(Exception e) {
                        var x = e;
                    }

                    Thread.Sleep( REQUEST_INTERVAL );
                }
            } ).Start();

        }

        internal static void Stop ()
        {
            _status = "stop";
        }

        public ConfigClient ()
        {
        }

        public  T GetData<T> ( string key )
        {
            ConfigInfo config=null;
            while (_status != "running")
            {
                if (_status == "stop")
                {
                    throw new Exception( "programa stoped" );
                }

                Thread.Sleep( 100 );
            }
            if (_configs.TryGetValue(key,out config ))
            {
                if (config == null || config.IsDeleted)
                {
                    throw new Exception( $"config key {key} not found" );
                }
                return config.GetValue<T>();
            }

            throw new Exception( $"config key {key} not found" );
        }

        public static void Observe ( string key )
        {
            lock (_lockObj) {
                _observeKeys.Add( key );
            }
        }

        public static void RemoveObserve( string key )
        {
            lock (_lockObj)
            {
                _observeKeys.Remove( key );
            }
        }
    }
}
