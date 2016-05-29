using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ConversionExpression : Expression
    {
        public Type To
        {
            get { return GetChild<Type>(0); }
            private set { SetChild(value, 0); }
        }

        public Expression From
        {
            get { return GetChild<Expression>(1); }
            private set { SetChild(value, 1); }
        }

        public ConversionExpression(Type toType, Expression from)
            : base(true, 2)
        {
            To = toType;
            From = from;
        }

        public override Expression Clone()
        {
            return new ConversionExpression(
                To.Clone(),
                From.Clone());
        }
    }
}
