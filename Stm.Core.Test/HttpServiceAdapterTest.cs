using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Stm.Core.SoaGovernance;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Stm.Core.Test
{
    [TestClass]
    public class HttpServiceAdapterTest
    {
        [TestMethod]
        public async Task TestExecuteServiceAsync ()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IMicroServiceLocator, MicroServiceLocator>();


            HttpServiceAdapter<IHttpServiceAdapterTestService> adapter = new HttpServiceAdapter<IHttpServiceAdapterTestService>( "http://{lb:fooservice}/{service}/{action}");


            IHttpServiceAdapterTestService obj = adapter.GetProxyObject( new Castle.DynamicProxy.ProxyGenerator() )( services.BuildServiceProvider() ) as IHttpServiceAdapterTestService;

            obj.Click(DateTime.Now);

            await obj.Notify();
            //obj.ShowError("haha");
            //await obj.Run();
            //var result = await obj.RunAddAsync( 1, 2 );
            //Assert.AreEqual( 3, result );

            var result = obj.Add( 10, 20 );
            Assert.AreEqual( 30, result );

            var response = obj.Login( new LoginReqeust { Mchid = 1, UserName = "2", Password = "3" }, "hha" );

            Assert.AreEqual( true, response.IsLoginSuccess );

            var num = await obj.NumAddAsync( 1, 3 );

            Assert.AreEqual( 4, num );

        }



        [TestMethod]
        public async Task TestServiceManagerAsync ()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<IMicroServiceLocator,MicroServiceLocator>();

            services.AddHttpService<IHttpServiceAdapterTestService>( "http://{lb:fooservice}/{service}/{action}");

            var provider = services.BuildServiceProvider();

            IHttpServiceAdapterTestService obj = provider.GetService<IHttpServiceAdapterTestService>();

            obj.Click( DateTime.Now );

            await obj.Notify();

            var result = obj.Add( 10, 20 );
            Assert.AreEqual( 30, result );

            var response = obj.Login( new LoginReqeust { Mchid = 1, UserName = "2", Password = "3" }, "hha" );

            Assert.AreEqual( true, response.IsLoginSuccess );

            var num = await obj.NumAddAsync( 1, 3 );

            Assert.AreEqual( 4, num );

        }
    }


    [Name("Foo")]
    public interface IHttpServiceAdapterTestService
    {
        int Add ( int a, int b );

        void Click ( DateTime time );

        LoginResponse Login ( LoginReqeust reqeust, string channel );

        RegisterResponse Register ( string tel, string password );

        Task Notify ();

        Task<int> NumAddAsync ( int a, int b );
    }

    public class LoginReqeust
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public int Mchid { get; set; }
    }

    public class LoginResponse
    {
        public bool IsLoginSuccess { get; set; }

        public string SessionId { get; set; }
    }
    public class RegisterResponse
    {
        public bool IsRegisterSuccess { get; set; }

        public string SessionId { get; set; }
    }

    public class MicroServiceLocator : IMicroServiceLocator
    {
        public MicroServiceInfo FindService ( string serviceName )
        {
            return new MicroServiceInfo
            {
                Host = "localhost",
                Port = 44383
            };
        }
        public List<MicroServiceInfo> FindServices ( string serviceName )
        {
            throw new NotImplementedException();
        }
    }
}
