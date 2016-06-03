using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class SelectorExpression : Expression
    {
        public Expression Left
        {
            get { return GetChild<Expression>(0); }
            private set { SetChild(value, 0); }
        }

        public string Right { get; private set; }

        public SelectorExpression(Expression left, string right)
            : base(true, 1)
        {
            Left = left;
            Right = right;
        }

        public override Expression CloneExpr()
        {
            return new SelectorExpression(
                Left.CloneExpr(),
                Right);
        }
    }
}
