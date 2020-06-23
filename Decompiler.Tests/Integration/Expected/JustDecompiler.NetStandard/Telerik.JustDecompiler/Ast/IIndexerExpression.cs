using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast
{
	public interface IIndexerExpression
	{
		ExpressionCollection Indices
		{
			get;
			set;
		}

		Expression Target
		{
			get;
			set;
		}
	}
}