using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Utils
{
    public static class ExceptionExtensions
    {
        public static bool IsExceptionType<TException> (this Exception exception )
        {
            var type = typeof( TException );

            var exp = exception;

            while (exp != null)
            {
                if (type == exp.GetType() || type.IsAssignableFrom( exp.GetType() ))
                {
                    return true;
                }
                exp = exp.InnerException;
            }

            return false;
        }

        public static TException GetExceptionOfType<TException> ( this Exception exception ) where TException : class
        {
            var type = typeof( TException );

            var exp = exception;

            while (exp != null)
            {
                if (type == exp.GetType() || type.IsAssignableFrom( exp.GetType() ))
                {
                    return exp as TException;
                }
                exp = exp.InnerException;
            }

            return null;
        }
    }
}
