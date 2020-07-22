using System;
using System.Collections.ObjectModel;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class CatchClauseCollection : Collection<CatchClause>
	{
		public CatchClauseCollection()
		{
			base();
			return;
		}
	}
}