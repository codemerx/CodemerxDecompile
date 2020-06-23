using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast
{
	public class BaseCodeTransformer : ICodeTransformer
	{
		private long visitsOnStack;

		public BaseCodeTransformer()
		{
		}

		private ICodeNode DoVisit(ICodeNode node)
		{
			if (node == null)
			{
				return null;
			}
			switch (node.CodeNodeType)
			{
				case CodeNodeType.BlockStatement:
				{
					return this.VisitBlockStatement((BlockStatement)node);
				}
				case CodeNodeType.UnsafeBlock:
				{
					throw new ArgumentException();
				}
				case CodeNodeType.GotoStatement:
				{
					return this.VisitGotoStatement((GotoStatement)node);
				}
				case CodeNodeType.IfStatement:
				{
					return this.VisitIfStatement((IfStatement)node);
				}
				case CodeNodeType.IfElseIfStatement:
				{
					return this.VisitIfElseIfStatement((IfElseIfStatement)node);
				}
				case CodeNodeType.ExpressionStatement:
				{
					return this.VisitExpressionStatement((ExpressionStatement)node);
				}
				case CodeNodeType.ThrowExpression:
				{
					return this.VisitThrowExpression((ThrowExpression)node);
				}
				case CodeNodeType.WhileStatement:
				{
					return this.VisitWhileStatement((WhileStatement)node);
				}
				case CodeNodeType.DoWhileStatement:
				{
					return this.VisitDoWhileStatement((DoWhileStatement)node);
				}
				case CodeNodeType.BreakStatement:
				{
					return this.VisitBreakStatement((BreakStatement)node);
				}
				case CodeNodeType.ContinueStatement:
				{
					return this.VisitContinueStatement((ContinueStatement)node);
				}
				case CodeNodeType.ForStatement:
				{
					return this.VisitForStatement((ForStatement)node);
				}
				case CodeNodeType.ForEachStatement:
				{
					return this.VisitForEachStatement((ForEachStatement)node);
				}
				case CodeNodeType.ConditionCase:
				{
					return this.VisitConditionCase((ConditionCase)node);
				}
				case CodeNodeType.DefaultCase:
				{
					return this.VisitDefaultCase((DefaultCase)node);
				}
				case CodeNodeType.SwitchStatement:
				{
					return this.VisitSwitchStatement((SwitchStatement)node);
				}
				case CodeNodeType.CatchClause:
				{
					return this.VisitCatchClause((CatchClause)node);
				}
				case CodeNodeType.TryStatement:
				{
					return this.VisitTryStatement((TryStatement)node);
				}
				case CodeNodeType.BlockExpression:
				{
					return this.VisitBlockExpression((BlockExpression)node);
				}
				case CodeNodeType.MethodInvocationExpression:
				{
					return this.VisitMethodInvocationExpression((MethodInvocationExpression)node);
				}
				case CodeNodeType.MethodReferenceExpression:
				{
					return this.VisitMethodReferenceExpression((MethodReferenceExpression)node);
				}
				case CodeNodeType.DelegateCreationExpression:
				{
					return this.VisitDelegateCreationExpression((DelegateCreationExpression)node);
				}
				case CodeNodeType.LiteralExpression:
				{
					return this.VisitLiteralExpression((LiteralExpression)node);
				}
				case CodeNodeType.UnaryExpression:
				{
					return this.VisitUnaryExpression((UnaryExpression)node);
				}
				case CodeNodeType.BinaryExpression:
				{
					return this.VisitBinaryExpression((BinaryExpression)node);
				}
				case CodeNodeType.ArgumentReferenceExpression:
				{
					return this.VisitArgumentReferenceExpression((ArgumentReferenceExpression)node);
				}
				case CodeNodeType.VariableReferenceExpression:
				{
					return this.VisitVariableReferenceExpression((VariableReferenceExpression)node);
				}
				case CodeNodeType.VariableDeclarationExpression:
				{
					return this.VisitVariableDeclarationExpression((VariableDeclarationExpression)node);
				}
				case CodeNodeType.ThisReferenceExpression:
				{
					return this.VisitThisReferenceExpression((ThisReferenceExpression)node);
				}
				case CodeNodeType.BaseReferenceExpression:
				{
					return this.VisitBaseReferenceExpression((BaseReferenceExpression)node);
				}
				case CodeNodeType.FieldReferenceExpression:
				{
					return this.VisitFieldReferenceExpression((FieldReferenceExpression)node);
				}
				case CodeNodeType.ExplicitCastExpression:
				{
					return this.VisitExplicitCastExpression((ExplicitCastExpression)node);
				}
				case CodeNodeType.ImplicitCastExpression:
				{
					return this.VisitImplicitCastExpression((ImplicitCastExpression)node);
				}
				case CodeNodeType.SafeCastExpression:
				{
					return this.VisitSafeCastExpression((SafeCastExpression)node);
				}
				case CodeNodeType.CanCastExpression:
				{
					return this.VisitCanCastExpression((CanCastExpression)node);
				}
				case CodeNodeType.TypeOfExpression:
				{
					return this.VisitTypeOfExpression((TypeOfExpression)node);
				}
				case CodeNodeType.ConditionExpression:
				{
					return this.VisitConditionExpression((ConditionExpression)node);
				}
				case CodeNodeType.FixedStatement:
				{
					return this.VisitFixedStatement((FixedStatement)node);
				}
				case CodeNodeType.ArrayCreationExpression:
				{
					return this.VisitArrayCreationExpression((ArrayCreationExpression)node);
				}
				case CodeNodeType.ArrayIndexerExpression:
				{
					return this.VisitArrayIndexerExpression((ArrayIndexerExpression)node);
				}
				case CodeNodeType.ObjectCreationExpression:
				{
					return this.VisitObjectCreationExpression((ObjectCreationExpression)node);
				}
				case CodeNodeType.DefaultObjectExpression:
				{
					return this.VisitDefaultObjectExpression((DefaultObjectExpression)node);
				}
				case CodeNodeType.PropertyReferenceExpression:
				{
					return this.VisitPropertyReferenceExpression((PropertyReferenceExpression)node);
				}
				case CodeNodeType.TypeReferenceExpression:
				{
					return this.VisitTypeReferenceExpression((TypeReferenceExpression)node);
				}
				case CodeNodeType.UsingStatement:
				{
					return this.VisitUsingStatement((UsingStatement)node);
				}
				case CodeNodeType.StackAllocExpression:
				{
					return this.VisitStackAllocExpression((StackAllocExpression)node);
				}
				case CodeNodeType.SizeOfExpression:
				{
					return this.VisitSizeOfExpression((SizeOfExpression)node);
				}
				case CodeNodeType.MakeRefExpression:
				{
					return this.VisitMakeRefExpression((MakeRefExpression)node);
				}
				case CodeNodeType.EventReferenceExpression:
				{
					return this.VisitEventReferenceExpression((EventReferenceExpression)node);
				}
				case CodeNodeType.EnumExpression:
				{
					return this.VisitEnumExpression((EnumExpression)node);
				}
				case CodeNodeType.LambdaExpression:
				{
					return this.VisitLambdaExpression((LambdaExpression)node);
				}
				case CodeNodeType.DelegateInvokeExpression:
				{
					return this.VisitDelegateInvokeExpression((DelegateInvokeExpression)node);
				}
				case CodeNodeType.BaseCtorExpression:
				{
					return this.VisitBaseCtorExpression((BaseCtorExpression)node);
				}
				case CodeNodeType.ThisCtorExpression:
				{
					return this.VisitThisCtorExpression((ThisCtorExpression)node);
				}
				case CodeNodeType.YieldReturnExpression:
				{
					return this.VisitYieldReturnExpression((YieldReturnExpression)node);
				}
				case CodeNodeType.YieldBreakExpression:
				{
					return this.VisitYieldBreakExpression((YieldBreakExpression)node);
				}
				case CodeNodeType.LockStatement:
				{
					return this.VisitLockStatement((LockStatement)node);
				}
				case CodeNodeType.ReturnExpression:
				{
					return this.VisitReturnExpression((ReturnExpression)node);
				}
				case CodeNodeType.EmptyStatement:
				{
					return this.VisitEmptyStatement((EmptyStatement)node);
				}
				case CodeNodeType.DynamicMemberReferenceExpression:
				{
					return this.VisitDynamicMemberReferenceExpression((DynamicMemberReferenceExpression)node);
				}
				case CodeNodeType.DynamicConstructorInvocationExpression:
				{
					return this.VisitDynamicConstructorInvocationExpression((DynamicConstructorInvocationExpression)node);
				}
				case CodeNodeType.DynamicIndexerExpression:
				{
					return this.VisitDynamicIndexerExpression((DynamicIndexerExpression)node);
				}
				case CodeNodeType.BoxExpression:
				{
					return this.VisitBoxExpression((BoxExpression)node);
				}
				case CodeNodeType.AnonymousPropertyInitializerExpression:
				{
					return this.VisitAnonymousPropertyInitializerExpression((AnonymousPropertyInitializerExpression)node);
				}
				case CodeNodeType.LambdaParameterExpression:
				{
					return this.VisitLambdaParameterExpression((LambdaParameterExpression)node);
				}
				case CodeNodeType.AwaitExpression:
				{
					return this.VisitAwaitExpression((AwaitExpression)node);
				}
				case CodeNodeType.ArrayLengthExpression:
				{
					return this.VisitArrayLengthExpression((ArrayLengthExpression)node);
				}
				case CodeNodeType.ExceptionStatement:
				{
					return this.VisitExceptionStatement((ExceptionStatement)node);
				}
				case CodeNodeType.BreakSwitchCaseStatement:
				{
					return this.VisitBreakSwitchCaseStatement((BreakSwitchCaseStatement)node);
				}
				case CodeNodeType.CaseGotoStatement:
				{
					return this.VisitCaseGotoStatement((CaseGotoStatement)node);
				}
				case CodeNodeType.FinallyClause:
				{
					return this.VisitFinallyClause((FinallyClause)node);
				}
				case CodeNodeType.ShortFormReturnExpression:
				{
					return this.VisitShortFormReturnExpression((ShortFormReturnExpression)node);
				}
				case CodeNodeType.AnonymousObjectCreationExpression:
				{
					return this.VisitAnonymousObjectCreationExpression((AnonymousObjectCreationExpression)node);
				}
				case CodeNodeType.FromClause:
				{
					return this.VisitFromClause((FromClause)node);
				}
				case CodeNodeType.SelectClause:
				{
					return this.VisitSelectClause((SelectClause)node);
				}
				case CodeNodeType.WhereClause:
				{
					return this.VisitWhereClause((WhereClause)node);
				}
				case CodeNodeType.GroupClause:
				{
					return this.VisitGroupClause((GroupClause)node);
				}
				case CodeNodeType.OrderByClause:
				{
					return this.VisitOrderByClause((OrderByClause)node);
				}
				case CodeNodeType.JoinClause:
				{
					return this.VisitJoinClause((JoinClause)node);
				}
				case CodeNodeType.LetClause:
				{
					return this.VisitLetClause((LetClause)node);
				}
				case CodeNodeType.IntoClause:
				{
					return this.VisitIntoClause((IntoClause)node);
				}
				case CodeNodeType.LinqQueryExpression:
				{
					return this.VisitLinqQueryExpression((LinqQueryExpression)node);
				}
				case CodeNodeType.ArrayVariableCreationExpression:
				{
					return this.VisitArrayVariableDeclarationExpression((ArrayVariableDeclarationExpression)node);
				}
				case CodeNodeType.ArrayAssignmentVariableReferenceExpression:
				{
					return this.VisitArrayAssignmentVariableReferenceExpression((ArrayVariableReferenceExpression)node);
				}
				case CodeNodeType.ArrayAssignmentFieldReferenceExpression:
				{
					return this.VisitArrayAssignmentFieldReferenceExpression((ArrayAssignmentFieldReferenceExpression)node);
				}
				case CodeNodeType.PropertyInitializerExpression:
				{
					return this.VisitPropertyInitializerExpression((PropertyInitializerExpression)node);
				}
				case CodeNodeType.FieldInitializerExpression:
				{
					return this.VisitFieldInitializerExpression((FieldInitializerExpression)node);
				}
				case CodeNodeType.ParenthesesExpression:
				{
					return this.VisitParenthesesExpression((ParenthesesExpression)node);
				}
				case CodeNodeType.InitializerExpression:
				{
					return this.VisitInitializerExpression((InitializerExpression)node);
				}
				case CodeNodeType.CheckedExpression:
				{
					return this.VisitCheckedExpression((CheckedExpression)node);
				}
				case CodeNodeType.MemberHandleExpression:
				{
					return this.VisitMemberHandleExpression((MemberHandleExpression)node);
				}
				case CodeNodeType.AutoPropertyConstructorInitializerExpression:
				{
					return this.VisitAutoPropertyConstructorInitializerExpression((AutoPropertyConstructorInitializerExpression)node);
				}
				case CodeNodeType.RaiseEventExpression:
				{
					return this.VisitRaiseEventExpression((RaiseEventExpression)node);
				}
				case CodeNodeType.RefVariableDeclarationExpression:
				{
					return this.VisitRefVariableDeclarationExpression((RefVariableDeclarationExpression)node);
				}
				case CodeNodeType.RefReturnExpression:
				{
					return this.VisitRefReturnExpression((RefReturnExpression)node);
				}
				default:
				{
					throw new ArgumentException();
				}
			}
		}

		public virtual ICodeNode Visit(ICodeNode node)
		{
			this.visitsOnStack += (long)1;
			if (this.visitsOnStack == (long)0x24e)
			{
				this.visitsOnStack = (long)0;
				throw new Exception("Stack overflow while traversing code tree in transform.");
			}
			ICodeNode codeNode = this.DoVisit(node);
			this.visitsOnStack -= (long)1;
			return codeNode;
		}

		protected virtual TCollection Visit<TCollection, TElement>(TCollection original)
		where TCollection : class, IList<TElement>, new()
		where TElement : class, ICodeNode
		{
			TCollection tCollection = default(TCollection);
			for (int i = 0; i < original.Count; i++)
			{
				TElement tElement = (TElement)this.Visit(original[i]);
				if (tCollection != null)
				{
					if (tElement != null)
					{
						tCollection.Add(tElement);
					}
				}
				else if ((object)tElement != (object)original[i])
				{
					tCollection = Activator.CreateInstance<TCollection>();
					for (int j = 0; j < i; j++)
					{
						tCollection.Add(original[j]);
					}
					if (tElement != null)
					{
						tCollection.Add(tElement);
					}
				}
			}
			TCollection tCollection1 = tCollection;
			if (tCollection1 == null)
			{
				tCollection1 = original;
			}
			return tCollection1;
		}

		public virtual ICollection<Statement> Visit(StatementCollection node)
		{
			return this.Visit<StatementCollection, Statement>(node);
		}

		public virtual ICollection<Expression> Visit(ExpressionCollection node)
		{
			return this.Visit<ExpressionCollection, Expression>(node);
		}

		public virtual ICollection<SwitchCase> Visit(SwitchCaseCollection node)
		{
			return this.Visit<SwitchCaseCollection, SwitchCase>(node);
		}

		public virtual ICollection<CatchClause> Visit(CatchClauseCollection node)
		{
			return this.Visit<CatchClauseCollection, CatchClause>(node);
		}

		public virtual ICodeNode VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			node.Initializer = (InitializerExpression)this.Visit(node.Initializer);
			return node;
		}

		public ICodeNode VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			node.Field = (FieldReferenceExpression)this.Visit(node.Field);
			node.Dimensions = (ExpressionCollection)this.Visit(node.Dimensions);
			return node;
		}

		public virtual ICodeNode VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			node.Variable = (VariableReferenceExpression)this.Visit(node.Variable);
			node.Dimensions = (ExpressionCollection)this.Visit(node.Dimensions);
			return node;
		}

		public virtual ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			node.Dimensions = (ExpressionCollection)this.Visit(node.Dimensions);
			node.Initializer = (InitializerExpression)this.Visit(node.Initializer);
			return node;
		}

		public virtual ICodeNode VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			return this.VisitIIndexerExpression(node);
		}

		public virtual ICodeNode VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			return node;
		}

		public virtual ICodeNode VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			node.Variable = (VariableDeclarationExpression)this.Visit(node.Variable);
			node.Dimensions = (ExpressionCollection)this.Visit(node.Dimensions);
			return node;
		}

		public virtual ICodeNode VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node)
		{
			return node;
		}

		public ICodeNode VisitAwaitExpression(AwaitExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			return this.VisitCtorExpression(node);
		}

		public virtual ICodeNode VisitBaseReferenceExpression(BaseReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			node.Left = (Expression)this.Visit(node.Left);
			node.Right = (Expression)this.Visit(node.Right);
			return node;
		}

		public virtual ICodeNode VisitBlockExpression(BlockExpression node)
		{
			node.Expressions = (ExpressionCollection)this.Visit(node.Expressions);
			return node;
		}

		public virtual ICodeNode VisitBlockStatement(BlockStatement node)
		{
			node.Statements = (StatementCollection)this.Visit(node.Statements);
			return node;
		}

		public virtual ICodeNode VisitBoxExpression(BoxExpression node)
		{
			node.BoxedExpression = (Expression)this.Visit(node.BoxedExpression);
			return node;
		}

		public virtual ICodeNode VisitBreakStatement(BreakStatement node)
		{
			return node;
		}

		public ICodeNode VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitCanCastExpression(CanCastExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitCaseGotoStatement(CaseGotoStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitCatchClause(CatchClause node)
		{
			node.Body = (BlockStatement)this.Visit(node.Body);
			node.Variable = (VariableDeclarationExpression)this.Visit(node.Variable);
			node.Filter = (Statement)this.Visit(node.Filter);
			return node;
		}

		public virtual ICodeNode VisitCheckedExpression(CheckedExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitConditionCase(ConditionCase node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitConditionExpression(ConditionExpression node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			node.Then = (Expression)this.Visit(node.Then);
			node.Else = (Expression)this.Visit(node.Else);
			return node;
		}

		public virtual ICodeNode VisitContinueStatement(ContinueStatement node)
		{
			return node;
		}

		private ICodeNode VisitCtorExpression(MethodInvocationExpression node)
		{
			node.Arguments = (ExpressionCollection)this.Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitDefaultCase(DefaultCase node)
		{
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			node.MethodExpression = (Expression)this.Visit(node.MethodExpression);
			return node;
		}

		public virtual ICodeNode VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			node.Arguments = (ExpressionCollection)this.Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitDoWhileStatement(DoWhileStatement node)
		{
			node.Body = (BlockStatement)this.Visit(node.Body);
			node.Condition = (Expression)this.Visit(node.Condition);
			return node;
		}

		public virtual ICodeNode VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
		{
			node.Arguments = (ExpressionCollection)this.Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitDynamicIndexerExpression(DynamicIndexerExpression node)
		{
			return this.VisitIIndexerExpression(node);
		}

		public virtual ICodeNode VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			if (node.IsMethodInvocation)
			{
				node.InvocationArguments = (ExpressionCollection)this.Visit(node.InvocationArguments);
			}
			return node;
		}

		public virtual ICodeNode VisitEmptyStatement(EmptyStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitEnumExpression(EnumExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitEventReferenceExpression(EventReferenceExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			return node;
		}

		public virtual ICodeNode VisitExceptionStatement(ExceptionStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public ICodeNode VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			return node;
		}

		public ICodeNode VisitFinallyClause(FinallyClause node)
		{
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitFixedStatement(FixedStatement node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitForEachStatement(ForEachStatement node)
		{
			node.Variable = (VariableDeclarationExpression)this.Visit(node.Variable);
			node.Collection = (Expression)this.Visit(node.Collection);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitForStatement(ForStatement node)
		{
			node.Initializer = (Expression)this.Visit(node.Initializer);
			node.Condition = (Expression)this.Visit(node.Condition);
			node.Increment = (Expression)this.Visit(node.Increment);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public ICodeNode VisitFromClause(FromClause node)
		{
			node.Identifier = (Expression)this.Visit(node.Identifier);
			node.Collection = (Expression)this.Visit(node.Collection);
			return node;
		}

		public virtual ICodeNode VisitGotoStatement(GotoStatement node)
		{
			return node;
		}

		public ICodeNode VisitGroupClause(GroupClause node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			node.GroupKey = (Expression)this.Visit(node.GroupKey);
			return node;
		}

		public virtual ICodeNode VisitIfElseIfStatement(IfElseIfStatement node)
		{
			for (int i = 0; i < node.ConditionBlocks.Count; i++)
			{
				KeyValuePair<Expression, BlockStatement> item = node.ConditionBlocks[i];
				Expression expression = (Expression)this.Visit(item.Key);
				item = node.ConditionBlocks[i];
				BlockStatement blockStatement = (BlockStatement)this.Visit(item.Value);
				blockStatement.Parent = node;
				node.ConditionBlocks[i] = new KeyValuePair<Expression, BlockStatement>(expression, blockStatement);
			}
			node.Else = (BlockStatement)this.Visit(node.Else);
			return node;
		}

		public virtual ICodeNode VisitIfStatement(IfStatement node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			node.Then = (BlockStatement)this.Visit(node.Then);
			node.Else = (BlockStatement)this.Visit(node.Else);
			return node;
		}

		private ICodeNode VisitIIndexerExpression(IIndexerExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			node.Indices = (ExpressionCollection)this.Visit(node.Indices);
			return (ICodeNode)node;
		}

		public virtual ICodeNode VisitImplicitCastExpression(ImplicitCastExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitInitializerExpression(InitializerExpression node)
		{
			node.Expression = (BlockExpression)this.Visit(node.Expression);
			return node;
		}

		public ICodeNode VisitIntoClause(IntoClause node)
		{
			node.Identifier = (VariableReferenceExpression)this.Visit(node.Identifier);
			return node;
		}

		public ICodeNode VisitJoinClause(JoinClause node)
		{
			node.InnerIdentifier = (Expression)this.Visit(node.InnerIdentifier);
			node.InnerCollection = (Expression)this.Visit(node.InnerCollection);
			node.OuterKey = (Expression)this.Visit(node.OuterKey);
			node.InnerKey = (Expression)this.Visit(node.InnerKey);
			return node;
		}

		public virtual ICodeNode VisitLambdaExpression(LambdaExpression node)
		{
			node.Arguments = (ExpressionCollection)this.Visit(node.Arguments);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public ICodeNode VisitLambdaParameterExpression(LambdaParameterExpression node)
		{
			return node;
		}

		public ICodeNode VisitLetClause(LetClause node)
		{
			node.Identifier = (VariableReferenceExpression)this.Visit(node.Identifier);
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitLinqQueryExpression(LinqQueryExpression node)
		{
			for (int i = 0; i < node.Clauses.Count; i++)
			{
				node.Clauses[i] = (QueryClause)this.Visit(node.Clauses[i]);
			}
			return node;
		}

		public virtual ICodeNode VisitLiteralExpression(LiteralExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitLockStatement(LockStatement node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitMakeRefExpression(MakeRefExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitMemberHandleExpression(MemberHandleExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			node.MethodExpression = (MethodReferenceExpression)this.Visit(node.MethodExpression);
			node.Arguments = (ExpressionCollection)this.Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			return node;
		}

		public virtual ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			node.Arguments = (ExpressionCollection)this.Visit(node.Arguments);
			node.Initializer = (InitializerExpression)this.Visit(node.Initializer);
			return node;
		}

		public ICodeNode VisitOrderByClause(OrderByClause node)
		{
			for (int i = 0; i < node.ExpressionToOrderDirectionMap.Count; i++)
			{
				KeyValuePair<Expression, OrderDirection> item = node.ExpressionToOrderDirectionMap[i];
				node.ExpressionToOrderDirectionMap[i] = new KeyValuePair<Expression, OrderDirection>((Expression)this.Visit(item.Key), item.Value);
			}
			return node;
		}

		public virtual ICodeNode VisitParenthesesExpression(ParenthesesExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public ICodeNode VisitPropertyInitializerExpression(PropertyInitializerExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			node.Target = (Expression)this.Visit(node.Target);
			node.Arguments = (ExpressionCollection)this.Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitRaiseEventExpression(RaiseEventExpression node)
		{
			this.Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitRefReturnExpression(RefReturnExpression node)
		{
			node.Value = (Expression)this.Visit(node.Value);
			return node;
		}

		public virtual ICodeNode VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitReturnExpression(ReturnExpression node)
		{
			node.Value = (Expression)this.Visit(node.Value);
			return node;
		}

		public virtual ICodeNode VisitSafeCastExpression(SafeCastExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public ICodeNode VisitSelectClause(SelectClause node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitShortFormReturnExpression(ShortFormReturnExpression node)
		{
			node.Value = (Expression)this.Visit(node.Value);
			return node;
		}

		public virtual ICodeNode VisitSizeOfExpression(SizeOfExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitStackAllocExpression(StackAllocExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			SwitchCaseCollection switchCaseCollection = new SwitchCaseCollection();
			foreach (SwitchCase @case in node.Cases)
			{
				switchCaseCollection.Add(@case);
			}
			node.Cases = (SwitchCaseCollection)this.Visit(switchCaseCollection);
			return node;
		}

		public virtual ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			return this.VisitCtorExpression(node);
		}

		public virtual ICodeNode VisitThisReferenceExpression(ThisReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitThrowExpression(ThrowExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitTryStatement(TryStatement node)
		{
			node.Try = (BlockStatement)this.Visit(node.Try);
			node.CatchClauses = (CatchClauseCollection)this.Visit(node.CatchClauses);
			node.Fault = (BlockStatement)this.Visit(node.Fault);
			node.Finally = (FinallyClause)this.Visit(node.Finally);
			return node;
		}

		public virtual ICodeNode VisitTypeOfExpression(TypeOfExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitTypeReferenceExpression(TypeReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitUnaryExpression(UnaryExpression node)
		{
			node.Operand = (Expression)this.Visit(node.Operand);
			return node;
		}

		public virtual ICodeNode VisitUsingStatement(UsingStatement node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			return node;
		}

		public ICodeNode VisitWhereClause(WhereClause node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			return node;
		}

		public virtual ICodeNode VisitWhileStatement(WhileStatement node)
		{
			node.Condition = (Expression)this.Visit(node.Condition);
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitYieldBreakExpression(YieldBreakExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitYieldReturnExpression(YieldReturnExpression node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			return node;
		}
	}
}