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
				return new PropertyInitializerExpression.u003cget_Childrenu003ed__20(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 85;
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
				return this.get_Property().get_Name();
			}
		}

		public PropertyDefinition Property
		{
			get;
			protected set;
		}

		public PropertyInitializerExpression(PropertyDefinition property, TypeReference type)
		{
			this(property, type, null);
			return;
		}

		public PropertyInitializerExpression(PropertyDefinition property, TypeReference type, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Property(property);
			this.expressionType = type;
			return;
		}

		public override Expression Clone()
		{
			return new PropertyInitializerExpression(this.get_Property(), this.expressionType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new PropertyInitializerExpression(this.get_Property(), this.expressionType, null);
		}

		public override bool Equals(Expression other)
		{
			return this == other;
		}
	}
}