using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IdentifierList : Base
    {
        public List<string> Items { get; private set; }
        public IdentifierList()
        {
            Items = new List<string>();
        }
    }
}
