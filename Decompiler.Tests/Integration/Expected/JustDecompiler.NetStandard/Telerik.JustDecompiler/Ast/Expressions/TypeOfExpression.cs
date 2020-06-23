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
	public class TypeOfExpression : Expression
	{
		private readonly TypeReference typeReference;

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
				return Telerik.JustDecompiler.Ast.CodeNodeType.TypeOfExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.typeReference;
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

		public TypeOfExpression(TypeReference type, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Type = type;
			this.typeReference = new TypeReference("System", "Type", this.Type.Module.TypeSystem.Boolean.Module, this.Type.Module.TypeSystem.Boolean.Scope);
		}

		public override Expression Clone()
		{
			return new TypeOfExpression(this.Type, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new TypeOfExpression(this.Type, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is TypeOfExpression))
			{
				return false;
			}
			return this.Type.FullName == (other as TypeOfExpression).Type.FullName;
		}
	}
}