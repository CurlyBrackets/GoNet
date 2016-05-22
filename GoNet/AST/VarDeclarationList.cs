using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class VarDeclarationList : Base
    {
        public List<VarDeclaration> Items { get; private set; }
        public VarDeclarationList()
        {
            Items = new List<VarDeclaration>();
        }

    }
}
