using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    abstract class Expression : Node
    {
        public Type ResolvedType
        {
            get
            {
                return GetChild<Type>(0);
            }
            set
            {
                SetChild(value, 0);
            }
        }

        protected Expression(bool container, int index = 0)
            : base(true, index+1) { }

        public abstract Expression Clone();
    }
}
