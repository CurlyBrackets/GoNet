using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ReturnStatement : Statement
    {
        public ReturnStatement()
        {

        }

        public override Statement CloneStatement()
        {
            var ret = new ReturnStatement();
            for (int i = 0; i < NumChildren(); i++)
                ret.AddChild(GetChild(i)?.Clone());
            return ret;
        }
    }
}
