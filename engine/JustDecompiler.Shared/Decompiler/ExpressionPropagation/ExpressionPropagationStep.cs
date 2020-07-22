using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.ExpressionPropagation
{
    class ExpressionPropagationStep : IDecompilationStep
    {
        /// NOTE:
        /// Need to add dead code removal logic, as at the moment in some cases, assignments to unused variables appear.
        /// This does not change the logic of the program, but adds unneeded statements.
        /// This algorithm can be expanded to work for all variables, not just pointer phi variables.
        /// It should also be expanded to analyze over more than one block of expressions.
        /// This will probably require the movement of this step after the StatementDecompiler.
        private DecompilationContext context;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.context = context;
            PropagateExpressions();
            return body;
        }

        private void PropagateExpressions()
        {
            foreach (IList<Expression> blockExpressions in context.MethodContext.Expressions.BlockExpressions.Values)
            {
                IEnumerable<BinaryExpression> assignments = GetAssignmentExpressions(blockExpressions);
                foreach (BinaryExpression assignment in assignments)
                {
                    List<Expression> reachedExpressions = GetDominatedExpressions(assignment, blockExpressions);
                    for (int i = 0; i < reachedExpressions.Count; i++)
                    {
                        TryPropagate(reachedExpressions[i], assignment.Left, assignment.Right);
                    }
                }
                for (int i = 0; i < blockExpressions.Count; i++)
                {
                    blockExpressions[i] = TrySimplifyExpression(blockExpressions[i]);
                }
            }
        }
  
        private Expression TrySimplifyExpression(Expression expression)
        {
            Simplifier sim = new Simplifier();
            return sim.Visit(expression) as Expression;
        }

        private void TryPropagate(Expression dominatedExpression, Expression original, Expression replacement)
        {
            ExpressionTreeTransformer ett = new ExpressionTreeTransformer(original, replacement);
            ett.Visit(dominatedExpression);
        }

        private List<Expression> GetDominatedExpressions(BinaryExpression assignment, IList<Expression> blockExpressions)
        {
            int assignmentIndex = blockExpressions.IndexOf(assignment);
            Expression assignedValue = assignment.Right;
            List<Expression> result = new List<Expression>();
            //ExpressionTreeVisitor etv = new ExpressionTreeVisitor();
            //IEnumerable<VariableReference> variablesUsed = etv.GetUsedVariables(assignedValue);
            //ICollection<ParameterReference> argumentsUsed = etv.GetUsedArguments(assignedValue);

            /// If this is some form of autoassignment
            if (ChangesAssignedExpression(assignment.Right, assignment.Left, assignment.Left))
            {
                return result;
            }

            for (int i = assignmentIndex + 1; i < blockExpressions.Count; i++)
            {
                //TODO: Add breaks and checks
                Expression currentExpression = blockExpressions[i];
                if (ChangesAssignedExpression(currentExpression, assignedValue, assignment.Left))
                {
                    break;
                }
                result.Add(blockExpressions[i]);
            }
            return result;
        }

        private bool ChangesAssignedExpression(Expression currentExpression, Expression assignedValue, Expression assignmentRecipient)
        {
            if (currentExpression is MethodInvocationExpression)
            {
                /// This can be more refined
                /// Check can be made if the method has access to the fields, used in the assigned value.
                /// Also, if there are no type fields used to create "assignedValue", check for local variables/arguments being passed
                /// ByRef to the called method.
				MethodInvocationExpression currentExpressionAsInvocation = currentExpression as MethodInvocationExpression;
				/// Allow the propagation of the target for the method.
				/// eg: 
				/// MyType& a = &b;
				/// (*a).SomeMethod();
				/// should be propagated to
				/// b.SomeMethod();
				if (assignedValue.ExpressionType.IsByReference)
				{
					if (currentExpressionAsInvocation.MethodExpression.Target != null)
					{
						Expression target = currentExpressionAsInvocation.MethodExpression.Target;
						if (target.Equals(assignmentRecipient))
						{
							return false;
						}
						if (target is UnaryExpression && (target as UnaryExpression).Operator == UnaryOperator.AddressDereference)
						{
							Expression operand = (target as UnaryExpression).Operand;
							if (operand.Equals(assignmentRecipient))
							{
								return false;
							}
						}
					}
				}
				return true;
            }
            if (currentExpression is BinaryExpression == false)
            {
                return false;
            }
            BinaryExpression binEx = currentExpression as BinaryExpression;
            if (binEx.Left.Equals(assignedValue))
            {
                return true;
            }

            //TODO: Add checks if key parts of <assignedValue> are changed, like indexers and/or targets.
            if (assignedValue is ArrayIndexerExpression)
            {
                ArrayIndexerExpression indexer = assignedValue as ArrayIndexerExpression;
                if (binEx.Left.Equals(indexer.Target))
                {
                    /// The array is being changed
                    return true;
                }
                foreach (Expression index in indexer.Indices)
                {
                    /// One of the indexes is being changed.
                    if (binEx.Left.Equals(index))
                    {
                        return true;
                    }
                }
            }
            /// Add checks for unary expression targets, cast targets and so on.
            if (assignedValue is UnaryExpression)
            {
				return ChangesAssignedExpression(currentExpression, (assignedValue as UnaryExpression).Operand, assignmentRecipient);
            }

            if (assignedValue is ExplicitCastExpression)
            {
				return ChangesAssignedExpression(currentExpression, (assignedValue as ExplicitCastExpression).Expression, assignmentRecipient);            
            }

            if (assignedValue is CanCastExpression)
            {
				return ChangesAssignedExpression(currentExpression, (assignedValue as CanCastExpression).Expression, assignmentRecipient);
            }
            return false;
        }

        /// <summary>
        /// Gets a collection of all assignment expressions.
        /// </summary>
        /// <param name="blockExpressions">The list of expressions in a single block.</param>
        /// <returns>Returns enumeration containing the found assignments.</returns>
        private IEnumerable<BinaryExpression> GetAssignmentExpressions(IList<Expression> blockExpressions)
        {
            List<BinaryExpression> assignments = new List<BinaryExpression>();
            foreach (Expression expression in blockExpressions)
            {
                if (expression is BinaryExpression && (expression as BinaryExpression).IsAssignmentExpression)
                {
                    BinaryExpression binary = expression as BinaryExpression;
                    if (CanBePropagated(binary.Right))
                    {
                        /// Only dup pointer variables should be propagated
                        /// Propagating all variables breaks patterns in later steps, for instance in RebuildLock statements
                        /// or making chained constructor calls.
                        if (binary.Left is VariableReferenceExpression)
                        {
							VariableReferenceExpression leftSide = binary.Left as VariableReferenceExpression;
							if (/*leftSide.Variable.Name.IndexOf("PhiVariable") == 0 &&*/
								leftSide.ExpressionType.IsByReference || leftSide.ExpressionType.IsPointer )
							{
								assignments.Add(binary);
							}
                        }
                    }
                }
            }
            return assignments;
        }

        /// <summary>
        /// Checks if the expression can be coppied or moved, without changing the semantics of the program. Additional checks must be made to find the scope of the supposed move.
        /// </summary>
        /// <param name="expression">The expression in question.</param>
        /// <returns>Returns true, if the expression can be safely moved/coppied.</returns>
        private bool CanBePropagated(Expression expression)
        {
            ExpressionTreeVisitor etv = new ExpressionTreeVisitor();
            return etv.CanBePropagated(expression);
        }

        class ExpressionTreeVisitor : BaseCodeVisitor
        {
            enum SearchState
            {
                Propagation,
            }

            private bool canBePropagated;
            private SearchState state;

            public bool CanBePropagated(Expression expression)
            {
                canBePropagated = true;
                state = SearchState.Propagation;
                base.Visit(expression);
                return canBePropagated;
            }

            public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitMethodInvocationExpression(node);
            }

            public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitDelegateCreationExpression(node);
            }

            public override void VisitMethodReferenceExpression(MethodReferenceExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitMethodReferenceExpression(node);
            }

            public override void VisitArrayCreationExpression(ArrayCreationExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitArrayCreationExpression(node);
            }

            public override void VisitObjectCreationExpression(ObjectCreationExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitObjectCreationExpression(node);
            }

			public override void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
			{
				if (state == SearchState.Propagation)
				{
					canBePropagated = false;
					return;
				}
				base.VisitAnonymousObjectCreationExpression(node);
			}

            public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitPropertyReferenceExpression(node);
            }

            public override void VisitStackAllocExpression(StackAllocExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitStackAllocExpression(node);
            }

            public override void VisitMakeRefExpression(MakeRefExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitMakeRefExpression(node);
            }

            public override void VisitLambdaExpression(LambdaExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitLambdaExpression(node);
            }

            public override void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitDelegateInvokeExpression(node);
            }

            public override void VisitBaseCtorExpression(BaseCtorExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitBaseCtorExpression(node);
            }

            public override void VisitThisCtorExpression(ThisCtorExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitThisCtorExpression(node);
            }

            public override void VisitYieldReturnExpression(YieldReturnExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitYieldReturnExpression(node);
            }

            public override void VisitYieldBreakExpression(YieldBreakExpression node)
            {
                if (state == SearchState.Propagation)
                {
                    canBePropagated = false;
                    return;
                }
                base.VisitYieldBreakExpression(node);
            }
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
                Expression exp = node as Expression;
                if (exp != null && exp.Equals(original))
                {
                    return replacement.CloneExpressionOnly();
                }
                return base.Visit(node);
            }

        }

        public class Simplifier : BaseCodeTransformer
        {
            public override ICodeNode VisitUnaryExpression(UnaryExpression node)
            {
                if (node.Operator == UnaryOperator.AddressDereference)
                {
					if (node.Operand is UnaryExpression)
					{
						UnaryExpression unaryOperand = node.Operand as UnaryExpression;
						if (unaryOperand.Operator == UnaryOperator.AddressOf || unaryOperand.Operator == UnaryOperator.AddressReference)
						{
							return Visit(unaryOperand.Operand);
						}
					}
					if (node.Operand is ExplicitCastExpression && node.Operand.ExpressionType.IsByReference)
					{
						ExplicitCastExpression theCast = node.Operand as ExplicitCastExpression;
						TypeReference targetType = (theCast.ExpressionType as ByReferenceType).ElementType;
						ExplicitCastExpression result = new ExplicitCastExpression(theCast.Expression, targetType, theCast.MappedInstructions);
						return Visit(result);
					}
                }
                return base.VisitUnaryExpression(node);
            }
        }
    }
  
  
}
