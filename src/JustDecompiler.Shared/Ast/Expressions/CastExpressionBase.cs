using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public abstract class CastExpressionBase : Expression
    {
        public CastExpressionBase(Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions)
            : base(instructions)
        {
            this.Expression = expression;
            this.TargetType = targetType;
        }

        public override bool HasType
        {
            get
            {
                return true;
            }
        }

        public Expression Expression { get; set; }

        public TypeReference TargetType { get; set; }

        public override TypeReference ExpressionType
        {
            get
            {
                return this.TargetType;
            }
            set
            {
                this.TargetType = value;
            }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return this.Expression;
            }
        }

        public override bool Equals(Expression other)
        {
            if (!(other is CastExpressionBase))
            {
                return false;
            }
            if (this.TargetType.FullName != (other as CastExpressionBase).TargetType.FullName)
            {
                return false;
            }
            return this.Expression.Equals((other as CastExpressionBase).Expression);
        }
    }
}
