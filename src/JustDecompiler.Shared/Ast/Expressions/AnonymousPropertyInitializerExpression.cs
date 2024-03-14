using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class AnonymousPropertyInitializerExpression : PropertyInitializerExpression
    {
		public AnonymousPropertyInitializerExpression(PropertyDefinition property, TypeReference type)
			: this(property, type, null)
		{
		}

		public AnonymousPropertyInitializerExpression(PropertyDefinition property, TypeReference type, IEnumerable<Instruction> instructions)
            : base(property, type, instructions)
        {
		}

        public bool IsKey
        {
            get
            {
                return this.Property.SetMethod == null;
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.AnonymousPropertyInitializerExpression; }
        }

        public override Expression Clone()
        {
            return new AnonymousPropertyInitializerExpression(this.Property, this.expressionType, this.instructions);
        }

        public override Expression CloneExpressionOnly()
        {
			return new AnonymousPropertyInitializerExpression(this.Property, this.expressionType, null);
        }
    }
}
