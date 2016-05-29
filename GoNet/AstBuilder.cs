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
        private IScope m_currentScope;

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
                

                ret.Body = context.block().Accept(this) as Block;

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
                    ret.AddChild(new Parameter(node.Text, type.Clone()));

                return ret;
            }
            else
                return new Parameter(string.Empty, type);
        }

        public override Base VisitBlock([NotNull] GolangParser.BlockContext context)
        {
            var ret = new Block();

            var temp = m_currentScope;
            m_currentScope = ret;

            var sl = context.statement_list().Accept(this) as StatementList;

            m_currentScope = temp;
            while (sl.NumChildren() > 0)
                ret.AddChild(sl.GetChild<Statement>(0));

            return ret;
        }

        public override Base VisitStatement_list([NotNull] GolangParser.Statement_listContext context)
        {
            var ret = new StatementList();

            foreach(var child in context.children)
            {
                if (child is GolangParser.StatementContext)
                {
                    var raw = child.Accept(this);
                    switch (raw)
                    {
                        case Statement s:
                            ret.AddChild(s);
                            break;
                        case StatementList list:
                            while (list.NumChildren() > 0)
                                ret.AddChild(list.GetChild<Statement>(0));
                            break;
                    }
                }
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
                while(el.NumChildren() > 0)
                    ret.AddChild(el.GetChild<Expression>(0));
            }

            return ret;
        }

        public override Base VisitPrimary_expr([NotNull] GolangParser.Primary_exprContext context)
        {
            Expression primaryExpr = null;
            if (context.primary_expr() != null)
                primaryExpr = context.primary_expr().Accept(this) as Expression;

            if(context.arguments() != null)
            {
                var args = context.arguments();

                ExpressionList exprList = null;
                if (args.expression_list() != null)
                    exprList = args.expression_list().Accept(this) as ExpressionList;
                return new InvocationExpression(
                    primaryExpr,
                    exprList,
                    args.EllipsesOperator() != null);
            }
            else if(context.index() != null)
            {
                return new IndexExpression(
                    primaryExpr,
                    context.index().Accept(this) as Expression);
            }

            return base.VisitPrimary_expr(context);
        }

        public override Base VisitComposite_literal([NotNull] GolangParser.Composite_literalContext context)
        {
            var type = context.literal_type().Accept(this) as AST.Type;
            var els = context.literal_value().Accept(this) as KeyedElementList;

            if(type is IndeterminateArrayType)
            {
                type = new ArrayType(
                    (type as IndeterminateArrayType).ElementType,
                    els.NumChildren());
            }

            return new CompositeLiteral(
                type,
                els);
        }

        public override Base VisitLiteral_value([NotNull] GolangParser.Literal_valueContext context)
        {
            return context.element_list().Accept(this);
        }

        public override Base VisitLiteral_type([NotNull] GolangParser.Literal_typeContext context)
        {
            if(context.EllipsesOperator() != null)
            {
                return new IndeterminateArrayType(
                    context.type().Accept(this) as AST.Type);
            }

            return base.VisitLiteral_type(context);
        }

        public override Base VisitArray_type([NotNull] GolangParser.Array_typeContext context)
        {
            int length = 0;
            var lengthExpr = context.expression().Accept(this) as Expression;

            switch (lengthExpr)
            {
                case IntegerLiteral il:
                    length = (int)il.Value;
                    break;
                default:
                    throw new InvalidOperationException("Array size must be constant");
            }

            return new ArrayType(
                context.type().Accept(this) as AST.Type,
                length);
        }

        public override Base VisitElement_list([NotNull] GolangParser.Element_listContext context)
        {
            var list = new KeyedElementList();

            for (int i = 0; context.keyed_element(i) != null; i++)
                list.AddChild(
                    context.keyed_element(i).Accept(this) as KeyedElement);

            return list;
        }

        public override Base VisitKeyed_element([NotNull] GolangParser.Keyed_elementContext context)
        {
            Expression key = null;
            if (context.key() != null)
            {
                if (context.key().field_name() != null)
                {
                    var node = context.key().field_name().Accept(this) as RawNode;
                    key = new IdentifierExpression(node.Text);
                }
                else if(context.key().expression() != null)
                {
                    key = context.key().expression().Accept(this) as Expression;
                }
            }

            var element = context.element().Accept(this) as Expression;
            return new KeyedElement(
                key,
                element);
        }

        public override Base VisitUnary_expr([NotNull] GolangParser.Unary_exprContext context)
        {
            if(context.unary_op() != null)
            {
                return new UnaryExpression(
                    (context.unary_op().Accept(this) as UnaryOpWrapper).Op,
                    context.unary_expr().Accept(this) as Expression);                
            }
            else
                return base.VisitUnary_expr(context);
        }

        public override Base VisitExpression_list([NotNull] GolangParser.Expression_listContext context)
        {
            var ret = new ExpressionList();

            foreach (var child in context.children)
            {
                if (child is GolangParser.ExpressionContext)
                    ret.AddChild(child.Accept(this) as Expression);
            }

            return ret;
        }

        public override Base VisitIdentifier_list([NotNull] GolangParser.Identifier_listContext context)
        {
            var ret = new RawNodeList();
            
            foreach (var child in context.children)
            {
                if (child is ITerminalNode)
                {
                    var rn = child.Accept(this) as RawNode;
                    if(rn.Text != ",")
                        ret.Items.Add(rn);
                }
            }

            return ret;
        }
        public override Base VisitExpression([NotNull] GolangParser.ExpressionContext context)
        {
            if (context.unary_expr() != null)
                return context.unary_expr().Accept(this);
            else
            {
                var newBe = new BinaryExpression(
                    (context.binary_op().Accept(this) as BinaryOpWrapper).Op,
                    context.expression(0).Accept(this) as Expression,
                    context.expression(1).Accept(this) as Expression);
                return FixPrecedence(newBe);
            }
        }

        public override Base VisitUnary_op([NotNull] GolangParser.Unary_opContext context)
        {
            EUnaryOp op = EUnaryOp.Unknown;
            switch (context.GetText())
            {
                case "-":
                    op = EUnaryOp.Negative;
                    break;
                case "+":
                    op = EUnaryOp.Positive;
                    break;
                case "!":
                    op = EUnaryOp.Not;
                    break;
                case "^":
                    op = EUnaryOp.Xor;
                    break;
                case "*":
                    op = EUnaryOp.Dereference;
                    break;
                case "&":
                    op = EUnaryOp.Reference;
                    break;
                case "<-":
                    op = EUnaryOp.Send;
                    break;
            }

            return new UnaryOpWrapper(op);
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
                BigInteger bi = 0;
                var text = context.IntegerLiteral().GetText();
                if (text.StartsWith("0x", StringComparison.InvariantCultureIgnoreCase))
                    bi = BigInteger.Parse(text.Substring(2), System.Globalization.NumberStyles.HexNumber);
                else if (text.StartsWith("0"))
                    bi = text.Substring(1).Aggregate(new BigInteger(), (b, c) => b * 8 + c - '0');
                else
                    bi = BigInteger.Parse(text);

                return new IntegerLiteral(bi);
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
            var ret = new StatementList();

            var identList = context.identifier_list().Accept(this) as RawNodeList;
            var exprList = context.expression_list().Accept(this) as ExpressionList;

            if (identList.Items.Count > 1 && exprList.NumChildren() == 1 && exprList.GetChild(0) is InvocationExpression)
            {
                var idents = new ExpressionList();
                foreach (var ident in identList.Items)
                    idents.AddChild(new IdentifierExpression(ident.Text));

                ret.AddChild(
                    new ReturnAssignment(
                        idents,
                        exprList.GetChild<InvocationExpression>(0)));
            }
            else
            {
                for (int i = 0; i < identList.Items.Count; i++)
                {
                    var vd = new VarDeclaration(identList.Items[i].Text, new ExpressionType(exprList.GetChild<Expression>(0)));
                    m_currentScope.AddVarDeclaration(vd);
                    ret.AddChild(
                        new Assignment(
                            new IdentifierExpression(vd.Identifier),
                            exprList.GetChild<Expression>(i),
                            AssignmentType.Normal));
                }
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

        public override Base VisitOperand([NotNull] GolangParser.OperandContext context)
        {
            if(context.OpenParen() != null)
                return new ExpressionWrapper(context.expression().Accept(this) as Expression);
            return base.VisitOperand(context);
        }

        public override Base VisitConst_spec([NotNull] GolangParser.Const_specContext context)
        {
            if (context.expression_list() != null)
            {
                var idents = context.identifier_list().Accept(this) as RawNodeList;
                var exprs = context.expression_list().Accept(this) as ExpressionList;
                // TODO ignore const type for now

                foreach(var ident in idents.Items)
                {
                    m_currentScope.AddConstDeclaration(
                        new ConstDeclaration(
                            ident.Text,
                            exprs.GetChild<Expression>(0)));
                }
            }

            return base.VisitConst_spec(context);
        }

        public override Base VisitExpr_switch_statement([NotNull] GolangParser.Expr_switch_statementContext context)
        {
            var ret = new ExpressionSwitch();
            if (context.simple_statement() != null)
                ret.Preamble = context.simple_statement().Accept(this) as Statement;
            if (context.expression() != null)
                ret.Expression = context.expression().Accept(this) as Expression;

            for (int i = 0; context.expr_case_clause(i) != null; i++)
            {
                ret.AddChild(context.expr_case_clause(i).Accept(this) as ExpressionSwitchClause);
            }

            return ret;
        }

        public override Base VisitExpr_switch_case([NotNull] GolangParser.Expr_switch_caseContext context)
        {
            if (context.expression_list() != null)
                return context.expression_list().Accept(this);
            return new DefaultCase();
        }

        public override Base VisitExpr_case_clause([NotNull] GolangParser.Expr_case_clauseContext context)
        {
            var @case = context.expr_switch_case().Accept(this) as Node;
            var sl = context.statement_list().Accept(this) as StatementList;

            return new ExpressionSwitchClause(
                @case, sl);
        }

        public override Base VisitArguments([NotNull] GolangParser.ArgumentsContext context)
        {
            return base.VisitArguments(context);
        }

        public override Base VisitTerminal(ITerminalNode node)
        {
            return new RawNode(node.GetText());
        }

        public override Base VisitAssignment([NotNull] GolangParser.AssignmentContext context)
        {
            var left = context.expression_list(0).Accept(this) as ExpressionList;
            var right = context.expression_list(1).Accept(this) as ExpressionList;
            var type = context.assign_op().Accept(this) as AssignmentTypeWrapper;

            var ret = new StatementList();

            if (type.Type == AssignmentType.Normal && left.NumChildren() > 1 && right.NumChildren() == 1 && right.GetChild(0) is InvocationExpression)
            {
                ret.AddChild(
                    new ReturnAssignment(
                        left,
                        right.GetChild<InvocationExpression>(0)));
            }
            else
            {
                while (left.NumChildren() > 0)
                {
                    ret.AddChild(
                        new Assignment(
                            left.GetChild<Expression>(0),
                            right.GetChild<Expression>(0),
                            type.Type));
                }
            }

            return ret;
        }

        public override Base VisitGoto_statement([NotNull] GolangParser.Goto_statementContext context)
        {
            return new GotoStatement(
                (context.label().Accept(this) as RawNode).Text);
        }

        public override Base VisitLabeled_statement([NotNull] GolangParser.Labeled_statementContext context)
        {
            return new LabeledStatement(
                context.Identifier().GetText(),
                context.statement().Accept(this) as Statement);
        } 

        public override Base VisitVar_spec([NotNull] GolangParser.Var_specContext context)
        {
            var identList = context.identifier_list().Accept(this) as RawNodeList;
            AST.Type type = null;
            if (context.type() != null)
                type = context.type().Accept(this) as AST.Type;

            ExpressionList exprList = null;
            if (context.expression_list() != null)
                exprList = context.expression_list().Accept(this) as ExpressionList;

            var sl = new StatementList();
            for (int i = 0; i < identList.Items.Count; i++)
            {
                VarDeclaration vd;
                if(type != null)
                    vd = new VarDeclaration(identList.Items[i].Text, type.Clone());
                else //expr list should be defined here
                    vd = new VarDeclaration(identList.Items[i].Text, new ExpressionType(exprList.GetChild<Expression>(0)));

                m_currentScope.AddVarDeclaration(vd);
                if(exprList != null)
                {
                    var assign = new Assignment(
                            new IdentifierExpression(identList.Items[i].Text),
                            exprList.GetChild<Expression>(0),
                            AssignmentType.Normal);

                    if (m_currentScope == m_currentPackage)
                        m_currentPackage.AddStaticInitializer(assign);
                    else
                        sl.AddChild(assign);
                }
            }
            return sl;
        }

        public override Base VisitAssign_op([NotNull] GolangParser.Assign_opContext context)
        {
            var ret = new AssignmentTypeWrapper();

            switch (context.GetText())
            {
                case "=":
                    ret.Type = AssignmentType.Normal;
                    break;
                case "+=":
                    ret.Type = AssignmentType.Additive;
                    break;
                case "-=":
                    ret.Type = AssignmentType.Sutractive;
                    break;
                case "*=":
                    ret.Type = AssignmentType.Multiplicative;
                    break;
                case "/=":
                    ret.Type = AssignmentType.Divisive;
                    break;
                case "%=":
                    ret.Type = AssignmentType.Modulus;
                    break;
                case "&=":
                    ret.Type = AssignmentType.And;
                    break;
                case "|=":
                    ret.Type = AssignmentType.Or;
                    break;
                case "^=":
                    ret.Type = AssignmentType.Xor;
                    break;
                case "<<=":
                    ret.Type = AssignmentType.ShiftLeft;
                    break;
                case ">>=":
                    ret.Type = AssignmentType.ShiftRight;
                    break;
                case "&^=":
                    ret.Type = AssignmentType.AndNot;
                    break;
                
            }

            return ret;
        }

        private BinaryExpression FixPrecedence(BinaryExpression active)
        {
            var beLeft = active.Left as BinaryExpression;
            var beRight = active.Right as BinaryExpression;

            if(beLeft != null && HasLowerPrecdence(beLeft.Operation, active.Operation))
            {
                active.Left = beLeft.Right;
                beLeft.Right = FixPrecedence(active);
                return beLeft;
            }

            if (beRight != null && HasLowerPrecdence(beRight.Operation, active.Operation))
            {
                active.Right = beRight.Left;
                beRight.Left = FixPrecedence(active);
                return beRight;
            }

            return active;
        }

        private bool HasLowerPrecdence(BinaryOp a, BinaryOp b)
        {
            return PrecedenceLevel(a) < PrecedenceLevel(b);
        }

        private int PrecedenceLevel(BinaryOp op)
        {
            switch (op)
            {
                case BinaryOp.Multiply:
                case BinaryOp.Divide:
                case BinaryOp.Modulus:
                case BinaryOp.ShiftLeft:
                case BinaryOp.ShiftRight:
                case BinaryOp.And:
                case BinaryOp.AndNot:
                    return 5;
                case BinaryOp.Add:
                case BinaryOp.Subtract:
                case BinaryOp.Or:
                case BinaryOp.Xor:
                    return 4;
                case BinaryOp.LogicalEquals:
                case BinaryOp.NotEquals:
                case BinaryOp.LessThan:
                case BinaryOp.LessEqual:
                case BinaryOp.GreaterThan:
                case BinaryOp.GreaterEqual:
                    return 3;
                case BinaryOp.LogicalAnd:
                    return 2;
                case BinaryOp.LogicalOr:
                    return 1;
            }

            return 0;
        }
    }
}
