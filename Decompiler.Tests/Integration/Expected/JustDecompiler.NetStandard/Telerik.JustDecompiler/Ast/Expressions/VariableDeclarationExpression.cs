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
	public class VariableDeclarationExpression : Expression
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.VariableDeclarationExpression;
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

		public VariableDefinition Variable
		{
			get;
			set;
		}

		public VariableDeclarationExpression(VariableDefinition variable, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Variable = variable;
		}

		public override Expression Clone()
		{
			return new VariableDeclarationExpression(this.Variable, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new VariableDeclarationExpression(this.Variable, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is VariableDeclarationExpression))
			{
				return false;
			}
			VariableDeclarationExpression variableDeclarationExpression = other as VariableDeclarationExpression;
			return this.Variable.Resolve() == variableDeclarationExpression.Variable.Resolve();
		}
	}
}