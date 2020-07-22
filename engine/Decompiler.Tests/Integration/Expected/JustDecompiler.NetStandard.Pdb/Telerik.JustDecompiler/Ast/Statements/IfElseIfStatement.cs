using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class IfElseIfStatement : BasePdbStatement
	{
		private List<KeyValuePair<Expression, BlockStatement>> conditionBlocks;

		private BlockStatement else;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new IfElseIfStatement.u003cget_Childrenu003ed__4(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 4;
			}
		}

		public List<KeyValuePair<Expression, BlockStatement>> ConditionBlocks
		{
			get
			{
				return this.conditionBlocks;
			}
			set
			{
				this.conditionBlocks = value;
				V_0 = this.conditionBlocks.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						V_1.get_Value().set_Parent(this);
					}
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
				return;
			}
		}

		public BlockStatement Else
		{
			get
			{
				return this.else;
			}
			set
			{
				this.else = value;
				if (this.else != null)
				{
					this.else.set_Parent(this);
				}
				return;
			}
		}

		public IfElseIfStatement(List<KeyValuePair<Expression, BlockStatement>> conditionBlocks, BlockStatement else)
		{
			base();
			this.set_ConditionBlocks(conditionBlocks);
			this.set_Else(else);
			return;
		}

		public override Statement Clone()
		{
			return this.CloneStatement(true);
		}

		private Statement CloneStatement(bool copyInstructions)
		{
			V_0 = new List<KeyValuePair<Expression, BlockStatement>>();
			V_3 = this.conditionBlocks.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					if (copyInstructions)
					{
						stackVariable11 = V_4.get_Key().Clone();
					}
					else
					{
						stackVariable11 = V_4.get_Key().CloneExpressionOnly();
					}
					V_5 = stackVariable11;
					if (copyInstructions)
					{
						stackVariable16 = (BlockStatement)V_4.get_Value().Clone();
					}
					else
					{
						stackVariable16 = (BlockStatement)V_4.get_Value().CloneStatementOnly();
					}
					V_0.Add(new KeyValuePair<Expression, BlockStatement>(V_5, stackVariable16));
				}
			}
			finally
			{
				((IDisposable)V_3).Dispose();
			}
			V_1 = null;
			if (this.else != null)
			{
				if (copyInstructions)
				{
					stackVariable39 = (BlockStatement)this.else.Clone();
				}
				else
				{
					stackVariable39 = (BlockStatement)this.else.CloneStatementOnly();
				}
				V_1 = stackVariable39;
			}
			V_2 = new IfElseIfStatement(V_0, V_1);
			this.CopyParentAndLabel(V_2);
			return V_2;
		}

		public override Statement CloneStatementOnly()
		{
			return this.CloneStatement(false);
		}
	}
}