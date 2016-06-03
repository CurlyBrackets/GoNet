using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;
using System.Numerics;

namespace GoNet
{
    class ConstantEvaluator : AstProcessor
    {
        private bool m_evaluating, m_failed;
        private Stack<Expression> m_evaluationStack;

        public ConstantEvaluator()
        {
            m_evaluationStack = new Stack<Expression>();
        }

        public override void Process(Node input)
        {
            switch (input)
            {
                case ConstDeclaration cd:
                    if (!IsConstant(cd.Value))
                    {
                        m_evaluating = true;
                        m_failed = false;
                        Process(cd.Value);
                        if(!m_failed && m_evaluationStack.Count > 0)
                            cd.Replace(cd.Value, m_evaluationStack.Pop());
                        m_evaluating = false;

                        if (!IsConstant(cd.Value))
                            Console.WriteLine(cd.Value);
                    }
                    return;
                case IntegerLiteral il:
                    if (!m_evaluating)
                        break;

                    m_evaluationStack.Push(il);
                    return;
                case FloatLiteral fl:
                    if (!m_evaluating)
                        break;

                    m_evaluationStack.Push(fl);
                    return;
                case IdentifierExpression ie:
                    var val = ResolveConstantIdentifier(ie, ie.Identifier);

                    if (m_evaluating)
                    {
                        if (val != null)
                            m_evaluationStack.Push(val);
                        else
                            m_failed = true;
                    }
                    else
                    {
                        if (val != null)
                            ie.Parent.Replace(ie, val);
                    }
                    return;
                case ExpressionWrapper ew:
                    Process(ew.Expr);
                    return;
                case UnaryExpression ue:
                    if (!m_evaluating)
                        break;

                    Process(ue.Expr);
                    if (m_failed)
                        return;

                    var expr = m_evaluationStack.Pop();
                    switch (ue.Op)
                    {
                        case EUnaryOp.Negative:
                            switch (expr)
                            {
                                case IntegerLiteral il:
                                    m_evaluationStack.Push(
                                        new IntegerLiteral(
                                            -il.Value));
                                    break;
                                case FloatLiteral fl:
                                    m_evaluationStack.Push(
                                        new FloatLiteral(
                                            -fl.Value));
                                    break;
                                case ImaginaryLiteral il:
                                    throw new NotImplementedException("Complex values not supported");
                            }
                            return;
                        case EUnaryOp.Positive:
                            m_evaluationStack.Push(expr);
                            return;
                    }

                    break;
                case BinaryExpression be:
                    if (!m_evaluating)
                        break;

                    Process(be.Left);
                    if (m_failed)
                        return;

                    Process(be.Right);
                    if (m_failed)
                        return;

                    Expression left = null, right = null;

                    switch (be.Operation)
                    {
                        case BinaryOp.Unknown:
                            break;
                        case BinaryOp.LogicalOr:
                            break;
                        case BinaryOp.LogicalAnd:
                            break;
                        case BinaryOp.LogicalEquals:
                            break;
                        case BinaryOp.NotEquals:
                            break;
                        case BinaryOp.LessThan:
                            break;
                        case BinaryOp.LessEqual:
                            break;
                        case BinaryOp.GreaterEqual:
                            break;
                        case BinaryOp.GreaterThan:
                            break;
                        case BinaryOp.Add:
                            break;
                        case BinaryOp.Subtract:
                            break;
                        case BinaryOp.Or:
                            break;
                        case BinaryOp.Xor:
                            break;
                        case BinaryOp.Multiply:
                            break;
                        case BinaryOp.Divide:
                            right = m_evaluationStack.Pop();
                            left = m_evaluationStack.Pop();
                            if ((!(left is IntegerLiteral) && !(left is FloatLiteral) && !(left is ImaginaryLiteral)) ||
                                (!(right is IntegerLiteral) && !(right is FloatLiteral) && !(right is ImaginaryLiteral)))
                                throw new Exception("Division operation must have integer, floating point, or complex operands");

                            switch (left)
                            {
                                case IntegerLiteral il:
                                    switch (right)
                                    {
                                        case IntegerLiteral il2:
                                            m_evaluationStack.Push(
                                                new IntegerLiteral(
                                                    il.Value / il2.Value));
                                            break;
                                        case FloatLiteral fl2:
                                            m_evaluationStack.Push(
                                                new FloatLiteral(
                                                    il.Value / fl2.Value));
                                            break;
                                        case ImaginaryLiteral il2:
                                            throw new NotImplementedException("Complex values not supported");
                                        default:
                                            break;
                                    }
                                    break;
                                case FloatLiteral fl:
                                    switch (right)
                                    {
                                        case IntegerLiteral il2:
                                            m_evaluationStack.Push(
                                                new FloatLiteral(
                                                    fl.Value / il2.Value));
                                            break;
                                        case FloatLiteral fl2:
                                            m_evaluationStack.Push(
                                                new FloatLiteral(
                                                    fl.Value / fl2.Value));
                                            break;
                                        case ImaginaryLiteral il2:
                                            throw new NotImplementedException("Complex values not supported");
                                        default:
                                            break;
                                    }
                                    break;
                                case ImaginaryLiteral il:
                                    throw new NotImplementedException("Complex values not supported");
                                default:
                                    break;
                            }

                            return;
                        case BinaryOp.Modulus:
                            break;
                        case BinaryOp.ShiftLeft:
                            right = m_evaluationStack.Pop();
                            left = m_evaluationStack.Pop();
                            if (!(left is IntegerLiteral) || !(right is IntegerLiteral) || (right as IntegerLiteral).Value < 0)
                                throw new Exception("Shift operation must have integral operands");

                            m_evaluationStack.Push(
                                new IntegerLiteral(
                                    (left as IntegerLiteral).Value << (int)(right as IntegerLiteral).Value));

                            return;
                        case BinaryOp.ShiftRight:
                            break;
                        case BinaryOp.And:
                            break;
                        case BinaryOp.AndNot:
                            break;
                        default:
                            break;
                    }

                    return;                
                default:
                    break;
            }

            if (m_evaluating)
                throw new Exception("Evaluation error: " + input);

            base.Process(input);
        }

        private bool IsConstant(Expression e)
        {
            var literal = 
                (e is IntegerLiteral) ||
                (e is FloatLiteral) ||
                (e is ImaginaryLiteral) ||
                (e is RuneLiteral) ||
                (e is StringLiteral);
            if (literal)
                return true;

            var comp = e as CompositeLiteral;
            if(comp != null)
            {
                foreach(var kve in comp.Elements.FilteredChildren<KeyedElement>())
                {
                    if (!IsConstant(kve.Element))
                        return false;
                }
                return true;
            }

            return false;
        }

        private Expression ResolveConstantIdentifier(Node node, string identifier)
        {
            if (identifier == "true")
                return new BooleanLiteral(true);
            else if (identifier == "false")
                return new BooleanLiteral(false);
            else
            {
                while (node != null)
                {
                    var scope = node as IScope;
                    if (scope != null)
                    {
                        for (int i = 0; i < scope.NumConstDeclarations(); i++)
                        {
                            var cd = scope.GetConstDeclaration(i);
                            if (cd.Identifier == identifier)
                                return cd.Value;
                        }
                    }

                    node = node.Parent;
                }
            }

            return null;
        }
    }
}
