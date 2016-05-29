using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ConditionClause : Node
    {
        public Expression Expression
        {
            get { return GetChild<Expression>(0); }
            private set { SetChild(value, 0); }
        }

        public ConditionClause(Expression expr)
            : base(true, 1)
        {
            Expression = expr;
        }
    }
}
