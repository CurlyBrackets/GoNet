using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class Signature : Node
    {
        public Parameters Returns
        {
            get
            {
                return GetChild<Parameters>(0);
            }
            set
            {
                SetChild(value, 0);
            }
        }
        public Parameters Parameters
        {
            get
            {
                return GetChild<Parameters>(1);
            }
            set
            {
                SetChild(value, 1);
            }
        }

        public Signature()
            : base(true, 2)
        {

        }

        public override Node Clone()
        {
            return new Signature()
            {
                Returns = Returns.Clone() as Parameters,
                Parameters = Parameters.Clone() as Parameters,
            };
        }
    }
}
