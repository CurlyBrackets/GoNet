using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;
using System.IO;
using GoNet.Utils;

namespace GoNet
{
    class AstPrinter : AstProcessor
    {
        private TextWriter m_o;
        private int m_indent;
        private bool m_functionLeaf;

        public AstPrinter(TextWriter o = null)
        {
            if (o == null)
                m_o = Console.Out;
            else
                m_o = o;
            m_indent = 0;
            m_functionLeaf = false;
        }

        public override void Process(Node input)
        {
            switch (input)
            {
                case Package p:
                    if (p.Imported)
                        return;

                    PrintIndent();
                    m_o.WriteLine($"Package {p.Name}");
                    IncreaseIndent();
                    DoScope(p);
                    for (int i = 0; i < p.NumChildren(); i++)
                    {
                        var child = p.GetChild(i);
                        if (!(child is ConstDeclaration) && !(child is TypeDeclaration) && !(child is VarDeclaration))
                            Process(child);
                    }
                    DecreaseIndent();
                    break;
                case Function f:
                    PrintIndent();
                    m_o.WriteLine($"Function: {f.Name}");
                    IncreaseIndent();
                    PrintIndent();
                    m_o.WriteLine("Signature:");
                    IncreaseIndent();
                    Process(f.Signature);
                    DecreaseIndent();

                    if (f.IsExternal)
                    {
                        PrintIndent();
                        m_o.WriteLine("Exported");
                    }
                    else if(!m_functionLeaf)
                    {
                        PrintIndent();
                        m_o.WriteLine("Body:");
                        IncreaseIndent();
                        Process(f.Body);
                        DecreaseIndent();
                    }

                    DecreaseIndent();
                    break;
                case Signature s:
                    NestedField("Returns", s.Returns);
                    NestedField("Parameters", s.Parameters);
                    break;
                case Parameter p:
                    PrintIndent();
                    m_o.WriteLine($"Parameter: {p.Name}");
                    IncreaseIndent();
                    NestedField("Type", p.Type);
                    DecreaseIndent();
                    break;
                case TypeName tn:
                    PrintIndent();
                    m_o.WriteLine($"TypeName: {tn.Name}");
                    break;
                case Block b:
                    PrintIndent();
                    m_o.WriteLine("Block:");
                    IncreaseIndent();
                    DoScope(b);
                    for (int i = 0; i < b.NumChildren(); i++) {
                        var child = b.GetChild(i);
                        if (!(child is ConstDeclaration) && !(child is TypeDeclaration) && !(child is VarDeclaration))
                            Process(child);
                    }
                    DecreaseIndent();
                    break;
                case ReturnAssignment ra:
                    PrintIndent();
                    m_o.WriteLine("Return assignment:");
                    IncreaseIndent();
                    NestedField("Subjects", ra.Identifiers);
                    NestedField("Invocation", ra.Invocation);
                    DecreaseIndent();
                    break;
                case IfStatement ifs:
                    PrintIndent();
                    m_o.WriteLine("If statement:");
                    IncreaseIndent();
                    NestedField("Preamble", ifs.Preamble);
                    NestedField("Condition", ifs.Condition);
                    NestedField("True branch", ifs.True);
                    NestedField("False branch", ifs.False);

                    DecreaseIndent();
                    break;
                case ReturnStatement rs:
                    PrintIndent();
                    m_o.WriteLine("Return:");
                    IncreaseIndent();
                    base.Process(rs);
                    DecreaseIndent();
                    break;
                case IdentifierExpression ie:
                    PrintIndent();
                    m_o.WriteLine($"[Identifier {ie.Identifier}]");
                    break;
                case BinaryExpression be:
                    PrintIndent();
                    m_o.WriteLine("Binary expression:");
                    IncreaseIndent();
                    PrintIndent();
                    m_o.WriteLine($"Operation {be.Operation}");
                    NestedField("Left", be.Left);
                    NestedField("Right", be.Right);
                    DecreaseIndent();
                    break;
                case IntegerLiteral il:
                    PrintIndent();
                    m_o.WriteLine($"[IntLit {il.Value}]");
                    break;
                case UnaryExpression ue:
                    PrintIndent();
                    m_o.WriteLine("Unary expression:");
                    IncreaseIndent();
                    PrintIndent();
                    m_o.WriteLine($"Operation {ue.Op}");
                    NestedField("Subexpr", ue.Expr);
                    DecreaseIndent();
                    break;
                case InvocationExpression ie:
                    PrintIndent();
                    m_o.WriteLine("Invocation expression:");
                    IncreaseIndent();
                    if (ie.VariableLength)
                    {
                        PrintIndent();
                        m_o.WriteLine("Variable length");
                    }

                    m_functionLeaf = true;
                    NestedField("Subject", ie.Subject);
                    m_functionLeaf = false;
                    NestedField("Arguments", ie.Arguments);
                    DecreaseIndent();
                    break;
                case ExpressionSwitch es:
                    PrintIndent();
                    m_o.WriteLine("Expression switch");
                    IncreaseIndent();
                    NestedField("Preamble", es.Preamble);
                    NestedField("Expression", es.Expression);
                    PrintIndent();
                    m_o.WriteLine("Clauses:");
                    IncreaseIndent();
                    base.Process(es);
                    DecreaseIndent();
                    DecreaseIndent();
                    break;
                case ExpressionSwitchClause esc:
                    PrintIndent();
                    m_o.WriteLine("Expression switch clause");
                    IncreaseIndent();
                    DoScope(esc);
                    NestedField("Condition", esc.Condition);
                    NestedField("Statements", esc.Statements);
                    DecreaseIndent();
                    break;
                case Assignment a:
                    PrintIndent();
                    m_o.WriteLine("Assignment");
                    IncreaseIndent();
                    PrintIndent();
                    m_o.WriteLine($"Type: {a.Operation}");
                    NestedField("Identifier", a.Identifier);
                    NestedField("Value", a.Value);
                    DecreaseIndent();
                    break;
                case VarDeclaration vd:
                    PrintIndent();
                    m_o.WriteLine($"Var decl: {vd.Identifier}");
                    IncreaseIndent();
                    NestedField("Type", vd.Type);
                    DecreaseIndent();
                    break;
                case ExpressionType et:
                    PrintIndent();
                    m_o.WriteLine("Expression type:");
                    IncreaseIndent();
                    NestedField("Expression", et.Expr);
                    DecreaseIndent();
                    break;
                case ConstDeclaration cd:
                    PrintIndent();
                    m_o.WriteLine("Const decl:");
                    IncreaseIndent();
                    PrintIndent();
                    m_o.WriteLine($"Identifier: {cd.Identifier}");
                    NestedField("Value", cd.Value);
                    DecreaseIndent();
                    break;
                case FloatLiteral fl:
                    PrintIndent();
                    m_o.WriteLine($"[FloatLit {fl.Value.ToDecimalString()}]");
                    break;
                case DefaultCase d:
                    PrintIndent();
                    m_o.WriteLine("Default");
                    break;
                case ArrayType at:
                    PrintIndent();
                    m_o.WriteLine("ArrayType:");
                    IncreaseIndent();
                    if (at.AnyLength)
                    {
                        PrintIndent();
                        m_o.WriteLine("Any length");
                    }
                    else
                    {
                        PrintIndent();
                        m_o.WriteLine($"Length {at.Length}");
                    }
                    NestedField("Element type", at.ElementType);
                    DecreaseIndent();
                    break;
                case CompositeLiteral cl:
                    PrintIndent();
                    m_o.WriteLine("Composite literal:");
                    IncreaseIndent();
                    NestedField("Type", cl.Type);
                    NestedField("Elements", cl.Elements);
                    DecreaseIndent();
                    break;
                case KeyedElement ke:
                    PrintIndent();
                    m_o.WriteLine("Keyed Element:");
                    IncreaseIndent();
                    NestedField("Key", ke.Key);
                    NestedField("Value", ke.Element);
                    DecreaseIndent();
                    break;
                case IndexExpression ie:
                    PrintIndent();
                    m_o.WriteLine("Index expression:");
                    IncreaseIndent();
                    NestedField("Target", ie.Target);
                    NestedField("Index", ie.Index);
                    DecreaseIndent();
                    break;
                case ForStatement fs:
                    PrintIndent();
                    m_o.WriteLine("For statement:");
                    IncreaseIndent();
                    DoScope(fs);
                    NestedField("Clause", fs.Clause);
                    NestedField("Body", fs.Body);
                    DecreaseIndent();
                    break;
                case ConditionClause cc:
                    PrintIndent();
                    m_o.WriteLine("Condition clause:");
                    IncreaseIndent();
                    NestedField("Expression", cc.Expression);
                    DecreaseIndent();
                    break;
                case IterativeClause ic:
                    PrintIndent();
                    m_o.WriteLine("Iterative clause:");
                    IncreaseIndent();
                    NestedField("Preamble", ic.Preamble);
                    NestedField("Condition", ic.Condition);
                    NestedField("Afterword", ic.Afterword);
                    DecreaseIndent();
                    break;
                case RangeClause rc:
                    PrintIndent();
                    m_o.WriteLine("Range clause:");
                    IncreaseIndent();
                    NestedField("Values", rc.Values);
                    NestedField("Iterator", rc.Iterator);
                    DecreaseIndent();
                    break;
                case IteratorType it:
                    PrintIndent();
                    m_o.WriteLine("Iterator type:");
                    IncreaseIndent();
                    NestedField("Base type", it.BaseType);
                    PrintIndent();
                    m_o.WriteLine($"Index: {it.Index}");
                    DecreaseIndent();
                    break;
                case IncDecStatement ids:
                    PrintIndent();
                    m_o.WriteLine("IncDec Statement:");
                    IncreaseIndent();
                    NestedField("Expression", ids.Expression);
                    PrintIndent();
                    m_o.WriteLine($"Is increment: {ids.Increment}");
                    DecreaseIndent();
                    break;
                case FallthroughStatement fs:
                    PrintIndent();
                    m_o.WriteLine("Fallthrough Statement:");
                    break;
                case QualifiedIdentifier qi:
                    PrintIndent();
                    m_o.WriteLine($"[Qualified ident {qi.Scope}::{qi.Identifier}]");
                    break;
                case ConversionExpression ce:
                    PrintIndent();
                    m_o.WriteLine("Conversion expression:");
                    IncreaseIndent();
                    NestedField("To", ce.To);
                    NestedField("From", ce.From);
                    DecreaseIndent();
                    break;
                case SelectorExpression se:
                    PrintIndent();
                    m_o.WriteLine("Selector expression:");
                    IncreaseIndent();
                    NestedField("Left", se.Left);
                    PrintIndent();
                    m_o.WriteLine($"Right: {se.Right}");
                    DecreaseIndent();
                    break;
                case ImportDeclaration id:
                    PrintIndent();
                    m_o.WriteLine("Import declaration");
                    IncreaseIndent();
                    PrintIndent();
                    m_o.WriteLine($"Package: {id.Package}");
                    if (!string.IsNullOrEmpty(id.Alias))
                    {
                        PrintIndent();
                        m_o.WriteLine($"Alias: {id.Alias}");
                    }
                    DecreaseIndent();
                    break;
                case AnyType t:
                    PrintIndent();
                    m_o.WriteLine("Any type");
                    break;
                case SliceType st:
                    PrintIndent();
                    m_o.WriteLine("Slice type");
                    IncreaseIndent();
                    NestedField("Element type", st.ElementType);
                    DecreaseIndent();
                    break;
                case MapType mt:
                    PrintIndent();
                    m_o.WriteLine("Map type");
                    IncreaseIndent();
                    NestedField("Key type", mt.KeyType);
                    NestedField("Value type", mt.ValueType);
                    DecreaseIndent();
                    break;
                case ChannelType ct:
                    PrintIndent();
                    m_o.WriteLine("Channel type");
                    IncreaseIndent();
                    NestedField("Element type", ct.ElementType);
                    DecreaseIndent();
                    break;
                case BuiltinType bt:
                    PrintIndent();
                    m_o.WriteLine($"Builtin type {bt.Type}");
                    break;
                case RealType rt:
                    PrintIndent();
                    m_o.WriteLine($"Real type {rt.Type}");
                    break;
                case ParameterVariable pv:
                    PrintIndent();
                    m_o.WriteLine($"Parameter variable: {(pv.Reference ? "&" : "")}{pv.Slot}");
                    break;
                case LocalVariable lv:
                    PrintIndent();
                    m_o.WriteLine($"Local variable: {(lv.Reference ? "&" : "")}{lv.Slot}");
                    break;
                case ReturnVariable rv:
                    PrintIndent();
                    m_o.WriteLine($"Return variable: {rv.Slot}");
                    break;
                case BooleanLiteral bl:
                    PrintIndent();
                    m_o.WriteLine($"Boolean literal: {bl.IsTrue}");
                    break;
                default:
                    base.Process(input);
                    break;
            }
        }

        private void DoScope(IScope s)
        {
            if (s.NumConstDeclarations() > 0) {
                PrintIndent();
                m_o.WriteLine("Const declarations:");
                IncreaseIndent();
                for (int i = 0; i < s.NumConstDeclarations(); i++)
                    Process(s.GetConstDeclaration(i));
                DecreaseIndent();
            }

            if (s.NumTypeDeclarations() > 0)
            {
                PrintIndent();
                m_o.WriteLine("Type declarations:");
                IncreaseIndent();
                for (int i = 0; i < s.NumTypeDeclarations(); i++)
                    Process(s.GetTypeDeclaration(i));
                DecreaseIndent();
            }

            if (s.NumVarDeclarations() > 0)
            {
                PrintIndent();
                m_o.WriteLine("Var declarations:");
                IncreaseIndent();
                for (int i = 0; i < s.NumVarDeclarations(); i++)
                    Process(s.GetVarDeclaration(i));
                DecreaseIndent();
            }
        }

        private void NestedField(string name, Node field, bool disableNullCheck = false)
        {
            if (field != null || disableNullCheck)
            {
                PrintIndent();
                m_o.WriteLine(name + ":");
                IncreaseIndent();
                Process(field);
                DecreaseIndent();
            }
        }

        private void PrintIndent()
        {
            for (int i = 0; i < m_indent; i++)
                m_o.Write("|  ");
        }

        private void IncreaseIndent()
        {
            m_indent++;
        }

        private void DecreaseIndent()
        {
            m_indent--;
        }
    }
}
