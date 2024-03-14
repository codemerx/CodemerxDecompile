using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		}

		private bool CanBePropagated(Expression expression)
		{
			return (new ExpressionPropagationStep.ExpressionTreeVisitor()).CanBePropagated(expression);
		}

		private bool ChangesAssignedExpression(Expression currentExpression, Expression assignedValue, Expression assignmentRecipient)
		{
			bool flag;
			if (currentExpression is MethodInvocationExpression)
			{
				MethodInvocationExpression methodInvocationExpression = currentExpression as MethodInvocationExpression;
				if (assignedValue.ExpressionType.get_IsByReference() && methodInvocationExpression.MethodExpression.Target != null)
				{
					Expression target = methodInvocationExpression.MethodExpression.Target;
					if (target.Equals(assignmentRecipient))
					{
						return false;
					}
					if (target is UnaryExpression && (target as UnaryExpression).Operator == UnaryOperator.AddressDereference && (target as UnaryExpression).Operand.Equals(assignmentRecipient))
					{
						return false;
					}
				}
				return true;
			}
			if (!(currentExpression is BinaryExpression))
			{
				return false;
			}
			BinaryExpression binaryExpression = currentExpression as BinaryExpression;
			if (binaryExpression.Left.Equals(assignedValue))
			{
				return true;
			}
			if (assignedValue is ArrayIndexerExpression)
			{
				ArrayIndexerExpression arrayIndexerExpression = assignedValue as ArrayIndexerExpression;
				if (binaryExpression.Left.Equals(arrayIndexerExpression.Target))
				{
					return true;
				}
				using (IEnumerator<Expression> enumerator = arrayIndexerExpression.Indices.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						Expression current = enumerator.Current;
						if (!binaryExpression.Left.Equals(current))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					if (assignedValue is UnaryExpression)
					{
						return this.ChangesAssignedExpression(currentExpression, (assignedValue as UnaryExpression).Operand, assignmentRecipient);
					}
					if (assignedValue is ExplicitCastExpression)
					{
						return this.ChangesAssignedExpression(currentExpression, (assignedValue as ExplicitCastExpression).Expression, assignmentRecipient);
					}
					if (!(assignedValue is CanCastExpression))
					{
						return false;
					}
					return this.ChangesAssignedExpression(currentExpression, (assignedValue as CanCastExpression).Expression, assignmentRecipient);
				}
				return flag;
			}
			if (assignedValue is UnaryExpression)
			{
				return this.ChangesAssignedExpression(currentExpression, (assignedValue as UnaryExpression).Operand, assignmentRecipient);
			}
			if (assignedValue is ExplicitCastExpression)
			{
				return this.ChangesAssignedExpression(currentExpression, (assignedValue as ExplicitCastExpression).Expression, assignmentRecipient);
			}
			if (!(assignedValue is CanCastExpression))
			{
				return false;
			}
			return this.ChangesAssignedExpression(currentExpression, (assignedValue as CanCastExpression).Expression, assignmentRecipient);
		}

		private IEnumerable<BinaryExpression> GetAssignmentExpressions(IList<Expression> blockExpressions)
		{
			List<BinaryExpression> binaryExpressions = new List<BinaryExpression>();
			foreach (Expression blockExpression in blockExpressions)
			{
				if (!(blockExpression is BinaryExpression) || !(blockExpression as BinaryExpression).IsAssignmentExpression)
				{
					continue;
				}
				BinaryExpression binaryExpression = blockExpression as BinaryExpression;
				if (!this.CanBePropagated(binaryExpression.Right) || !(binaryExpression.Left is VariableReferenceExpression))
				{
					continue;
				}
				VariableReferenceExpression left = binaryExpression.Left as VariableReferenceExpression;
				if (!left.ExpressionType.get_IsByReference() && !left.ExpressionType.get_IsPointer())
				{
					continue;
				}
				binaryExpressions.Add(binaryExpression);
			}
			return binaryExpressions;
		}

		private List<Expression> GetDominatedExpressions(BinaryExpression assignment, IList<Expression> blockExpressions)
		{
			int num = blockExpressions.IndexOf(assignment);
			Expression right = assignment.Right;
			List<Expression> expressions = new List<Expression>();
			if (this.ChangesAssignedExpression(assignment.Right, assignment.Left, assignment.Left))
			{
				return expressions;
			}
			for (int i = num + 1; i < blockExpressions.Count && !this.ChangesAssignedExpression(blockExpressions[i], right, assignment.Left); i++)
			{
				expressions.Add(blockExpressions[i]);
			}
			return expressions;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.PropagateExpressions();
			return body;
		}

		private void PropagateExpressions()
		{
			foreach (IList<Expression> value in this.context.MethodContext.Expressions.BlockExpressions.Values)
			{
				foreach (BinaryExpression assignmentExpression in this.GetAssignmentExpressions(value))
				{
					List<Expression> dominatedExpressions = this.GetDominatedExpressions(assignmentExpression, value);
					for (int i = 0; i < dominatedExpressions.Count; i++)
					{
						this.TryPropagate(dominatedExpressions[i], assignmentExpression.Left, assignmentExpression.Right);
					}
				}
				for (int j = 0; j < value.Count; j++)
				{
					value[j] = this.TrySimplifyExpression(value[j]);
				}
			}
		}

		private void TryPropagate(Expression dominatedExpression, Expression original, Expression replacement)
		{
			(new ExpressionPropagationStep.ExpressionTreeTransformer(original, replacement)).Visit(dominatedExpression);
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
				this.replacement = replacement;
				this.original = original;
			}

			public override ICodeNode Visit(ICodeNode node)
			{
				Expression expression = node as Expression;
				if (expression != null && expression.Equals(this.original))
				{
					return this.replacement.CloneExpressionOnly();
				}
				return base.Visit(node);
			}
		}

		private class ExpressionTreeVisitor : BaseCodeVisitor
		{
			private bool canBePropagated;

			private ExpressionPropagationStep.ExpressionTreeVisitor.SearchState state;

			public ExpressionTreeVisitor()
			{
			}

			public bool CanBePropagated(Expression expression)
			{
				this.canBePropagated = true;
				this.state = ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation;
				base.Visit(expression);
				return this.canBePropagated;
			}

			public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitAnonymousObjectCreationExpression(node);
			}

			public override void VisitArrayCreationExpression(ArrayCreationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitArrayCreationExpression(node);
			}

			public override void VisitBaseCtorExpression(BaseCtorExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitBaseCtorExpression(node);
			}

			public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitDelegateCreationExpression(node);
			}

			public override void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitDelegateInvokeExpression(node);
			}

			public override void VisitLambdaExpression(LambdaExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitLambdaExpression(node);
			}

			public override void VisitMakeRefExpression(MakeRefExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitMakeRefExpression(node);
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitMethodInvocationExpression(node);
			}

			public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitMethodReferenceExpression(node);
			}

			public override void VisitObjectCreationExpression(ObjectCreationExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitObjectCreationExpression(node);
			}

			public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitPropertyReferenceExpression(node);
			}

			public override void VisitStackAllocExpression(StackAllocExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitStackAllocExpression(node);
			}

			public override void VisitThisCtorExpression(ThisCtorExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitThisCtorExpression(node);
			}

			public override void VisitYieldBreakExpression(YieldBreakExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitYieldBreakExpression(node);
			}

			public override void VisitYieldReturnExpression(YieldReturnExpression node)
			{
				if (this.state == ExpressionPropagationStep.ExpressionTreeVisitor.SearchState.Propagation)
				{
					this.canBePropagated = false;
					return;
				}
				base.VisitYieldReturnExpression(node);
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
			}

			public override ICodeNode VisitUnaryExpression(UnaryExpression node)
			{
				if (node.Operator == UnaryOperator.AddressDereference)
				{
					if (node.Operand is UnaryExpression)
					{
						UnaryExpression operand = node.Operand as UnaryExpression;
						if (operand.Operator == UnaryOperator.AddressOf || operand.Operator == UnaryOperator.AddressReference)
						{
							return this.Visit(operand.Operand);
						}
					}
					if (node.Operand is ExplicitCastExpression && node.Operand.ExpressionType.get_IsByReference())
					{
						ExplicitCastExpression explicitCastExpression = node.Operand as ExplicitCastExpression;
						TypeReference elementType = (explicitCastExpression.ExpressionType as ByReferenceType).get_ElementType();
						ExplicitCastExpression explicitCastExpression1 = new ExplicitCastExpression(explicitCastExpression.Expression, elementType, explicitCastExpression.MappedInstructions);
						return this.Visit(explicitCastExpression1);
					}
				}
				return base.VisitUnaryExpression(node);
			}
		}
	}
}