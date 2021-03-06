﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class Function : Node
    {
        public bool IsExported {
            get
            {
                return char.IsUpper(Name[0]);
            }
        }
        public string Name { get; private set; }
        public Type Owner { get; private set; }

        public Signature Signature
        {
            get
            {
                return GetChild<Signature>(0);
            }
            set
            {
                SetChild(value, 0);
            }
        }

        public Node Body
        {
            get
            {
                return GetChild<Node>(1);
            }
            set
            {
                SetChild(value, 1);
            }
        }
        public bool IsExternal { get { return Body == null; } }

        public Function(string name, Type owner)
            : base(true, 2)
        {
            Name = name;
            Owner = owner;
        }

        public override Node Clone()
        {
            return new Function(
                Name,
                Owner?.CloneType())
            {
                Signature = Signature.Clone() as Signature,
                Body = Body != null ? Body.Clone() : null,
            };
        }
    }
}
