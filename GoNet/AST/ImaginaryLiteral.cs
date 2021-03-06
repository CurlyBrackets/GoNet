﻿using Numerics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ImaginaryLiteral : Expression
    {
        public BigRational Value { get; private set; }
        public ImaginaryLiteral(BigRational value)
            : base(false)
        {
            Value = value;
        }

        public override Expression CloneExpr()
        {
            return new ImaginaryLiteral(Value);
        }
    }
}
