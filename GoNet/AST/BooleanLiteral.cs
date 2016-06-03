using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class BooleanLiteral : Expression
    {
        public bool IsTrue { get; private set; }
        public BooleanLiteral(bool isTrue)
            : base(false)
        {
            IsTrue = isTrue;
        }

        public override Expression CloneExpr()
        {
            return new BooleanLiteral(IsTrue);
        }
    }
}
