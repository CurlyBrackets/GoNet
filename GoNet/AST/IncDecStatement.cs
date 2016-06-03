using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IncDecStatement : Statement
    {
        public Expression Expression
        {
            get { return GetChild<Expression>(0); }
            private set { SetChild(value, 0); }
        }

        public bool Increment { get; private set; }

        public IncDecStatement(Expression expr, bool increment)
            : base(1)
        {
            Expression = expr;
            Increment = increment;
        }

        public override Statement CloneStatement()
        {
            return new IncDecStatement(
                Expression.CloneExpr(),
                Increment);
        }
    }
}
