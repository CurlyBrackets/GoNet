using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;
using System.Reflection.Emit;
using System.Reflection;
using GoNet.IL;

namespace GoNet
{
    class Compiler : AstProcessor
    {
        private const string LibraryExtension = ".dll";
        private const string ApplicationExtension = ".exe";
        private const string AssemblyNamePrefix = "GoNet";

        private string m_filename;
        private AppDomain m_ad;
        private AssemblyBuilder m_ab;
        private ModuleBuilder m_modb;
        private TypeBuilder m_tb;
        private MethodBuilder m_mb;

        private bool m_doingReturns, m_doingParams;
        private int m_returnIndex, m_paramIndex;

        public Compiler(string name, bool library)
        {
            var an = new AssemblyName() { Name = AssemblyNamePrefix + name };
            m_ad = AppDomain.CurrentDomain;
            m_ab = m_ad.DefineDynamicAssembly(an, AssemblyBuilderAccess.Save);

            m_filename = name + (library ? LibraryExtension : ApplicationExtension);
            m_modb = m_ab.DefineDynamicModule(an.Name, m_filename);

            m_doingParams = false;
            m_doingReturns = false;
        }

        public void Finalize()
        {
            m_ab.Save(m_filename);
        }

        public override void Process(Node input)
        {
            switch (input) {
                case Package p:
                    m_tb = m_modb.DefineType(p.Name, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.Abstract | TypeAttributes.Sealed | TypeAttributes.BeforeFieldInit);
                    base.Process(input);
                    m_tb.CreateType();
                    break;
                case Function f:
                    m_mb = m_tb.DefineMethod(
                        f.Name,
                        (f.IsExported ? MethodAttributes.Public : MethodAttributes.Assembly) |
                        (f.Owner == null ? MethodAttributes.Static : 0) |
                        (f.IsExternal ? MethodAttributes.PinvokeImpl : 0) |
                        MethodAttributes.Final | MethodAttributes.HideBySig,
                        ResolveType(f.Signature.Returns.GetChild<Parameter>(0).Type),
                        AssembleParameters(f.Signature)
                        );
                    m_returnIndex = 0;
                    m_paramIndex = 0;

                    base.Process(input);
                    break;
                case IlBlock ilb:
                    BasicBlock basicBlock = ilb.GetChild<BasicBlock>(0);
                    Schedule(basicBlock);
                    base.Process(input);
                    break;
                case Parameter p:
                    if (m_doingParams)
                    {
                        m_mb.DefineParameter(
                            m_paramIndex++,
                            ParameterAttributes.In,
                            p.Name);
                    }
                    else if(m_doingReturns)
                    {
                        if (m_returnIndex == 0)
                            m_returnIndex++;
                        else
                        {
                            m_mb.DefineParameter(
                                m_paramIndex++,
                                ParameterAttributes.Out,
                                p.Name);
                            m_returnIndex++;
                        }                        
                    }
                    break;
                case Signature s:
                    m_doingReturns = true;
                    base.Process(s.Returns);
                    m_doingReturns = false;
                    m_doingParams = true;
                    base.Process(s.Parameters);
                    m_doingParams = false;
                    break;
                default:
                    base.Process(input);
                    break;
            }
        }

        private void Schedule(BasicBlock block)
        {
            var ilgen = m_mb.GetILGenerator();
            List<BasicBlock> orderedBlocks = new List<BasicBlock>(), trueBlocks = new List<BasicBlock>();
            BasicBlock currentBlock = block;
            var labels = new Dictionary<int, Label>();

            while (currentBlock != null)
            {
                var label = ilgen.DefineLabel();
                labels.Add(currentBlock.BlockIndex, label);

                orderedBlocks.Add(currentBlock);
                trueBlocks.Add(currentBlock.TrueBranch as BasicBlock);
                currentBlock = currentBlock.FalseBranch as BasicBlock;
            }

            while(trueBlocks.Count > 0)
            {
                currentBlock = trueBlocks[0];
                trueBlocks.RemoveAt(0);

                while (currentBlock != null)
                {
                    var label = ilgen.DefineLabel();
                    labels.Add(currentBlock.BlockIndex, label);

                    orderedBlocks.Add(currentBlock);
                    trueBlocks.Add(currentBlock.TrueBranch as BasicBlock);
                    currentBlock = currentBlock.FalseBranch as BasicBlock;
                }
            }

            foreach(var ob in orderedBlocks)
            {
                ilgen.MarkLabel(labels[ob.BlockIndex]);
                foreach(var inst in ob.Instructions.FilteredChildren<Instruction>())
                {
                    if(inst.Type == EInstruction.Call || inst.Type == EInstruction.Callvirt)
                    {
                        throw new NotImplementedException("Calls inside the same module are not implemented");
                    }
                    else if(inst.Type == EInstruction.Calli)
                    {
                        throw new NotImplementedException("Calls to other libraries are not implemented");
                    }
                    else if (IsBranch(inst.Type))
                    {
                        ilgen.Emit(GetOpCode(inst.Type), labels[ob.TrueBranch.BlockIndex]);
                    }
                    else
                    {
                        if (inst.Argument != null)
                        {
                            ilgen.GetType()
                                .GetMethod(
                                    "Emit", 
                                    new System.Type[] { typeof(OpCode), inst.Argument.GetType() })
                                .Invoke(
                                    ilgen, 
                                    new object[] { GetOpCode(inst.Type), inst.Argument });
                        }
                        else
                            ilgen.Emit(GetOpCode(inst.Type));
                    }
                }
            }
        }

        private bool IsBranch(EInstruction inst)
        {
            return inst >= EInstruction.Beq && inst <= EInstruction.BrTrue_S;
        }

        private OpCode GetOpCode(EInstruction type)
        {
            return (OpCode)(typeof(OpCodes).GetField(type.ToString(), BindingFlags.Static | BindingFlags.Public).GetValue(null));
        }

        private System.Type ResolveType(AST.Type t)
        {
            switch (t)
            {
                case BuiltinType bt:
                    return ResolveBuiltinType(bt.Type);
                default:
                    return null;
            }
        }

        private System.Type[] AssembleParameters(Signature s)
        {
            var intermediate = new List<System.Type>();
            for (int i = 1; i < s.Returns.NumChildren(); i++) {
                var t = ResolveType(s.Returns.GetChild<Parameter>(i).Type);
                intermediate.Add(t.MakeByRefType());
            }
            for(int i = 0; i < s.Parameters.NumChildren(); i++)
            {
                intermediate.Add(ResolveType(s.Parameters.GetChild<Parameter>(i).Type));
            }

            return intermediate.ToArray();
        }

        private System.Type ResolveBuiltinType(EBuiltinType bt)
        {
            switch (bt)
            {
                case EBuiltinType.Uint8:
                    return typeof(byte);
                case EBuiltinType.Uint16:
                    return typeof(ushort);
                case EBuiltinType.Uint32:
                    return typeof(uint);
                case EBuiltinType.Uint64:
                    return typeof(ulong);
                case EBuiltinType.Int8:
                    return typeof(sbyte);
                case EBuiltinType.Int16:
                    return typeof(short);
                case EBuiltinType.Int32:
                    return typeof(int);
                case EBuiltinType.Int64:
                    return typeof(long);
                case EBuiltinType.Float32:
                    return typeof(float);
                case EBuiltinType.Float64:
                    return typeof(double);
                case EBuiltinType.Complex64:
                    throw new NotImplementedException();
                case EBuiltinType.Complex128:
                    throw new NotImplementedException();
                case EBuiltinType.Uintptr:
                    return typeof(UIntPtr);
                default:
                    break;
            }

            return null;
        }
    }
}
