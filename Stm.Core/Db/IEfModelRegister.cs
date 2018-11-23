using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Db
{
    public interface IEfModelRegister
    {
        void Register ( ModelBuilder modelBuilder );
    }
}
