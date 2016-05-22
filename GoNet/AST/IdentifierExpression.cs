﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class IdentifierExpression : Expression
    {
        public string Identifier { get; private set; }
        public IdentifierExpression(string id)
        {
            Identifier = id;
        }
    }
}
