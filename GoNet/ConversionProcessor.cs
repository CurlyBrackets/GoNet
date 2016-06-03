using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;

namespace GoNet
{
    class ConversionProcessor : AstProcessor
    {
        private HashSet<string> m_registeredTypes;

        public ConversionProcessor()
        {
            m_registeredTypes = new HashSet<string>();
        }

        private void PopulateBuiltin()
        {
            foreach (var name in BuiltinType.Names)
                m_registeredTypes.Add(name);
        }

        public override void Process(Node input)
        {
            switch(input)
            {
                case Root r:
                    m_registeredTypes.Clear();
                    PopulateBuiltin();
                    foreach(var p in r.FilteredChildren<Package>())
                    {
                        if(p.Imported)
                        {
                            for (int i = 0; i < p.NumTypeDeclarations(); i++)
                                m_registeredTypes.Add(
                                    p.Name + "." + p.GetTypeDeclaration(i).Name);
                        }
                    }
                    break;
                case InvocationExpression ie:
                    if (ie.Arguments != null && ie.Arguments.NumChildren() == 1)
                    {
                        var t = CanBeType(ie.Subject);
                        if (t != null) {
                            var arg = ie.Arguments.GetChild<Expression>(0);
                            ie.Parent.Replace(
                                ie,
                                new ConversionExpression(
                                    t, arg));
                            Process(arg);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Not a conversion: " + ie.ToString());
                    }
                    break;
                case Package p:
                    if (p.Imported)
                        return;
                    break;
            }

            base.Process(input);
        }

        private AST.Type CanBeType(Node n)
        {
            bool isPointer = false;
            while (n is ExpressionWrapper)
                n = (n as ExpressionWrapper).Expr;

            if (n is UnaryExpression && (n as UnaryExpression).Op == EUnaryOp.Dereference)
            {
                isPointer = true;
                n = (n as UnaryExpression).Expr;
            }

            AST.Type ret = null;
            switch (n)
            {
                case IdentifierExpression ie:
                    if (m_registeredTypes.Contains(ie.Identifier))
                        ret = new TypeName(ie.Identifier);
                    break;
                case SelectorExpression se:
                    var fullname = ProcessSelector(se);
                    if (m_registeredTypes.Contains(fullname))
                        ret = new TypeName(fullname);
                    break;
                default:
                    break;
            }

            if (isPointer && ret != null)
                ret = new PointerType(ret);
            return ret;
        }

        private string ProcessSelector(Expression e)
        {
            switch (e) {
                case IdentifierExpression ie:
                    return ie.Identifier;
                case SelectorExpression se:
                    return ProcessSelector(se.Left) + "." + se.Right;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
