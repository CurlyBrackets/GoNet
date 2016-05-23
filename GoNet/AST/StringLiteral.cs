using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class StringLiteral : Expression
    {
        public string Value { get; private set; }
        public StringLiteral(string value)
            : base(false)
        {
            Value = value;
        }
    }
}
