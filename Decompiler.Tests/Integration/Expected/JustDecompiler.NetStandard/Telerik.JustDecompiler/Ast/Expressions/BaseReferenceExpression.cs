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
	public class BaseReferenceExpression : Expression
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.BaseReferenceExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.TargetType.Resolve();
			}
			set
			{
				throw new NotSupportedException("Base reference expressions cannot change their type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public TypeReference TargetType
		{
			get;
			private set;
		}

		public BaseReferenceExpression(TypeReference targetType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.TargetType = targetType;
		}

		public override Expression Clone()
		{
			return new BaseReferenceExpression(this.TargetType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new BaseReferenceExpression(this.TargetType, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is BaseReferenceExpression))
			{
				return false;
			}
			return this.TargetType.get_FullName() == (other as BaseReferenceExpression).TargetType.get_FullName();
		}
	}
}