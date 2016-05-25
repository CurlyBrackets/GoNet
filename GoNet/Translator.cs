using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;
using GoNet.IL;

namespace GoNet
{
    class Translator : AstProcessor
    {
        enum EPendingLinkType
        {
            True,
            False,
            Default,
        }

        struct PendingLink
        {
            public Node From;
            public EPendingLinkType Type;
            public Node To;
        }

        struct BlockState
        {
            public Block Block;
            public int CurrentIndex;
        }

        private List<PendingLink> m_pendingLinks;
        private Node m_activeNode;
        private BasicBlock m_activeBasicBlock;
        private Instructions m_inst;
        private Dictionary<Node, BasicBlock> m_mapping;
        private bool m_marked;
        private bool m_conditionCanBranch;

        public Translator()
        {
            m_mapping = new Dictionary<Node, BasicBlock>();
            m_pendingLinks = new List<PendingLink>();
            m_stack = new Stack<BlockState>();
            m_conditionCanBranch = false;
        }

        private void NewBlock(Node current)
        {
            m_activeNode = current;
            m_activeBasicBlock = new BasicBlock();
            m_inst = new Instructions();
            m_activeBasicBlock.Instructions = m_inst;
            
            m_mapping.Add(current, m_activeBasicBlock);
            m_marked = false;
        }

        private void ProcessMark(Node current)
        {
            if (m_marked)
                NewBlock(current);
        }

        private void MarkNext()
        {
            m_marked = true;
        }

        private void CreateDummyBlock(Node target)
        {
            var bb = new BasicBlock();
            var inst = new Instructions();
            bb.Instructions = inst;

            inst.AddChild(new Instruction(EInstruction.Br));

            m_mapping.Add(target,bb);
        }

        private Stack<BlockState> m_stack;
        private Block m_activeBlock;
        private int m_activeIndex;

        private void PushBlock(Block newBlock)
        {
            if (m_activeBlock != null)
            {
                m_stack.Push(
                    new BlockState()
                    {
                        Block = m_activeBlock,
                        CurrentIndex = m_activeIndex,
                    });
            }

            m_activeBlock = newBlock;
            m_activeIndex = 0;
        }

        private void PopBlock()
        {
            if (m_stack.Count > 0)
            {
                var state = m_stack.Pop();
                m_activeBlock = state.Block;
                m_activeIndex = state.CurrentIndex;
            }
            else
            {
                m_activeBlock = null;
                m_activeIndex = -1;
            }
        }

        private bool TopOfStack()
        {
            return m_stack.Count == 0;
        }

        private Node NextChild()
        {
            if(m_activeIndex + 1 < m_activeBlock.NumChildren())
                return m_activeBlock.GetChild(m_activeIndex+1);
            return null;
        }

        private void CreateLink(Node from, EPendingLinkType type, Node to)
        {
            m_pendingLinks.Add(
                new PendingLink()
                {
                    From = from,
                    Type = type,
                    To = to,
                });
        }

        private void GatherLeaves(BasicBlock block, List<BasicBlock> leaves)
        {
            if (!block.TerminatingBlock)
            {
                var trueExists = block.TrueBranch != null;
                var falseExists = block.FalseBranch != null;

                if (!trueExists && !falseExists)
                    leaves.Add(block);
                else
                {
                    if (trueExists)
                        GatherLeaves(block.TrueBranch, leaves);
                    if (falseExists)
                        GatherLeaves(block.FalseBranch, leaves);
                }
            }
        }

        public override void Process(Node input)
        {
            switch (input) {
                case Function f:
                    var ilb = new IlBlock();
                    m_mapping.Clear();
                    m_pendingLinks.Clear();
                    m_stack.Clear();
                    m_activeBlock = null;

                    NewBlock(input);
                    ilb.AddChild(m_activeBasicBlock);

                    base.Process(input);
                    f.Body = ilb;

                    var fromBlocks = new List<BasicBlock>();
                    foreach(var pendingLink in m_pendingLinks)
                    {
                        fromBlocks.Clear();
                        var from = m_mapping[pendingLink.From];
                        if (pendingLink.Type == EPendingLinkType.Default)
                            GatherLeaves(from, fromBlocks);
                        else
                            fromBlocks.Add(from); // temp

                        var to = m_mapping[pendingLink.To];
                        if (from == null || to == null)
                            throw new Exception("Invalid basic block link");

                        foreach (var block in fromBlocks)
                        {
                            switch (pendingLink.Type)
                            {
                                case EPendingLinkType.True:
                                    block.TrueBranch = to;
                                    break;
                                case EPendingLinkType.False:
                                    block.FalseBranch = to;
                                    break;
                                case EPendingLinkType.Default:
                                    block.TrueBranch = to;
                                    break;
                                default:
                                    break;
                            }
                        }
                    }

                    break;
                case Block b:
                    PushBlock(b);

                    for (; m_activeIndex < b.NumChildren(); m_activeIndex++)
                        Process(b.GetChild(m_activeIndex));

                    PopBlock();
                    break;
                case IfStatement ifs:
                    ProcessMark(ifs);

                    CreateLink(m_activeNode, EPendingLinkType.True, ifs.True);
                    bool directFalseLink = false;
                    if (ifs.False != null)
                        CreateLink(m_activeNode, EPendingLinkType.False, ifs.False);
                    else
                        directFalseLink = true;

                    if (NextChild() != null)
                    {
                        CreateLink(ifs.True, EPendingLinkType.True, NextChild());
                        if (directFalseLink)
                            CreateLink(m_activeNode, EPendingLinkType.False, NextChild());
                        else
                            CreateLink(ifs.False, EPendingLinkType.True, NextChild());
                    }
                    else if (TopOfStack())
                    {
                        CreateDummyBlock(ifs);
                        CreateLink(ifs.True, EPendingLinkType.True, ifs);
                        if (directFalseLink)
                            CreateLink(m_activeNode, EPendingLinkType.False, ifs);
                        else
                            CreateLink(ifs.False, EPendingLinkType.True, ifs);
                    }

                    if (ifs.Preamble != null)
                        Process(ifs.Preamble);
                    m_conditionCanBranch = true;
                    Process(ifs.Condition);
                    m_conditionCanBranch = false;

                    NewBlock(ifs.True);
                    Process(ifs.True);
                    if (ifs.False != null)
                    {
                        NewBlock(ifs.False);
                        Process(ifs.False);
                    }

                    MarkNext(); // branches created, therefore next one is marked to start a new block
                    break;
                case BinaryExpression be:
                    Process(be.Left);
                    Process(be.Right);

                    switch (be.Operation)
                    {
                        case BinaryOp.Unknown:
                            break;
                        case BinaryOp.LogicalOr:
                            break;
                        case BinaryOp.LogicalAnd:
                            break;
                        case BinaryOp.LogicalEquals:
                            if (m_conditionCanBranch)
                                m_inst.AddChild(new Instruction(EInstruction.Beq));
                            else
                                m_inst.AddChild(new Instruction(EInstruction.Ceq));
                            break;
                        case BinaryOp.NotEquals:
                            break;
                        case BinaryOp.LessThan:
                            if (m_conditionCanBranch)
                                m_inst.AddChild(new Instruction(EInstruction.Blt));
                            else
                                m_inst.AddChild(new Instruction(EInstruction.Clt));
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
                            break;
                        case BinaryOp.Modulus:
                            break;
                        case BinaryOp.ShiftLeft:
                            break;
                        case BinaryOp.ShiftRight:
                            break;
                        case BinaryOp.And:
                            break;
                        case BinaryOp.AndNot:
                            break;
                        default:
                            break;
                    }
                    break;
                case UnaryExpression ue:
                    Process(ue.Expr);

                    switch (ue.Op)
                    {
                        case EUnaryOp.Unknown:
                            break;
                        case EUnaryOp.Positive:
                            break;
                        case EUnaryOp.Negative:
                            m_inst.AddChild(new Instruction(EInstruction.Neg));
                            break;
                        case EUnaryOp.Not:
                            break;
                        case EUnaryOp.Xor:
                            break;
                        case EUnaryOp.Dereference:
                            break;
                        case EUnaryOp.Reference:
                            break;
                        case EUnaryOp.Send:
                            break;
                        default:
                            break;
                    }
                    break;
                case ParameterVariable pv:
                    if (pv.Reference)
                        throw new NotImplementedException();
                    else
                        LoadArg(pv.Slot);
                    break;
                case IntegerLiteral il:
                    int val = (int)il.Value;
                    switch (val)
                    {
                        case 0:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_0));
                            break;
                        case 1:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_1));
                            break;
                        case 2:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_2));
                            break;
                        case 3:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_3));
                            break;
                        case 4:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_4));
                            break;
                        case 5:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_5));
                            break;
                        case 6:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_6));
                            break;
                        case 7:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_7));
                            break;
                        case 8:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_8));
                            break;
                        case -1:
                            m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_M1));
                            break;
                        default:
                            if (val <= 255)
                                m_inst.AddChild(new Instruction(EInstruction.Ldc_I4_S) { Argument = (byte)val });
                            else
                                m_inst.AddChild(new Instruction(EInstruction.Ldc_I4) { Argument = val });
                            break;
                    }
                    break;
                case ReturnStatement rs:
                    ProcessMark(rs);

                    for(int i = rs.NumChildren() - 1; i >= 0; i--)
                    {
                        if (i > 0)
                            LoadArg(i - 1);

                        var expr = rs.GetChild<Expression>(i);
                        Process(expr);

                        if (i > 0)
                            StoreIndirect(expr.ResolvedType);
                    }

                    m_inst.AddChild(new Instruction(EInstruction.Ret));
                    m_activeBasicBlock.TerminatingBlock = true;
                    break;
                default:
                    base.Process(input);
                    break;
            }
        }

        private void LoadArg(int index)
        {
            switch (index)
            {
                case 0:
                    m_inst.AddChild(new Instruction(EInstruction.Ldarg_0));
                    break;
                case 1:
                    m_inst.AddChild(new Instruction(EInstruction.Ldarg_1));
                    break;
                case 2:
                    m_inst.AddChild(new Instruction(EInstruction.Ldarg_2));
                    break;
                case 3:
                    m_inst.AddChild(new Instruction(EInstruction.Ldarg_3));
                    break;
                default:
                    if (index <= 255)
                        m_inst.AddChild(new Instruction(EInstruction.Ldarg_S) { Argument = (byte)index });
                    else
                        m_inst.AddChild(new Instruction(EInstruction.Ldarg) { Argument = index });
                    break;
            }
        }

        private void StoreIndirect(AST.Type type)
        {
            switch (type)
            {
                case BuiltinType bt:
                    switch (bt.Type)
                    {
                        case EBuiltinType.Uint8:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I1));
                            break;
                        case EBuiltinType.Uint16:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I2));
                            break;
                        case EBuiltinType.Uint32:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I4));
                            break;
                        case EBuiltinType.Uint64:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I8));
                            break;
                        case EBuiltinType.Int8:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I1));
                            break;
                        case EBuiltinType.Int16:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I2));
                            break;
                        case EBuiltinType.Int32:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I4));
                            break;
                        case EBuiltinType.Int64:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I8));
                            break;
                        case EBuiltinType.Float32:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_R4));
                            break;
                        case EBuiltinType.Float64:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_R8));
                            break;
                        case EBuiltinType.Uintptr:
                            m_inst.AddChild(new Instruction(EInstruction.Stind_I8));
                            break;
                        default:
                            m_inst.AddChild(new Instruction(EInstruction.StindRef));
                            break;
                    }
                    break;
                default:
                    m_inst.AddChild(new Instruction(EInstruction.StindRef));
                    break;
            }
        }

    }
}
