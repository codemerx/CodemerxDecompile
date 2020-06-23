using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class MergeUnaryAndBinaryExpression : BaseCodeVisitor, IDecompilationStep
	{
		private TypeSystem typeSystem;

		public MergeUnaryAndBinaryExpression()
		{
		}

		private bool IsConditionExpression(Expression expression)
		{
			if (expression is BinaryExpression || expression is UnaryExpression)
			{
				return true;
			}
			return expression is ConditionExpression;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			this.Visit(block);
			return block;
		}

		private void TryMergeExpressions(ConditionStatement node)
		{
			if (!(node.Condition is UnaryExpression))
			{
				return;
			}
			UnaryExpression condition = (UnaryExpression)node.Condition;
			if (condition.Operator != UnaryOperator.LogicalNot)
			{
				return;
			}
			if (condition.Operand is MethodInvocationExpression || condition.Operand is PropertyReferenceExpression)
			{
				return;
			}
			if (!this.IsConditionExpression(condition.Operand))
			{
				return;
			}
			node.Condition = Negator.Negate(condition.Operand, this.typeSystem);
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.TryMergeExpressions(node);
			base.VisitDoWhileStatement(node);
		}

		public override void VisitForStatement(ForStatement node)
		{
			this.TryMergeExpressions(node);
			base.VisitForStatement(node);
		}

		public override void VisitIfStatement(IfStatement node)
		{
			this.TryMergeExpressions(node);
			base.VisitIfStatement(node);
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			this.TryMergeExpressions(node);
			base.VisitWhileStatement(node);
		}
	}
}