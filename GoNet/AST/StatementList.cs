using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class StatementList : Base
    {
        public List<Statement> Items { get; private set; }
        public StatementList()
        {
            Items = new List<Statement>();
        }
    }
}
