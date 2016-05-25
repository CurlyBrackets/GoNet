using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class UnaryOpWrapper : Base
    {
        public EUnaryOp Op { get; private set; }

        public UnaryOpWrapper(EUnaryOp op)
        {
            Op = op;
        }
    }
}
