using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class TypeName : Type
    {
        public string Name { get; private set; }
        public TypeName(string name)
            : base(false)
        {
            Name = name;
        }

        public override Type Clone()
        {
            return new TypeName(Name);
        }
    }
}
