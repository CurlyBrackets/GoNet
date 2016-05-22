using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class VarDeclaration : Node
    {
        public string Identifier { get; private set; }
        public Expression Value
        {
            get
            {
                return GetChild<Expression>(0);
            }
            private set
            {
                SetChild(value, 0);
            }
        }

        public VarDeclaration(string id, Expression value)
            : base(true, 1)
        {
            Identifier = id;
            Value = value;
        }
    }
}
