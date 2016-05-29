using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class RangeClause : Node
    {
        public Expression Iterator
        {
            get { return GetChild<Expression>(0); }
            private set { SetChild(value, 0); }
        }

        public ExpressionList Values
        {
            get { return GetChild<ExpressionList>(1); }
            private set { SetChild(value, 1); }
        }

        public RangeClause(ExpressionList values, Expression iterator)
            : base(true, 2)
        {
            Iterator = iterator;
            Values = values;
        }
    }
}
