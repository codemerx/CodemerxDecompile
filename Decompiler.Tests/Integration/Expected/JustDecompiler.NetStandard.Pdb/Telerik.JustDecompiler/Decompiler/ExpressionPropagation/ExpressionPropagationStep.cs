using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.ExpressionPropagation
{
	internal class ExpressionPropagationStep : IDecompilationStep
	{
		private DecompilationContext context;

		public ExpressionPropagationStep()
		{
			base();
			return;
		}

		private bool CanBePropagated(Expression expression)
		{
			return (new ExpressionPropagationStep.ExpressionTreeVisitor()).CanBePropagated(expression);
		}

		private bool ChangesAssignedExpression(Expression currentExpression, Expression assignedValue, Expression assignmentRecipient)
		{
			if (currentExpression as MethodInvocationExpression != null)
			{
				V_1 = currentExpression as MethodInvocationExpression;
				if (assignedValue.get_ExpressionType().get_IsByReference() && V_1.get_MethodExpression().get_Target() != null)
				{
					V_2 = V_1.get_MethodExpression().get_Target();
					if (V_2.Equals(assignmentRecipient))
					{
						return false;
					}
					if (V_2 as UnaryExpression != null && (V_2 as UnaryExpression).get_Operator() == 8 && (V_2 as UnaryExpression).get_Operand().Equals(assignmentRecipient))
					{
						return false;
					}
				}
				return true;
			}
			if (currentExpression as BinaryExpression == null)
			{
				return false;
			}
			V_0 = currentExpression as BinaryExpression;
			if (V_0.get_Left().Equals(assignedValue))
			{
				return true;
			}
			if (assignedValue as ArrayIndexerExpression != null)
			{
				V_3 = assignedValue as ArrayIndexerExpression;
				if (V_0.get_Left().Equals(V_3.get_Target()))
				{
					return true;
				}
				V_4 = V_3.get_Indices().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						if (!V_0.get_Left().Equals(V_5))
						{
							continue;
						}
						V_6 = true;
						goto Label1;
					}
					goto Label0;
				}
				finally
				{
					if (V_4 != null)
					{
						V_4.Dispose();
					}
				}
			Label1:
				return V_6;
			}
		Label0:
			if (assignedValue as UnaryExpression != null)
			{
				return this.ChangesAssignedExpression(currentExpression, (assignedValue as UnaryExpression).get_Operand(), assignmentRecipient);
			}
			if (assignedValue as ExplicitCastExpression != null)
			{
				return this.ChangesAssignedExpression(currentExpression, (assignedValue as ExplicitCastExpression).get_Expression(), assignmentRecipient);
			}
			if (assignedValue as CanCastExpression == null)
			{
				return false;
			}
			return this.ChangesAssignedExpression(currentExpression, (assignedValue as CanCastExpression).get_Expression(), assignmentRecipient);
		}

		private IEnumerable<BinaryExpression> GetAssignmentExpressions(IList<Expression> blockExpressions)
		{
			V_0 = new List<BinaryExpression>();
			V_1 = blockExpressions.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as BinaryExpression == null || !(V_2 as BinaryExpression).get_IsAssignmentExpression())
					{
						continue;
					}
					V_3 = V_2 as BinaryExpression;
					if (!this.CanBePropagated(V_3.get_Right()) || V_3.get_Left() as VariableReferenceExpression == null)
					{
						continue;
					}
					V_4 = V_3.get_Left() as VariableReferenceExpression;
					if (!V_4.get_ExpressionType().get_IsByReference() && !V_4.get_ExpressionType().get_IsPointer())
					{
						continue;
					}
					V_0.Add(V_3);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		private List<Expression> GetDominatedExpressions(BinaryExpression assignment, IList<Expression> blockExpressions)
		{
			V_0 = blockExpressions.IndexOf(assignment);
			V_1 = assignment.get_Right();
			V_2 = new List<Expression>();
			if (this.ChangesAssignedExpression(assignment.get_Right(), assignment.get_Left(), assignment.get_Left()))
			{
				return V_2;
			}
			V_3 = V_0 + 1;
			while (V_3 < blockExpressions.get_Count() && !this.ChangesAssignedExpression(blockExpressions.get_Item(V_3), V_1, assignment.get_Left()))
			{
				V_2.Add(blockExpressions.get_Item(V_3));
				V_3 = V_3 + 1;
			}
			return V_2;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.PropagateExpressions();
			return body;
		}

		private void PropagateExpressions()
		{
			V_0 = this.context.get_MethodContext().get_Expressions().get_BlockExpressions().get_Values().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = this.GetAssignmentExpressions(V_1).GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							V_4 = this.GetDominatedExpressions(V_3, V_1);
							V_5 = 0;
							while (V_5 < V_4.get_Count())
							{
								this.TryPropagate(V_4.get_Item(V_5), V_3.get_Left(), V_3.get_Right());
								V_5 = V_5 + 1;
							}
						}
					}
					finally
					{
						if (V_2 != null)
						{
							V_2.Dispose();
						}
					}
					V_6 = 0;
					while (V_6 < V_1.get_Count())
					{
						V_1.set_Item(V_6, this.TrySimplifyExpression(V_1.get_Item(V_6)));
						V_6 = V_6 + 1;
					}
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void TryPropagate(Expression dominatedExpression, Expression original, Expression replacement)
		{
			dummyVar0 = (new ExpressionPropagationStep.ExpressionTreeTransformer(original, replacement)).Visit(dominatedExpression);
			return;
		}

		private Expression TrySimplifyExpression(Expression expression)
		{
			return (new ExpressionPropagationStep.Simplifier()).Visit(expression) as Expression;
		}

		public class ExpressionTreeTransformer : BaseCodeTransformer
		{
			private readonly Expression replacement;

			private readonly Expression original;

			public ExpressionTreeTransformer(Expression original, Expression replacement)
			{
				base();
				this.replacement = replacement;
				this.original = original;
				return;
			}

			public override ICodeNode Visit(ICodeNode node)
			{
				V_0 = node as Expression;
				if (V_0 != null && V_0.Equals(this.original))
				{
					return this.replacement.CloneExpressionOnly();
				}
				return this.Visit(node);
			}
		}

		private class ExpressionTreeVisitor : BaseCodeVisitor
		{
			private bool canBePropagated;

			private ExpressionPropagationStep.ExpressionTreeVisitor.SearchState state;

			public ExpressionTreeVisitor()
			{
				base();
				return;
			}

			public bool CanBePropagated(Expression expression)
			{
				this.canBePropagated = true;
				this.state = 0;
				this.Visit(expression);
				return this.canBePropagated;
			}

			public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitAnonymousObjectCreationExpression(node);
				return;
			}

			public override void VisitArrayCreationExpression(ArrayCreationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitArrayCreationExpression(node);
				return;
			}

			public override void VisitBaseCtorExpression(BaseCtorExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitBaseCtorExpression(node);
				return;
			}

			public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitDelegateCreationExpression(node);
				return;
			}

			public override void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitDelegateInvokeExpression(node);
				return;
			}

			public override void VisitLambdaExpression(LambdaExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitLambdaExpression(node);
				return;
			}

			public override void VisitMakeRefExpression(MakeRefExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitMakeRefExpression(node);
				return;
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitMethodInvocationExpression(node);
				return;
			}

			public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitMethodReferenceExpression(node);
				return;
			}

			public override void VisitObjectCreationExpression(ObjectCreationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitObjectCreationExpression(node);
				return;
			}

			public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitPropertyReferenceExpression(node);
				return;
			}

			public override void VisitStackAllocExpression(StackAllocExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitStackAllocExpression(node);
				return;
			}

			public override void VisitThisCtorExpression(ThisCtorExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitThisCtorExpression(node);
				return;
			}

			public override void VisitYieldBreakExpression(YieldBreakExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitYieldBreakExpression(node);
				return;
			}

			public override void VisitYieldReturnExpression(YieldReturnExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				this.VisitYieldReturnExpression(node);
				return;
			}

			private enum SearchState
			{
				Propagation
			}
		}

		public class Simplifier : BaseCodeTransformer
		{
			public Simplifier()
			{
				base();
				return;
			}

			public override ICodeNode VisitUnaryExpression(UnaryExpression node)
			{
				if (node.get_Operator() == 8)
				{
					if (node.get_Operand() as UnaryExpression != null)
					{
						V_0 = node.get_Operand() as UnaryExpression;
						if (V_0.get_Operator() == 9 || V_0.get_Operator() == 7)
						{
							return this.Visit(V_0.get_Operand());
						}
					}
					if (node.get_Operand() as ExplicitCastExpression != null && node.get_Operand().get_ExpressionType().get_IsByReference())
					{
						V_1 = node.get_Operand() as ExplicitCastExpression;
						V_2 = (V_1.get_ExpressionType() as ByReferenceType).get_ElementType();
						V_3 = new ExplicitCastExpression(V_1.get_Expression(), V_2, V_1.get_MappedInstructions());
						return this.Visit(V_3);
					}
				}
				return this.VisitUnaryExpression(node);
			}
		}
	}
}