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
                case TypeName tn:
                    var t = BuiltinType.FromTypeName(tn.Name);
                    if(t != null)
                    {
                        
                        break;
                    }
                    break;
                default:
                    base.Process(input);
                    break;
            }
        }


    }
}
