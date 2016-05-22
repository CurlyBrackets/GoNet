using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ConstDeclaration : Node
    {
        public string Identifier { get; private set; }
        public Expression Value
        {
            get
            {
                return GetChild<Expression>(0);
            }
            set
            {
                SetChild(value, 0);
            }
        }

        public ConstDeclaration(string identifier, Expression value)
            : base(true, 1)
        {
            Identifier = identifier;
            Value = value;
        }
    }
}
