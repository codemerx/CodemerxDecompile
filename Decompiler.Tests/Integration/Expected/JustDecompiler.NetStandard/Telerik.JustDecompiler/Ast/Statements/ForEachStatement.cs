using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class ForEachStatement : BasePdbStatement
	{
		private BlockStatement body;

		private readonly List<Instruction> mappedFinallyInstructions;

		private readonly List<Instruction> mappedConditionInstructions;

		private VariableDeclarationExpression @var;

		public BlockStatement Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
				if (this.body != null)
				{
					this.body.Parent = this;
				}
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ForEachStatement forEachStatement = null;
				yield return forEachStatement.Collection;
				yield return forEachStatement.Variable;
				if (forEachStatement.body != null)
				{
					yield return forEachStatement.body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ForEachStatement;
			}
		}

		public Expression Collection
		{
			get;
			set;
		}

		public IEnumerable<Instruction> ConditionInstructions
		{
			get
			{
				ForEachStatement forEachStatement = null;
				foreach (Instruction mappedConditionInstruction in forEachStatement.mappedConditionInstructions)
				{
					yield return mappedConditionInstruction;
				}
			}
		}

		public IEnumerable<Instruction> FinallyInstructions
		{
			get
			{
				ForEachStatement forEachStatement = null;
				foreach (Instruction mappedFinallyInstruction in forEachStatement.mappedFinallyInstructions)
				{
					yield return mappedFinallyInstruction;
				}
			}
		}

		public VariableDeclarationExpression Variable
		{
			get
			{
				return this.@var;
			}
			set
			{
				this.@var = value;
			}
		}

		public ForEachStatement(VariableDeclarationExpression variable, Expression expression, BlockStatement body, IEnumerable<Instruction> conditionInstructions, IEnumerable<Instruction> finallyInstructions)
		{
			this.Variable = variable;
			this.Collection = expression;
			this.Body = body;
			this.mappedConditionInstructions = new List<Instruction>();
			if (conditionInstructions != null)
			{
				this.mappedConditionInstructions.AddRange(conditionInstructions);
				this.mappedConditionInstructions.Sort((Instruction x, Instruction y) => x.get_Offset().CompareTo(y.get_Offset()));
			}
			this.mappedFinallyInstructions = new List<Instruction>();
			if (finallyInstructions != null)
			{
				this.mappedFinallyInstructions.AddRange(finallyInstructions);
				this.mappedFinallyInstructions.Sort((Instruction x, Instruction y) => x.get_Offset().CompareTo(y.get_Offset()));
			}
		}

		public override Statement Clone()
		{
			BlockStatement blockStatement;
			if (this.Body != null)
			{
				blockStatement = this.body.Clone() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			BlockStatement blockStatement1 = blockStatement;
			ForEachStatement forEachStatement = new ForEachStatement(this.Variable.Clone() as VariableDeclarationExpression, this.Collection.Clone(), blockStatement1, this.mappedConditionInstructions, this.mappedFinallyInstructions);
			base.CopyParentAndLabel(forEachStatement);
			return forEachStatement;
		}

		public override Statement CloneStatementOnly()
		{
			BlockStatement blockStatement;
			if (this.Body != null)
			{
				blockStatement = this.body.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			BlockStatement blockStatement1 = blockStatement;
			ForEachStatement forEachStatement = new ForEachStatement(this.Variable.CloneExpressionOnly() as VariableDeclarationExpression, this.Collection.CloneExpressionOnly(), blockStatement1, null, null);
			base.CopyParentAndLabel(forEachStatement);
			return forEachStatement;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			return base.MergeSortedEnumerables(new IEnumerable<Instruction>[] { this.mappedConditionInstructions, this.mappedFinallyInstructions });
		}
	}
}