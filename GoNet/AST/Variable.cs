using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    abstract class Variable : Expression
    {
        public Variable(bool container = false)
            : base(container, 1)
        {

        }
    }
}
