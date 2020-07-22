using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildVBForStatements : RebuildForStatements
	{
		public RebuildVBForStatements()
		{
			base();
			return;
		}

		protected override bool CheckTheLoop(WhileStatement theWhile, VariableReference forVariable)
		{
			if (!this.CheckTheLoop(theWhile, forVariable))
			{
				stackVariable4 = false;
			}
			else
			{
				stackVariable4 = theWhile.get_Condition() as BinaryExpression != null;
			}
			if (!stackVariable4)
			{
				return false;
			}
			V_0 = (theWhile.get_Body().get_Statements().get_Item(theWhile.get_Body().get_Statements().get_Count() - 1) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_0 != null)
			{
				V_1 = V_0.get_Right() as BinaryExpression;
				if (V_1 != null && V_1.get_Operator() == 1 || V_1.get_Operator() == 3)
				{
					V_2 = V_1.get_Left() as VariableReferenceExpression;
					if (V_2 != null && (object)V_2.get_Variable() == (object)forVariable)
					{
						return true;
					}
				}
			}
			return false;
		}
	}
}