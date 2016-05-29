using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    class CompositeLiteral : Expression
    {
        public Type Type
        {
            get { return GetChild<Type>(0); }
            private set { SetChild(value, 0); }
        }

        public KeyedElementList Elements
        {
            get { return GetChild<KeyedElementList>(1); }
            set { SetChild(value, 1); }
        }

        public CompositeLiteral(Type t, KeyedElementList els)
            : base(true, 2)
        {
            Type = t;
            Elements = els;
        }

        public override Expression Clone()
        {
            var newList = new KeyedElementList();
            foreach(var ke in Elements.FilteredChildren<KeyedElement>())
            {
                newList.AddChild(
                    new KeyedElement(
                        ke.Key == null ? null : ke.Key.Clone(),
                        ke.Element.Clone()));
            }

            return new CompositeLiteral(
                Type.Clone(),
                newList);
        }
    }
}
