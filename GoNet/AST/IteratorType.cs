using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IteratorType : Type
    {
        public Type BaseType
        {
            get { return GetChild<Type>(0); }
            private set { SetChild(value, 0); }
        }

        public int Index { get; private set; }

        public IteratorType(Type baseType, int index)
            :base(true, 1)
        {
            BaseType = baseType;
            Index = index;
        }

        public override Type Clone()
        {
            return new IteratorType(
                BaseType.Clone(),
                Index);
        }
    }
}
