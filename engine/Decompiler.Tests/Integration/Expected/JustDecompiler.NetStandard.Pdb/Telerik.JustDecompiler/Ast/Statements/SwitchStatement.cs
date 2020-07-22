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
	public class SwitchStatement : ConditionStatement
	{
		private SwitchCaseCollection cases;

		private SwitchCase[] casesAsArray;

		private bool needsRefreshing;

		private readonly Instruction switchInstruction;

		public IEnumerable<SwitchCase> Cases
		{
			get
			{
				if (this.needsRefreshing)
				{
					this.casesAsArray = new SwitchCase[this.cases.get_Count()];
					V_0 = 0;
					V_1 = this.cases.GetEnumerator();
					try
					{
						while (V_1.MoveNext())
						{
							V_2 = V_1.get_Current();
							stackVariable19 = V_0;
							V_0 = stackVariable19 + 1;
							this.casesAsArray[stackVariable19] = V_2;
						}
					}
					finally
					{
						if (V_1 != null)
						{
							V_1.Dispose();
						}
					}
					this.needsRefreshing = false;
				}
				return this.casesAsArray;
			}
			set
			{
				this.cases = new SwitchCaseCollection();
				V_0 = value.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.AddCase(V_1);
					}
				}
				finally
				{
					if (V_0 != null)
					{
						V_0.Dispose();
					}
				}
				this.needsRefreshing = true;
				return;
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new SwitchStatement.u003cget_Childrenu003ed__9(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 15;
			}
		}

		public Instruction SwitchInstruction
		{
			get
			{
				return this.switchInstruction;
			}
		}

		public SwitchStatement(Expression condition, Instruction instruction)
		{
			this.cases = new SwitchCaseCollection();
			base(condition);
			this.needsRefreshing = true;
			this.switchInstruction = instruction;
			return;
		}

		public void AddCase(SwitchCase case)
		{
			this.cases.Add(case);
			case.set_Parent(this);
			this.needsRefreshing = true;
			return;
		}

		public override Statement Clone()
		{
			return this.CloneStatement(true);
		}

		private Statement CloneStatement(bool copyInstructions)
		{
			if (copyInstructions)
			{
				stackVariable6 = new SwitchStatement(this.get_Condition().Clone(), this.switchInstruction);
			}
			else
			{
				stackVariable6 = new SwitchStatement(this.get_Condition().CloneExpressionOnly(), null);
			}
			V_0 = stackVariable6;
			V_1 = this.cases.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					stackVariable14 = V_0;
					if (copyInstructions)
					{
						stackVariable17 = V_2.Clone();
					}
					else
					{
						stackVariable17 = V_2.CloneStatementOnly();
					}
					stackVariable14.AddCase((SwitchCase)stackVariable17);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			return this.CloneStatement(false);
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			stackVariable1 = new SwitchStatement.u003cGetOwnInstructionsu003ed__7(-2);
			stackVariable1.u003cu003e4__this = this;
			return stackVariable1;
		}
	}
}