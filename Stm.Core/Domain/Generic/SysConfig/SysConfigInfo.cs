using Stm.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Domain.Generic
{
    public class SysConfigInfo
    {

        /// <summary>
        /// 键名
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public long Version { get; set; }

        /// <summary>
        /// 生效时间
        /// </summary>
        public DateTime EffectiveDt { get; set; }

        /// <summary>
        /// 是否已删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 最后值，用于第一次获取到还没生效的值
        /// </summary>
        public string LastValue { get; set; }

        public T GetValue<T> ()
        {
            var val = Value;
            if (DateTime.Now < EffectiveDt)
            {
                val = LastValue;
            }

            if (val == null) return default( T );
            var type = typeof( T );
            if (type == typeof( string ) ||
               type == typeof( long ) ||
               type == typeof( int ) ||
               type == typeof( float ) ||
               type == typeof( double ) ||
               type == typeof( byte ) ||
               type == typeof( DateTime ) ||
               type == typeof( Guid ) ||
               type == typeof( short ) ||
               type == typeof( decimal ))
            {
                return (T)Convert.ChangeType( val, typeof( T ) );
            }

            return JsonUtil.ToModel<T>( val );
        }

        public string GetHashKey ()
        {
            return Key + "_" + Value + "_" + LastValue + "_" + Version + "_" + EffectiveDt.ToString() + "_" + IsDeleted.ToString();
        }
    }
}
