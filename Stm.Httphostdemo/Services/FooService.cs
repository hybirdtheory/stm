using Stm.Core;
using Stm.Httphostdemo.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stm.Httphostdemo.Services
{
    public class FooService : IFooService
    {
        public int Add ( int a, int b )
        {
            return a + b;
        }

        public void Click ( DateTime time )
        {
            
        }

        public LoginResponse Login ( LoginReqeust reqeust, string channel )
        {
            return new LoginResponse
            {
                IsLoginSuccess = true,
                SessionId = channel+"."+reqeust.UserName+ "." + reqeust.Password+ "." + reqeust.Mchid
            };
        }

        public async Task Notify ()
        {
            return;
        }

        public async Task<int> NumAddAsync ( int a, int b )
        {
            return await Task.FromResult( a + b );
        }

        public RegisterResponse Register ( string tel, string password )
        {
            throw new BaseException( "暂不开放注册", StandradErrorCodes.NormalError );
        }
    }
}
