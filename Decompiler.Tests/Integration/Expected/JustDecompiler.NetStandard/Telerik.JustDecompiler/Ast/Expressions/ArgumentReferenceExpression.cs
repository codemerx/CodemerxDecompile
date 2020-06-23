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
	public class ArgumentReferenceExpression : Expression
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.ArgumentReferenceExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Parameter.ParameterType;
			}
			set
			{
				throw new NotSupportedException("Type of Argument reference expression cannot be changed.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public ParameterReference Parameter
		{
			get;
			set;
		}

		public ArgumentReferenceExpression(ParameterReference parameter, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Parameter = parameter;
		}

		public override Expression Clone()
		{
			return new ArgumentReferenceExpression(this.Parameter, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArgumentReferenceExpression(this.Parameter, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ArgumentReferenceExpression))
			{
				return false;
			}
			return this.Parameter.Index == (other as ArgumentReferenceExpression).Parameter.Index;
		}
	}
}