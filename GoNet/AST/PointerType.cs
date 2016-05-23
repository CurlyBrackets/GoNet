using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class PointerType : Type
    {
        public Type ReferenceType
        {
            get
            {
                return GetChild<Type>(0);
            }
            private set
            {
                SetChild(value, 0);
            }
        }
        public PointerType(Type refType)
            :base(true, 1)
        {
            ReferenceType = refType;
        }
    }
}
