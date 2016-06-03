using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class RealType : Type
    {
        public System.Type Type
        {
            get; private set;
        }

        public RealType(System.Type t)
            : base(false)
        {
            Type = t;
        }

        public override Type CloneType()
        {
            return new RealType(Type);
        }
    }
}
