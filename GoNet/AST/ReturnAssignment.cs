using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ReturnAssignment : Statement
    {
        public ExpressionList Identifiers
        {
            get { return GetChild<ExpressionList>(0); }
            private set { SetChild(value, 0); }
        }

        public InvocationExpression Invocation
        {
            get { return GetChild<InvocationExpression>(1); }
            private set { SetChild(value, 1); }
        }

        public ReturnAssignment(ExpressionList el, InvocationExpression ie)
            : base(2)
        {
            Identifiers = el;
            Invocation = ie;
        }
    }
}
