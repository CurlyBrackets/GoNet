using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class BinaryExpression : Expression
    {
        public BinaryOp Operation { get; private set; }
        public Expression Left { get; private set; }
        public Expression Right { get; private set; }

        public BinaryExpression(BinaryOp op, Expression left, Expression right)
        {
            Operation = op;
            Left = left;
            Right = right;
        }
    }
}
