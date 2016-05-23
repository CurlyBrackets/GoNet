using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ParameterVariable : Variable
    {
        public int Slot { get; private set; }
        public bool Reference { get; private set; }

        public ParameterVariable(int slot, bool reference)
        {
            Slot = slot;
            Reference = reference;
        }
    }
}
