using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    internal class AnyType : Type
    {
        public AnyType()
            : base(false)
        {

        }

        public override Type CloneType()
        {
            return new AnyType();
        }
    }

    abstract class Type : Node
    {
        protected Type(bool container, int limit = 0)
            : base(container, limit)
        {

        }

        public abstract Type CloneType();
        public override Node Clone()
        {
            return CloneType();
        }

        public static Type Any
        {
            get
            {
                return new AnyType();
            }
        }
    }
}
