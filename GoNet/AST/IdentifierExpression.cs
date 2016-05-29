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
            : base(false)
        {
            Identifier = id;
        }

        public override Expression Clone()
        {
            return new IdentifierExpression(Identifier);
        }
    }
}
