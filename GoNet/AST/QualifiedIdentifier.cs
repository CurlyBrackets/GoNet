using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class QualifiedIdentifier : Expression
    {
        public string Scope { get; private set; }
        public string Identifier { get; private set; }

        public QualifiedIdentifier(string scope, string id)
            : base(false)
        {
            Scope = scope;
            Identifier = id;
        }

        public override Expression Clone()
        {
            return new QualifiedIdentifier(
                Scope,
                Identifier);
        }
    }
}
