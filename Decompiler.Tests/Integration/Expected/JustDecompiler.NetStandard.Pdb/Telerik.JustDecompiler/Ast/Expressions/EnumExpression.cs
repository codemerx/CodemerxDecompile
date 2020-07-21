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
				return new EnumExpression.u003cget_Childrenu003ed__2(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 49;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Field().get_DeclaringType();
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
				return this.get_Field().get_Name();
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public EnumExpression(FieldDefinition fieldDefinition, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Field(fieldDefinition);
			return;
		}

		public override Expression Clone()
		{
			return new EnumExpression(this.get_Field(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new EnumExpression(this.get_Field(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as EnumExpression == null)
			{
				return false;
			}
			return String.op_Equality(this.get_Field().get_FullName(), (other as EnumExpression).get_Field().get_FullName());
		}
	}
}