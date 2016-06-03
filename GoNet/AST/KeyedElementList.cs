using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class KeyedElementList : Node
    {
        public KeyedElementList()
            : base(true)
        {

        }

        public override Node Clone()
        {
            var ret = new KeyedElementList();
            for (int i = 0; i < NumChildren(); i++)
                ret.AddChild(GetChild(i)?.Clone());
            return ret;
        }
    }
}
