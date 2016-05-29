using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ExpressionSwitchClause : Scope
    {
        public Node Condition
        {
            get
            {
                return GetChild(0);
            }
            set
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
            set
            {
                SetChild(value, 1);
            }
        }

        public ExpressionSwitchClause()
            : base(2)
        {

        }
    }
}
