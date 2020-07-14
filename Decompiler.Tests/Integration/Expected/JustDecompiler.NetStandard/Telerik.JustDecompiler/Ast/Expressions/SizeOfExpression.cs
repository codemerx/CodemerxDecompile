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
	public class SizeOfExpression : Expression
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.SizeOfExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Type.get_Module().get_TypeSystem().get_UInt32();
			}
			set
			{
				throw new NotSupportedException("The return value of sizeof is always unsigned int");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public TypeReference Type
		{
			get;
			set;
		}

		public SizeOfExpression(TypeReference type, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Type = type;
		}

		public override Expression Clone()
		{
			return new SizeOfExpression(this.Type, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new SizeOfExpression(this.Type, null)
			{
				Type = this.Type
			};
		}

		public override bool Equals(Expression other)
		{
			if (!(other is SizeOfExpression))
			{
				return false;
			}
			return this.Type.get_FullName() == (other as SizeOfExpression).Type.get_FullName();
		}
	}
}