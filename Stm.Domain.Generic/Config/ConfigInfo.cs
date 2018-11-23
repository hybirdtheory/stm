using Stm.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Domain.Generic
{
    public class ConfigInfo
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


        public T GetValue<T> ()
        {
            if (Value == null) return default( T );
            if (Value.GetType() == typeof( string ) ||
               Value.GetType() == typeof( long ) ||
               Value.GetType() == typeof( int ) ||
               Value.GetType() == typeof( float ) ||
               Value.GetType() == typeof( double ) ||
               Value.GetType() == typeof( byte ) ||
               Value.GetType() == typeof( DateTime ) ||
               Value.GetType() == typeof( Guid ) ||
               Value.GetType() == typeof( short ) ||
               Value.GetType() == typeof( decimal ))
            {
                return (T)Convert.ChangeType( Value, typeof( T ) );
            }

            return JsonUtil.ToModel<T>( Value );
        }
    }
}
