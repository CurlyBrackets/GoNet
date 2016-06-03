﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class ChannelType : Type
    {
        public Type ElementType
        {
            get { return GetChild<Type>(0); }
            private set { SetChild(value, 0); }
        }

        public ChannelType(Type elementType)
            : base(true, 1)
        {
            ElementType = elementType;
        }

        public override Type CloneType()
        {
            return new ChannelType(
                ElementType.CloneType());
        }
    }
}
