using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ReturnVariable : Variable
    {
        public int Slot { get; private set; }

        public ReturnVariable(int slot, bool ignore)
        {
            Slot = slot;
        }

        public override Expression CloneExpr()
        {
            return new ReturnVariable(Slot, false);
        }
    }
}
