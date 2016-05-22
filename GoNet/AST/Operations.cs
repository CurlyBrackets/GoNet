using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    enum BinaryOp
    {
        Unknown,
        LogicalOr,
        LogicalAnd,
        LogicalEquals,
        NotEquals,
        LessThan,
        LessEqual,
        GreaterEqual,
        GreaterThan,
        Add,
        Subtract,
        Or,
        Xor,
        Multiply,
        Divide,
        Modulus,
        ShiftLeft,
        ShiftRight,
        And,
        AndNot,
    }

    class BinaryOpWrapper : Base
    {
        public BinaryOp Op { get; private set; }
        public BinaryOpWrapper(BinaryOp op)
        {
            Op = op;
        }
    }
}
