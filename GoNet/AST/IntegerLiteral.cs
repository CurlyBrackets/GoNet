using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IntegerLiteral : Expression
    {
        public BigInteger Value { get; private set; }
        public IntegerLiteral(BigInteger value)
        {
            Value = value;
        }
    }
}
