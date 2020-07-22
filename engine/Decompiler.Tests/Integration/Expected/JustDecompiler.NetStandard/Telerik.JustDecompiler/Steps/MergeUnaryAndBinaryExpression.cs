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
			base();
			return;
		}

		private bool IsConditionExpression(Expression expression)
		{
			if (expression as BinaryExpression != null || expression as UnaryExpression != null)
			{
				return true;
			}
			return expression as ConditionExpression != null;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.Visit(block);
			return block;
		}

		private void TryMergeExpressions(ConditionStatement node)
		{
			if (node.get_Condition() as UnaryExpression == null)
			{
				return;
			}
			V_0 = (UnaryExpression)node.get_Condition();
			if (V_0.get_Operator() != 1)
			{
				return;
			}
			if (V_0.get_Operand() as MethodInvocationExpression != null || V_0.get_Operand() as PropertyReferenceExpression != null)
			{
				return;
			}
			if (!this.IsConditionExpression(V_0.get_Operand()))
			{
				return;
			}
			node.set_Condition(Negator.Negate(V_0.get_Operand(), this.typeSystem));
			return;
		}

		public override void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.TryMergeExpressions(node);
			this.VisitDoWhileStatement(node);
			return;
		}

		public override void VisitForStatement(ForStatement node)
		{
			this.TryMergeExpressions(node);
			this.VisitForStatement(node);
			return;
		}

		public override void VisitIfStatement(IfStatement node)
		{
			this.TryMergeExpressions(node);
			this.VisitIfStatement(node);
			return;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			this.TryMergeExpressions(node);
			this.VisitWhileStatement(node);
			return;
		}
	}
}