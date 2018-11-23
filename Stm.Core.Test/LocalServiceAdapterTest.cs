using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Stm.Core.Test
{
    [TestClass]
    public class LocalServiceAdapterTest
    {
        [TestMethod]
        public async Task TestExecuteServiceAsync ()
        {
            LocalServiceAdapter<ILocalServiceAdapterTestService> adapter = new LocalServiceAdapter<ILocalServiceAdapterTestService>( serviceProvider => new LocalServiceAdapterTestService() );


            ILocalServiceAdapterTestService obj = adapter.GetProxyObject( new Castle.DynamicProxy.ProxyGenerator() )(null) as ILocalServiceAdapterTestService;

            obj.ShowInfo();
            obj.ShowError("haha");
            await obj.Run();
            var result = await obj.RunAddAsync( 1, 2 );
            Assert.AreEqual( 3, result );

            result = obj.Add( 10, 20 );
            Assert.AreEqual( 30, result );

        }

        [TestMethod]
        public async Task TestServiceManagerAsync ()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddLocalService<ILocalServiceAdapterTestService, LocalServiceAdapterTestService>( ServiceLifetime.Scoped );

            var provider= services.BuildServiceProvider();

            ILocalServiceAdapterTestService obj = provider.GetService<ILocalServiceAdapterTestService>();

            obj.ShowInfo();
            obj.ShowError( "haha" );
            await obj.Run();
            var result = await obj.RunAddAsync( 1, 2 );
            Assert.AreEqual( 3, result );

            result = obj.Add( 10, 20 );
            Assert.AreEqual( 30, result );

        }
    }


    public interface ILocalServiceAdapterTestService
    {
        void ShowInfo ();

        void ShowError ( string erroinfo );

        int Add ( int a, int b );

        Task Run ();

        Task<int> RunAddAsync ( int a, int b );
    }

    public class LocalServiceAdapterTestService : ILocalServiceAdapterTestService
    {
        public int Add ( int a, int b )
        {
            return a + b;
        }

        public Task Run ()
        {
            Console.WriteLine( "run info" );

            return Task.CompletedTask;
        }

        public async Task<int> RunAddAsync ( int a, int b )
        {
            return await Task.Run( () => a + b );
        }

        public void ShowError ( string erroinfo )
        {
            Console.WriteLine( "show "+ erroinfo );
        }

        public void ShowInfo ()
        {
            Console.WriteLine( "show info" );
        }
    }
}
