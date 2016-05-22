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
        {
            BaseType = t;
        }
    }
}
