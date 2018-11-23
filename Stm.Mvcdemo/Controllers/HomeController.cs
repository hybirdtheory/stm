using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Stm.Core.Domain.Generic;
using Stm.Core.Security;
using Stm.Mvcdemo.Models;
using Microsoft.Extensions.DependencyInjection;
using Stm.Core.Db;

namespace Stm.Mvcdemo.Controllers
{
    public class HomeController : Controller
    {
        OrderShardingDbcontext _shardingDbcontext;

        public HomeController( OrderShardingDbcontext shardingDbcontext )
        {
            _shardingDbcontext = shardingDbcontext;
        }

        public async Task<IActionResult> Index ()
        {

           var orders=  await _shardingDbcontext.PageQueryAsync<OrderInfo>( t => t.OrderId%2==0, 10, "desc", 10 );


            //for(var i = 1; i < 100; i++)
            //{
            //    OrderInfo order = new OrderInfo
            //    {
            //        OrderContent = i.ToString(),
            //        OrderId = i
            //    };

            //    await dbContext.AddAsync( order );
            //}

            //await dbContext.SaveChanges();



            return View();
        }

        [Audit("访问关于我们:index@{index}")]
        public IActionResult About (int index)
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact ()
        {
            StmPrincipal principal = new StmPrincipal();
            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaim( new Claim( Core.Security.ClaimTypes.Permissions, "GetId" ) );
            identity.AddClaim( new Claim( Core.Security.ClaimTypes.Id, "1" ) );
            identity.AddClaim( new Claim( Core.Security.ClaimTypes.Username, "ADMIN" ) );
            principal.AddIdentity( identity );

            var stmPrincipalPersistor =HttpContext.RequestServices.GetService<IStmPrincipalPersistor>();

            stmPrincipalPersistor.SavePrincipal( principal );


            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy ()
        {
            return View();
        }

        [ResponseCache( Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
        public IActionResult Error ()
        {
            return View( new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier } );
        }
    }
}
