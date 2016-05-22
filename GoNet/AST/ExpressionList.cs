using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ExpressionList : Base
    {
        public List<Expression> Items { get; private set; }
        public ExpressionList()
        {
            Items = new List<Expression>();
        }
    }
}
