using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IlBlock : Node
    {
        public IlBlock()
            : base(true)
        {

        }

        public override Node Clone()
        {
            var ret = new IlBlock();
            for (int i = 0; i < NumChildren(); i++)
                ret.AddChild(GetChild(i)?.Clone());
            return ret;
        }
    }
}
