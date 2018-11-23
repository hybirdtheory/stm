using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stm.Httphostdemo.Contracts
{
    public interface IFooService
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
}
