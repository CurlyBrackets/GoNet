using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class DefaultCase : Node
    {
        public DefaultCase()
            : base(false)
        {

        }

        public override Node Clone()
        {
            return new DefaultCase();
        }
    }
}
