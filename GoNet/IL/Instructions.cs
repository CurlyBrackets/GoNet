using GoNet.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.IL
{
    class Instructions : Node
    {
        public Instructions()
            : base(true)
        {

        }

        public override Node Clone()
        {
            var ret = new Instructions();
            for (int i = 0; i < NumChildren(); i++)
                ret.AddChild(GetChild(i)?.Clone());
            return ret;
        }
    }
}
