using IdGen;
using Stm.Core.Domain.Generic;
using Stm.Core.SoaGovernance;
using Stm.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Stm.IdService
{
    public class SnowflakeIdService : INumberIdService
    {
        /// <summary>
        /// 分布式全局排序id
        /// </summary>
        private int _globalIndex;

        /// <summary>
        /// 雪花生成器
        /// </summary>
        private IdGenerator _idGenerator;

        public SnowflakeIdService ( IOptions<ServiceInfoRegisterConfig> serviceCfg )
        {
            var index = serviceCfg.Value.GetTagValue( "idservice_index" );

            _globalIndex = int.Parse( index );

            if (_globalIndex >= 1024 || _globalIndex<0)
            {
                throw new Exception( "Snowflake generatorId must greater than 0 and less than 1024" );
            }
            _idGenerator = new IdGenerator( _globalIndex );
        }

        public long GetId ( string key )
        {

            return _idGenerator.CreateId();
        }
    }
}
