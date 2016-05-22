using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class Package : Scope
    {
        public string Name { get; private set; }

        public Dictionary<string, Function> Functions { get; private set; }
        public Dictionary<string, ImportDeclaration> Imports { get; private set; }

        public Package(string name)
        {
            Name = name;
            Functions = new Dictionary<string, Function>();
            Imports = new Dictionary<string, ImportDeclaration>();
        }
    }
}
