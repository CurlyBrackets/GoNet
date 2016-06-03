using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class Root : Node
    {
        public Dictionary<string, int> m_packages;

        public Root()
            : base(true)
        {
            m_packages = new Dictionary<string, int>();
        }

        public Package GetPackage(string name, bool imported = false)
        {
            if (m_packages.ContainsKey(name))
                return GetChild<Package>(m_packages[name]);

            var package = new Package(name, imported);
            AddChild(package);
            m_packages.Add(name, NumChildren() - 1);
            return package;
        }

        public override Node Clone()
        {
            var ret = new Root();
            
            return ret;
        }
    }
}
