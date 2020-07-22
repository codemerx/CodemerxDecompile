using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using System.Text;
using System;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
	public class ExpressionDecompilerData
	{
        private readonly Dictionary<int, IList<Expression>> blockToExpressionStack;
        private readonly Dictionary<int, VariableReferenceExpression> exceptionHandlerStartToVariable;

        public ExpressionDecompilerData()
        {
            blockToExpressionStack = new Dictionary<int, IList<Expression>>();
            exceptionHandlerStartToVariable = new Dictionary<int, VariableReferenceExpression>();
        }

        public Dictionary<int, IList<Expression>> BlockExpressions
        {
			get { return blockToExpressionStack; }
		}

        public Dictionary<int, VariableReferenceExpression> ExceptionHandlerStartToVariable
        {
			get { return exceptionHandlerStartToVariable; }
		}

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (int blockStart in blockToExpressionStack.Keys)
            {
                sb.AppendFormat("Block starting at {0:X4} offset:",blockStart);
                sb.AppendLine();
                sb.AppendLine("{");
                string tabbedIn = TabIn(PrintExpressions(blockToExpressionStack[blockStart]));
                sb.Append(tabbedIn);
                sb.AppendLine("}");
                sb.AppendLine();
            }
            return sb.ToString();
        }

        private string TabIn(string startingString)
        {
            StringBuilder sb = new StringBuilder();
            string[] lines = startingString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in lines)
            {
                string lineToAdd = line.Trim();
                sb.Append("\t");
                sb.AppendLine(lineToAdd);
            }
            return sb.ToString();
        }

        private string PrintExpressions(IEnumerable<Expression> stack)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Expression expr in stack)
            {
                sb.AppendLine(expr.ToCodeString());
            }
            return sb.ToString();
        }
	}
}
