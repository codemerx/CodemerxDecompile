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

		private VariableDeclarationExpression var;

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
					this.body.set_Parent(this);
				}
				return;
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ForEachStatement.u003cget_Childrenu003ed__9(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 12;
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
				stackVariable1 = new ForEachStatement.u003cget_ConditionInstructionsu003ed__6(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public IEnumerable<Instruction> FinallyInstructions
		{
			get
			{
				stackVariable1 = new ForEachStatement.u003cget_FinallyInstructionsu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public VariableDeclarationExpression Variable
		{
			get
			{
				return this.var;
			}
			set
			{
				this.var = value;
				return;
			}
		}

		public ForEachStatement(VariableDeclarationExpression variable, Expression expression, BlockStatement body, IEnumerable<Instruction> conditionInstructions, IEnumerable<Instruction> finallyInstructions)
		{
			base();
			this.set_Variable(variable);
			this.set_Collection(expression);
			this.set_Body(body);
			this.mappedConditionInstructions = new List<Instruction>();
			if (conditionInstructions != null)
			{
				this.mappedConditionInstructions.AddRange(conditionInstructions);
				stackVariable25 = this.mappedConditionInstructions;
				stackVariable26 = ForEachStatement.u003cu003ec.u003cu003e9__7_0;
				if (stackVariable26 == null)
				{
					dummyVar0 = stackVariable26;
					stackVariable26 = new Comparison<Instruction>(ForEachStatement.u003cu003ec.u003cu003e9.u003cu002ectoru003eb__7_0);
					ForEachStatement.u003cu003ec.u003cu003e9__7_0 = stackVariable26;
				}
				stackVariable25.Sort(stackVariable26);
			}
			this.mappedFinallyInstructions = new List<Instruction>();
			if (finallyInstructions != null)
			{
				this.mappedFinallyInstructions.AddRange(finallyInstructions);
				stackVariable17 = this.mappedFinallyInstructions;
				stackVariable18 = ForEachStatement.u003cu003ec.u003cu003e9__7_1;
				if (stackVariable18 == null)
				{
					dummyVar1 = stackVariable18;
					stackVariable18 = new Comparison<Instruction>(ForEachStatement.u003cu003ec.u003cu003e9.u003cu002ectoru003eb__7_1);
					ForEachStatement.u003cu003ec.u003cu003e9__7_1 = stackVariable18;
				}
				stackVariable17.Sort(stackVariable18);
			}
			return;
		}

		public override Statement Clone()
		{
			if (this.get_Body() != null)
			{
				stackVariable5 = this.body.Clone() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = stackVariable5;
			V_1 = new ForEachStatement(this.get_Variable().Clone() as VariableDeclarationExpression, this.get_Collection().Clone(), V_0, this.mappedConditionInstructions, this.mappedFinallyInstructions);
			this.CopyParentAndLabel(V_1);
			return V_1;
		}

		public override Statement CloneStatementOnly()
		{
			if (this.get_Body() != null)
			{
				stackVariable5 = this.body.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = stackVariable5;
			V_1 = new ForEachStatement(this.get_Variable().CloneExpressionOnly() as VariableDeclarationExpression, this.get_Collection().CloneExpressionOnly(), V_0, null, null);
			this.CopyParentAndLabel(V_1);
			return V_1;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			stackVariable2 = new IEnumerable<Instruction>[2];
			stackVariable2[0] = this.mappedConditionInstructions;
			stackVariable2[1] = this.mappedFinallyInstructions;
			return this.MergeSortedEnumerables(stackVariable2);
		}
	}
}