using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class LocalVariable : Variable
    {
        public int Slot { get; private set; }
        public bool Reference { get; private set; }

        public LocalVariable(int slot, bool reference)
            : base(false)
        {
            Slot = slot;
            Reference = reference;
        }

        public override Expression Clone()
        {
            return new LocalVariable(Slot, Reference);
        }
    }
}
