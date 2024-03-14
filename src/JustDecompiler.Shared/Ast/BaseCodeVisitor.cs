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
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast
{
	public class BaseCodeVisitor : ICodeVisitor
	{
		private long visitsOnStack = 0;

		public virtual void Visit(ICodeNode node)
		{
			visitsOnStack++;

			//A good place to watch for StackOverFlowException. That one cannot be effectively caught and results in app crash.
			//We replace it with our own custom exception here before it occurs. The number of allowed stack frames
			//is chosen empirically
			if (visitsOnStack == 600)
			{
				visitsOnStack = 0;
				throw new Exception("Stack overflow while traversing code tree in visit.");
			}

			if (null == node)
			{
				visitsOnStack--;
				return;
			}

			switch (node.CodeNodeType)
			{
				case CodeNodeType.UnsafeBlock:
					VisitUnsafeBlockStatement((UnsafeBlockStatement)node);
					break;
				case CodeNodeType.BlockStatement:
					VisitBlockStatement((BlockStatement)node);
					break;
				case CodeNodeType.ReturnExpression:
					VisitReturnExpression((ReturnExpression)node);
					break;
				case CodeNodeType.GotoStatement:
					VisitGotoStatement((GotoStatement)node);
					break;
				case CodeNodeType.IfStatement:
					VisitIfStatement((IfStatement)node);
					break;
				case CodeNodeType.IfElseIfStatement:
					VisitIfElseIfStatement((IfElseIfStatement)node);
					break;
				case CodeNodeType.ExpressionStatement:
					VisitExpressionStatement((ExpressionStatement)node);
					break;
				case CodeNodeType.ThrowExpression:
					VisitThrowExpression((ThrowExpression)node);
					break;
				case CodeNodeType.WhileStatement:
					VisitWhileStatement((WhileStatement)node);
					break;
				case CodeNodeType.DoWhileStatement:
					VisitDoWhileStatement((DoWhileStatement)node);
					break;
				case CodeNodeType.BreakStatement:
					VisitBreakStatement((BreakStatement)node);
					break;
				case CodeNodeType.ContinueStatement:
					VisitContinueStatement((ContinueStatement)node);
					break;
				case CodeNodeType.ForStatement:
					VisitForStatement((ForStatement)node);
					break;
				case CodeNodeType.ForEachStatement:
					VisitForEachStatement((ForEachStatement)node);
					break;
				case CodeNodeType.ConditionCase:
					VisitConditionCase((ConditionCase)node);
					break;
				case CodeNodeType.DefaultCase:
					VisitDefaultCase((DefaultCase)node);
					break;
				case CodeNodeType.SwitchStatement:
					VisitSwitchStatement((SwitchStatement)node);
					break;
				case CodeNodeType.CatchClause:
					VisitCatchClause((CatchClause)node);
					break;
				case CodeNodeType.TryStatement:
					VisitTryStatement((TryStatement)node);
					break;
				case CodeNodeType.BlockExpression:
					VisitBlockExpression((BlockExpression)node);
					break;
				case CodeNodeType.MethodInvocationExpression:
					VisitMethodInvocationExpression((MethodInvocationExpression)node);
					break;
				case CodeNodeType.MethodReferenceExpression:
					VisitMethodReferenceExpression((MethodReferenceExpression)node);
					break;
				case CodeNodeType.DelegateCreationExpression:
					VisitDelegateCreationExpression((DelegateCreationExpression)node);
					break;
				case CodeNodeType.LiteralExpression:
					VisitLiteralExpression((LiteralExpression)node);
					break;
				case CodeNodeType.UnaryExpression:
					VisitUnaryExpression((UnaryExpression)node);
					break;
				case CodeNodeType.BinaryExpression:
					VisitBinaryExpression((BinaryExpression)node);
					break;
				case CodeNodeType.ArgumentReferenceExpression:
					VisitArgumentReferenceExpression((ArgumentReferenceExpression)node);
					break;
				case CodeNodeType.VariableReferenceExpression:
					VisitVariableReferenceExpression((VariableReferenceExpression)node);
					break;
				case CodeNodeType.VariableDeclarationExpression:
					VisitVariableDeclarationExpression((VariableDeclarationExpression)node);
					break;
				case CodeNodeType.ThisReferenceExpression:
					VisitThisReferenceExpression((ThisReferenceExpression)node);
					break;
				case CodeNodeType.BaseReferenceExpression:
					VisitBaseReferenceExpression((BaseReferenceExpression)node);
					break;
				case CodeNodeType.FieldReferenceExpression:
					VisitFieldReferenceExpression((FieldReferenceExpression)node);
					break;
				case CodeNodeType.ExplicitCastExpression:
                    VisitExplicitCastExpression((ExplicitCastExpression)node);
					break;
                case CodeNodeType.ImplicitCastExpression:
                    VisitImplicitCastExpression((ImplicitCastExpression)node);
                    break;
                case CodeNodeType.SafeCastExpression:
					VisitSafeCastExpression((SafeCastExpression)node);
					break;
				case CodeNodeType.CanCastExpression:
					VisitCanCastExpression((CanCastExpression)node);
					break;
				case CodeNodeType.TypeOfExpression:
					VisitTypeOfExpression((TypeOfExpression)node);
					break;
				case CodeNodeType.ConditionExpression:
					VisitConditionExpression((ConditionExpression)node);
					break;
				case CodeNodeType.ArrayCreationExpression:
					VisitArrayCreationExpression((ArrayCreationExpression)node);
					break;
				case CodeNodeType.ArrayIndexerExpression:
					VisitArrayIndexerExpression((ArrayIndexerExpression)node);
					break;
				case CodeNodeType.ObjectCreationExpression:
					VisitObjectCreationExpression((ObjectCreationExpression)node);
					break;
				case CodeNodeType.DefaultObjectExpression:
					VisitDefaultObjectExpression((DefaultObjectExpression)node);
					break;
				case CodeNodeType.PropertyReferenceExpression:
					VisitPropertyReferenceExpression((PropertyReferenceExpression)node);
					break;
				case CodeNodeType.TypeReferenceExpression:
					VisitTypeReferenceExpression((TypeReferenceExpression)node);
					break;
				case CodeNodeType.UsingStatement:
					VisitUsingStatement((UsingStatement)node);
					break;
				case CodeNodeType.FixedStatement:
					VisitFixedStatement((FixedStatement)node);
					break;
				case CodeNodeType.StackAllocExpression:
					VisitStackAllocExpression((StackAllocExpression)node);
					break;
				case CodeNodeType.SizeOfExpression:
					VisitSizeOfExpression((SizeOfExpression)node);
					break;
				case CodeNodeType.MakeRefExpression:
					VisitMakeRefExpression((MakeRefExpression)node);
					break;
				case CodeNodeType.EventReferenceExpression:
					VisitEventReferenceExpression((EventReferenceExpression)node);
					break;
				case CodeNodeType.EnumExpression:
					VisitEnumExpression((EnumExpression)node);
					break;
				case CodeNodeType.LambdaExpression:
					VisitLambdaExpression((LambdaExpression)node);
					break;
				case CodeNodeType.DelegateInvokeExpression:
					VisitDelegateInvokeExpression((DelegateInvokeExpression)node);
					break;
				case CodeNodeType.BaseCtorExpression:
					VisitBaseCtorExpression((BaseCtorExpression)node);
					break;
				case CodeNodeType.ThisCtorExpression:
					VisitThisCtorExpression((ThisCtorExpression)node);
					break;
				case CodeNodeType.YieldReturnExpression:
					VisitYieldReturnExpression((YieldReturnExpression)node);
					break;
				case CodeNodeType.YieldBreakExpression:
					VisitYieldBreakExpression((YieldBreakExpression)node);
					break;
				case CodeNodeType.LockStatement:
					VisitLockStatement((LockStatement)node);
					break;
				case CodeNodeType.EmptyStatement:
					VisitEmptyStatement((EmptyStatement)node);
					break;
				case CodeNodeType.DynamicMemberReferenceExpression:
					VisitDynamicMemberReferenceExpression((DynamicMemberReferenceExpression)node);
					break;
				case CodeNodeType.DynamicConstructorInvocationExpression:
					VisitDynamicConstructorInvocationExpression((DynamicConstructorInvocationExpression)node);
					break;
				case CodeNodeType.DynamicIndexerExpression:
					VisitDynamicIndexerExpression((DynamicIndexerExpression)node);
					break;
				case CodeNodeType.BoxExpression:
					VisitBoxExpression((BoxExpression)node);
					break;
				case CodeNodeType.LambdaParameterExpression:
					VisitLambdaParameterExpression((LambdaParameterExpression)node);
					break;
				case CodeNodeType.AwaitExpression:
					VisitAwaitExpression((AwaitExpression)node);
					break;
				case CodeNodeType.ArrayLengthExpression:
					VisitArrayLengthExpression((ArrayLengthExpression)node);
					break;
				case CodeNodeType.ExceptionStatement:
					VisitExceptionStatement((ExceptionStatement)node);
					break;
				case CodeNodeType.BreakSwitchCaseStatement:
					VisitBreakSwitchCaseStatement((BreakSwitchCaseStatement)node);
					break;
				case CodeNodeType.CaseGotoStatement:
					VisitCaseGotoStatement((CaseGotoStatement)node);
					break;
                case CodeNodeType.FinallyClause:
                    VisitFinallyClause((FinallyClause)node);
                    break;
				case CodeNodeType.ShortFormReturnExpression:
					VisitShortFormReturnExpression((ShortFormReturnExpression)node);
					break;
				case CodeNodeType.AnonymousObjectCreationExpression:
					VisitAnonymousObjectCreationExpression((AnonymousObjectCreationExpression)node);
					break;
                case CodeNodeType.FromClause:
                    VisitFromClause((FromClause)node);
                    break;
                case CodeNodeType.WhereClause:
                    VisitWhereClause((WhereClause)node);
                    break;
                case CodeNodeType.SelectClause:
                    VisitSelectClause((SelectClause)node);
                    break;
                case CodeNodeType.OrderByClause:
                    VisitOrderByClause((OrderByClause)node);
                    break;
                case CodeNodeType.GroupClause:
                    VisitGroupClause((GroupClause)node);
                    break;
                case CodeNodeType.JoinClause:
                    VisitJoinClause((JoinClause)node);
                    break;
                case CodeNodeType.LetClause:
                    VisitLetClause((LetClause)node);
                    break;
                case CodeNodeType.IntoClause:
                    VisitIntoClause((IntoClause)node);
                    break;
                case CodeNodeType.LinqQueryExpression:
                    VisitLinqQueryExpression((LinqQueryExpression)node);
                    break;
				case CodeNodeType.ArrayVariableCreationExpression:
					VisitArrayVariableDeclarationExpression((ArrayVariableDeclarationExpression)node);
					break;
				case CodeNodeType.ArrayAssignmentVariableReferenceExpression:
					VisitArrayAssignmentVariableReferenceExpression((ArrayVariableReferenceExpression)node);
					break;
				case CodeNodeType.ArrayAssignmentFieldReferenceExpression:
					VisitArrayAssignmentFieldReferenceExpression((ArrayAssignmentFieldReferenceExpression)node);
					break;
				case CodeNodeType.AnonymousPropertyInitializerExpression:
					VisitAnonymousPropertyInitializerExpression((AnonymousPropertyInitializerExpression)node);
					break;
				case CodeNodeType.PropertyInitializerExpression:
					VisitPropertyInitializerExpression((PropertyInitializerExpression)node);
					break;
				case CodeNodeType.FieldInitializerExpression:
					VisitFieldInitializerExpression((FieldInitializerExpression)node);
					break;
				case CodeNodeType.ParenthesesExpression:
					VisitParenthesesExpression((ParenthesesExpression)node);
					break;
				case CodeNodeType.InitializerExpression:
					VisitInitializerExpression((InitializerExpression)node);
					break;
                case CodeNodeType.CheckedExpression:
                    VisitCheckedExpression((CheckedExpression)node);
                    break;
                case CodeNodeType.MemberHandleExpression:
                    VisitMemberHandleExpression((MemberHandleExpression)node);
                    break;
                case CodeNodeType.AutoPropertyConstructorInitializerExpression:
                    VisitAutoPropertyConstructorInitializerExpression((AutoPropertyConstructorInitializerExpression)node);
                    break;
                case CodeNodeType.RaiseEventExpression:
                    VisitRaiseEventExpression((RaiseEventExpression)node);
                    break;
                case CodeNodeType.RefVariableDeclarationExpression:
                    VisitRefVariableDeclarationExpression((RefVariableDeclarationExpression)node);
                    break;
                case CodeNodeType.RefReturnExpression:
                    VisitRefReturnExpression((RefReturnExpression)node);
                    break;
				default:
					throw new ArgumentException();
			}

			visitsOnStack--;
		}

		public virtual void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			Visit(node.Initializer);
		}

		public virtual void VisitExceptionStatement(ExceptionStatement node)
		{
		}

		public virtual void VisitEmptyStatement(EmptyStatement node)
		{
		}

		public virtual void VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			Visit(node.Target);
		}

		public virtual void VisitUnsafeBlockStatement(UnsafeBlockStatement unsafeBlock)
		{
			Visit(unsafeBlock.Statements);
		}

		public virtual void Visit(IEnumerable collection)
		{
			foreach (ICodeNode node in collection)
				Visit(node);
		}

		public virtual void VisitBlockStatement(BlockStatement node)
		{
			Visit(node.Statements);
		}

		public virtual void VisitReturnExpression(ReturnExpression node)
		{
			Visit(node.Value);
		}

		public virtual void VisitShortFormReturnExpression(ShortFormReturnExpression node)
		{
			Visit(node.Value);
		}

		public virtual void VisitGotoStatement(GotoStatement node)
		{
		}

		public virtual void VisitIfStatement(IfStatement node)
		{
			Visit(node.Condition);
			Visit(node.Then);
			Visit(node.Else);
		}

		public virtual void VisitIfElseIfStatement(IfElseIfStatement node)
		{
			for (int i = 0; i < node.ConditionBlocks.Count; i++)
			{
				KeyValuePair<Expression, BlockStatement> pair = node.ConditionBlocks[i];
				Visit(pair.Key);
				Visit(pair.Value);
			}

			Visit(node.Else);
		}

		public virtual void VisitExpressionStatement(ExpressionStatement node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitThrowExpression(ThrowExpression node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitWhileStatement(WhileStatement node)
		{
			Visit(node.Condition);
			Visit(node.Body);
		}

		public virtual void VisitUsingStatement(UsingStatement node)
		{
			Visit(node.Expression);
			Visit(node.Body);
		}

		public virtual void VisitFixedStatement(FixedStatement node)
		{
			Visit(node.Expression);
			Visit(node.Body);
		}

		public virtual void VisitDoWhileStatement(DoWhileStatement node)
		{
			Visit(node.Body);
			Visit(node.Condition);
		}

		public virtual void VisitBreakStatement(BreakStatement node)
		{
		}

		public virtual void VisitContinueStatement(ContinueStatement node)
		{
		}

		public virtual void VisitForStatement(ForStatement node)
		{
			Visit(node.Initializer);
			Visit(node.Condition);
			Visit(node.Increment);
			Visit(node.Body);
		}

		public virtual void VisitForEachStatement(ForEachStatement node)
		{
			Visit(node.Variable);
			Visit(node.Collection);
			Visit(node.Body);
		}

		public virtual void VisitConditionCase(ConditionCase node)
		{
			Visit(node.Condition);
			Visit(node.Body);
		}

		public virtual void VisitDefaultCase(DefaultCase node)
		{
			Visit(node.Body);
		}

		public virtual void VisitSwitchStatement(SwitchStatement node)
		{
			Visit(node.Condition);
			Visit(node.Cases);
		}

		public virtual void VisitCatchClause(CatchClause node)
		{
			Visit(node.Body);
			Visit(node.Variable);
            Visit(node.Filter);
		}

		public virtual void VisitTryStatement(TryStatement node)
		{
			Visit(node.Try);
			Visit(node.CatchClauses);
			Visit(node.Fault);
			Visit(node.Finally);
		}

		public virtual void VisitBlockExpression(BlockExpression node)
		{
			Visit(node.Expressions);
		}

		public virtual void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			Visit(node.MethodExpression);
			Visit(node.Arguments);
		}

		public virtual void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			Visit(node.Target);
		}

		public virtual void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			Visit(node.Target);
			Visit(node.MethodExpression);
		}

		public virtual void VisitLiteralExpression(LiteralExpression node)
		{
		}

		public virtual void VisitUnaryExpression(UnaryExpression node)
		{
			Visit(node.Operand);
		}

		public virtual void VisitBinaryExpression(BinaryExpression node)
		{
			Visit(node.Left);
			Visit(node.Right);
		}

		public virtual void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
		}

		public virtual void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
		}

		public virtual void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
		}

		public virtual void VisitThisReferenceExpression(ThisReferenceExpression node)
		{
		}

		public virtual void VisitBaseReferenceExpression(BaseReferenceExpression node)
		{
		}

		public virtual void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			Visit(node.Target);
		}

		public virtual void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			Visit(node.Expression);
		}

        public virtual void VisitImplicitCastExpression(ImplicitCastExpression node)
        {
            Visit(node.Expression);
        }

		public virtual void VisitSafeCastExpression(SafeCastExpression node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitCanCastExpression(CanCastExpression node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitTypeOfExpression(TypeOfExpression node)
		{
		}

		public virtual void VisitConditionExpression(ConditionExpression node)
		{
			Visit(node.Condition);
			Visit(node.Then);
			Visit(node.Else);
		}

		public virtual void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			Visit(node.Dimensions);
			Visit(node.Initializer);
		}

		public virtual void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			VisitIIndexerExpression(node);
		}

		public virtual void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			Visit(node.Arguments);
			Visit(node.Initializer);
		}

		public virtual void VisitDefaultObjectExpression(DefaultObjectExpression node)
		{

		}

		public virtual void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			Visit(node.Target);
			Visit(node.Arguments);
		}

		public virtual void VisitTypeReferenceExpression(TypeReferenceExpression node)
		{
		}

		public virtual void VisitStackAllocExpression(StackAllocExpression node)
		{
            Visit(node.Expression);
		}

		public virtual void VisitSizeOfExpression(SizeOfExpression node)
		{

		}

		public virtual void VisitMakeRefExpression(MakeRefExpression node)
		{
            Visit(node.Expression);
		}

		public virtual void VisitEventReferenceExpression(EventReferenceExpression node)
		{
			Visit(node.Target);
		}

		public virtual void VisitEnumExpression(EnumExpression node)
		{

		}

		public virtual void VisitLambdaExpression(LambdaExpression node)
		{
			Visit(node.Body);
		}

		public virtual void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
            Visit(node.Target);
            Visit(node.Arguments);
		}

		public virtual void VisitBaseCtorExpression(BaseCtorExpression node)
		{
            Visit(node.Arguments);
		}

		public virtual void VisitThisCtorExpression(ThisCtorExpression node)
		{
            Visit(node.Arguments);
		}

		public virtual void VisitYieldReturnExpression(YieldReturnExpression node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitYieldBreakExpression(YieldBreakExpression node)
		{

		}

		public virtual void VisitLockStatement(LockStatement node)
		{
			Visit(node.Expression);
			Visit(node.Body);
		}

		public virtual void VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
		{
			Visit(node.Target);
			if (node.IsMethodInvocation)
			{
				Visit(node.InvocationArguments);
			}
		}

		public virtual void VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
		{
			Visit(node.Arguments);
		}

		public virtual void VisitDynamicIndexerExpression(DynamicIndexerExpression node)
		{
			VisitIIndexerExpression(node);
		}

		protected virtual void VisitIIndexerExpression(IIndexerExpression node)
		{
			Visit(node.Target);
			Visit(node.Indices);
		}

		public virtual void VisitBoxExpression(BoxExpression node)
		{
			Visit(node.BoxedExpression);
		}

		public virtual void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
		{
		}

        public virtual void VisitPropertyInitializerExpression(PropertyInitializerExpression node)
        {
        }

		public virtual void VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
		}

		public virtual void VisitLambdaParameterExpression(LambdaParameterExpression node)
		{
		}

		public virtual void VisitAwaitExpression(AwaitExpression node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node)
		{
		}

		public virtual void VisitCaseGotoStatement(CaseGotoStatement node)
		{
		}

        public void VisitFinallyClause(FinallyClause node)
        {
            Visit(node.Body);
        }

        public virtual void VisitFromClause(FromClause node)
        {
            Visit(node.Identifier);
            Visit(node.Collection);
        }

        public virtual void VisitWhereClause(WhereClause node)
        {
            Visit(node.Condition);
        }

        public virtual void VisitSelectClause(SelectClause node)
        {
            Visit(node.Expression);
        }

        public virtual void VisitOrderByClause(OrderByClause node)
        {
            foreach (KeyValuePair<Expression, OrderDirection> pair in node.ExpressionToOrderDirectionMap)
            {
                Visit(pair.Key);
            }
        }

        public virtual void VisitJoinClause(JoinClause node)
        {
            Visit(node.InnerIdentifier);
            Visit(node.InnerCollection);
            Visit(node.OuterKey);
            Visit(node.InnerKey);
        }

        public virtual void VisitGroupClause(GroupClause node)
        {
            Visit(node.Expression);
            Visit(node.GroupKey);
        }

        public virtual void VisitLetClause(LetClause node)
        {
            Visit(node.Identifier);
            Visit(node.Expression);
        }

        public virtual void VisitIntoClause(IntoClause node)
        {
            Visit(node.Identifier);
        }

        public virtual void VisitLinqQueryExpression(LinqQueryExpression node)
        {
            foreach (QueryClause clause in node.Clauses)
            {
                Visit(clause);
            }
        }

		public virtual void VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			Visit(node.Variable);
			Visit(node.Dimensions);
		}

		public virtual void VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			Visit(node.Variable);
			Visit(node.Dimensions);
		}

		public virtual void VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			Visit(node.Field);
			Visit(node.Dimensions);
		}

		public virtual void VisitParenthesesExpression(ParenthesesExpression node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitInitializerExpression(InitializerExpression node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitCheckedExpression(CheckedExpression node)
		{
			Visit(node.Expression);
		}

		public virtual void VisitMemberHandleExpression(MemberHandleExpression node)
		{
		}

        public virtual void VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node)
        {
        }

        public virtual void VisitRaiseEventExpression(RaiseEventExpression node)
        {
            Visit(node.Arguments);
        }

        public virtual void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
        {
        }

        public virtual void VisitRefReturnExpression(RefReturnExpression node)
        {
            Visit(node.Value);
        }
    }
}