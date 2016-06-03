using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ExpressionType : Type
    {
        public Expression Expr
        {
            get { return GetChild<Expression>(0); }
            set { SetChild(value, 0); }
        }

        public ExpressionType(Expression expr)
            : base(true, 1)
        {
            Expr = expr.CloneExpr();
        }

        public override Type CloneType()
        {
            return new ExpressionType(Expr.CloneExpr());
        }
    }
}
