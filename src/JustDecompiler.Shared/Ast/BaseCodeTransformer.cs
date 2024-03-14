#region license
//
//	(C) 2005 - 2007 db4objects Inc. http://www.db4o.com
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
// Warning: generated do not edit
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast
{
	public class BaseCodeTransformer : ICodeTransformer
	{
		private long visitsOnStack = 0;

		private ICodeNode DoVisit(ICodeNode node)
		{
			if (node == null)
				return null;

			switch (node.CodeNodeType)
			{
				case CodeNodeType.BlockStatement:
					return VisitBlockStatement((BlockStatement)node);
				case CodeNodeType.ReturnExpression:
					return VisitReturnExpression((ReturnExpression)node);
				case CodeNodeType.GotoStatement:
					return VisitGotoStatement((GotoStatement)node);
				case CodeNodeType.IfStatement:
					return VisitIfStatement((IfStatement)node);
				case CodeNodeType.IfElseIfStatement:
					return VisitIfElseIfStatement((IfElseIfStatement)node);
				case CodeNodeType.ExpressionStatement:
					return VisitExpressionStatement((ExpressionStatement)node);
				case CodeNodeType.ThrowExpression:
					return VisitThrowExpression((ThrowExpression)node);
				case CodeNodeType.WhileStatement:
					return VisitWhileStatement((WhileStatement)node);
				case CodeNodeType.DoWhileStatement:
					return VisitDoWhileStatement((DoWhileStatement)node);
				case CodeNodeType.BreakStatement:
					return VisitBreakStatement((BreakStatement)node);
				case CodeNodeType.ContinueStatement:
					return VisitContinueStatement((ContinueStatement)node);
				case CodeNodeType.ForStatement:
					return VisitForStatement((ForStatement)node);
				case CodeNodeType.ForEachStatement:
					return VisitForEachStatement((ForEachStatement)node);
				case CodeNodeType.ConditionCase:
					return VisitConditionCase((ConditionCase)node);
				case CodeNodeType.DefaultCase:
					return VisitDefaultCase((DefaultCase)node);
				case CodeNodeType.SwitchStatement:
					return VisitSwitchStatement((SwitchStatement)node);
				case CodeNodeType.CatchClause:
					return VisitCatchClause((CatchClause)node);
				case CodeNodeType.TryStatement:
					return VisitTryStatement((TryStatement)node);
				case CodeNodeType.BlockExpression:
					return VisitBlockExpression((BlockExpression)node);
				case CodeNodeType.MethodInvocationExpression:
					return VisitMethodInvocationExpression((MethodInvocationExpression)node);
				case CodeNodeType.MethodReferenceExpression:
					return VisitMethodReferenceExpression((MethodReferenceExpression)node);
				case CodeNodeType.DelegateCreationExpression:
					return VisitDelegateCreationExpression((DelegateCreationExpression)node);
				case CodeNodeType.LiteralExpression:
					return VisitLiteralExpression((LiteralExpression)node);
				case CodeNodeType.UnaryExpression:
					return VisitUnaryExpression((UnaryExpression)node);
				case CodeNodeType.BinaryExpression:
					return VisitBinaryExpression((BinaryExpression)node);
				case CodeNodeType.ArgumentReferenceExpression:
					return VisitArgumentReferenceExpression((ArgumentReferenceExpression)node);
				case CodeNodeType.VariableReferenceExpression:
					return VisitVariableReferenceExpression((VariableReferenceExpression)node);
				case CodeNodeType.VariableDeclarationExpression:
					return VisitVariableDeclarationExpression((VariableDeclarationExpression)node);
				case CodeNodeType.ThisReferenceExpression:
					return VisitThisReferenceExpression((ThisReferenceExpression)node);
				case CodeNodeType.BaseReferenceExpression:
					return VisitBaseReferenceExpression((BaseReferenceExpression)node);
				case CodeNodeType.FieldReferenceExpression:
					return VisitFieldReferenceExpression((FieldReferenceExpression)node);
				case CodeNodeType.ExplicitCastExpression:
					return VisitExplicitCastExpression((ExplicitCastExpression)node);
                case CodeNodeType.ImplicitCastExpression:
                    return VisitImplicitCastExpression((ImplicitCastExpression)node);
                case CodeNodeType.SafeCastExpression:
					return VisitSafeCastExpression((SafeCastExpression)node);
				case CodeNodeType.CanCastExpression:
					return VisitCanCastExpression((CanCastExpression)node);
				case CodeNodeType.TypeOfExpression:
					return VisitTypeOfExpression((TypeOfExpression)node);
				case CodeNodeType.ConditionExpression:
					return VisitConditionExpression((ConditionExpression)node);
				case CodeNodeType.ArrayCreationExpression:
					return VisitArrayCreationExpression((ArrayCreationExpression)node);
				case CodeNodeType.ArrayIndexerExpression:
					return VisitArrayIndexerExpression((ArrayIndexerExpression)node);
				case CodeNodeType.ObjectCreationExpression:
					return VisitObjectCreationExpression((ObjectCreationExpression)node);
				case CodeNodeType.DefaultObjectExpression:
					return VisitDefaultObjectExpression((DefaultObjectExpression)node);
				case CodeNodeType.PropertyReferenceExpression:
					return VisitPropertyReferenceExpression((PropertyReferenceExpression)node);
				case CodeNodeType.TypeReferenceExpression:
					return VisitTypeReferenceExpression((TypeReferenceExpression)node);
				case CodeNodeType.UsingStatement:
					return VisitUsingStatement((UsingStatement)node);
				case CodeNodeType.FixedStatement:
					return VisitFixedStatement((FixedStatement)node);
				case CodeNodeType.StackAllocExpression:
					return VisitStackAllocExpression((StackAllocExpression)node);
				case CodeNodeType.SizeOfExpression:
					return VisitSizeOfExpression((SizeOfExpression)node);
				case CodeNodeType.MakeRefExpression:
					return VisitMakeRefExpression((MakeRefExpression)node);
				case CodeNodeType.EnumExpression:
					return VisitEnumExpression((EnumExpression)node);
				case CodeNodeType.LambdaExpression:
					return VisitLambdaExpression((LambdaExpression)node);
				case CodeNodeType.DelegateInvokeExpression:
					return VisitDelegateInvokeExpression((DelegateInvokeExpression)node);
				case CodeNodeType.BaseCtorExpression:
					return VisitBaseCtorExpression((BaseCtorExpression)node);
				case CodeNodeType.ThisCtorExpression:
					return VisitThisCtorExpression((ThisCtorExpression)node);
				case CodeNodeType.YieldReturnExpression:
					return VisitYieldReturnExpression((YieldReturnExpression)node);
				case CodeNodeType.YieldBreakExpression:
					return VisitYieldBreakExpression((YieldBreakExpression)node);
				case CodeNodeType.LockStatement:
					return VisitLockStatement((LockStatement)node);
				case CodeNodeType.EmptyStatement:
					return VisitEmptyStatement((EmptyStatement)node);
				case CodeNodeType.DynamicMemberReferenceExpression:
					return VisitDynamicMemberReferenceExpression((DynamicMemberReferenceExpression)node);
				case CodeNodeType.DynamicConstructorInvocationExpression:
					return VisitDynamicConstructorInvocationExpression((DynamicConstructorInvocationExpression)node);
				case CodeNodeType.DynamicIndexerExpression:
					return VisitDynamicIndexerExpression((DynamicIndexerExpression)node);
				case CodeNodeType.EventReferenceExpression:
					return VisitEventReferenceExpression((EventReferenceExpression)node);
				case CodeNodeType.BoxExpression:
					return VisitBoxExpression((BoxExpression)node);
				case CodeNodeType.LambdaParameterExpression:
					return VisitLambdaParameterExpression((LambdaParameterExpression)node);
				case CodeNodeType.AwaitExpression:
					return VisitAwaitExpression((AwaitExpression)node);
				case CodeNodeType.ArrayLengthExpression:
					return VisitArrayLengthExpression((ArrayLengthExpression)node);
				case CodeNodeType.ExceptionStatement:
					return VisitExceptionStatement((ExceptionStatement)node);
				case CodeNodeType.BreakSwitchCaseStatement:
					return VisitBreakSwitchCaseStatement((BreakSwitchCaseStatement)node);
				case CodeNodeType.CaseGotoStatement:
					return VisitCaseGotoStatement((CaseGotoStatement)node);
				case CodeNodeType.FinallyClause:
					return VisitFinallyClause((FinallyClause)node);
				case CodeNodeType.ShortFormReturnExpression:
					return VisitShortFormReturnExpression((ShortFormReturnExpression)node);
				case CodeNodeType.AnonymousObjectCreationExpression:
					return VisitAnonymousObjectCreationExpression((AnonymousObjectCreationExpression)node);
				case CodeNodeType.FromClause:
					return VisitFromClause((FromClause)node);
				case CodeNodeType.WhereClause:
					return VisitWhereClause((WhereClause)node);
				case CodeNodeType.SelectClause:
					return VisitSelectClause((SelectClause)node);
				case CodeNodeType.GroupClause:
					return VisitGroupClause((GroupClause)node);
				case CodeNodeType.OrderByClause:
					return VisitOrderByClause((OrderByClause)node);
				case CodeNodeType.JoinClause:
					return VisitJoinClause((JoinClause)node);
				case CodeNodeType.LetClause:
					return VisitLetClause((LetClause)node);
				case CodeNodeType.IntoClause:
					return VisitIntoClause((IntoClause)node);
				case CodeNodeType.LinqQueryExpression:
					return VisitLinqQueryExpression((LinqQueryExpression)node);
				case CodeNodeType.ArrayVariableCreationExpression:
					return VisitArrayVariableDeclarationExpression((ArrayVariableDeclarationExpression)node);
				case CodeNodeType.ArrayAssignmentVariableReferenceExpression:
					return VisitArrayAssignmentVariableReferenceExpression((ArrayVariableReferenceExpression)node);
				case CodeNodeType.ArrayAssignmentFieldReferenceExpression:
					return VisitArrayAssignmentFieldReferenceExpression((ArrayAssignmentFieldReferenceExpression)node);
				case CodeNodeType.AnonymousPropertyInitializerExpression:
					return VisitAnonymousPropertyInitializerExpression((AnonymousPropertyInitializerExpression)node);
				case CodeNodeType.PropertyInitializerExpression:
					return VisitPropertyInitializerExpression((PropertyInitializerExpression)node);
				case CodeNodeType.FieldInitializerExpression:
					return VisitFieldInitializerExpression((FieldInitializerExpression)node);
				case CodeNodeType.ParenthesesExpression:
					return VisitParenthesesExpression((ParenthesesExpression)node);
				case CodeNodeType.InitializerExpression:
					return VisitInitializerExpression((InitializerExpression)node);
				case CodeNodeType.CheckedExpression:
					return VisitCheckedExpression((CheckedExpression)node);
				case CodeNodeType.MemberHandleExpression:
					return VisitMemberHandleExpression((MemberHandleExpression)node);
                case CodeNodeType.AutoPropertyConstructorInitializerExpression:
                    return VisitAutoPropertyConstructorInitializerExpression((AutoPropertyConstructorInitializerExpression)node);
                case CodeNodeType.RaiseEventExpression:
                    return VisitRaiseEventExpression((RaiseEventExpression)node);
                case CodeNodeType.RefVariableDeclarationExpression:
                    return VisitRefVariableDeclarationExpression((RefVariableDeclarationExpression)node);
                case CodeNodeType.RefReturnExpression:
                    return VisitRefReturnExpression((RefReturnExpression)node);
				default:
					throw new ArgumentException();
			}
		}

		public virtual ICodeNode VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			node.Initializer = (InitializerExpression)Visit(node.Initializer);
			return node;
		}

		public virtual ICodeNode VisitExceptionStatement(ExceptionStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			return node;
		}

		public virtual ICodeNode VisitEmptyStatement(EmptyStatement node)
		{
			return node;
		}

		public virtual ICodeNode Visit(ICodeNode node)
		{
			visitsOnStack++;

			//A good place to watch for StackOverFlowException. That one cannot be effectively caught and results in app crash.
			//We replace it with our own custom exception here before it occurs. The number of allowed stack frames
			//i chosen empirically
			if (visitsOnStack == 590)
			{
				visitsOnStack = 0;
				throw new Exception("Stack overflow while traversing code tree in transform.");
			}

			ICodeNode result = DoVisit(node);
			visitsOnStack--;

			return result;
		}

		protected virtual TCollection Visit<TCollection, TElement>(TCollection original)
			where TCollection : class, IList<TElement>, new()
			where TElement : class, ICodeNode
		{
			TCollection collection = null;

			for (int i = 0; i < original.Count; i++)
			{
				var element = (TElement)Visit(original[i]);

				if (collection != null)
				{
					if (element != null)
						collection.Add(element);

					continue;
				}

				//if (!EqualityComparer<TElement>.Default.Equals(element, original[i]))
				if (element != original[i])
				{
					collection = new TCollection();
					for (int j = 0; j < i; j++)
						collection.Add(original[j]);

					if (element != null)
						collection.Add(element);
				}
			}

			return collection ?? original;
		}

		public virtual ICollection<Statement> Visit(StatementCollection node)
		{
			return Visit<StatementCollection, Statement>(node);
		}

		public virtual ICollection<Expression> Visit(ExpressionCollection node)
		{
			return Visit<ExpressionCollection, Expression>(node);
		}

		public virtual ICollection<SwitchCase> Visit(SwitchCaseCollection node)
		{
			return Visit<SwitchCaseCollection, SwitchCase>(node);
		}

		public virtual ICollection<CatchClause> Visit(CatchClauseCollection node)
		{
			return Visit<CatchClauseCollection, CatchClause>(node);
		}

		public virtual ICodeNode VisitBlockStatement(BlockStatement node)
		{
			node.Statements = (StatementCollection)Visit(node.Statements);
			return node;
		}

		public virtual ICodeNode VisitReturnExpression(ReturnExpression node)
		{
			node.Value = (Expression)Visit(node.Value);
			return node;
		}

		public virtual ICodeNode VisitShortFormReturnExpression(ShortFormReturnExpression node)
		{
			node.Value = (Expression)Visit(node.Value);
			return node;
		}

		public virtual ICodeNode VisitGotoStatement(GotoStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitIfStatement(IfStatement node)
		{
			node.Condition = (Expression)Visit(node.Condition);
			node.Then = (BlockStatement)Visit(node.Then);
			node.Else = (BlockStatement)Visit(node.Else);
			return node;
		}

		public virtual ICodeNode VisitIfElseIfStatement(IfElseIfStatement node)
		{
			for (int i = 0; i < node.ConditionBlocks.Count; i++)
			{
				Expression transformedCondition = (Expression)Visit(node.ConditionBlocks[i].Key);
				BlockStatement transformedBlock = (BlockStatement)Visit(node.ConditionBlocks[i].Value);
				transformedBlock.Parent = node;
				node.ConditionBlocks[i] = new KeyValuePair<Expression, BlockStatement>(transformedCondition, transformedBlock);
			}

			node.Else = (BlockStatement)Visit(node.Else);

			return node;
		}

		public virtual ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitThrowExpression(ThrowExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitWhileStatement(WhileStatement node)
		{
			node.Condition = (Expression)Visit(node.Condition);
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitUsingStatement(UsingStatement node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitFixedStatement(FixedStatement node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitDoWhileStatement(DoWhileStatement node)
		{
			node.Body = (BlockStatement)Visit(node.Body);
			node.Condition = (Expression)Visit(node.Condition);
			return node;
		}

		public virtual ICodeNode VisitBreakStatement(BreakStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitContinueStatement(ContinueStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitForStatement(ForStatement node)
		{
			node.Initializer = (Expression)Visit(node.Initializer);
			node.Condition = (Expression)Visit(node.Condition);
			node.Increment = (Expression)Visit(node.Increment);
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitForEachStatement(ForEachStatement node)
		{
			node.Variable = (VariableDeclarationExpression)Visit(node.Variable);
			node.Collection = (Expression)Visit(node.Collection);
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitConditionCase(ConditionCase node)
		{
			node.Condition = (Expression)Visit(node.Condition);
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitDefaultCase(DefaultCase node)
		{
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			node.Condition = (Expression)Visit(node.Condition);
			SwitchCaseCollection nodeCases = new SwitchCaseCollection();
			foreach (SwitchCase @case in node.Cases)
			{
				nodeCases.Add(@case);
			}
			node.Cases = (SwitchCaseCollection)Visit(nodeCases);
			return node;
		}

		public virtual ICodeNode VisitCatchClause(CatchClause node)
		{
			node.Body = (BlockStatement)Visit(node.Body);
			node.Variable = (VariableDeclarationExpression)Visit(node.Variable);
            node.Filter = (Statement)Visit(node.Filter);
			return node;
		}

		public virtual ICodeNode VisitTryStatement(TryStatement node)
		{
			node.Try = (BlockStatement)Visit(node.Try);
			node.CatchClauses = (CatchClauseCollection)Visit(node.CatchClauses);
			node.Fault = (BlockStatement)Visit(node.Fault);
			node.Finally = (FinallyClause)Visit(node.Finally);
			return node;
		}

		public virtual ICodeNode VisitBlockExpression(BlockExpression node)
		{
			node.Expressions = (ExpressionCollection)Visit(node.Expressions);
			return node;
		}

		public virtual ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			node.MethodExpression = (MethodReferenceExpression)Visit(node.MethodExpression);
			node.Arguments = (ExpressionCollection)Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			return node;
		}

		public virtual ICodeNode VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			node.MethodExpression = (Expression)Visit(node.MethodExpression);
			return node;
		}

		public virtual ICodeNode VisitLiteralExpression(LiteralExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitUnaryExpression(UnaryExpression node)
		{
			node.Operand = (Expression)Visit(node.Operand);
			return node;
		}

		public virtual ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			node.Left = (Expression)Visit(node.Left);
			node.Right = (Expression)Visit(node.Right);
			return node;
		}

		public virtual ICodeNode VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitThisReferenceExpression(ThisReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitBaseReferenceExpression(BaseReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			return node;
		}

		public virtual ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

        public virtual ICodeNode VisitImplicitCastExpression(ImplicitCastExpression node)
        {
            node.Expression = (Expression)Visit(node.Expression);
            return node;
        }

        public virtual ICodeNode VisitSafeCastExpression(SafeCastExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitCanCastExpression(CanCastExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitTypeOfExpression(TypeOfExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitConditionExpression(ConditionExpression node)
		{
			node.Condition = (Expression)Visit(node.Condition);
			node.Then = (Expression)Visit(node.Then);
			node.Else = (Expression)Visit(node.Else);
			return node;
		}

		public virtual ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			node.Dimensions = (ExpressionCollection)Visit(node.Dimensions);
			node.Initializer = (InitializerExpression)Visit(node.Initializer);
			return node;
		}

		public virtual ICodeNode VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			return VisitIIndexerExpression(node);
		}

		public virtual ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			node.Arguments = (ExpressionCollection)Visit(node.Arguments);
			node.Initializer = (InitializerExpression)Visit(node.Initializer);
			return node;
		}

		public virtual ICodeNode VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			node.Arguments = (ExpressionCollection)Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitTypeReferenceExpression(TypeReferenceExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitStackAllocExpression(StackAllocExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitSizeOfExpression(SizeOfExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitMakeRefExpression(MakeRefExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitEnumExpression(EnumExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitLambdaExpression(LambdaExpression node)
		{
			node.Arguments = (ExpressionCollection)Visit(node.Arguments);
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			node.Arguments = (ExpressionCollection)Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitBaseCtorExpression(BaseCtorExpression node)
		{
			return VisitCtorExpression(node);
		}

		public virtual ICodeNode VisitThisCtorExpression(ThisCtorExpression node)
		{
			return VisitCtorExpression(node);
		}

		private ICodeNode VisitCtorExpression(MethodInvocationExpression node)
		{
			node.Arguments = (ExpressionCollection)Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitYieldReturnExpression(YieldReturnExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitYieldBreakExpression(YieldBreakExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitLockStatement(LockStatement node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}

		public virtual ICodeNode VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			if (node.IsMethodInvocation)
			{
				node.InvocationArguments = (ExpressionCollection)Visit(node.InvocationArguments);
			}
			return node;
		}

		public virtual ICodeNode VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
		{
			node.Arguments = (ExpressionCollection)Visit(node.Arguments);
			return node;
		}

		public virtual ICodeNode VisitDynamicIndexerExpression(DynamicIndexerExpression node)
		{
			return VisitIIndexerExpression(node);
		}

		private ICodeNode VisitIIndexerExpression(IIndexerExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			node.Indices = (ExpressionCollection)Visit(node.Indices);
			return (ICodeNode)node;
		}

		public virtual ICodeNode VisitEventReferenceExpression(EventReferenceExpression node)
		{
			node.Target = (Expression)Visit(node.Target);
			return node;
		}

		public virtual ICodeNode VisitBoxExpression(BoxExpression node)
		{
			node.BoxedExpression = (Expression)Visit(node.BoxedExpression);
			return node;
		}

		public ICodeNode VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
		{
			return node;
		}

		public ICodeNode VisitPropertyInitializerExpression(PropertyInitializerExpression node)
		{
			return node;
		}

		public ICodeNode VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
			return node;
		}

		public ICodeNode VisitLambdaParameterExpression(LambdaParameterExpression node)
		{
			return node;
		}

		public ICodeNode VisitAwaitExpression(AwaitExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public ICodeNode VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitCaseGotoStatement(CaseGotoStatement node)
		{
			return node;
		}

		public ICodeNode VisitFinallyClause(FinallyClause node)
		{
			node.Body = (BlockStatement)Visit(node.Body);
			return node;
		}


		public ICodeNode VisitFromClause(FromClause node)
		{
			node.Identifier = (Expression)Visit(node.Identifier);
			node.Collection = (Expression)Visit(node.Collection);
			return node;
		}

		public ICodeNode VisitWhereClause(WhereClause node)
		{
			node.Condition = (Expression)Visit(node.Condition);
			return node;
		}

		public ICodeNode VisitSelectClause(SelectClause node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public ICodeNode VisitOrderByClause(OrderByClause node)
		{
			for (int i = 0; i < node.ExpressionToOrderDirectionMap.Count; i++)
			{
				KeyValuePair<Expression, OrderDirection> pair = node.ExpressionToOrderDirectionMap[i];
				node.ExpressionToOrderDirectionMap[i] = new KeyValuePair<Expression, OrderDirection>((Expression)Visit(pair.Key), pair.Value);
			}
			return node;
		}

		public ICodeNode VisitJoinClause(JoinClause node)
		{
			node.InnerIdentifier = (Expression)Visit(node.InnerIdentifier);
			node.InnerCollection = (Expression)Visit(node.InnerCollection);
			node.OuterKey = (Expression)Visit(node.OuterKey);
			node.InnerKey = (Expression)Visit(node.InnerKey);
			return node;
		}

		public ICodeNode VisitGroupClause(GroupClause node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			node.GroupKey = (Expression)Visit(node.GroupKey);
			return node;
		}

		public ICodeNode VisitLetClause(LetClause node)
		{
			node.Identifier = (VariableReferenceExpression)Visit(node.Identifier);
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public ICodeNode VisitIntoClause(IntoClause node)
		{
			node.Identifier = (VariableReferenceExpression)Visit(node.Identifier);
			return node;
		}

		public virtual ICodeNode VisitLinqQueryExpression(LinqQueryExpression node)
		{
			for (int i = 0; i < node.Clauses.Count; i++)
			{
				node.Clauses[i] = (QueryClause)Visit(node.Clauses[i]);
			}
			return node;
		}

		public virtual ICodeNode VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			node.Variable = (VariableDeclarationExpression)Visit(node.Variable);
			node.Dimensions = (ExpressionCollection)Visit(node.Dimensions);
			return node;
		}

		public virtual ICodeNode VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			node.Variable = (VariableReferenceExpression)Visit(node.Variable);
			node.Dimensions = (ExpressionCollection)Visit(node.Dimensions);
			return node;
		}

		public virtual ICodeNode VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			node.Field = (FieldReferenceExpression)Visit(node.Field);
			node.Dimensions = (ExpressionCollection)Visit(node.Dimensions);
			return node;
		}

		public virtual ICodeNode VisitParenthesesExpression(ParenthesesExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitInitializerExpression(InitializerExpression node)
		{
			node.Expression = (BlockExpression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitCheckedExpression(CheckedExpression node)
		{
			node.Expression = (Expression)Visit(node.Expression);
			return node;
		}

		public virtual ICodeNode VisitMemberHandleExpression(MemberHandleExpression node)
		{
			return node;
		}

        public virtual ICodeNode VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node)
        {
            return node;
        }

        public virtual ICodeNode VisitRaiseEventExpression(RaiseEventExpression node)
        {
            Visit(node.Arguments);
            return node;
        }

        public virtual ICodeNode VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
        {
            return node;
        }

        public virtual ICodeNode VisitRefReturnExpression(RefReturnExpression node)
        {
            node.Value = (Expression)Visit(node.Value);
            return node;
        }
    }
}
