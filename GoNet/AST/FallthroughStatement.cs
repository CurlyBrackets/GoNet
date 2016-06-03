using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class FallthroughStatement : Statement
    {
        public FallthroughStatement()
            : base(0)
        {

        }

        public override Statement CloneStatement()
        {
            return new FallthroughStatement();
        }
    }
}
