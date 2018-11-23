using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using Newtonsoft.Json;

namespace Stm.Core
{
    /// <summary>
    /// 服务参数
    /// e.g.1:
    ///     方法定义：int foo(int a,int b)
    ///     方法调用：foo(1,2)
    ///     collection内容：{{"a":"1"},{"b":"2"}}
    ///               或者：{{":0":"1"},{":1":"2"}}
    /// e.g.2:
    ///     class clazz1{ string n,int m}
    ///     方法定义：int foo(int a,clazz1 b)
    ///     方法调用：foo(1,new clazz1{n="n",m=3})
    ///     collection内容：{{"a":"1"},{"b":"{\"n\":\"n\",\"m\":\"3\"}"}}
    ///               或者：{{":0":"1"},{":1":"{\"n\":\"n\",\"m\":\"3\"}"}}
    /// e.g.3:单参数展开形式，已禁用此模式，因为不易理解，接口复杂化
    ///     class clazz1{ string n,int m}
    ///     方法定义：int foo(clazz1 b)
    ///     方法调用：foo(new clazz1{n="n",m=3})
    ///     collection内容：{{"n":"n"},{"m":"3"}}
    ///     单参数展开形式不支持序号查找方式
    /// </summary>
    public class RemoteServiceParameterCollection:Dictionary<String,String>
    {
        /// <summary>
        /// 当方法只有一个class参数时是否展开参数
        /// 排除的类型：基元类型,string,DateTime，Guid,枚举,struct以及前面所述类型的可空形式
        /// </summary>
        [Obsolete( "用于MVC展开单个参数" )]
        public bool SingleParameterExpandMode { get; set; } 

        /// <summary>
        /// 是否是可展开类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [Obsolete("用于MVC展开单个参数")]
        private bool IsExpandAbleMode(Type type )
        {
            //如果是可空类型，获取可空类型的类型参数
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> ))
            {
                type= type.GetGenericArguments()[0];
            }

            Type[] types = new Type[]
            {
                typeof(sbyte),
                typeof(byte ),
                typeof(short ),
                typeof(ushort ),
                typeof(int ),
                typeof(uint ),
                typeof(long ),
                typeof(ulong),
                typeof(char ),
                typeof(float ),
                typeof(double ),
                typeof(bool),
                typeof(decimal),
                typeof(string),
                typeof(DateTime),
                typeof(Guid),
            };
            return !types.Contains( type )&&
                    !type.IsEnum&&
                    !type.IsValueType;
        }

        /// <summary>
        /// 转换成可invoke的参数
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns></returns>
        public object[] ConvertToParameters (MethodInfo methodInfo)
        {
            var declareParameters = methodInfo.GetParameters();

            if (declareParameters.Length == 0) return null;

            object[] result = new object[declareParameters.Length];

            for (int i=0;i<declareParameters.Length;i++)
            {
                var param = declareParameters[i];

                Type parameterType = param.ParameterType;
                String paramName = param.Name;

                String paramValue = null;
                //序号参数查找
                if (this.ContainsKey( ":"+i ))
                {
                    paramValue = this[":"+i];
                }
                //按名称查找
                else if (this.ContainsKey( paramName ))
                {
                    paramValue = this[paramName];
                }


                if (string.IsNullOrWhiteSpace( paramValue ))
                {
                    if (parameterType.IsClass)
                    {
                        result[i] = null;
                    }
                    else
                    {
                        result[i] = Activator.CreateInstance( parameterType );
                    }
                    continue;
                }

                //判断是否是json
                if (((paramValue.StartsWith( "{" ) && paramValue.EndsWith( "}" )) ||
                              (paramValue.StartsWith( "[" ) && paramValue.EndsWith( "]" ))) &&
                              parameterType != typeof( String )
                            )
                {
                    var value = JsonConvert.DeserializeObject( paramValue, parameterType );

                    result[i] =  value ;
                }
                else
                {
                    var value = Convert.ChangeType( paramValue, parameterType );

                    result[i] =  value ;
                }
            }

            return result;

        }

    }
}
