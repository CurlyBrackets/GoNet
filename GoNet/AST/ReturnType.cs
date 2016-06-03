using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ReturnType : Type
    {
        public InvocationExpression Expr
        {
            get { return GetChild<InvocationExpression>(0); }
            private set { SetChild(value, 0); }
        }

        public int Index { get; private set; }

        public ReturnType(InvocationExpression ie, int index = 0)
            : base(true, 1)
        {
            Expr = ie.CloneExpr() as InvocationExpression;
            Index = index;
        }

        public override Type CloneType()
        {
            return new ReturnType(
                Expr.CloneExpr() as InvocationExpression,
                Index);
        }
    }
}
