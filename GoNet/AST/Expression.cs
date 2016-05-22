using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    abstract class Expression : Node
    {
        protected Expression(bool container, int index = 0)
            : base(container, index) { }
    }
}
