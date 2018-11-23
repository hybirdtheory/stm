using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Stm.Core.CodeGeneration
{
    /// <summary>
    /// api客户端代码生成器
    /// </summary>
    public interface IApiCodeGenerator
    {
        /// <summary>
        /// 支持的语言
        /// </summary>
        string Languege { get; }

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="methodInfo">需要生成代码的目标方法</param>
        /// <param name="rootnamespace">根命名空间</param>
        /// <returns></returns>
        string Generate ( MethodInfo methodInfo, string rootnamespace );
    }
}
