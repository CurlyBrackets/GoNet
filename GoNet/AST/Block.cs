using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class Block : Statement, IScope
    {
        private Scope m_scope
        {
            get { return GetChild<Scope>(0); }
            set { SetChild(value, 0); }
        }

        public Block()
        {
            m_scope = new Scope(0);
        }

        public void AddConstDeclaration(ConstDeclaration cd)
        {
            m_scope.AddConstDeclaration(cd);
        }

        public void AddTypeDeclaration(TypeDeclaration td)
        {
            m_scope.AddTypeDeclaration(td);
        }

        public void AddVarDeclaration(VarDeclaration vd)
        {
            m_scope.AddVarDeclaration(vd);
        }

        public ConstDeclaration GetConstDeclaration(int index)
        {
            return m_scope.GetConstDeclaration(index);
        }

        public TypeDeclaration GetTypeDeclaration(int index)
        {
            return m_scope.GetTypeDeclaration(index);
        }

        public VarDeclaration GetVarDeclaration(int index)
        {
            return m_scope.GetVarDeclaration(index);
        }

        public int NumConstDeclarations()
        {
            return m_scope.NumConstDeclarations();
        }

        public int NumTypeDeclarations()
        {
            return m_scope.NumTypeDeclarations();
        }

        public int NumVarDeclarations()
        {
            return m_scope.NumVarDeclarations();
        }
    }
}
