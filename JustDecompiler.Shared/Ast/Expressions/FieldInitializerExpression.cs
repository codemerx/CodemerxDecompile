using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class FieldInitializerExpression : Expression
    {
		private TypeReference expressionType;

		public FieldInitializerExpression(FieldDefinition fieldDefinition, TypeReference type)
			: this(fieldDefinition, type, null)
		{
		}

		public FieldInitializerExpression(FieldDefinition fieldDefinition, TypeReference type, IEnumerable<Instruction> instructions)
            : base(instructions)
        {
			this.Field = fieldDefinition;
			this.expressionType = type;
        }

		public FieldDefinition Field { get; protected set; }

        public string Name
        {
            get
            {
                return Field.Name;
            }
        }

        public override bool Equals(Expression other)
        {
            return this == other;
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.FieldInitializerExpression; }
        }

        public override Expression Clone()
        {
            return new FieldInitializerExpression(this.Field, this.expressionType, this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
			return new FieldInitializerExpression(this.Field, this.expressionType, null);
        }

        public override bool HasType
        {
            get
            {
                return true;
            }
        }

        public override TypeReference ExpressionType
        {
			get
			{
				return this.expressionType;
			}
			set
			{
				throw new NotSupportedException("Field Initializer expressions cannot have type.");
			}
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }
    }
}
