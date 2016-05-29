using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;

namespace GoNet
{
    class AstResolver : AstProcessor
    {
        public class ResolvingScope
        {
            public Dictionary<string, Variable> Vars;
            public int LocalIndex, ParameterIndex, ReturnIndex;
        }

        private List<ResolvingScope> m_scopes;

        public ResolvingScope CurrentScope
        {
            get
            {
                return m_scopes.Last();
            }
        }

        public AstResolver()
        {
            m_scopes = new List<ResolvingScope>();
        }

        public override void Process(Node input)
        {
            switch (input)
            {
                case VarDeclaration vd:
                    DefineVariable<LocalVariable>(vd.Identifier, false, null);
                    base.Process(input);
                    break;
                case Signature s:
                    foreach (var p in s.Parameters.FilteredChildren<Parameter>())
                        DefineVariable<ParameterVariable>(p.Name, p.Type is PointerType, p.Type);
                    foreach (var r in s.Returns.FilteredChildren<Parameter>()) {
                        string id = r.Name;
                        if (string.IsNullOrEmpty(r.Name))
                            id = $"_r{CurrentScope.ReturnIndex}";
                        DefineVariable<ReturnVariable>(id, false, r.Type);
                    }

                    base.Process(input);
                    break;

                case IdentifierExpression ie:
                    ie.Parent.Replace(ie, Resolve(ie.Identifier));
                    break;

                case Package p:
                    PushScope();
                    base.Process(input);
                    PopScope();
                    break;
                case Function f:
                    PushScope();
                    base.Process(input);
                    PopScope();
                    break;
                default:
                    base.Process(input);
                    break;                
            }

            
        }

        private Variable Resolve(string identifier)
        {
            for(int i=m_scopes.Count-1;i >= 0; i--)
            {
                if (m_scopes[i].Vars.ContainsKey(identifier))
                    return m_scopes[i].Vars[identifier];
            }

            return null;
        }

        public void PushScope()
        {
            m_scopes.Add(new ResolvingScope() { Vars = new Dictionary<string, Variable>(), LocalIndex = 0 } );
        }

        public void PopScope()
        {
            m_scopes.RemoveAt(m_scopes.Count - 1);
        }

        private T DefineVariable<T>(string id, bool reference, AST.Type rawType) where T : Variable
        {
            int index = 0;
            if (typeof(T) == typeof(LocalVariable))
                index = CurrentScope.LocalIndex++;
            else if (typeof(T) == typeof(ParameterVariable))
                index = CurrentScope.ParameterIndex++;
            else if (typeof(T) == typeof(ReturnVariable))
                index = CurrentScope.ReturnIndex++;

            T inst = (T)Activator.CreateInstance(typeof(T), new object[] { index, reference });
            inst.ResolvedType = rawType.Clone();
            CurrentScope.Vars.Add(id, inst);
            return inst;
        }
    }
}
