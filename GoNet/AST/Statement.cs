﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    abstract class Statement : Node
    {
        protected Statement(int limit = 0)
            : base(true, limit)
        {

        }

        public abstract Statement CloneStatement();
        public override Node Clone()
        {
            return CloneStatement();
        }
    }
}
