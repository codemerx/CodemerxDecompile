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
	public class LockStatement : BasePdbStatement
	{
		private BlockStatement body;

		private readonly List<Instruction> mappedFinallyInstructions;

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
				stackVariable1 = new LockStatement.u003cget_Childrenu003ed__5(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 56;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public LockStatement(Telerik.JustDecompiler.Ast.Expressions.Expression expression, BlockStatement body, IEnumerable<Instruction> finallyInstructions)
		{
			base();
			this.set_Expression(expression);
			this.body = body;
			this.mappedFinallyInstructions = new List<Instruction>();
			if (finallyInstructions != null)
			{
				this.mappedFinallyInstructions.AddRange(finallyInstructions);
				stackVariable12 = this.mappedFinallyInstructions;
				stackVariable13 = LockStatement.u003cu003ec.u003cu003e9__2_0;
				if (stackVariable13 == null)
				{
					dummyVar0 = stackVariable13;
					stackVariable13 = new Comparison<Instruction>(LockStatement.u003cu003ec.u003cu003e9.u003cu002ectoru003eb__2_0);
					LockStatement.u003cu003ec.u003cu003e9__2_0 = stackVariable13;
				}
				stackVariable12.Sort(stackVariable13);
			}
			return;
		}

		public override Statement Clone()
		{
			V_0 = null;
			if (this.body != null)
			{
				V_0 = this.body.Clone() as BlockStatement;
			}
			V_1 = new LockStatement(this.get_Expression().Clone(), V_0, this.mappedFinallyInstructions);
			this.CopyParentAndLabel(V_1);
			return V_1;
		}

		public override Statement CloneStatementOnly()
		{
			if (this.body != null)
			{
				stackVariable5 = this.body.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = stackVariable5;
			V_1 = new LockStatement(this.get_Expression().CloneExpressionOnly(), V_0, null);
			this.CopyParentAndLabel(V_1);
			return V_1;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			stackVariable1 = new LockStatement.u003cGetOwnInstructionsu003ed__3(-2);
			stackVariable1.u003cu003e4__this = this;
			return stackVariable1;
		}
	}
}