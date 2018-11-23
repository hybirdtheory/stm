using System;
using System.Collections.Generic;
using System.Text;

namespace Stm.Core.Security
{
    public interface IStmPrincipalPersistor
    {
        StmPrincipal RestorePrincipal ();

        void SavePrincipal ( StmPrincipal principal );
    }
}
