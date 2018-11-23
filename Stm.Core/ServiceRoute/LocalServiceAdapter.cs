using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stm.Core
{
    /// <summary>
    /// 本地服务调用器
    /// </summary>
    public class LocalServiceAdapter<T> : ServiceAdapterBase
    {
        private Type _serviceContractType;
        private Func<IServiceProvider,T> _localImplServiceCreater;

        public LocalServiceAdapter(Func<IServiceProvider,T> localImplServiceCreater )
        {
            this._serviceContractType = typeof(T);
            if (!this._serviceContractType.IsInterface)
            {
                throw new Exception( $"type {this._serviceContractType} is not a interface" );
            }
            this._localImplServiceCreater = localImplServiceCreater;
        }

        public override Type ServiceContractType => _serviceContractType;

        public override string Protocol => ServiceCallProtocols.LOCAL;

        public override async Task<object> ExecuteServiceAsync ( IServiceProvider serviceProvider, MethodInfo methodInfo, params object[] parameters )
        {
            var localImplService = _localImplServiceCreater( serviceProvider );

            var result= methodInfo.Invoke( localImplService, parameters );

            return await Task.FromResult(result);
        }
    }



    public class LocalServiceAdapter : ServiceAdapterBase
    {
        private Type _serviceContractType;
        private Func<IServiceProvider, object> _localImplServiceCreater;

        public LocalServiceAdapter (Type typeInterface, Func<IServiceProvider, object> localImplServiceCreater )
        {
            this._serviceContractType = typeInterface;
            if (!this._serviceContractType.IsInterface)
            {
                throw new Exception( $"type {this._serviceContractType} is not a interface" );
            }
            this._localImplServiceCreater = localImplServiceCreater;
        }

        public override Type ServiceContractType => _serviceContractType;

        public override string Protocol => ServiceCallProtocols.LOCAL;

        public override async Task<object> ExecuteServiceAsync ( IServiceProvider serviceProvider, MethodInfo methodInfo, params object[] parameters )
        {
            var localImplService = _localImplServiceCreater( serviceProvider );

            var result = methodInfo.Invoke( localImplService, parameters );

            return await Task.FromResult( result );
        }
    }
}
