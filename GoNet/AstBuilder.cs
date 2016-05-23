using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using GoNet.AST;
using System.Numerics;
using Numerics;
using GoNet.Parser;

namespace GoNet
{
    class AstBuilder : Parser.GolangBaseVisitor<AST.Base>
    {
        private Root m_root;
        private Package m_currentPackage;
        private Scope m_currentScope;

        public AstBuilder(Root root)
        {
            m_root = root;
            Reset();
        }

        private void Reset()
        {
            m_currentPackage = null;
            m_currentScope = null;
        }

        public override Base VisitPackage_clause([NotNull] GolangParser.Package_clauseContext context)
        {
            var identifier = context.package_name().Identifier().Accept(this) as RawNode;
            Package ret = m_root.GetPackage(identifier.Text);

            m_currentPackage = ret;
            m_currentScope = ret;
            return ret;
        }

        public override Base VisitImport_spec([NotNull] GolangParser.Import_specContext context)
        {
            ImportDeclaration ret = null;
            var first = context.GetChild(0).Accept(this) as RawNode;
            if(context.ChildCount == 2)
            {
                var second = context.GetChild(1).Accept(this) as RawNode;
                ret = new ImportDeclaration(second.Text.Trim('"'), first.Text);
            }
            else
            {
                ret = new ImportDeclaration(first.Text.Trim('"'), string.Empty);
            }

            m_currentPackage.AddImport(ret);
            return ret;
        }

        public override Base VisitType_spec([NotNull] GolangParser.Type_specContext context)
        {
            TypeDeclaration ret = null;

            var name = context.Identifier().Accept(this) as RawNode;
            var type = context.type().Accept(this) as AST.Type;
            ret = new TypeDeclaration(name.Text, type);

            m_currentScope.AddTypeDeclaration(ret);
            return ret;
        }

        public override Base VisitPointer_type([NotNull] GolangParser.Pointer_typeContext context)
        {
            return new PointerType(context.type().Accept(this) as AST.Type);
        }

        public override Base VisitType_name([NotNull] GolangParser.Type_nameContext context)
        {
            return new TypeName(context.GetText());
        }

        public override Base VisitFunc_decl([NotNull] GolangParser.Func_declContext context)
        {
            var name = context.Identifier().Accept(this) as RawNode;

            // func decls are purely functions, not methods
            Function ret = new Function(name.Text, null);

            ret.Signature = context.signature().Accept(this) as Signature;
            if (context.block() != null)
            {
                var temp = m_currentScope;
                m_currentScope = ret;

                ret.Body = context.block().Accept(this) as Block;

                m_currentScope = temp;
            }

            m_currentPackage.AddFunctionDeclaration(ret);
            return ret;
        }

        public override Base VisitSignature([NotNull] GolangParser.SignatureContext context)
        {
            var ret = new Signature();

            ret.Parameters = context.parameters().Accept(this) as Parameters;
            if(context.result() != null)
            {
                var result = context.result().Accept(this);
                switch (result)
                {
                    case AST.Type t:
                        ret.Returns = new Parameters();
                        ret.Returns.AddChild(new Parameter(string.Empty, t));
                        break;
                    case Parameters p:
                        ret.Returns = p;
                        break;
                }
            }

            return ret;
        }

        public override Base VisitParameters([NotNull] GolangParser.ParametersContext context)
        {
            if (context.parameter_list() != null)
                return context.parameter_list().Accept(this);
            return null;
        }

        public override Base VisitParameter_list([NotNull] GolangParser.Parameter_listContext context)
        {
            var ret = new Parameters();

            foreach(var child in context.children)
            {
                if (child is GolangParser.Parameter_declContext)
                {
                    var decl = child.Accept(this);
                    switch (decl)
                    {
                        case Parameters ps:
                            while (ps.NumChildren() > 0)
                                ret.AddChild(ps.GetChild<Parameter>(0));
                            break;
                        case Parameter p:
                            ret.AddChild(p);
                            break;
                    }
                }
            }

            return ret;
        }

        public override Base VisitParameter_decl([NotNull] GolangParser.Parameter_declContext context)
        {
            var type = context.type().Accept(this) as AST.Type;
            if (context.EllipsesOperator() != null)
                type = new RangeType(type);

            if (context.identifier_list() != null)
            {
                var ret = new Parameters();

                var nodeList = context.identifier_list().Accept(this) as RawNodeList;
                foreach (var node in nodeList.Items)
                    ret.AddChild(new Parameter(node.Text, type));

                return ret;
            }
            else
                return new Parameter(string.Empty, type);
        }

        public override Base VisitBlock([NotNull] GolangParser.BlockContext context)
        {
            var sl = context.statement_list().Accept(this) as StatementList;

            var ret = new Block();

            foreach (var s in sl.Items)
                ret.AddChild(s);

            return ret;
        }

        public override Base VisitStatement_list([NotNull] GolangParser.Statement_listContext context)
        {
            var ret = new StatementList();

            foreach(var child in context.children)
            {
                if (child is GolangParser.StatementContext)
                    ret.Items.Add(child.Accept(this) as Statement);
            }

            return ret;
        }

        public override Base VisitIf_statement([NotNull] GolangParser.If_statementContext context)
        {
            var ret = new IfStatement(
                context.expression().Accept(this) as Expression,
                context.block(0).Accept(this) as Block);

            if (context.simple_statement() != null)
                ret.Preamble = context.simple_statement().Accept(this) as Statement;
            if (context.block(1) != null)
                ret.False = context.block(1).Accept(this) as Block;
            else if (context.if_statement() != null)
                ret.False = context.if_statement().Accept(this) as IfStatement;

            return ret;
        }

        public override Base VisitReturn_statement([NotNull] GolangParser.Return_statementContext context)
        {
            var ret = new ReturnStatement();

            if (context.expression_list() != null)
            {
                var el = context.expression_list().Accept(this) as ExpressionList;
                foreach (var e in el.Items)
                    ret.AddChild(e);
            }

            return ret;
        }

        public override Base VisitExpression_list([NotNull] GolangParser.Expression_listContext context)
        {
            var ret = new ExpressionList();

            foreach (var child in context.children)
            {
                if (child is GolangParser.ExpressionContext)
                    ret.Items.Add(child.Accept(this) as Expression);
            }

            return ret;
        }

        public override Base VisitIdentifier_list([NotNull] GolangParser.Identifier_listContext context)
        {
            var ret = new RawNodeList();

            foreach (var child in context.children)
            {
                if (child is ITerminalNode)
                    ret.Items.Add(child.Accept(this) as RawNode);
            }

            return ret;
        }
        public override Base VisitExpression([NotNull] GolangParser.ExpressionContext context)
        {
            if (context.unary_expr() != null)
                return context.unary_expr().Accept(this);
            else
            {
                return new BinaryExpression(
                    (context.binary_op().Accept(this) as BinaryOpWrapper).Op,
                    context.expression(0).Accept(this) as Expression,
                    context.expression(1).Accept(this) as Expression);
            }
        }

        public override Base VisitBinary_op([NotNull] GolangParser.Binary_opContext context)
        {
            BinaryOp op;
            switch (context.GetText())
            {
                case "||":
                    op = BinaryOp.LogicalOr;
                    break;
                case "&&":
                    op = BinaryOp.LogicalAnd;
                    break;
                case "==":
                    op = BinaryOp.LogicalEquals;
                    break;
                case "!=":
                    op = BinaryOp.NotEquals;
                    break;
                case "<":
                    op = BinaryOp.LessThan;
                    break;
                case "<=":
                    op = BinaryOp.LessEqual;
                    break;
                case ">=":
                    op = BinaryOp.GreaterEqual;
                    break;
                case ">":
                    op = BinaryOp.GreaterThan;
                    break;
                case "+":
                    op = BinaryOp.Add;
                    break;
                case "-":
                    op = BinaryOp.Subtract;
                    break;
                case "|":
                    op = BinaryOp.Or;
                    break;
                case "^":
                    op = BinaryOp.Xor;
                    break;
                case "*":
                    op = BinaryOp.Multiply;
                    break;
                case "/":
                    op = BinaryOp.Divide;
                    break;
                case "%":
                    op = BinaryOp.Modulus;
                    break;
                case "<<":
                    op = BinaryOp.ShiftLeft;
                    break;
                case ">>":
                    op = BinaryOp.ShiftRight;
                    break;
                case "&":
                    op = BinaryOp.And;
                    break;
                case "&^":
                    op = BinaryOp.AndNot;
                    break;

                default:
                    op = BinaryOp.Unknown;
                    break;
            }

            return new BinaryOpWrapper(op);
        }

        public override Base VisitBasic_literal([NotNull] GolangParser.Basic_literalContext context)
        {
            if (context.IntegerLiteral() != null)
            {
                return new IntegerLiteral(
                    BigInteger.Parse(context.IntegerLiteral().GetText()));
            }
            else if (context.FloatLiteral() != null)
            {
                return new FloatLiteral(
                    Utils.BigRationalExtensions.Parse(context.FloatLiteral().GetText()));
            }
            else if (context.ImaginaryLiteral() != null)
            {
                var text = context.ImaginaryLiteral().GetText();
                if (text.Contains('.') || text.IndexOf("e", StringComparison.InvariantCultureIgnoreCase) != -1)
                    return new ImaginaryLiteral(Utils.BigRationalExtensions.Parse(text));
                else
                    return new ImaginaryLiteral(BigInteger.Parse(text));
            }
            else if(context.RuneLiteral() != null)
            {
                var str = context.RuneLiteral().GetText().Trim('\'');
                return new RuneLiteral(char.ConvertToUtf32(str, 0));
            }
            else
            {
                var str = context.StringLiteral().GetText().Trim('"');
                return new StringLiteral(str);
            }
        }

        public override Base VisitShort_var_decl([NotNull] GolangParser.Short_var_declContext context)
        {
            var ret = new VarDeclarationList();

            var identList = context.identifier_list().Accept(this) as RawNodeList;
            var exprList = context.expression_list().Accept(this) as ExpressionList;

            for(int i = 0; i < identList.Items.Count; i++)
            {
                var vd = new VarDeclaration(identList.Items[i].Text, exprList.Items[i]);
                m_currentScope.AddVarDeclaration(vd);
                ret.Items.Add(vd);
            }

            return ret;
        }

        public override Base VisitOperand_name([NotNull] GolangParser.Operand_nameContext context)
        {
            string id = null;
            if (context.Identifier() != null)
                id = context.Identifier().GetText();
            else if (context.qualified_identifier() != null)
                id = context.qualified_identifier().GetText();

            return new IdentifierExpression(id);
        }

        public override Base VisitTerminal(ITerminalNode node)
        {
            return new RawNode(node.GetText());
        }
    }
}
