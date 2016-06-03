using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class StatementList : Node
    {
        public StatementList()
            : base(true)
        {
        }

        public override Node Clone()
        {
            var ret = new StatementList();

            for (int i = 0; i < NumChildren(); i++)
                ret.AddChild(GetChild(i)?.Clone());

            return ret;
        }
    }
}
