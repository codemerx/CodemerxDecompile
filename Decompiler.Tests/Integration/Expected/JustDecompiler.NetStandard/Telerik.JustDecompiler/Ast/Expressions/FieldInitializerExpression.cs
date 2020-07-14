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
	public class FieldInitializerExpression : Expression
	{
		private TypeReference expressionType;

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
				return Telerik.JustDecompiler.Ast.CodeNodeType.FieldInitializerExpression;
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

		public FieldDefinition Field
		{
			get;
			protected set;
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
				return this.Field.get_Name();
			}
		}

		public FieldInitializerExpression(FieldDefinition fieldDefinition, TypeReference type) : this(fieldDefinition, type, null)
		{
		}

		public FieldInitializerExpression(FieldDefinition fieldDefinition, TypeReference type, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Field = fieldDefinition;
			this.expressionType = type;
		}

		public override Expression Clone()
		{
			return new FieldInitializerExpression(this.Field, this.expressionType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new FieldInitializerExpression(this.Field, this.expressionType, null);
		}

		public override bool Equals(Expression other)
		{
			return this == other;
		}
	}
}