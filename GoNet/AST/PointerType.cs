using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class PointerType : Type
    {
        public Type ReferenceType { get; private set; }
        public PointerType(Type refType)
        {
            ReferenceType = refType;
        }
    }
}
