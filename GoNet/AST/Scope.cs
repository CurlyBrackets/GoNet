using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    abstract class Scope : Node
    {
        private List<int> m_constDeclarations, m_typeDeclarations, m_varDeclarations;

        protected Scope(int startIndex)
            : base(true)
        {
            for (int i = 0; i < startIndex; i++)
                SetChild(null, i);

            m_constDeclarations = new List<int>();
            m_typeDeclarations = new List<int>();
            m_varDeclarations = new List<int>();
        }

        public void AddConstDeclaration(ConstDeclaration cd)
        {
            AddChild(cd);
            m_constDeclarations.Add(NumChildren() - 1);
        }

        public void AddTypeDeclaration(TypeDeclaration td)
        {
            AddChild(td);
            m_typeDeclarations.Add(NumChildren() - 1);
        }

        public void AddVarDeclaration(VarDeclaration vd)
        {
            AddChild(vd);
            m_varDeclarations.Add(NumChildren() - 1);
        }

        public ConstDeclaration GetConstDeclaration(int index)
        {
            return GetChild<ConstDeclaration>(m_constDeclarations[index]);
        }

        public TypeDeclaration GetTypeDeclaration(int index)
        {
            return GetChild<TypeDeclaration>(m_typeDeclarations[index]);
        }

        public VarDeclaration GetVarDeclaration(int index)
        {
            return GetChild<VarDeclaration>(m_varDeclarations[index]);
        }

        public int NumConstDeclarations()
        {
            return m_constDeclarations.Count;
        }

        public int NumTypeDeclarations()
        {
            return m_typeDeclarations.Count;
        }

        public int NumVarDeclarations()
        {
            return m_varDeclarations.Count;
        }
    }
}
