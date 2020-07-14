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
	public class TypeReferenceExpression : Expression
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.TypeReferenceExpression;
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
				this.Type = value;
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

		public TypeReferenceExpression(TypeReference type, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Type = type;
		}

		public override Expression Clone()
		{
			return new TypeReferenceExpression(this.Type, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new TypeReferenceExpression(this.Type, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is TypeReferenceExpression))
			{
				return false;
			}
			TypeReferenceExpression typeReferenceExpression = other as TypeReferenceExpression;
			return this.Type.get_FullName() == typeReferenceExpression.Type.get_FullName();
		}
	}
}