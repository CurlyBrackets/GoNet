using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class Root : Base
    {
        public Dictionary<string, Package> Packages { get; private set; }
        public Root()
        {
            Packages = new Dictionary<string, Package>();
        }
    }
}
