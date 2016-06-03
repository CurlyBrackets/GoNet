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
        public bool AnyLength { get; private set; }

        public ArrayType(Type elementType)
            : base(true, 1)
        {
            ElementType = elementType;
            AnyLength = true;
            Length = 0;
        }

        public ArrayType(Type elementType, int length)
            : base(true, 1)
        {
            ElementType = elementType;
            Length = length;
            AnyLength = false;
        }

        public override Type CloneType()
        {
            if (AnyLength)
                return new ArrayType(ElementType.CloneType());
            else
                return new ArrayType(
                    ElementType.CloneType(),
                    Length);
        }
    }
}
