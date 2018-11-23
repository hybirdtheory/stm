using Consul;
using Microsoft.Extensions.Options;
using Stm.Core.SoaGovernance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stm.Core.Consul
{

    public class ConsulMicroServiceLocator : IMicroServiceLocator
    {
        ServiceInfoRegisterConfig _config = null;
        ConsulClient _consulClient = null;

        public ConsulMicroServiceLocator ( IOptions<ServiceInfoRegisterConfig> consulCfg )
        {
            _config = consulCfg.Value;
            _consulClient = new ConsulClient( x => x.Address = new Uri( $"http://{_config.ServiceCenterHost}:{_config.ServiceCenterPort}" ) );
        }

        public MicroServiceInfo FindService ( string serviceName )
        {

            var response = _consulClient.Agent.Services().Result.Response;


            var services = response.Values.Where( p => p.Service.Equals( serviceName, StringComparison.OrdinalIgnoreCase ) );

            if (!services.Any()) return null;

            //客户端负载均衡，随机选出一台服务
            Random rand = new Random();
            var index = rand.Next( services.Count() );
            var s = services.ElementAt( index );

            return new MicroServiceInfo
            {
                Name = s.Service,
                Host = s.Address,
                Port = s.Port,
                Id = s.ID,
                Tags = s.Tags.ToList()
            };
        }

        public List<MicroServiceInfo> FindServices ( string serviceName )
        {
            var response = _consulClient.Agent.Services().Result.Response;


            var services = response.Values.Where( p => p.Service.Equals( serviceName, StringComparison.OrdinalIgnoreCase ) );

            if (!services.Any()) return null;


            return services.Select( s => new MicroServiceInfo
            {
                Name = s.Service,
                Host = s.Address,
                Port = s.Port,
                Id = s.ID,
                Tags = s.Tags.ToList()
            } ).ToList();
        }
    }

}
