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
				return new BaseReferenceExpression.u003cget_Childrenu003ed__2(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 29;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_TargetType().Resolve();
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

		public BaseReferenceExpression(TypeReference targetType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_TargetType(targetType);
			return;
		}

		public override Expression Clone()
		{
			return new BaseReferenceExpression(this.get_TargetType(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new BaseReferenceExpression(this.get_TargetType(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as BaseReferenceExpression == null)
			{
				return false;
			}
			return String.op_Equality(this.get_TargetType().get_FullName(), (other as BaseReferenceExpression).get_TargetType().get_FullName());
		}
	}
}