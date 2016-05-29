using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class InvocationExpression : Expression
    {
        public Expression Subject
        {
            get
            {
                return GetChild<Expression>(2);
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

        public InvocationExpression(Expression subj, ExpressionList args, bool variableLength)
            : base(true, 2)
        {
            Subject = subj;
            Arguments = args;
            VariableLength = variableLength;
        }

        public override Expression Clone()
        {
            ExpressionList el = null;
            if(Arguments != null)
            {
                el = new ExpressionList();
                for (int i = 0; i < Arguments.NumChildren(); i++)
                    el.AddChild(Arguments.GetChild<Expression>(i).Clone());
            }

            return new InvocationExpression(
                Subject.Clone(),
                el,
                VariableLength);
        }
    }
}
