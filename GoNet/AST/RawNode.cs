using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class RawNode : Base
    {
        public string Text { get; private set; }
        public RawNode(string text)
        {
            Text = text;
        }
    }

    class RawNodeList : Base
    {
        public List<RawNode> Items { get; private set; }
        public RawNodeList()
        {
            Items = new List<RawNode>();
        }
    }
}
