using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class InvocationExpression : Expression
    {
        public Node Subject
        {
            get
            {
                return GetChild<Node>(2);
            }
            private set
            {
                SetChild(value, 2);
            }
        }

        public ExpressionList Arguments
        {
            get
            {
                return GetChild<ExpressionList>(1);
            }
            private set
            {
                SetChild(value, 1);
            }
        }

        public bool VariableLength
        {
            get; private set;
        }

        public InvocationExpression(Node subj, ExpressionList args, bool variableLength)
            : base(true, 2)
        {
            Subject = subj;
            Arguments = args;
            VariableLength = variableLength;
        }

        public override Expression CloneExpr()
        {
            ExpressionList el = null;
            if(Arguments != null)
            {
                el = new ExpressionList();
                for (int i = 0; i < Arguments.NumChildren(); i++)
                    el.AddChild(Arguments.GetChild<Expression>(i).CloneExpr());
            }

            return new InvocationExpression(
                Subject.Clone(),
                el,
                VariableLength);
        }
    }
}
