using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class SwitchStatement : ConditionStatement
	{
		private SwitchCaseCollection cases = new SwitchCaseCollection();

		private SwitchCase[] casesAsArray;

		private bool needsRefreshing;

		private readonly Instruction switchInstruction;

		public IEnumerable<SwitchCase> Cases
		{
			get
			{
				if (this.needsRefreshing)
				{
					this.casesAsArray = new SwitchCase[this.cases.Count];
					int num = 0;
					foreach (SwitchCase @case in this.cases)
					{
						int num1 = num;
						num = num1 + 1;
						this.casesAsArray[num1] = @case;
					}
					this.needsRefreshing = false;
				}
				return this.casesAsArray;
			}
			set
			{
				this.cases = new SwitchCaseCollection();
				foreach (SwitchCase switchCase in value)
				{
					this.AddCase(switchCase);
				}
				this.needsRefreshing = true;
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				SwitchStatement switchStatement = null;
				yield return switchStatement.Condition;
				foreach (SwitchCase @case in switchStatement.Cases)
				{
					yield return @case;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.SwitchStatement;
			}
		}

		public Instruction SwitchInstruction
		{
			get
			{
				return this.switchInstruction;
			}
		}

		public SwitchStatement(Expression condition, Instruction instruction) : base(condition)
		{
			this.needsRefreshing = true;
			this.switchInstruction = instruction;
		}

		public void AddCase(SwitchCase @case)
		{
			this.cases.Add(@case);
			@case.Parent = this;
			this.needsRefreshing = true;
		}

		public override Statement Clone()
		{
			return this.CloneStatement(true);
		}

		private Statement CloneStatement(bool copyInstructions)
		{
			SwitchStatement switchStatement = (copyInstructions ? new SwitchStatement(base.Condition.Clone(), this.switchInstruction) : new SwitchStatement(base.Condition.CloneExpressionOnly(), null));
			foreach (SwitchCase @case in this.cases)
			{
				switchStatement.AddCase((SwitchCase)((copyInstructions ? @case.Clone() : @case.CloneStatementOnly())));
			}
			base.CopyParentAndLabel(switchStatement);
			return switchStatement;
		}

		public override Statement CloneStatementOnly()
		{
			return this.CloneStatement(false);
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			SwitchStatement switchStatement = null;
			if (switchStatement.switchInstruction != null)
			{
				yield return switchStatement.switchInstruction;
			}
		}
	}
}