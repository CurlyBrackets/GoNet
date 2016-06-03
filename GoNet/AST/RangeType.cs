using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class RangeType : Type
    {
        public Type BaseType { get; private set; }
        public RangeType(Type t)
            : base(true, 1)
        {
            BaseType = t;
        }

        public override Type CloneType()
        {
            return new RangeType(BaseType.CloneType());
        }
    }
}
