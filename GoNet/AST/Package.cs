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
        public bool Imported { get; private set; }

        private List<int> m_functions, m_imports, m_staticInitializers;

        public Package(string name, bool imported = false)
            : base(0)
        {
            Name = name;
            m_functions = new List<int>();
            m_imports = new List<int>();
            m_staticInitializers = new List<int>();
            Imported = imported;
        }

        public void AddFunctionDeclaration(Function f)
        {
            AddChild(f);
            m_functions.Add(NumChildren() - 1);
        }

        public Function GetFunctionDeclaration(int index)
        {
            return GetChild<Function>(m_functions[index]);
        }

        public int NumFunctionDeclarations()
        {
            return m_functions.Count;
        }

        public void AddImport(ImportDeclaration id)
        {
            AddChild(id);
            m_imports.Add(NumChildren() - 1);
        }

        public ImportDeclaration GetImports(int index)
        {
            return GetChild<ImportDeclaration>(m_imports[index]);
        }

        public int NumImports()
        {
            return m_imports.Count;
        }

        public void AddStaticInitializer(Assignment a)
        {
            AddChild(a);
            m_staticInitializers.Add(NumChildren() - 1);
        }

        public Assignment GetStaticInitialzier(int i)
        {
            return GetChild<Assignment>(m_staticInitializers[i]);
        }

        public int NumStaticInitializers()
        {
            return m_staticInitializers.Count;
        }
    }
}
