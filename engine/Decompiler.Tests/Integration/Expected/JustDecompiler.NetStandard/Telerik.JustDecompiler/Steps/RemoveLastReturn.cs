using System;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class RemoveLastReturn : IDecompilationStep
	{
		public RemoveLastReturn()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			if (block.get_Statements().get_Count() <= 0)
			{
				return block;
			}
			V_0 = block.get_Statements().get_Count() - 1;
			V_1 = block.get_Statements().get_Item(V_0) as ExpressionStatement;
			if (V_1 == null)
			{
				return block;
			}
			if (V_1.get_Expression() as ReturnExpression != null && (V_1.get_Expression() as ReturnExpression).get_Value() == null && String.IsNullOrEmpty(V_1.get_Label()))
			{
				block.get_Statements().RemoveAt(V_0);
			}
			return block;
		}
	}
}