using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ExpressionSwitch : Statement
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

        public Expression Expression
        {
            get
            {
                return GetChild<Expression>(1);
            }
            set
            {
                SetChild(value, 1);
            }
        }

        public ExpressionSwitch()
            : base()
        {
            Preamble = null;
            Expression = null;
        }
    }
}
