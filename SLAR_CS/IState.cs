using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SLAR_CS
{
    internal interface IState
    {
        void SetState(object state);
        object GetState();
    }
}
