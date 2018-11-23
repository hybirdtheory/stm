using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stm.Core
{
    public static class ServiceJsonConvert
    {
        public static string SerializeObject(object value )
        {
            if (value == null) return null;
            var type = value.GetType();
            if (noConvert( type )) return value.ToString();
            if (typeof( IServiceDto ).IsAssignableFrom( type ))
            {
                return ((IServiceDto)value).ToDtoString();
            }
            return JsonConvert.SerializeObject( value );

           //IsoDateTimeConverter timeFormat = new IsoDateTimeConverter();
           // timeFormat.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
           // return JsonConvert.SerializeObject( value, Newtonsoft.Json.Formatting.None, timeFormat ) ;
        }

        private static bool noConvert ( Type type )
        {
            //如果是可空类型，获取可空类型的类型参数
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> ))
            {
                type = type.GetGenericArguments()[0];
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
            return types.Contains( type );
        }
    }
}
