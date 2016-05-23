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
        public virtual void Process(AST.Node input)
        {
            if (input != null && input.IsContainer)
            {
                for (int i = 0; i < input.NumChildren(); i++)
                {
                    Process(input.GetChild(i));
                }
            }
        }
    }
}
