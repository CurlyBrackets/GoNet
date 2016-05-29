using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class KeyedElement : Node
    {
        public Expression Key
        {
            get { return GetChild<Expression>(0); }
            set { SetChild(value, 0); }
        }

        public Expression Element
        {
            get { return GetChild<Expression>(1); }
            set { SetChild(value, 1); }
        }

        public KeyedElement(Expression key, Expression element)
            : base(true, 2)
        {
            Key = key;
            Element = element;
        }
    }
}
