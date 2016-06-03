using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IterativeClause : Node
    {
        public StatementList Preamble
        {
            get { return GetChild<StatementList>(0); }
            private set { SetChild(value, 0); }
        }

        public Expression Condition
        {
            get { return GetChild<Expression>(1); }
            private set { SetChild(value, 1); }
        }

        public Statement Afterword
        {
            get { return GetChild<Statement>(2); }
            private set { SetChild(value, 2); }
        }

        public IterativeClause(StatementList preamble, Expression condition, Statement afterword)
            : base(true, 3)
        {
            Preamble = preamble;
            Condition = condition;
            Afterword = afterword;
        }

        public override Node Clone()
        {
            return new IterativeClause(
                Preamble?.Clone() as StatementList,
                Condition?.CloneExpr(),
                Afterword?.CloneStatement());
        }
    }
}
