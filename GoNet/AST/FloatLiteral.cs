using Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class FloatLiteral : Expression
    {
        public BigRational Value { get; private set; }
        public FloatLiteral(BigRational value)
            : base(false)
        {
            Value = value;
        }

        public override Expression Clone()
        {
            return new FloatLiteral(Value);
        }
    }
}
