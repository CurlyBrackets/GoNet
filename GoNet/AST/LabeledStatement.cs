using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class LabeledStatement : Statement
    {
        public string Label { get; private set; }
        public Statement Statement
        {
            get { return GetChild<Statement>(0); }
            private set { SetChild(value, 0); }
        }

        public LabeledStatement(string label, Statement s)
            : base(1)
        {
            Label = label;
            Statement = s;
        }
    }
}
