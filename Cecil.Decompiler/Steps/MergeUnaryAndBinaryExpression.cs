#region license

//   (C) 2007 - 2008 Novell, Inc. http://www.novell.com
//   (C) 2007 - 2008 Jb Evain http://evain.net

//Permission is hereby granted, free of charge, to any person obtaining
//a copy of this software and associated documentation files (the
//"Software"), to deal in the Software without restriction, including
//without limitation the rights to use, copy, modify, merge, publish,
//distribute, sublicense, and/or sell copies of the Software, and to
//permit persons to whom the Software is furnished to do so, subject to
//the following conditions:

//The above copyright notice and this permission notice shall be
//included in all copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Steps
{

	public class MergeUnaryAndBinaryExpression : BaseCodeVisitor, IDecompilationStep
	{
		private TypeSystem typeSystem;

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			Visit(block);
			return block;
		}

		public override void VisitIfStatement(IfStatement node)
		{
			TryMergeExpressions(node);
			base.VisitIfStatement(node);
		}

		private void TryMergeExpressions(ConditionStatement node)
		{
			if (!(node.Condition is UnaryExpression))
			{
				return;
			}

			UnaryExpression unaryExpression = (UnaryExpression)node.Condition;

			if (unaryExpression.Operator != UnaryOperator.LogicalNot)
			{
				return;
			}

			if (unaryExpression.Operand is MethodInvocationExpression || unaryExpression.Operand is PropertyReferenceExpression)
			{
				return;
			}

			if (!IsConditionExpression(unaryExpression.Operand))
			{
				return;
			}

			node.Condition = Negator.Negate(unaryExpression.Operand, typeSystem);
		}

		private bool IsConditionExpression(Expression expression)
		{
			return expression is BinaryExpression || expression is UnaryExpression || expression is ConditionExpression;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			TryMergeExpressions(node);
			base.VisitWhileStatement(node);
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			TryMergeExpressions(node);
			base.VisitDoWhileStatement(node);
		}

		public override void VisitForStatement(ForStatement node)
		{
			TryMergeExpressions(node);
			base.VisitForStatement(node);
		}
	}
}