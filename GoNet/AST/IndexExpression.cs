using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IndexExpression : Expression
    {
        public Expression Target
        {
            get { return GetChild<Expression>(0); }
            private set { SetChild(value, 0); }
        }

        public Expression Index
        {
            get { return GetChild<Expression>(1); }
            private set { SetChild(value, 1); }
        }

        public IndexExpression(Expression target, Expression index)
            : base(true, 2)
        {

        }

        public override Expression Clone()
        {
            return new IndexExpression(
                Target.Clone(),
                Index.Clone());
        }
    }
}
