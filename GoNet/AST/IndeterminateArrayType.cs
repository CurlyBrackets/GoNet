using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IndeterminateArrayType : Type
    {
        public Type ElementType
        {
            get { return GetChild<Type>(0); }
            set { SetChild(value, 0); }
        }

        public IndeterminateArrayType(Type elType)
            : base(true, 1)
        {
            ElementType = elType;
        }

        public override Type Clone()
        {
            throw new NotImplementedException();
        }
    }
}
