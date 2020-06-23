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
	public class EnumExpression : Expression
	{
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.EnumExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Field.DeclaringType;
			}
			set
			{
				throw new NotSupportedException("Enum Expression cannot change its type.");
			}
		}

		public FieldDefinition Field
		{
			get;
			private set;
		}

		public string FieldName
		{
			get
			{
				return this.Field.Name;
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public EnumExpression(FieldDefinition fieldDefinition, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Field = fieldDefinition;
		}

		public override Expression Clone()
		{
			return new EnumExpression(this.Field, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new EnumExpression(this.Field, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is EnumExpression))
			{
				return false;
			}
			return this.Field.FullName == (other as EnumExpression).Field.FullName;
		}
	}
}