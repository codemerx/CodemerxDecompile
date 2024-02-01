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
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ThisReferenceExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.TargetType;
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

		public ThisReferenceExpression(TypeReference targetType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.TargetType = targetType;
		}

		public override Expression Clone()
		{
			return new ThisReferenceExpression(this.TargetType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ThisReferenceExpression(this.TargetType, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ThisReferenceExpression))
			{
				return false;
			}
			return this.TargetType.get_FullName() == (other as ThisReferenceExpression).TargetType.get_FullName();
		}
	}
}