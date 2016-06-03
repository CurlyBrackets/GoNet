using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class Parameter : Node
    {
        public string Name { get; private set; }
        public Type Type
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
        public Parameter(string name, Type type)
            : base(true, 1)
        {
            Name = name;
            Type = type;
        }

        public override Node Clone()
        {
            return new Parameter(
                Name,
                Type.CloneType());
        }
    }
}
