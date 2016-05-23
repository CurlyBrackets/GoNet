using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    abstract class Variable : Node
    {
        public Variable()
            : base(false)
        {

        }
    }
}
