using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ExpressionWrapper : Expression
    {
        public Expression Expr
        {
            get { return GetChild<Expression>(1); }
            private set { SetChild(value, 1); }
        }

        public ExpressionWrapper(Expression expr)
            : base(true,1)
        {
            Expr = expr;
        }

        public override Expression Clone()
        {
            return new ExpressionWrapper(Expr.Clone());
        }
    }
}
