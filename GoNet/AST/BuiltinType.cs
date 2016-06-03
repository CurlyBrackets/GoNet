using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    enum EBuiltinType
    {
        Uint8,
        Uint16,
        Uint32,
        Uint64,
        Uint = Uint64,

        Int8,
        Int16,
        Int32,
        Int64,
        Int = Int64,

        Float32,
        Float64,

        Complex64,
        Complex128,

        Uintptr,
        String,
        Bool = Int8,

        Byte = Uint8,
        Rune = Int32,
    }

    class BuiltinType : Type
    {
        public EBuiltinType Type { get; private set; }
    
        public BuiltinType(EBuiltinType type)
            : base(false)
        {
            Type = type;
        }

        private static readonly Dictionary<string, BuiltinType> Parts = new Dictionary<string, BuiltinType>();
        static BuiltinType(){
            foreach(var val in Enum.GetNames(typeof(EBuiltinType)))
                Parts.Add(val.ToLower(), new BuiltinType((EBuiltinType)Enum.Parse(typeof(EBuiltinType), val)));
        }

        public static IEnumerable<string> Names
        {
            get
            {
                return Parts.Keys;
            }
        }

        public static Type FromTypeName(string name)
        {
            if (Parts.ContainsKey(name))
                return Parts[name].CloneType();
            return null;
        }

        public override Type CloneType()
        {
            return new BuiltinType(Type);
        }
    }
}
