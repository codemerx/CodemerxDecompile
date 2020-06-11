using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class PropertyInitializerExpression : Expression
    {
		protected readonly TypeReference expressionType;

		public PropertyInitializerExpression(PropertyDefinition property, TypeReference type)
			: this(property, type, null)
		{
		}

		public PropertyInitializerExpression(PropertyDefinition property, TypeReference type, IEnumerable<Instruction> instructions)
            : base(instructions)
        {
            this.Property = property;
			this.expressionType = type;
        }

        public PropertyDefinition Property { get; protected set; }

        public string Name
        {
            get
            {
                return Property.Name;
            }
        }

        public override bool Equals(Expression other)
        {
            return this == other;
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.PropertyInitializerExpression; }
        }

        public override Expression Clone()
        {
            return new PropertyInitializerExpression(this.Property, this.expressionType, this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new PropertyInitializerExpression(this.Property, this.expressionType, null);
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
				throw new NotSupportedException("Property Initializer expressions cannot have type.");
			}
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }
    }
}
