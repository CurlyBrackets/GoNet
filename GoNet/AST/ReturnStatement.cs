﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ReturnStatement : Statement
    {
        public ExpressionList Expression { get; set; }
        public ReturnStatement()
        {

        }
    }
}
