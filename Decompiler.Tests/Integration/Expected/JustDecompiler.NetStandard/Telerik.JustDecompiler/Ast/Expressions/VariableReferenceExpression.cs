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
	public class VariableReferenceExpression : Expression
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.VariableReferenceExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Variable.VariableType;
			}
			set
			{
				this.Variable.VariableType = value;
			}
		}

		public bool IsByReference
		{
			get
			{
				return this.Variable.VariableType.IsByReference;
			}
		}

		public VariableReference Variable
		{
			get;
			set;
		}

		public VariableReferenceExpression(VariableReference variable, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Variable = variable;
		}

		public override Expression Clone()
		{
			return new VariableReferenceExpression(this.Variable, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new VariableReferenceExpression(this.Variable, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is VariableReferenceExpression))
			{
				return false;
			}
			VariableReferenceExpression variableReferenceExpression = other as VariableReferenceExpression;
			return this.Variable.Resolve() == variableReferenceExpression.Variable.Resolve();
		}
	}
}