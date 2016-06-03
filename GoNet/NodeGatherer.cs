using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;

namespace GoNet
{
    class NodeGatherer<T> : AstProcessor where T : Node
    {
        public List<T> Results { get; private set; }

        public NodeGatherer()
        {
            Results = new List<T>();
        }

        public override void Process(Node input)
        {
            if (input is T)
                Results.Add(input as T);

            base.Process(input);
        }
    }
}
