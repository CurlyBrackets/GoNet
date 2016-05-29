using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class VarDeclaration : Node
    {
        public string Identifier { get; private set; }
        public Type Type
        {
            get { return GetChild<Type>(0); }
            set { SetChild(value, 0); }
        }

        public VarDeclaration(string id, Type t)
            : base(true, 1)
        {
            Identifier = id;
            Type = t;
        }
    }
}
