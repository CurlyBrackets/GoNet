namespace GoNet.AST
{
    class MapType : Type
    {
        public Type KeyType
        {
            get { return GetChild<Type>(0); }
            private set { SetChild(value, 0); }
        }

        public Type ValueType
        {
            get { return GetChild<Type>(1); }
            private set { SetChild(value, 1); }
        }

        public MapType(Type keyType, Type valType)
            : base(true, 2)
        {
            KeyType = keyType;
            ValueType = valType;
        }

        public override Type CloneType()
        {
            return new MapType(
                KeyType.CloneType(),
                ValueType.CloneType());
        }
    }
}