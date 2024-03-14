using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class TryStatement : Statement
	{
		private BlockStatement @try;

		private CatchClauseCollection catchClauses = new CatchClauseCollection();

		private BlockStatement fault;

		private FinallyClause @finally;

		public CatchClauseCollection CatchClauses
		{
			get
			{
				return this.catchClauses;
			}
			set
			{
				this.catchClauses = value;
				foreach (CatchClause catchClause in this.catchClauses)
				{
					this.SetParentToThis(catchClause);
				}
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				TryStatement tryStatement = null;
				if (tryStatement.@try != null)
				{
					yield return tryStatement.@try;
				}
				if (tryStatement.@finally != null)
				{
					yield return tryStatement.@finally;
				}
				if (tryStatement.fault != null)
				{
					yield return tryStatement.fault;
				}
				foreach (CatchClause catchClause in tryStatement.CatchClauses)
				{
					yield return catchClause;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.TryStatement;
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
					this.fault.Parent = this;
				}
			}
		}

		public FinallyClause Finally
		{
			get
			{
				return this.@finally;
			}
			set
			{
				this.@finally = value;
				if (this.@finally != null)
				{
					this.@finally.Parent = this;
				}
			}
		}

		public BlockStatement Try
		{
			get
			{
				return this.@try;
			}
			set
			{
				this.@try = value;
				if (this.@try != null)
				{
					this.@try.Parent = this;
				}
			}
		}

		public TryStatement()
		{
		}

		public TryStatement(BlockStatement @try, BlockStatement fault, FinallyClause @finally)
		{
			this.Try = @try;
			this.Fault = fault;
			this.Finally = @finally;
		}

		public void AddToCatchClauses(CatchClause catchClause)
		{
			this.SetParentToThis(catchClause);
			this.CatchClauses.Add(catchClause);
		}

		public override Statement Clone()
		{
			BlockStatement blockStatement;
			FinallyClause finallyClause;
			BlockStatement blockStatement1;
			if (this.@try != null)
			{
				blockStatement1 = this.@try.Clone() as BlockStatement;
			}
			else
			{
				blockStatement1 = null;
			}
			if (this.fault != null)
			{
				blockStatement = this.fault.Clone() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			BlockStatement blockStatement2 = blockStatement;
			if (this.@finally != null)
			{
				finallyClause = this.@finally.Clone() as FinallyClause;
			}
			else
			{
				finallyClause = null;
			}
			TryStatement tryStatement = new TryStatement(blockStatement1, blockStatement2, finallyClause);
			foreach (CatchClause catchClause in this.catchClauses)
			{
				tryStatement.AddToCatchClauses((CatchClause)catchClause.Clone());
			}
			base.CopyParentAndLabel(tryStatement);
			return tryStatement;
		}

		public override Statement CloneStatementOnly()
		{
			BlockStatement blockStatement;
			FinallyClause finallyClause;
			BlockStatement blockStatement1;
			if (this.@try != null)
			{
				blockStatement1 = this.@try.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				blockStatement1 = null;
			}
			if (this.fault != null)
			{
				blockStatement = this.fault.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			BlockStatement blockStatement2 = blockStatement;
			if (this.@finally != null)
			{
				finallyClause = this.@finally.CloneStatementOnly() as FinallyClause;
			}
			else
			{
				finallyClause = null;
			}
			TryStatement tryStatement = new TryStatement(blockStatement1, blockStatement2, finallyClause);
			foreach (CatchClause catchClause in this.catchClauses)
			{
				tryStatement.AddToCatchClauses((CatchClause)catchClause.CloneStatementOnly());
			}
			base.CopyParentAndLabel(tryStatement);
			return tryStatement;
		}

		private void SetParentToThis(CatchClause catchClause)
		{
			if (catchClause.Body != null)
			{
				catchClause.Body.Parent = this;
			}
			if (catchClause.Filter != null)
			{
				catchClause.Filter.Parent = this;
			}
		}
	}
}