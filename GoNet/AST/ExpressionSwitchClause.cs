using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ExpressionSwitchClause : Node
    {
        public Node Condition
        {
            get
            {
                return GetChild(0);
            }
            private set
            {
                SetChild(value, 0);
            }
        }

        public StatementList Statements
        {
            get
            {
                return GetChild<StatementList>(1);
            }
            private set
            {
                SetChild(value, 1);
            }
        }

        public ExpressionSwitchClause(Node condition, StatementList sl)
            : base(true, 2)
        {
            Condition = condition;
            Statements = sl;
        }
    }
}
