using Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ImaginaryLiteral : Base
    {
        public BigRational Value { get; private set; }
        public ImaginaryLiteral(BigRational value)
        {
            Value = value;
        }
    }
}
