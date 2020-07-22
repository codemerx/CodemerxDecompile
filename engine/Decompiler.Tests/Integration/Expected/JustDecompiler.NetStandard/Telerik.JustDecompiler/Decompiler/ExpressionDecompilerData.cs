using System;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
	public class ExpressionDecompilerData
	{
		private readonly Dictionary<int, IList<Expression>> blockToExpressionStack;

		private readonly Dictionary<int, VariableReferenceExpression> exceptionHandlerStartToVariable;

		public Dictionary<int, IList<Expression>> BlockExpressions
		{
			get
			{
				return this.blockToExpressionStack;
			}
		}

		public Dictionary<int, VariableReferenceExpression> ExceptionHandlerStartToVariable
		{
			get
			{
				return this.exceptionHandlerStartToVariable;
			}
		}

		public ExpressionDecompilerData()
		{
			base();
			this.blockToExpressionStack = new Dictionary<int, IList<Expression>>();
			this.exceptionHandlerStartToVariable = new Dictionary<int, VariableReferenceExpression>();
			return;
		}

		private string PrintExpressions(IEnumerable<Expression> stack)
		{
			V_0 = new StringBuilder();
			V_1 = stack.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					dummyVar0 = V_0.AppendLine(V_2.ToCodeString());
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0.ToString();
		}

		private string TabIn(string startingString)
		{
			V_0 = new StringBuilder();
			stackVariable3 = new String[1];
			stackVariable3[0] = Environment.get_NewLine();
			V_1 = startingString.Split(stackVariable3, 1);
			V_2 = 0;
			while (V_2 < (int)V_1.Length)
			{
				V_3 = V_1[V_2].Trim();
				dummyVar0 = V_0.Append("\t");
				dummyVar1 = V_0.AppendLine(V_3);
				V_2 = V_2 + 1;
			}
			return V_0.ToString();
		}

		public override string ToString()
		{
			V_0 = new StringBuilder();
			V_1 = this.blockToExpressionStack.get_Keys().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					dummyVar0 = V_0.AppendFormat("Block starting at {0:X4} offset:", V_2);
					dummyVar1 = V_0.AppendLine();
					dummyVar2 = V_0.AppendLine("{");
					V_3 = this.TabIn(this.PrintExpressions(this.blockToExpressionStack.get_Item(V_2)));
					dummyVar3 = V_0.Append(V_3);
					dummyVar4 = V_0.AppendLine("}");
					dummyVar5 = V_0.AppendLine();
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0.ToString();
		}
	}
}