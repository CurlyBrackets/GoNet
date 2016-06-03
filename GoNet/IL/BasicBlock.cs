using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using GoNet.AST;

namespace GoNet.IL
{
    class BasicBlock : Node
    {
        public bool TerminatingBlock { get; set; }
        public BasicBlock TrueBranch
        {
            get
            {
                return GetChild<BasicBlock>(0);
            }
            set
            {
                SetChild(value, 0);
            }
        }

        public BasicBlock FalseBranch
        {
            get
            {
                return GetChild<BasicBlock>(1);
            }
            set
            {
                SetChild(value, 1);
            }
        }

        public Instructions Instructions
        {
            get
            {
                return GetChild<Instructions>(2);
            }
            set
            {
                SetChild(value, 2);
            }
        }

        private static int s_index = 0;
        public int BlockIndex { get; private set; }

        public BasicBlock()
            : base(true, 3)
        {
            BlockIndex = Interlocked.Increment(ref s_index);
        }

        public override Node Clone()
        {
            return new BasicBlock()
            {
                TrueBranch = TrueBranch?.Clone() as BasicBlock,
                FalseBranch = FalseBranch?.Clone() as BasicBlock,
                TerminatingBlock = TerminatingBlock,
                Instructions = Instructions?.Clone() as Instructions,
            };
        }
    }
}
