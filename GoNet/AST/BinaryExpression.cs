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
        public Expression Left
        {
            get
            {
                return GetChild<Expression>(0);
            }
            private set
            {
                SetChild(value, 0);
            }
        }

        public Expression Right
        {
            get
            {
                return GetChild<Expression>(1);
            }
            private set
            {
                SetChild(value, 1);
            }
        }

        public BinaryExpression(BinaryOp op, Expression left, Expression right)
            : base(true, 2)
        {
            Operation = op;
            Left = left;
            Right = right;
        }
    }
}
