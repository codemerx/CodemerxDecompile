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
	public class ThisReferenceExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new ThisReferenceExpression.u003cget_Childrenu003ed__6(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 28;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_TargetType();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		public TypeReference TargetType
		{
			get;
			private set;
		}

		public ThisReferenceExpression(TypeReference targetType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_TargetType(targetType);
			return;
		}

		public override Expression Clone()
		{
			return new ThisReferenceExpression(this.get_TargetType(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ThisReferenceExpression(this.get_TargetType(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as ThisReferenceExpression == null)
			{
				return false;
			}
			return String.op_Equality(this.get_TargetType().get_FullName(), (other as ThisReferenceExpression).get_TargetType().get_FullName());
		}
	}
}