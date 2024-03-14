using System;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast;
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
			this.blockToExpressionStack = new Dictionary<int, IList<Expression>>();
			this.exceptionHandlerStartToVariable = new Dictionary<int, VariableReferenceExpression>();
		}

		private string PrintExpressions(IEnumerable<Expression> stack)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Expression expression in stack)
			{
				stringBuilder.AppendLine(expression.ToCodeString());
			}
			return stringBuilder.ToString();
		}

		private string TabIn(string startingString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string[] strArray = startingString.Split(new String[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < (int)strArray.Length; i++)
			{
				string str = strArray[i].Trim();
				stringBuilder.Append("\t");
				stringBuilder.AppendLine(str);
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (int key in this.blockToExpressionStack.Keys)
			{
				stringBuilder.AppendFormat("Block starting at {0:X4} offset:", key);
				stringBuilder.AppendLine();
				stringBuilder.AppendLine("{");
				string str = this.TabIn(this.PrintExpressions(this.blockToExpressionStack[key]));
				stringBuilder.Append(str);
				stringBuilder.AppendLine("}");
				stringBuilder.AppendLine();
			}
			return stringBuilder.ToString();
		}
	}
}