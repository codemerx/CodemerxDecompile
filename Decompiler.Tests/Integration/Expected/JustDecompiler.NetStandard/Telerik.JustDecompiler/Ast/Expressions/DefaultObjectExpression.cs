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
	public class DefaultObjectExpression : Expression
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.DefaultObjectExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Type;
			}
			set
			{
				throw new NotSupportedException("Default object creation expression cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return (object)this.Type != (object)null;
			}
		}

		public TypeReference Type
		{
			get;
			set;
		}

		public DefaultObjectExpression(TypeReference type, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Type = type;
		}

		public override Expression Clone()
		{
			return new DefaultObjectExpression(this.Type, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new DefaultObjectExpression(this.Type, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is DefaultObjectExpression))
			{
				return false;
			}
			return this.Type.get_FullName() == (other as DefaultObjectExpression).Type.get_FullName();
		}
	}
}