using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class CompositeLiteral : Expression
    {
        public CompositeLiteral()
            : base(true, 2)
        {

        }

        public override Expression Clone()
        {
            throw new NotImplementedException();
        }
    }
}
