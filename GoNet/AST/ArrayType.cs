using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ArrayType : Type
    {
        public Type ElementType
        {
            get { return GetChild<Type>(0); }
            set { SetChild(value, 0); }
        }

        public int Length { get; private set; }

        public ArrayType(Type elementType, int length)
            : base(true, 1)
        {
            ElementType = elementType;
            Length = length;
        }

        public override Type Clone()
        {
            return new ArrayType(
                ElementType.Clone(),
                Length);
        }
    }
}
