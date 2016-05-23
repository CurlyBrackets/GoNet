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
        public Expression Value
        {
            get
            {
                return GetChild<Expression>(0);
            }
            set
            {
                SetChild(value, 0);
            }
        }

        public LocalVariable(int slot, bool reference)
            : base(true)
        {
            Slot = slot;
            Reference = reference;
        }
    }
}
