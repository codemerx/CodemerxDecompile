using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RemoveLastReturn : IDecompilationStep
	{
		public RemoveLastReturn()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			if (block.Statements.Count <= 0)
			{
				return block;
			}
			int count = block.Statements.Count - 1;
			ExpressionStatement item = block.Statements[count] as ExpressionStatement;
			if (item == null)
			{
				return block;
			}
			if (item.Expression is ReturnExpression && (item.Expression as ReturnExpression).Value == null && String.IsNullOrEmpty(item.Label))
			{
				block.Statements.RemoveAt(count);
			}
			return block;
		}
	}
}