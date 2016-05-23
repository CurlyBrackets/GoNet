using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class RuneLiteral : Expression
    {
        public int Value { get; private set; }
        public RuneLiteral(int value)
            : base(false)
        {
            Value = value;
        }
    }
}
