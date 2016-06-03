using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class OverloadedFunction : Node
    {
        public string Identifier { get; private set; }
        public OverloadedFunction(string id)
            : base(true)
        {
            Identifier = id;
        }

        public override Node Clone()
        {
            var ret = new OverloadedFunction(Identifier);

            for (int i = 0; i < NumChildren(); i++)
                ret.AddChild(GetChild(i)?.Clone());

            return ret;
        }
    }
}
