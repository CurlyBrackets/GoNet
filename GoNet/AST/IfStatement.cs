using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IfStatement : Statement
    {
        public Statement Preamble { get; set; }
        public Expression Condition { get; private set; }
        public Block True { get; private set; }
        public Statement False { get; set; }

        public IfStatement(Expression condition, Block t)
        {
            Condition = condition;
            True = t;
        }
    }
}
