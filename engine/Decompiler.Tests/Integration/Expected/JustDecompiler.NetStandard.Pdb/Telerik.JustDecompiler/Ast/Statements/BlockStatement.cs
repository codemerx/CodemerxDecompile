using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class BlockStatement : Statement
	{
		private StatementCollection statements;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new BlockStatement.u003cget_Childrenu003ed__4(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 0;
			}
		}

		public StatementCollection Statements
		{
			get
			{
				return this.statements;
			}
			set
			{
				this.statements = value;
				V_0 = this.statements.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_0.get_Current().set_Parent(this);
					}
				}
				finally
				{
					if (V_0 != null)
					{
						V_0.Dispose();
					}
				}
				return;
			}
		}

		public BlockStatement()
		{
			this.statements = new StatementCollection();
			base();
			return;
		}

		public void AddStatement(Statement statement)
		{
			this.AddStatementAt(this.statements.get_Count(), statement);
			return;
		}

		public void AddStatementAt(int index, Statement statement)
		{
			this.statements.Insert(index, statement);
			statement.set_Parent(this);
			return;
		}

		public override Statement Clone()
		{
			V_0 = new BlockStatement();
			this.CopyParentAndLabel(V_0);
			V_1 = this.statements.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.AddStatement(V_2.Clone());
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new BlockStatement();
			this.CopyParentAndLabel(V_0);
			V_1 = this.statements.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.AddStatement(V_2.CloneStatementOnly());
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}
	}
}