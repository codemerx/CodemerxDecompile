using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class PropertyInitializerExpression : Expression
	{
		protected readonly TypeReference expressionType;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.PropertyInitializerExpression;
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

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public string Name
		{
			get
			{
				return this.Property.Name;
			}
		}

		public PropertyDefinition Property
		{
			get;
			protected set;
		}

		public PropertyInitializerExpression(PropertyDefinition property, TypeReference type) : this(property, type, null)
		{
		}

		public PropertyInitializerExpression(PropertyDefinition property, TypeReference type, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Property = property;
			this.expressionType = type;
		}

		public override Expression Clone()
		{
			return new PropertyInitializerExpression(this.Property, this.expressionType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new PropertyInitializerExpression(this.Property, this.expressionType, null);
		}

		public override bool Equals(Expression other)
		{
			return this == other;
		}
	}
}