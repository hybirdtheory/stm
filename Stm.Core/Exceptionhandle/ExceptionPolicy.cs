using Stm.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace Stm.Core
{
    /// <summary>
    /// 异常处理策略,无环境对象，无返回对象
    /// </summary>
    public class ExceptionPolicy:ExceptionPolicy<VOID, VOID>
    {

    }

    /// <summary>
    /// 异常处理策略
    /// </summary>
    /// <typeparam name="TEnvironment">环境对象类型</typeparam>
    /// <typeparam name="TResult">返回对象类型</typeparam>
    public class ExceptionPolicy<TEnvironment, TResult>
    {
        private List<ExceptionPolicyItem<TEnvironment, TResult>> _items;

        public ExceptionPolicy ()
        {
            _items = new List<ExceptionPolicyItem<TEnvironment, TResult>>();
        }

        public ExceptionPolicy<TEnvironment, TResult> Handle ( Func<ExceptionContext<TEnvironment, TResult>, bool> criteria, Action<ExceptionContext<TEnvironment, TResult>> handle, bool breakout = true )
        {
            _items.Add( new ExceptionPolicyItem<TEnvironment, TResult>
            {
                Criteria = criteria,
                Handle = handle,
                BreakOut=breakout
            } );

            return this;
        }

        /// <summary>
        /// 当发生TException类型异常时，执行handle
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout">是否跳出执行链</param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleException<TException> ( Action<ExceptionContext<TEnvironment, TResult>> handle, bool breakout = true ) where TException : Exception
        {
            return Handle(
                ctx => ctx.Exception.IsExceptionType<TException>(),
                ctx => handle( ctx ),
                breakout
            );
        }

        /// <summary>
        /// 当发生TException类型异常时，执行handle，并以handle的执行结果作为替换的返回结果
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="handle"></param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleException<TException> ( Func<TException, TResult> handle, bool breakout = true ) where TException : Exception
        {
            return HandleException<TException>( 
                    ctx => ctx.Result = handle( ctx.Exception.GetExceptionOfType<TException>()), 
                    breakout );
        }

        /// <summary>
        /// 当发生TException类型异常时，执行handle
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="output">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleException<TException> ( Action<TException> handle, bool breakout = true ) where TException : Exception
        {
            return HandleException<TException>( exp => { handle( exp ); return default( TResult ); }, breakout );
        }

        /// <summary>
        /// 当发生特定异常码BaseException时，执行handle
        /// </summary>
        /// <param name="code">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleBaseException ( int code, Action<ExceptionContext<TEnvironment, TResult>> handle, bool breakout = true )
        {
            return Handle(
                ctx => ctx.Exception.IsExceptionType<BaseException>() && ctx.Exception.GetExceptionOfType<BaseException>().Code == code,
                ctx => handle( ctx ),
                breakout
            );
        }

        /// <summary>
        /// 当发生特定异常码BaseException时，执行handle，并以handle的执行结果作为替换的返回结果
        /// </summary>
        /// <param name="code">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleBaseException (int code, Func<BaseException, TResult> handle, bool breakout = true )
        {
            return HandleBaseException( 
                code, 
                ctx =>ctx.Result = handle( ctx.Exception.GetExceptionOfType<BaseException>()), 
                breakout );
        }

        /// <summary>
        /// 当发生TException类型异常时，执行handle
        /// </summary>
        /// <param name="code">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleBaseException ( int code, Action<BaseException> handle, bool breakout = true )
        {
            return HandleBaseException( code, exp => { handle( exp ); return default( TResult ); }, breakout );
        }


        /// <summary>
        /// 当发生特定异常码BaseException时，执行handle
        /// </summary>
        /// <param name="codes">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleBaseException ( int[] codes, Action<ExceptionContext<TEnvironment, TResult>> handle, bool breakout = true )
        {
            return Handle(
                ctx => ctx.Exception.IsExceptionType<BaseException>() && (codes ?? new int[] { }).Contains( ctx.Exception.GetExceptionOfType<BaseException>().Code ),
                ctx => handle( ctx ),
                breakout
            );
        }


        /// <summary>
        /// 当发生特定异常码BaseException时，执行handle，并以handle的执行结果作为替换的返回结果
        /// </summary>
        /// <param name="codes">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleBaseException ( int[] codes, Func<BaseException, TResult> handle, bool breakout = true )
        {
            return HandleBaseException(
                codes,
                ctx => ctx.Result = handle( ctx.Exception.GetExceptionOfType<BaseException>() ),
                breakout );
        }


        /// <summary>
        /// 当发生TException类型异常时，执行handle
        /// </summary>
        /// <param name="codes">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> HandleBaseException ( int[] codes, Action<BaseException> handle, bool breakout = true )
        {
            return HandleBaseException( codes, exp => { handle( exp ); return default( TResult ); }, breakout );
        }

        /// <summary>
        /// 把特定异常转换成BaseException
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="transfor"></param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> TransException<TException>(Func<TException,BaseException> transfor, bool breakout = false ) where TException : Exception
        {
            return HandleException<TException>( ctx => ctx.Exception = transfor( ctx.Exception.GetExceptionOfType<TException>()), breakout );
        }

        /// <summary>
        /// 把特定异常转换成BaseException
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="transToExceptionCode">转换到的目标异常码</param>
        /// <param name="transToExceptionMessage">目标异常信息，{0}表示原始错误信息</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> TransException<TException> ( int transToExceptionCode, string transToExceptionMessage = "{0}", bool breakout = false ) where TException : Exception
        {
            return HandleException<TException>( 
                ctx => ctx.Exception = new BaseException( string.Format( transToExceptionMessage, ctx.Exception.Message ), transToExceptionCode, ctx.Exception ), 
                breakout );
        }

        /// <summary>
        ///  当发生特定异常码BaseException时，转换成另一个异常码
        /// </summary>
        /// <param name="originalExceptionCode">原始异常码</param>
        /// <param name="transToExceptionCode">转换到的目标异常码</param>
        /// <param name="transToExceptionMessage">目标异常信息，{0}表示原始错误信息</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicy<TEnvironment, TResult> TransBaseExceptionCode ( int originalExceptionCode,int transToExceptionCode,string transToExceptionMessage="{0}", bool breakout = false )
        {
            return HandleBaseException(
                    originalExceptionCode,
                    ctx => ctx.Exception = new BaseException( string.Format( transToExceptionMessage, ctx.Exception.Message ), transToExceptionCode, ctx.Exception ),
                    breakout
                );
        }

        public TResult Execute (Func<TResult> func,TEnvironment environment )
        {
            ExceptionContext<TEnvironment, TResult> context = new ExceptionContext<TEnvironment, TResult>
            {
                ExceteFunc = func,
                Environment= environment
            };

            try
            {
                return  func();
            }catch(Exception e)
            {
                context.OriginalException = e;
                context.Exception = e;

                foreach (var item in _items)
                {
                    if (item.Criteria( context))
                    {
                        item.Handle( context );

                        if (item.BreakOut) break;
                    }
                }

                if (context.ExceptionIsHandled)
                {
                    return context.Result;
                }
                throw context.Exception;
            }
        }

        public void Execute(Action action, TEnvironment environment )
        {
             Execute( () => { action(); return default( TResult ); }, environment );
        }

        public ExceptionPolicy<TEnvironment, TResult> Import ( ExceptionPolicyItemBundle<TEnvironment, TResult> bundle )
        {
            if (bundle == null) return this;

            _items.AddRange( bundle.PolicyItems );

            return this;
        }
    }

    /// <summary>
    /// 异常策略项
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ExceptionPolicyItem<TEnvironment, TResult>
    {
        /// <summary>
        /// 匹配条件
        /// </summary>
        public Func<ExceptionContext<TEnvironment, TResult>, bool> Criteria { get; set; }

        /// <summary>
        /// 转换方式
        /// </summary>
        public Action<ExceptionContext<TEnvironment, TResult>> Handle { get; set; }

        /// <summary>
        /// 跳出执行链
        /// </summary>
        public bool BreakOut { get; set; } = true;
    }


    /// <summary>
    /// 异常上下文
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ExceptionContext<TEnvironment,TResult>
    {
        /// <summary>
        /// 正在执行的程序方法
        /// </summary>
        public Func<TResult> ExceteFunc { get; set; }

        /// <summary>
        /// 异常信息
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 原始错误信息
        /// </summary>
        public Exception OriginalException { get; set; }


        private TResult result;

        /// <summary>
        /// 返回结果
        /// </summary>
        public TResult Result {
            get { return result; }
            set { result = value; ExceptionIsHandled = true; }
        }

        /// <summary>
        /// 环境对象
        /// </summary>
        public TEnvironment Environment { get; set; }

        /// <summary>
        /// 错误是否已处理
        /// </summary>
        public bool ExceptionIsHandled { get; set; }
    }

    public class ExceptionContext : ExceptionContext<VOID,VOID> { }

    /// <summary>
    /// 异常策略集合
    /// </summary>
    /// <typeparam name="TEnvironment">环境对象类型</typeparam>
    /// <typeparam name="TResult">返回对象类型</typeparam>
    public class ExceptionPolicyItemBundle<TEnvironment, TResult>
    {
        public List<ExceptionPolicyItem<TEnvironment, TResult>> PolicyItems;

        public ExceptionPolicyItemBundle ()
        {
            PolicyItems = new List<ExceptionPolicyItem<TEnvironment, TResult>>();
        }


        public ExceptionPolicyItemBundle<TEnvironment, TResult> Handle ( Func<ExceptionContext<TEnvironment, TResult>, bool> criteria, Action<ExceptionContext<TEnvironment, TResult>> handle, bool breakout = true )
        {
            PolicyItems.Add( new ExceptionPolicyItem<TEnvironment, TResult>
            {
                Criteria = criteria,
                Handle = handle,
                BreakOut = breakout
            } );

            return this;
        }

        /// <summary>
        /// 当发生TException类型异常时，执行handle
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout">是否跳出执行链</param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleException<TException> ( Action<ExceptionContext<TEnvironment, TResult>> handle, bool breakout = true ) where TException : Exception
        {
            return Handle(
                ctx => ctx.Exception.IsExceptionType<TException>(),
                ctx => handle(  ctx ),
                breakout
            );
        }

        /// <summary>
        /// 当发生TException类型异常时，执行handle，并以handle的执行结果作为替换的返回结果
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="handle"></param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleException<TException> ( Func<TException, TResult> handle, bool breakout = true ) where TException : Exception
        {
            return HandleException<TException>(
                    ctx => ctx.Result = handle( ctx.Exception.GetExceptionOfType< TException>( ) ),
                    breakout );
        }

        /// <summary>
        /// 当发生TException类型异常时，执行handle
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="output">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleException<TException> ( Action<TException> handle, bool breakout = true ) where TException : Exception
        {
            return HandleException<TException>( exp => { handle( exp ); return default( TResult ); }, breakout );
        }

        #region  发生特定异常码BaseException时
        /// <summary>
        /// 当发生特定异常码BaseException时，执行handle
        /// </summary>
        /// <param name="code">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleBaseException ( int code, Action<ExceptionContext<TEnvironment, TResult>> handle, bool breakout = true )
        {
            return Handle(
                ctx => ctx.Exception.IsExceptionType<BaseException>() && ctx.Exception.GetExceptionOfType<BaseException>( ).Code == code,
                ctx => handle( ctx ),
                breakout
            );
        }

        /// <summary>
        /// 当发生特定异常码BaseException时，执行handle，并以handle的执行结果作为替换的返回结果
        /// </summary>
        /// <param name="code">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleBaseException ( int code, Func<BaseException, TResult> handle, bool breakout = true )
        {
            return HandleBaseException(
                code,
                ctx => ctx.Result = handle( ctx.Exception.GetExceptionOfType<BaseException>()),
                breakout );
        }

        /// <summary>
        /// 当发生TException类型异常时，执行handle
        /// </summary>
        /// <param name="code">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleBaseException ( int code, Action<BaseException> handle, bool breakout = true )
        {
            return HandleBaseException( code, exp => { handle( exp ); return default( TResult ); }, breakout );
        }


        /// <summary>
        /// 当发生特定异常码BaseException时，执行handle
        /// </summary>
        /// <param name="codes">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleBaseException ( int[] codes, Action<ExceptionContext<TEnvironment, TResult>> handle, bool breakout = true )
        {
            return Handle(
                ctx => ctx.Exception.IsExceptionType<BaseException>() && (codes??new int[] { }).Contains( ctx.Exception.GetExceptionOfType<BaseException>().Code),
                ctx => handle( ctx ),
                breakout
            );
        }


        /// <summary>
        /// 当发生特定异常码BaseException时，执行handle，并以handle的执行结果作为替换的返回结果
        /// </summary>
        /// <param name="codes">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleBaseException ( int[] codes, Func<BaseException, TResult> handle, bool breakout = true )
        {
            return HandleBaseException(
                codes,
                ctx => ctx.Result = handle( ctx.Exception.GetExceptionOfType<BaseException>() ),
                breakout );
        }


        /// <summary>
        /// 当发生TException类型异常时，执行handle
        /// </summary>
        /// <param name="codes">异常码</param>
        /// <param name="handle">处理方法</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> HandleBaseException ( int[] codes, Action<BaseException> handle, bool breakout = true )
        {
            return HandleBaseException( codes, exp => { handle( exp ); return default( TResult ); }, breakout );
        }
        #endregion

        /// <summary>
        /// 把特定异常转换成BaseException
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="transfor"></param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> TransException<TException> ( Func<TException, BaseException> transfor, bool breakout = false ) where TException : Exception
        {
            return HandleException<TException>( ctx => ctx.Exception = transfor( ctx.Exception.GetExceptionOfType<TException>() ), breakout );
        }

        /// <summary>
        /// 把特定异常转换成BaseException
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="transToExceptionCode">转换到的目标异常码</param>
        /// <param name="transToExceptionMessage">目标异常信息，{0}表示原始错误信息</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> TransException<TException> ( int transToExceptionCode, string transToExceptionMessage = "{0}", bool breakout = false ) where TException : Exception
        {
            return HandleException<TException>(
                ctx => ctx.Exception = new BaseException( string.Format( transToExceptionMessage, ctx.Exception.Message ), transToExceptionCode, ctx.Exception ),
                breakout );
        }

        /// <summary>
        ///  当发生特定异常码BaseException时，转换成另一个异常码
        /// </summary>
        /// <param name="originalExceptionCode">原始异常码</param>
        /// <param name="transToExceptionCode">转换到的目标异常码</param>
        /// <param name="transToExceptionMessage">目标异常信息，{0}表示原始错误信息</param>
        /// <param name="breakout"></param>
        /// <returns></returns>
        public ExceptionPolicyItemBundle<TEnvironment, TResult> TransBaseExceptionCode ( int originalExceptionCode, int transToExceptionCode, string transToExceptionMessage = "{0}", bool breakout = false )
        {
            return HandleBaseException(
                    originalExceptionCode,
                    ctx => ctx.Exception = new BaseException( string.Format( transToExceptionMessage, ctx.Exception.Message ), transToExceptionCode, ctx.Exception ),
                    breakout
                );
        }

        
    }

    /// <summary>
    /// 异常策略集合,无环境对象，无返回对象
    /// </summary>
    public class ExceptionPolicyItemBundle : ExceptionPolicyItemBundle<VOID, VOID> { }
}
