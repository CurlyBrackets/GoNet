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
        public override void Process(Node input)
        {
            switch (input) {
                case Function f:
                    var ilb = new IlBlock();
                    var bb1 = new BasicBlock();
                    var bb2 = new BasicBlock();
                    var bb3 = new BasicBlock();
                    var bb4 = new BasicBlock();
                    var bb5 = new BasicBlock();

                    bb1.Instructions = new Instructions();
                    bb1.Instructions.AddChild(new Instruction(EInstruction.Ldarg_0));
                    bb1.Instructions.AddChild(new Instruction(EInstruction.Ldc_R8) { Argument = 0.0 });
                    bb1.Instructions.AddChild(new Instruction(EInstruction.Blt));

                    bb2.Instructions = new Instructions();
                    bb2.Instructions.AddChild(new Instruction(EInstruction.Ldarg_0));
                    bb2.Instructions.AddChild(new Instruction(EInstruction.Neg));
                    bb2.Instructions.AddChild(new Instruction(EInstruction.Ret));

                    bb3.Instructions = new Instructions();
                    bb3.Instructions.AddChild(new Instruction(EInstruction.Ldarg_0));
                    bb3.Instructions.AddChild(new Instruction(EInstruction.Ldc_R8) { Argument = 0.0 });
                    bb3.Instructions.AddChild(new Instruction(EInstruction.Beq));

                    bb4.Instructions = new Instructions();
                    bb4.Instructions.AddChild(new Instruction(EInstruction.Ldc_R8) { Argument = 0.0 });
                    bb4.Instructions.AddChild(new Instruction(EInstruction.Ret));

                    bb5.Instructions = new Instructions();
                    bb5.Instructions.AddChild(new Instruction(EInstruction.Ldarg_0));
                    bb5.Instructions.AddChild(new Instruction(EInstruction.Ret));

                    ilb.AddChild(bb1);
                    bb1.TrueBranch = bb2;
                    bb1.FalseBranch = bb3;

                    bb3.TrueBranch = bb4;
                    bb3.FalseBranch = bb5;

                    bb2.TerminatingBlock = true;
                    bb4.TerminatingBlock = true;
                    bb5.TerminatingBlock = true;

                    f.Body = ilb;
                    break;
                default:
                    base.Process(input);
                    break;
            }
        }

    }
}
