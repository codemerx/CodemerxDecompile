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
				return 63;
			}
		}

		public bool IsKey
		{
			get
			{
				return this.get_Property().get_SetMethod() == null;
			}
		}

		public AnonymousPropertyInitializerExpression(PropertyDefinition property, TypeReference type)
		{
			this(property, type, null);
			return;
		}

		public AnonymousPropertyInitializerExpression(PropertyDefinition property, TypeReference type, IEnumerable<Instruction> instructions)
		{
			base(property, type, instructions);
			return;
		}

		public override Expression Clone()
		{
			return new AnonymousPropertyInitializerExpression(this.get_Property(), this.expressionType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new AnonymousPropertyInitializerExpression(this.get_Property(), this.expressionType, null);
		}
	}
}