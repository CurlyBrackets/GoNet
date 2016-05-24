using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class TypeDeclaration : Type
    {
        public string Name { get; private set; }
        public Type Value
        {
            get
            {
                return GetChild<Type>(0);
            }
            private set
            {
                SetChild(value, 0);
            }
        }

        public TypeDeclaration(string name, Type value)
            : base(true, 1)
        {
            Name = name;
            Value = value;
        }

        public override Type Clone()
        {
            return new TypeDeclaration(Name, Value.Clone());
        }
    }
}
