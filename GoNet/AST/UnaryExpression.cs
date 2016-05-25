using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    enum EUnaryOp
    {
        Unknown,
        Positive,
        Negative,
        Not,
        Xor, //?
        Dereference,
        Reference,
        Send,
    }

    class UnaryExpression : Expression
    {
        public Expression Expr
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

        public EUnaryOp Op { get; private set; }

        public UnaryExpression(EUnaryOp op, Expression expr)
            : base(true, 1)
        {
            Expr = expr;
            Op = op;
        }
    }
}
