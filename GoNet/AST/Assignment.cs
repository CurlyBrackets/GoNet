using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoNet.AST
{
    enum AssignmentType
    {
        Normal,
        Additive,
        Sutractive,
        Multiplicative,
        Divisive,
        Modulus,
        And,
        Or,
        Xor,
        ShiftLeft,
        ShiftRight,
        AndNot,
    }

    class AssignmentTypeWrapper : Base
    {
        public AssignmentType Type { get; set; }
    }

    class Assignment : Statement
    {
        public Expression Identifier
        {
            get
            {
                return GetChild<Expression>(0);
            }
            set
            {
                SetChild(value, 0);
            }
        }

        public Expression Value
        {
            get
            {
                return GetChild<Expression>(1);
            }
            set
            {
                SetChild(value, 1);
            }
        }

        public AssignmentType Operation
        {
            get; private set;
        }

        public Assignment(Expression identifier, Expression value, AssignmentType op)
            : base(2)
        {
            Identifier = identifier;
            Value = value;
            Operation = op;
        }

        public override Statement CloneStatement()
        {
            return new Assignment(
                Identifier.CloneExpr(),
                Value.CloneExpr(),
                Operation);
        }
    }
}
