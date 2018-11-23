using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core
{
    /// <summary>
    /// 服务传输对象
    /// </summary>
    public interface IServiceDto
    {
        String ToDtoString ();

        void FromDtoString (String dtoString);
    }
}
