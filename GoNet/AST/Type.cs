using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    abstract class Type : Node
    {
        protected Type(bool container, int limit = 0)
            : base(container, limit)
        {

        }
    }
}
