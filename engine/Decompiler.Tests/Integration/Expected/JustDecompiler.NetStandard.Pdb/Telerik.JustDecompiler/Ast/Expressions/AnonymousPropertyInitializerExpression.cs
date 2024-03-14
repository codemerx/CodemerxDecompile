using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class AnonymousPropertyInitializerExpression : PropertyInitializerExpression
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.AnonymousPropertyInitializerExpression;
			}
		}

		public bool IsKey
		{
			get
			{
				return base.Property.get_SetMethod() == null;
			}
		}

		public AnonymousPropertyInitializerExpression(PropertyDefinition property, TypeReference type) : this(property, type, null)
		{
		}

		public AnonymousPropertyInitializerExpression(PropertyDefinition property, TypeReference type, IEnumerable<Instruction> instructions) : base(property, type, instructions)
		{
		}

		public override Expression Clone()
		{
			return new AnonymousPropertyInitializerExpression(base.Property, this.expressionType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new AnonymousPropertyInitializerExpression(base.Property, this.expressionType, null);
		}
	}
}