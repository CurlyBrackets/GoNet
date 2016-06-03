using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ExpressionList : Node
    {
        public ExpressionList()
            : base(true)
        {
            
        }

        public override Node Clone()
        {
            var ret = new ExpressionList();
            for (int i = 0; i < NumChildren(); i++)
                ret.AddChild(GetChild(i)?.Clone());
            return ret;
        }
    }
}
