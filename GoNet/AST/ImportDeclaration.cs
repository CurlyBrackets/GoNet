using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ImportDeclaration : Base
    {
        public string Package { get; private set; }
        public string Alias { get; private set; }
        public string Name {
            get
            {
                if (!string.IsNullOrEmpty(Alias))
                {
                    if (Alias == "_" || Alias == ".")
                        return string.Empty;
                    return Alias;
                }

                return Package.Substring(Package.LastIndexOf('/') + 1);
            }
        }

        public ImportDeclaration(string package, string alias)
        {
            Package = package;
            Alias = alias;
        }
    }
}
