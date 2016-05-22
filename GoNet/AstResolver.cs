using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoNet.AST;

namespace GoNet
{
    class AstResolver : AstProcessor<object>
    {
        public object Process(Base input)
        {
            switch (input)
            {

            }

            return base.Process(input);
        }
    }
}
