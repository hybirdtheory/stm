using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Stm.Core.CodeGeneration
{
    public class AndroidApiCodeGenerator : IApiCodeGenerator
    {
        public string Languege => "android";

        private string requestDtoTemplate =
            "/****   {requestClazz}  *****/\r\n" +
            "package {requestPackage};\r\n\r\n" +
            "import java.io.Serializable;\r\n\r\n" +
            "public class {requestClazz}\r\n" +
            "{\r\n" +
            "{properties}" +
            "{innerclasses}" +
            "}\r\n";

        private string responseDtoTemplate =
            "/****   {responseClazz}  *****/\r\n" +
            "package {reponsePackage};\r\n\r\n" +
            "import java.io.Serializable;\r\n\r\n" +
            "public class {responseClazz} {responseExtend} {responseBaseClazz}\r\n" +
            "{\r\n" +
            "{properties}" +
            "{innerclasses}" +
            "}\r\n";

        private string innerClassDtoTemplate =
            "   public class {innerClazz}\r\n" +
            "   {\r\n" +
            "{properties}" +
            "   }\r\n";

        private string propertyTemplate =
            "{indent}private {propertyType} {fieldName};\r\n" +
            "{indent}public {propertyType} get{propertyName}() {return {fieldName};}\r\n\r\n";

        private Dictionary<Type, string> typeMap = new Dictionary<Type, string>
        {
            {typeof(int),"Integer" },
            {typeof(string),"String" },
            {typeof(bool),"Boolean" },
            {typeof(sbyte),"Integer" },
            {typeof(byte),"Integer" },
            {typeof(short),"Integer" },
            {typeof(ushort),"Integer" },
            {typeof(uint),"Integer" },
            {typeof(long),"Integer" },
            {typeof(ulong),"Integer" },
            {typeof(char),"char" },
            {typeof(float),"Float" },
            {typeof(double),"Double" },
            {typeof(decimal),"Float" },
            {typeof(DateTime),"java.util.Date" },
            {typeof(Guid),"String" },
        };

        private string ListTypeMap = "java.util.List<{0}>";
        private string ObjectTypeMap = "Object";
        private string EnumTypeMap = "Integer";
        //继承关键字
        private string ExtendsStr = "extends";

        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="methodInfo">需要生成代码的目标方法</param>
        /// <param name="rootnamespace">根命名空间</param>
        /// <returns></returns>
        public string Generate ( MethodInfo methodInfo, string rootnamespace )
        {
            var dtoNameBase = (methodInfo.Name.EndsWith( "Request" ) ? methodInfo.Name.Substring( 0, methodInfo.Name.Length - 7 ) : methodInfo.Name).ToLower();
            var package = rootnamespace.ToLower() + "."+ dtoNameBase;
            var requestClazz = UpcaseFirst(dtoNameBase) + "Request";
            var responseClazz = UpcaseFirst(dtoNameBase) + "Response";


            //包含的非常规字段类型,type是字段类型，bool是是否已生成代码
            Dictionary<Type,bool> innerClasses = new Dictionary<Type, bool>();

            //如果有参数，遍历参数，并生成请求类dto代码
            var sbRequestDto =string.Empty;
            if (methodInfo.GetParameters().Any())
            {
                List<KeyValuePair<Type, string>> paramTypeNameMap = new List<KeyValuePair<Type, string>>();

                var parameters = methodInfo.GetParameters();
                foreach(var param in parameters)
                {
                    var paramType = param.ParameterType;
                    var paramName = param.Name;

                    paramTypeNameMap.Add(new KeyValuePair<Type, string>( paramType, paramName ));
                }

                //生成参数代码
                string strProperty = GenerateProperty( paramTypeNameMap, innerClasses );

                StringBuilder sbInner = new StringBuilder();
                //如果有内部类,生成内部类代码
                if (innerClasses.Any())
                {
                    while (innerClasses.Any(t=>!t.Value))//如果还有未生成代码的类
                    {
                        var innerType = innerClasses.First(t=>!t.Value).Key;
                        var innerProperties = innerType.GetProperties();
                        List<KeyValuePair<Type, string>> innerPropertiesTypeNameMap = new List<KeyValuePair<Type, string>>();
                        foreach(var innerProperty in innerProperties)
                        {
                            innerPropertiesTypeNameMap.Add(new KeyValuePair<Type, string>( innerProperty.PropertyType, innerProperty.Name ));
                        }

                        string innerClassCode = innerClassDtoTemplate
                            .Replace( "{innerClazz}", innerType.Name)
                            .Replace( "{properties}", GenerateProperty( innerPropertiesTypeNameMap, innerClasses, "      " ) );

                        //生成内部类代码
                        sbInner.Append( innerClassCode );

                        //设置已生成代码
                        innerClasses[innerType] = true;

                    }
                }


                //替换模板
                sbRequestDto = requestDtoTemplate
                    .Replace( "{requestClazz}", requestClazz )
                    .Replace( "{requestPackage}", package )
                    .Replace( "{properties}", strProperty )
                    .Replace( "{innerclasses}", sbInner.ToString() );

            }



            var sbResponseDto = string.Empty;
            var returnType = methodInfo.ReturnType;
            //如果有返回类型，生成返回类dto代码
            if (returnType != typeof( void ) && returnType != typeof( Task ))
            {
                //基类类名
                string responseBaseClazz = "";
                //是否有基类
                bool hasBaseType = false;
                //返回类型是否是数组
                bool returnTypeIsList = false;

                //如果返回类型是Task<>类型，获取Task<>类型的类型参数
                if (returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof( Task<> ) )
                {
                    returnType = returnType.GetGenericArguments()[0];
                }
                //如果是数组类型，获取数组原始类型
                if (returnType.IsGenericType &&
                    (returnType.GetGenericTypeDefinition() == typeof( IList<> ) ||
                     returnType.GetGenericTypeDefinition() == typeof( List<> ))
                )
                {
                    returnTypeIsList = true;
                    returnType = returnType.GetGenericArguments()[0];
                }
                else if (returnType.IsArray)
                {
                    returnTypeIsList = true;
                    returnType = returnType.GetElementType();
                }

                var baseType = returnType.BaseType;
                if (baseType != null&&baseType == typeof( object )) baseType = null;
                if (baseType != null)
                {
                    hasBaseType = true;
                    responseBaseClazz = baseType.Name;
                }

                //不是基本类型才生成dto
                if (!returnType.IsEnum && !typeMap.ContainsKey( returnType ))
                {
                    var responseTypeProperties = returnType.GetProperties();
                    List<KeyValuePair<Type, string>> responseTypePropertiesTypeNameMap = new List<KeyValuePair<Type, string>>();
                    foreach (var responseProperty in responseTypeProperties)
                    {
                        responseTypePropertiesTypeNameMap.Add(new KeyValuePair<Type, string>( responseProperty.PropertyType, responseProperty.Name ));
                    }

                    //生成参数代码
                    string strResponseTypeProperties = GenerateProperty( responseTypePropertiesTypeNameMap, innerClasses );

                    //如果有内部类,生成内部类代码
                    StringBuilder sbInner = new StringBuilder();
                    if (innerClasses.Any( t => !t.Value ))
                    {
                        while (innerClasses.Any( t => !t.Value ))//如果还有未生成代码的类
                        {
                            var innerType = innerClasses.First( t => !t.Value ).Key;
                            var innerProperties = innerType.GetProperties();
                            List<KeyValuePair<Type, string>> innerPropertiesTypeNameMap = new List<KeyValuePair<Type, string>>();
                            foreach (var innerProperty in innerProperties)
                            {
                                innerPropertiesTypeNameMap.Add(new KeyValuePair<Type, string>( innerProperty.PropertyType, innerProperty.Name) );
                            }

                            string innerClassCode = innerClassDtoTemplate
                                .Replace( "{innerClazz}", innerType.Name )
                                .Replace( "{properties}", GenerateProperty( innerPropertiesTypeNameMap, innerClasses, "      " ) );

                            //生成内部类代码
                            sbInner.Append( innerClassCode );

                            //设置已生成代码
                            innerClasses[innerType] = true;

                        }
                    }


                    //替换模板
                    sbResponseDto = responseDtoTemplate
                        .Replace( "{responseClazz}", returnTypeIsList?string.Format(ListTypeMap, responseClazz): responseClazz )
                        .Replace( "{reponsePackage}", package )
                        .Replace( "{responseExtend}", hasBaseType?ExtendsStr:"" )
                        .Replace( "{responseBaseClazz}", responseBaseClazz )
                        .Replace( "{properties}", strResponseTypeProperties )
                        .Replace( "{innerclasses}", sbInner.ToString() );


                }


            }



            return sbRequestDto + sbResponseDto;
        }

        private string GenerateProperty ( List<KeyValuePair<Type,string>> properties, Dictionary<Type, bool> innerClasses,string indent = "   " )
        {

            var sbItems = new StringBuilder();

            foreach (var property in properties)
            {
                var paramType = property.Key;
                var paramName = property.Value;

                //是否是数组
                var islist = false;

                //如果是数组类型，获取数组原始类型
                if (paramType.IsGenericType &&
                    (paramType.GetGenericTypeDefinition() == typeof( IList<> ) ||
                     paramType.GetGenericTypeDefinition() == typeof( List<> ))
                )
                {
                    islist = true;
                    paramType = paramType.GetGenericArguments()[0];
                }
                else if (paramType.IsArray)
                {
                    islist = true;
                    paramType = paramType.GetElementType();
                }

                //如果是可空类型，获取真实类型
                if (paramType.IsGenericType && paramType.GetGenericTypeDefinition() == typeof( Nullable<> ))
                {
                    paramType = paramType.GetGenericArguments()[0];
                }

                var transType = ObjectTypeMap;

                if (paramType.IsEnum)
                {
                    transType = EnumTypeMap;
                }
                else if (typeMap.ContainsKey( paramType ))
                {
                    transType = typeMap[paramType];
                }
                else
                {
                    transType = paramType.Name;

                    if (!innerClasses.ContainsKey(paramType ))
                    {
                        innerClasses.Add( paramType ,false);
                    }
                }

                //如果是数组类，包上List<>
                var typeStr= islist ? string.Format( ListTypeMap, transType ) : transType;

                //替换模板
                sbItems.Append( propertyTemplate
                    .Replace( "{propertyType}", typeStr )
                    .Replace( "{fieldName}",paramName)
                    .Replace( "{propertyName}", UpcaseFirst(paramName ))
                    .Replace( "{indent}",indent)
                    );
            }

            return sbItems.ToString();
        }

        /// <summary>
        /// 首字母大写
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string UpcaseFirst(string str )
        {
            if (string.IsNullOrWhiteSpace( str )) return str;

            if (str.Length == 1) return str.ToUpper();

            return str.Substring( 0, 1 ).ToUpper() + str.Substring( 1, str.Length - 1 );
        }
    }
}
