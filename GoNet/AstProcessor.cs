using GoNet.AST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet
{
    abstract class AstProcessor
    {
        public virtual void Process(AST.Base input)
        {
            switch (input)
            {
                case Block b:
                    Process(b.Statements);
                    break;
                case ExpressionList el:
                    foreach (var e in el.Items)
                        Process(e);
                    break;
                case FloatLiteral fl:
                    break;
                case Function f:

                    break;
                case IdentifierExpression ie:
                    break;
                case IfStatement ifs:
                    if (ifs.Preamble != null)
                        Process(ifs.Preamble);
                    if (ifs.False != null)
                        Process(ifs.False);
                    Process(ifs.Condition);
                    Process(ifs.True);
                    break;
                case ImaginaryLiteral il:
                    break;
                case ImportDeclaration id:
                    break;
                case IntegerLiteral il:
                    break;
                case Package p:
                    foreach (var constPair in p.ConstDeclarations)
                        Process(constPair.Value);
                    foreach (var importPair in p.Imports)
                        Process(importPair.Value);
                    foreach (var varPair in p.VarDeclarations)
                        Process(varPair.Value);
                    foreach (var typePair in p.TypeDeclarations)
                        Process(typePair.Value);
                    foreach (var funcPair in p.Functions)
                        Process(funcPair.Value);
                    break;
                case Parameter p:
                    Process(p.Type);
                    break;
                case Parameters ps:
                    
                    break;
            }
        }
    }
}
