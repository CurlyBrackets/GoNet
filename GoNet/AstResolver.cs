using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;
using GoNet.Utils;

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
        private BinnedDictionary<string, Function> m_packageFuncs;

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
            m_packageFuncs = new BinnedDictionary<string, Function>();
        }

        private Signature GenerateSignature(Parameter[] parms, Parameter[] rets)
        {
            var ret = new Signature();

            if (parms.Length > 0)
            {
                ret.Parameters = new Parameters();
                foreach (var p in parms)
                    ret.Parameters.AddChild(p);
            }
            if(rets.Length > 0)
            {
                ret.Returns = new Parameters();
                foreach (var r in rets)
                    ret.Returns.AddChild(r);
            }

            return ret;
        }

        private void PopulateBuiltins()
        {
            #region len
            // array
            m_packageFuncs.Add(
                "len", new Function("len", null)
                {
                    Signature = GenerateSignature(
                        new Parameter[] { new Parameter("arr", new ArrayType(AST.Type.Any)) },
                        new Parameter[] { new Parameter(null, BuiltinType.FromTypeName("int")) })
                });
            // pointer to array
            m_packageFuncs.Add(
                "len", new Function("len", null)
                {
                    Signature = GenerateSignature(
                        new Parameter[] { new Parameter("arr", new PointerType(new ArrayType(AST.Type.Any))) },
                        new Parameter[] { new Parameter(null, BuiltinType.FromTypeName("int")) })
                });
            // slice
            m_packageFuncs.Add(
                "len", new Function("len", null)
                {
                    Signature = GenerateSignature(
                        new Parameter[] { new Parameter("slice", new SliceType(AST.Type.Any)) },
                        new Parameter[] { new Parameter(null, BuiltinType.FromTypeName("int")) })
                });
            // map
            m_packageFuncs.Add(
                "len", new Function("len", null)
                {
                    Signature = GenerateSignature(
                        new Parameter[] { new Parameter("map", new MapType(AST.Type.Any, AST.Type.Any)) },
                        new Parameter[] { new Parameter(null, BuiltinType.FromTypeName("int")) })
                });
            // string
            m_packageFuncs.Add(
                "len", new Function("len", null)
                {
                    Signature = GenerateSignature(
                        new Parameter[] { new Parameter("str", BuiltinType.FromTypeName("string")) },
                        new Parameter[] { new Parameter(null, BuiltinType.FromTypeName("int")) })
                });
            // channel
            m_packageFuncs.Add(
                "len", new Function("len", null)
                {
                    Signature = GenerateSignature(
                        new Parameter[] { new Parameter("channel", new ChannelType(AST.Type.Any)) },
                        new Parameter[] { new Parameter(null, BuiltinType.FromTypeName("int")) })
                });
            #endregion
        }

        public override void Process(Node input)
        {
            switch (input)
            {
                case VarDeclaration vd:
                    Process(vd.Type);
                    DefineVariable<LocalVariable>(vd.Identifier, false, vd.Type);
                    break;
                case ExpressionType et:
                    Process(et.Expr);
                    break;
                case Signature s:
                    if (s.Parameters != null)
                    {
                        foreach (var p in s.Parameters.FilteredChildren<Parameter>())
                            DefineVariable<ParameterVariable>(p.Name, p.Type is PointerType, p.Type);
                    }

                    if (s.Returns != null)
                    {
                        foreach (var r in s.Returns.FilteredChildren<Parameter>())
                        {
                            string id = r.Name;
                            if (string.IsNullOrEmpty(r.Name))
                                id = $"_r{CurrentScope.ReturnIndex}";
                            DefineVariable<ReturnVariable>(id, false, r.Type);
                        }
                    }

                    base.Process(input);
                    break;

                case IdentifierExpression ie:
                    var func = ResolveFunction(ie.Identifier);
                    if (func != null)
                    {
                        ie.Parent.Replace(ie, func);
                        break;
                    }

                    var val = Resolve(ie.Identifier);
                    if (val != null)
                        ie.Parent.Replace(ie, val);
                    else
                        throw new Exception($"Unbound identifier: {ie.Identifier}");

                    break;

                case Package p:
                    if (p.Imported)
                        return;

                    m_packageFuncs.Clear();
                    PopulateBuiltins();
                    for (int i = 0; i < p.NumFunctionDeclarations(); i++)
                    {
                        var f = p.GetFunctionDeclaration(i);
                        m_packageFuncs.Add(f.Name, f);
                    }             
                               
                    PushScope();
                    DoScope(p);
                    for (int i = 0; i < p.NumChildren(); i++)
                    {
                        var c = p.GetChild(i);
                        if (!(c is VarDeclaration))
                            Process(c);
                        else
                            Process((c as VarDeclaration).Type);
                    }
                    PopScope();
                    break;
                case Block b:
                    PushScope();
                    DoScope(b);
                    for (int i = 1; i < b.NumChildren(); i++)
                        Process(b.GetChild(i));
                    PopScope();
                    break;
                case ForStatement fs:
                    PushScope();
                    DoScope(fs);
                    for (int i = 1; i < fs.NumChildren(); i++)
                        Process(fs.GetChild(i));
                    PopScope();
                    break;
                case ExpressionSwitchClause esc:
                    PushScope();
                    Process(esc.Condition);
                    DoScope(esc);
                    Process(esc.Statements);
                    PopScope();
                    break;
                case Function f:
                    PushScope();
                    base.Process(f);
                    PopScope();
                    break;
                default:
                    base.Process(input);
                    break;                
            }

            
        }

        private void DoScope(IScope s)
        {
            for (int i = 0; i < s.NumVarDeclarations(); i++) {
                var vd = s.GetVarDeclaration(i);
                Process(vd.Type);
                DefineVariable<LocalVariable>(vd.Identifier, false, vd.Type);
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

        private Node ResolveFunction(string identifier)
        {
            if (m_packageFuncs.ContainsKey(identifier))
            {
                var res = m_packageFuncs[identifier];
                if (res.Count == 1)
                    return res[0];
                else
                {
                    var ret = new OverloadedFunction(identifier);
                    foreach (var f in res)
                        ret.AddChild(f.Clone());
                    return ret;
                }
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
            inst.ResolvedType = rawType.CloneType();
            CurrentScope.Vars.Add(id, inst);
            return inst;
        }
    }
}
