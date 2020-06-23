using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast
{
	public class BaseCodeVisitor : ICodeVisitor
	{
		private long visitsOnStack;

		public BaseCodeVisitor()
		{
		}

		public virtual void Visit(ICodeNode node)
		{
			this.visitsOnStack += (long)1;
			if (this.visitsOnStack == (long)0x258)
			{
				this.visitsOnStack = (long)0;
				throw new Exception("Stack overflow while traversing code tree in visit.");
			}
			if (node == null)
			{
				this.visitsOnStack -= (long)1;
				return;
			}
			switch (node.CodeNodeType)
			{
				case CodeNodeType.BlockStatement:
				{
					this.VisitBlockStatement((BlockStatement)node);
					break;
				}
				case CodeNodeType.UnsafeBlock:
				{
					this.VisitUnsafeBlockStatement((UnsafeBlockStatement)node);
					break;
				}
				case CodeNodeType.GotoStatement:
				{
					this.VisitGotoStatement((GotoStatement)node);
					break;
				}
				case CodeNodeType.IfStatement:
				{
					this.VisitIfStatement((IfStatement)node);
					break;
				}
				case CodeNodeType.IfElseIfStatement:
				{
					this.VisitIfElseIfStatement((IfElseIfStatement)node);
					break;
				}
				case CodeNodeType.ExpressionStatement:
				{
					this.VisitExpressionStatement((ExpressionStatement)node);
					break;
				}
				case CodeNodeType.ThrowExpression:
				{
					this.VisitThrowExpression((ThrowExpression)node);
					break;
				}
				case CodeNodeType.WhileStatement:
				{
					this.VisitWhileStatement((WhileStatement)node);
					break;
				}
				case CodeNodeType.DoWhileStatement:
				{
					this.VisitDoWhileStatement((DoWhileStatement)node);
					break;
				}
				case CodeNodeType.BreakStatement:
				{
					this.VisitBreakStatement((BreakStatement)node);
					break;
				}
				case CodeNodeType.ContinueStatement:
				{
					this.VisitContinueStatement((ContinueStatement)node);
					break;
				}
				case CodeNodeType.ForStatement:
				{
					this.VisitForStatement((ForStatement)node);
					break;
				}
				case CodeNodeType.ForEachStatement:
				{
					this.VisitForEachStatement((ForEachStatement)node);
					break;
				}
				case CodeNodeType.ConditionCase:
				{
					this.VisitConditionCase((ConditionCase)node);
					break;
				}
				case CodeNodeType.DefaultCase:
				{
					this.VisitDefaultCase((DefaultCase)node);
					break;
				}
				case CodeNodeType.SwitchStatement:
				{
					this.VisitSwitchStatement((SwitchStatement)node);
					break;
				}
				case CodeNodeType.CatchClause:
				{
					this.VisitCatchClause((CatchClause)node);
					break;
				}
				case CodeNodeType.TryStatement:
				{
					this.VisitTryStatement((TryStatement)node);
					break;
				}
				case CodeNodeType.BlockExpression:
				{
					this.VisitBlockExpression((BlockExpression)node);
					break;
				}
				case CodeNodeType.MethodInvocationExpression:
				{
					this.VisitMethodInvocationExpression((MethodInvocationExpression)node);
					break;
				}
				case CodeNodeType.MethodReferenceExpression:
				{
					this.VisitMethodReferenceExpression((MethodReferenceExpression)node);
					break;
				}
				case CodeNodeType.DelegateCreationExpression:
				{
					this.VisitDelegateCreationExpression((DelegateCreationExpression)node);
					break;
				}
				case CodeNodeType.LiteralExpression:
				{
					this.VisitLiteralExpression((LiteralExpression)node);
					break;
				}
				case CodeNodeType.UnaryExpression:
				{
					this.VisitUnaryExpression((UnaryExpression)node);
					break;
				}
				case CodeNodeType.BinaryExpression:
				{
					this.VisitBinaryExpression((BinaryExpression)node);
					break;
				}
				case CodeNodeType.ArgumentReferenceExpression:
				{
					this.VisitArgumentReferenceExpression((ArgumentReferenceExpression)node);
					break;
				}
				case CodeNodeType.VariableReferenceExpression:
				{
					this.VisitVariableReferenceExpression((VariableReferenceExpression)node);
					break;
				}
				case CodeNodeType.VariableDeclarationExpression:
				{
					this.VisitVariableDeclarationExpression((VariableDeclarationExpression)node);
					break;
				}
				case CodeNodeType.ThisReferenceExpression:
				{
					this.VisitThisReferenceExpression((ThisReferenceExpression)node);
					break;
				}
				case CodeNodeType.BaseReferenceExpression:
				{
					this.VisitBaseReferenceExpression((BaseReferenceExpression)node);
					break;
				}
				case CodeNodeType.FieldReferenceExpression:
				{
					this.VisitFieldReferenceExpression((FieldReferenceExpression)node);
					break;
				}
				case CodeNodeType.ExplicitCastExpression:
				{
					this.VisitExplicitCastExpression((ExplicitCastExpression)node);
					break;
				}
				case CodeNodeType.ImplicitCastExpression:
				{
					this.VisitImplicitCastExpression((ImplicitCastExpression)node);
					break;
				}
				case CodeNodeType.SafeCastExpression:
				{
					this.VisitSafeCastExpression((SafeCastExpression)node);
					break;
				}
				case CodeNodeType.CanCastExpression:
				{
					this.VisitCanCastExpression((CanCastExpression)node);
					break;
				}
				case CodeNodeType.TypeOfExpression:
				{
					this.VisitTypeOfExpression((TypeOfExpression)node);
					break;
				}
				case CodeNodeType.ConditionExpression:
				{
					this.VisitConditionExpression((ConditionExpression)node);
					break;
				}
				case CodeNodeType.FixedStatement:
				{
					this.VisitFixedStatement((FixedStatement)node);
					break;
				}
				case CodeNodeType.ArrayCreationExpression:
				{
					this.VisitArrayCreationExpression((ArrayCreationExpression)node);
					break;
				}
				case CodeNodeType.ArrayIndexerExpression:
				{
					this.VisitArrayIndexerExpression((ArrayIndexerExpression)node);
					break;
				}
				case CodeNodeType.ObjectCreationExpression:
				{
					this.VisitObjectCreationExpression((ObjectCreationExpression)node);
					break;
				}
				case CodeNodeType.DefaultObjectExpression:
				{
					this.VisitDefaultObjectExpression((DefaultObjectExpression)node);
					break;
				}
				case CodeNodeType.PropertyReferenceExpression:
				{
					this.VisitPropertyReferenceExpression((PropertyReferenceExpression)node);
					break;
				}
				case CodeNodeType.TypeReferenceExpression:
				{
					this.VisitTypeReferenceExpression((TypeReferenceExpression)node);
					break;
				}
				case CodeNodeType.UsingStatement:
				{
					this.VisitUsingStatement((UsingStatement)node);
					break;
				}
				case CodeNodeType.StackAllocExpression:
				{
					this.VisitStackAllocExpression((StackAllocExpression)node);
					break;
				}
				case CodeNodeType.SizeOfExpression:
				{
					this.VisitSizeOfExpression((SizeOfExpression)node);
					break;
				}
				case CodeNodeType.MakeRefExpression:
				{
					this.VisitMakeRefExpression((MakeRefExpression)node);
					break;
				}
				case CodeNodeType.EventReferenceExpression:
				{
					this.VisitEventReferenceExpression((EventReferenceExpression)node);
					break;
				}
				case CodeNodeType.EnumExpression:
				{
					this.VisitEnumExpression((EnumExpression)node);
					break;
				}
				case CodeNodeType.LambdaExpression:
				{
					this.VisitLambdaExpression((LambdaExpression)node);
					break;
				}
				case CodeNodeType.DelegateInvokeExpression:
				{
					this.VisitDelegateInvokeExpression((DelegateInvokeExpression)node);
					break;
				}
				case CodeNodeType.BaseCtorExpression:
				{
					this.VisitBaseCtorExpression((BaseCtorExpression)node);
					break;
				}
				case CodeNodeType.ThisCtorExpression:
				{
					this.VisitThisCtorExpression((ThisCtorExpression)node);
					break;
				}
				case CodeNodeType.YieldReturnExpression:
				{
					this.VisitYieldReturnExpression((YieldReturnExpression)node);
					break;
				}
				case CodeNodeType.YieldBreakExpression:
				{
					this.VisitYieldBreakExpression((YieldBreakExpression)node);
					break;
				}
				case CodeNodeType.LockStatement:
				{
					this.VisitLockStatement((LockStatement)node);
					break;
				}
				case CodeNodeType.ReturnExpression:
				{
					this.VisitReturnExpression((ReturnExpression)node);
					break;
				}
				case CodeNodeType.EmptyStatement:
				{
					this.VisitEmptyStatement((EmptyStatement)node);
					break;
				}
				case CodeNodeType.DynamicMemberReferenceExpression:
				{
					this.VisitDynamicMemberReferenceExpression((DynamicMemberReferenceExpression)node);
					break;
				}
				case CodeNodeType.DynamicConstructorInvocationExpression:
				{
					this.VisitDynamicConstructorInvocationExpression((DynamicConstructorInvocationExpression)node);
					break;
				}
				case CodeNodeType.DynamicIndexerExpression:
				{
					this.VisitDynamicIndexerExpression((DynamicIndexerExpression)node);
					break;
				}
				case CodeNodeType.BoxExpression:
				{
					this.VisitBoxExpression((BoxExpression)node);
					break;
				}
				case CodeNodeType.AnonymousPropertyInitializerExpression:
				{
					this.VisitAnonymousPropertyInitializerExpression((AnonymousPropertyInitializerExpression)node);
					break;
				}
				case CodeNodeType.LambdaParameterExpression:
				{
					this.VisitLambdaParameterExpression((LambdaParameterExpression)node);
					break;
				}
				case CodeNodeType.AwaitExpression:
				{
					this.VisitAwaitExpression((AwaitExpression)node);
					break;
				}
				case CodeNodeType.ArrayLengthExpression:
				{
					this.VisitArrayLengthExpression((ArrayLengthExpression)node);
					break;
				}
				case CodeNodeType.ExceptionStatement:
				{
					this.VisitExceptionStatement((ExceptionStatement)node);
					break;
				}
				case CodeNodeType.BreakSwitchCaseStatement:
				{
					this.VisitBreakSwitchCaseStatement((BreakSwitchCaseStatement)node);
					break;
				}
				case CodeNodeType.CaseGotoStatement:
				{
					this.VisitCaseGotoStatement((CaseGotoStatement)node);
					break;
				}
				case CodeNodeType.FinallyClause:
				{
					this.VisitFinallyClause((FinallyClause)node);
					break;
				}
				case CodeNodeType.ShortFormReturnExpression:
				{
					this.VisitShortFormReturnExpression((ShortFormReturnExpression)node);
					break;
				}
				case CodeNodeType.AnonymousObjectCreationExpression:
				{
					this.VisitAnonymousObjectCreationExpression((AnonymousObjectCreationExpression)node);
					break;
				}
				case CodeNodeType.FromClause:
				{
					this.VisitFromClause((FromClause)node);
					break;
				}
				case CodeNodeType.SelectClause:
				{
					this.VisitSelectClause((SelectClause)node);
					break;
				}
				case CodeNodeType.WhereClause:
				{
					this.VisitWhereClause((WhereClause)node);
					break;
				}
				case CodeNodeType.GroupClause:
				{
					this.VisitGroupClause((GroupClause)node);
					break;
				}
				case CodeNodeType.OrderByClause:
				{
					this.VisitOrderByClause((OrderByClause)node);
					break;
				}
				case CodeNodeType.JoinClause:
				{
					this.VisitJoinClause((JoinClause)node);
					break;
				}
				case CodeNodeType.LetClause:
				{
					this.VisitLetClause((LetClause)node);
					break;
				}
				case CodeNodeType.IntoClause:
				{
					this.VisitIntoClause((IntoClause)node);
					break;
				}
				case CodeNodeType.LinqQueryExpression:
				{
					this.VisitLinqQueryExpression((LinqQueryExpression)node);
					break;
				}
				case CodeNodeType.ArrayVariableCreationExpression:
				{
					this.VisitArrayVariableDeclarationExpression((ArrayVariableDeclarationExpression)node);
					break;
				}
				case CodeNodeType.ArrayAssignmentVariableReferenceExpression:
				{
					this.VisitArrayAssignmentVariableReferenceExpression((ArrayVariableReferenceExpression)node);
					break;
				}
				case CodeNodeType.ArrayAssignmentFieldReferenceExpression:
				{
					this.VisitArrayAssignmentFieldReferenceExpression((ArrayAssignmentFieldReferenceExpression)node);
					break;
				}
				case CodeNodeType.PropertyInitializerExpression:
				{
					this.VisitPropertyInitializerExpression((PropertyInitializerExpression)node);
					break;
				}
				case CodeNodeType.FieldInitializerExpression:
				{
					this.VisitFieldInitializerExpression((FieldInitializerExpression)node);
					break;
				}
				case CodeNodeType.ParenthesesExpression:
				{
					this.VisitParenthesesExpression((ParenthesesExpression)node);
					break;
				}
				case CodeNodeType.InitializerExpression:
				{
					this.VisitInitializerExpression((InitializerExpression)node);
					break;
				}
				case CodeNodeType.CheckedExpression:
				{
					this.VisitCheckedExpression((CheckedExpression)node);
					break;
				}
				case CodeNodeType.MemberHandleExpression:
				{
					this.VisitMemberHandleExpression((MemberHandleExpression)node);
					break;
				}
				case CodeNodeType.AutoPropertyConstructorInitializerExpression:
				{
					this.VisitAutoPropertyConstructorInitializerExpression((AutoPropertyConstructorInitializerExpression)node);
					break;
				}
				case CodeNodeType.RaiseEventExpression:
				{
					this.VisitRaiseEventExpression((RaiseEventExpression)node);
					break;
				}
				case CodeNodeType.RefVariableDeclarationExpression:
				{
					this.VisitRefVariableDeclarationExpression((RefVariableDeclarationExpression)node);
					break;
				}
				case CodeNodeType.RefReturnExpression:
				{
					this.VisitRefReturnExpression((RefReturnExpression)node);
					break;
				}
				default:
				{
					throw new ArgumentException();
				}
			}
			this.visitsOnStack -= (long)1;
		}

		public virtual void Visit(IEnumerable collection)
		{
			foreach (ICodeNode codeNode in collection)
			{
				this.Visit(codeNode);
			}
		}

		public virtual void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			this.Visit(node.Initializer);
		}

		public virtual void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
		{
		}

		public virtual void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
		}

		public virtual void VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			this.Visit(node.Field);
			this.Visit(node.Dimensions);
		}

		public virtual void VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			this.Visit(node.Variable);
			this.Visit(node.Dimensions);
		}

		public virtual void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			this.Visit(node.Dimensions);
			this.Visit(node.Initializer);
		}

		public virtual void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			this.VisitIIndexerExpression(node);
		}

		public virtual void VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			this.Visit(node.Target);
		}

		public virtual void VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			this.Visit(node.Variable);
			this.Visit(node.Dimensions);
		}

		public virtual void VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node)
		{
		}

		public virtual void VisitAwaitExpression(AwaitExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.Visit(node.Arguments);
		}

		public virtual void VisitBaseReferenceExpression(BaseReferenceExpression node)
		{
		}

		public virtual void VisitBinaryExpression(BinaryExpression node)
		{
			this.Visit(node.Left);
			this.Visit(node.Right);
		}

		public virtual void VisitBlockExpression(BlockExpression node)
		{
			this.Visit(node.Expressions);
		}

		public virtual void VisitBlockStatement(BlockStatement node)
		{
			this.Visit(node.Statements);
		}

		public virtual void VisitBoxExpression(BoxExpression node)
		{
			this.Visit(node.BoxedExpression);
		}

		public virtual void VisitBreakStatement(BreakStatement node)
		{
		}

		public virtual void VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node)
		{
		}

		public virtual void VisitCanCastExpression(CanCastExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitCaseGotoStatement(CaseGotoStatement node)
		{
		}

		public virtual void VisitCatchClause(CatchClause node)
		{
			this.Visit(node.Body);
			this.Visit(node.Variable);
			this.Visit(node.Filter);
		}

		public virtual void VisitCheckedExpression(CheckedExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitConditionCase(ConditionCase node)
		{
			this.Visit(node.Condition);
			this.Visit(node.Body);
		}

		public virtual void VisitConditionExpression(ConditionExpression node)
		{
			this.Visit(node.Condition);
			this.Visit(node.Then);
			this.Visit(node.Else);
		}

		public virtual void VisitContinueStatement(ContinueStatement node)
		{
		}

		public virtual void VisitDefaultCase(DefaultCase node)
		{
			this.Visit(node.Body);
		}

		public virtual void VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
		}

		public virtual void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			this.Visit(node.Target);
			this.Visit(node.MethodExpression);
		}

		public virtual void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			this.Visit(node.Target);
			this.Visit(node.Arguments);
		}

		public virtual void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.Visit(node.Body);
			this.Visit(node.Condition);
		}

		public virtual void VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
		{
			this.Visit(node.Arguments);
		}

		public virtual void VisitDynamicIndexerExpression(DynamicIndexerExpression node)
		{
			this.VisitIIndexerExpression(node);
		}

		public virtual void VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
		{
			this.Visit(node.Target);
			if (node.IsMethodInvocation)
			{
				this.Visit(node.InvocationArguments);
			}
		}

		public virtual void VisitEmptyStatement(EmptyStatement node)
		{
		}

		public virtual void VisitEnumExpression(EnumExpression node)
		{
		}

		public virtual void VisitEventReferenceExpression(EventReferenceExpression node)
		{
			this.Visit(node.Target);
		}

		public virtual void VisitExceptionStatement(ExceptionStatement node)
		{
		}

		public virtual void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitExpressionStatement(ExpressionStatement node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
		}

		public virtual void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			this.Visit(node.Target);
		}

		public void VisitFinallyClause(FinallyClause node)
		{
			this.Visit(node.Body);
		}

		public virtual void VisitFixedStatement(FixedStatement node)
		{
			this.Visit(node.Expression);
			this.Visit(node.Body);
		}

		public virtual void VisitForEachStatement(ForEachStatement node)
		{
			this.Visit(node.Variable);
			this.Visit(node.Collection);
			this.Visit(node.Body);
		}

		public virtual void VisitForStatement(ForStatement node)
		{
			this.Visit(node.Initializer);
			this.Visit(node.Condition);
			this.Visit(node.Increment);
			this.Visit(node.Body);
		}

		public virtual void VisitFromClause(FromClause node)
		{
			this.Visit(node.Identifier);
			this.Visit(node.Collection);
		}

		public virtual void VisitGotoStatement(GotoStatement node)
		{
		}

		public virtual void VisitGroupClause(GroupClause node)
		{
			this.Visit(node.Expression);
			this.Visit(node.GroupKey);
		}

		public virtual void VisitIfElseIfStatement(IfElseIfStatement node)
		{
			for (int i = 0; i < node.ConditionBlocks.Count; i++)
			{
				KeyValuePair<Expression, BlockStatement> item = node.ConditionBlocks[i];
				this.Visit(item.Key);
				this.Visit(item.Value);
			}
			this.Visit(node.Else);
		}

		public virtual void VisitIfStatement(IfStatement node)
		{
			this.Visit(node.Condition);
			this.Visit(node.Then);
			this.Visit(node.Else);
		}

		protected virtual void VisitIIndexerExpression(IIndexerExpression node)
		{
			this.Visit(node.Target);
			this.Visit(node.Indices);
		}

		public virtual void VisitImplicitCastExpression(ImplicitCastExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitInitializerExpression(InitializerExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitIntoClause(IntoClause node)
		{
			this.Visit(node.Identifier);
		}

		public virtual void VisitJoinClause(JoinClause node)
		{
			this.Visit(node.InnerIdentifier);
			this.Visit(node.InnerCollection);
			this.Visit(node.OuterKey);
			this.Visit(node.InnerKey);
		}

		public virtual void VisitLambdaExpression(LambdaExpression node)
		{
			this.Visit(node.Body);
		}

		public virtual void VisitLambdaParameterExpression(LambdaParameterExpression node)
		{
		}

		public virtual void VisitLetClause(LetClause node)
		{
			this.Visit(node.Identifier);
			this.Visit(node.Expression);
		}

		public virtual void VisitLinqQueryExpression(LinqQueryExpression node)
		{
			foreach (QueryClause clause in node.Clauses)
			{
				this.Visit(clause);
			}
		}

		public virtual void VisitLiteralExpression(LiteralExpression node)
		{
		}

		public virtual void VisitLockStatement(LockStatement node)
		{
			this.Visit(node.Expression);
			this.Visit(node.Body);
		}

		public virtual void VisitMakeRefExpression(MakeRefExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitMemberHandleExpression(MemberHandleExpression node)
		{
		}

		public virtual void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.Visit(node.MethodExpression);
			this.Visit(node.Arguments);
		}

		public virtual void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			this.Visit(node.Target);
		}

		public virtual void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			this.Visit(node.Arguments);
			this.Visit(node.Initializer);
		}

		public virtual void VisitOrderByClause(OrderByClause node)
		{
			foreach (KeyValuePair<Expression, OrderDirection> expressionToOrderDirectionMap in node.ExpressionToOrderDirectionMap)
			{
				this.Visit(expressionToOrderDirectionMap.Key);
			}
		}

		public virtual void VisitParenthesesExpression(ParenthesesExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitPropertyInitializerExpression(PropertyInitializerExpression node)
		{
		}

		public virtual void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			this.Visit(node.Target);
			this.Visit(node.Arguments);
		}

		public virtual void VisitRaiseEventExpression(RaiseEventExpression node)
		{
			this.Visit(node.Arguments);
		}

		public virtual void VisitRefReturnExpression(RefReturnExpression node)
		{
			this.Visit(node.Value);
		}

		public virtual void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
		{
		}

		public virtual void VisitReturnExpression(ReturnExpression node)
		{
			this.Visit(node.Value);
		}

		public virtual void VisitSafeCastExpression(SafeCastExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitSelectClause(SelectClause node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitShortFormReturnExpression(ShortFormReturnExpression node)
		{
			this.Visit(node.Value);
		}

		public virtual void VisitSizeOfExpression(SizeOfExpression node)
		{
		}

		public virtual void VisitStackAllocExpression(StackAllocExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitSwitchStatement(SwitchStatement node)
		{
			this.Visit(node.Condition);
			this.Visit(node.Cases);
		}

		public virtual void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.Visit(node.Arguments);
		}

		public virtual void VisitThisReferenceExpression(ThisReferenceExpression node)
		{
		}

		public virtual void VisitThrowExpression(ThrowExpression node)
		{
			this.Visit(node.Expression);
		}

		public virtual void VisitTryStatement(TryStatement node)
		{
			this.Visit(node.Try);
			this.Visit(node.CatchClauses);
			this.Visit(node.Fault);
			this.Visit(node.Finally);
		}

		public virtual void VisitTypeOfExpression(TypeOfExpression node)
		{
		}

		public virtual void VisitTypeReferenceExpression(TypeReferenceExpression node)
		{
		}

		public virtual void VisitUnaryExpression(UnaryExpression node)
		{
			this.Visit(node.Operand);
		}

		public virtual void VisitUnsafeBlockStatement(UnsafeBlockStatement unsafeBlock)
		{
			this.Visit(unsafeBlock.Statements);
		}

		public virtual void VisitUsingStatement(UsingStatement node)
		{
			this.Visit(node.Expression);
			this.Visit(node.Body);
		}

		public virtual void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
		}

		public virtual void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
		}

		public virtual void VisitWhereClause(WhereClause node)
		{
			this.Visit(node.Condition);
		}

		public virtual void VisitWhileStatement(WhileStatement node)
		{
			this.Visit(node.Condition);
			this.Visit(node.Body);
		}

		public virtual void VisitYieldBreakExpression(YieldBreakExpression node)
		{
		}

		public virtual void VisitYieldReturnExpression(YieldReturnExpression node)
		{
			this.Visit(node.Expression);
		}
	}
}