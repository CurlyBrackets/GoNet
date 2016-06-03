using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;

namespace GoNet
{
    class TypeChecker : AstProcessor
    {
        public override void Process(Node input)
        {
            switch (input) {
                case Package p:
                    if (p.Imported)
                        return;
                    base.Process(p);
                    break;
                case TypeName tn:
                    var t = BuiltinType.FromTypeName(tn.Name);
                    if(t != null)
                    {
                        tn.Parent.Replace(tn, t);
                        break;
                    }
                    // resolve from type decls
                    // resolve from imports
                    break;
                case IntegerLiteral il:
                    il.ResolvedType = BuiltinType.FromTypeName("int");
                    break;
                default:
                    base.Process(input);
                    break;
            }
        }


    }
}
