using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class GotoStatement : Statement
    {
        public string Label { get; private set; }
        public GotoStatement(string label)
            : base(0)
        {
            Label = label;
        }

        public override Statement CloneStatement()
        {
            return new GotoStatement(Label);
        }
    }
}
