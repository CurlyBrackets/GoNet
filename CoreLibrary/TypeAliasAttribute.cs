using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLibrary
{
    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public sealed class TypeAliasAttribute : Attribute
    {
        public string TypeName
        {
            get; private set;
        }

        public Type Type
        {
            get; private set;
        }

        // This is a positional argument
        public TypeAliasAttribute(string typeName, Type type)
        {
            TypeName = typeName;
            Type = type;
        }
    }
}
