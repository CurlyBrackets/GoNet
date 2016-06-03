using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IfStatement : Statement
    {
        public Statement Preamble
        {
            get
            {
                return GetChild<Statement>(0);
            }
            set
            {
                SetChild(value, 0);
            }
        }

        public Expression Condition
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

        public Block True
        {
            get
            {
                return GetChild<Block>(2);
            }
            private set
            {
                SetChild(value, 2);
            }
        }

        public Statement False
        {
            get
            {
                return GetChild<Statement>(3);
            }
            set
            {
                SetChild(value, 3);
            }
        }

        public IfStatement(Expression condition, Block t)
            : base(4)
        {
            Condition = condition;
            True = t;
        }

        public override Statement CloneStatement()
        {
            return new IfStatement(
                Condition.CloneExpr(),
                True.Clone() as Block)
            {
                Preamble = Preamble?.CloneStatement(),
                False = False?.CloneStatement()
            };
        }
    }
}
