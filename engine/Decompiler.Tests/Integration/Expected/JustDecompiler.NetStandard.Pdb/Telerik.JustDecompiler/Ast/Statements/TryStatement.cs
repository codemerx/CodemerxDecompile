using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class TryStatement : Statement
	{
		private BlockStatement try;

		private CatchClauseCollection catchClauses;

		private BlockStatement fault;

		private FinallyClause finally;

		public CatchClauseCollection CatchClauses
		{
			get
			{
				return this.catchClauses;
			}
			set
			{
				this.catchClauses = value;
				V_0 = this.catchClauses.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						this.SetParentToThis(V_1);
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

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new TryStatement.u003cget_Childrenu003ed__7(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 17;
			}
		}

		public BlockStatement Fault
		{
			get
			{
				return this.fault;
			}
			set
			{
				this.fault = value;
				if (this.fault != null)
				{
					this.fault.set_Parent(this);
				}
				return;
			}
		}

		public FinallyClause Finally
		{
			get
			{
				return this.finally;
			}
			set
			{
				this.finally = value;
				if (this.finally != null)
				{
					this.finally.set_Parent(this);
				}
				return;
			}
		}

		public BlockStatement Try
		{
			get
			{
				return this.try;
			}
			set
			{
				this.try = value;
				if (this.try != null)
				{
					this.try.set_Parent(this);
				}
				return;
			}
		}

		public TryStatement()
		{
			this.catchClauses = new CatchClauseCollection();
			base();
			return;
		}

		public TryStatement(BlockStatement try, BlockStatement fault, FinallyClause finally)
		{
			this.catchClauses = new CatchClauseCollection();
			base();
			this.set_Try(try);
			this.set_Fault(fault);
			this.set_Finally(finally);
			return;
		}

		public void AddToCatchClauses(CatchClause catchClause)
		{
			this.SetParentToThis(catchClause);
			this.get_CatchClauses().Add(catchClause);
			return;
		}

		public override Statement Clone()
		{
			if (this.try != null)
			{
				stackVariable5 = this.try.Clone() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			if (this.fault != null)
			{
				stackVariable11 = this.fault.Clone() as BlockStatement;
			}
			else
			{
				stackVariable11 = null;
			}
			V_0 = stackVariable11;
			if (this.finally != null)
			{
				stackVariable17 = this.finally.Clone() as FinallyClause;
			}
			else
			{
				stackVariable17 = null;
			}
			V_2 = new TryStatement(stackVariable5, V_0, stackVariable17);
			V_3 = this.catchClauses.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					V_2.AddToCatchClauses((CatchClause)V_4.Clone());
				}
			}
			finally
			{
				if (V_3 != null)
				{
					V_3.Dispose();
				}
			}
			this.CopyParentAndLabel(V_2);
			return V_2;
		}

		public override Statement CloneStatementOnly()
		{
			if (this.try != null)
			{
				stackVariable5 = this.try.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			if (this.fault != null)
			{
				stackVariable11 = this.fault.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				stackVariable11 = null;
			}
			V_0 = stackVariable11;
			if (this.finally != null)
			{
				stackVariable17 = this.finally.CloneStatementOnly() as FinallyClause;
			}
			else
			{
				stackVariable17 = null;
			}
			V_2 = new TryStatement(stackVariable5, V_0, stackVariable17);
			V_3 = this.catchClauses.GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					V_2.AddToCatchClauses((CatchClause)V_4.CloneStatementOnly());
				}
			}
			finally
			{
				if (V_3 != null)
				{
					V_3.Dispose();
				}
			}
			this.CopyParentAndLabel(V_2);
			return V_2;
		}

		private void SetParentToThis(CatchClause catchClause)
		{
			if (catchClause.get_Body() != null)
			{
				catchClause.get_Body().set_Parent(this);
			}
			if (catchClause.get_Filter() != null)
			{
				catchClause.get_Filter().set_Parent(this);
			}
			return;
		}
	}
}