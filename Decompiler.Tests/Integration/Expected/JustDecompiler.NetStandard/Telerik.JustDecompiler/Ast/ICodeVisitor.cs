using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast
{
	public interface ICodeVisitor
	{
		void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node);

		void VisitArgumentReferenceExpression(ArgumentReferenceExpression node);

		void VisitArrayCreationExpression(ArrayCreationExpression node);

		void VisitArrayIndexerExpression(ArrayIndexerExpression node);

		void VisitArrayLengthExpression(ArrayLengthExpression node);

		void VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node);

		void VisitAwaitExpression(AwaitExpression node);

		void VisitBaseReferenceExpression(BaseReferenceExpression node);

		void VisitBinaryExpression(BinaryExpression node);

		void VisitBlockExpression(BlockExpression node);

		void VisitBlockStatement(BlockStatement node);

		void VisitBoxExpression(BoxExpression node);

		void VisitBreakStatement(BreakStatement node);

		void VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node);

		void VisitCanCastExpression(CanCastExpression node);

		void VisitCaseGotoStatement(CaseGotoStatement node);

		void VisitCatchClause(CatchClause node);

		void VisitCheckedExpression(CheckedExpression node);

		void VisitConditionCase(ConditionCase node);

		void VisitConditionExpression(ConditionExpression node);

		void VisitContinueStatement(ContinueStatement node);

		void VisitDefaultCase(DefaultCase node);

		void VisitDefaultObjectExpression(DefaultObjectExpression node);

		void VisitDelegateCreationExpression(DelegateCreationExpression node);

		void VisitDoWhileStatement(DoWhileStatement node);

		void VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node);

		void VisitDynamicIndexerExpression(DynamicIndexerExpression node);

		void VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node);

		void VisitEnumExpression(EnumExpression node);

		void VisitEventReferenceExpression(EventReferenceExpression node);

		void VisitExplicitCastExpression(ExplicitCastExpression node);

		void VisitExpressionStatement(ExpressionStatement node);

		void VisitFieldInitializerExpression(FieldInitializerExpression node);

		void VisitFieldReferenceExpression(FieldReferenceExpression node);

		void VisitFinallyClause(FinallyClause node);

		void VisitForEachStatement(ForEachStatement node);

		void VisitForStatement(ForStatement node);

		void VisitFromClause(FromClause node);

		void VisitGotoStatement(GotoStatement node);

		void VisitGroupClause(GroupClause node);

		void VisitIfElseIfStatement(IfElseIfStatement node);

		void VisitIfStatement(IfStatement node);

		void VisitImplicitCastExpression(ImplicitCastExpression node);

		void VisitInitializerExpression(InitializerExpression node);

		void VisitIntoClause(IntoClause node);

		void VisitJoinClause(JoinClause node);

		void VisitLambdaParameterExpression(LambdaParameterExpression node);

		void VisitLetClause(LetClause node);

		void VisitLinqQueryExpression(LinqQueryExpression node);

		void VisitLiteralExpression(LiteralExpression node);

		void VisitMemberHandleExpression(MemberHandleExpression node);

		void VisitMethodInvocationExpression(MethodInvocationExpression node);

		void VisitMethodReferenceExpression(MethodReferenceExpression node);

		void VisitObjectCreationExpression(ObjectCreationExpression node);

		void VisitOrderByClause(OrderByClause node);

		void VisitParenthesesExpression(ParenthesesExpression node);

		void VisitPropertyInitializerExpression(PropertyInitializerExpression node);

		void VisitPropertyReferenceExpression(PropertyReferenceExpression node);

		void VisitRaiseEventExpression(RaiseEventExpression node);

		void VisitRefReturnExpression(RefReturnExpression node);

		void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node);

		void VisitReturnExpression(ReturnExpression node);

		void VisitSafeCastExpression(SafeCastExpression node);

		void VisitSelectClause(SelectClause node);

		void VisitSwitchStatement(SwitchStatement node);

		void VisitThisReferenceExpression(ThisReferenceExpression node);

		void VisitThrowExpression(ThrowExpression node);

		void VisitTryStatement(TryStatement node);

		void VisitTypeOfExpression(TypeOfExpression node);

		void VisitTypeReferenceExpression(TypeReferenceExpression node);

		void VisitUnaryExpression(UnaryExpression node);

		void VisitUnsafeBlockStatement(UnsafeBlockStatement node);

		void VisitVariableDeclarationExpression(VariableDeclarationExpression node);

		void VisitVariableReferenceExpression(VariableReferenceExpression node);

		void VisitWhereClause(WhereClause node);

		void VisitWhileStatement(WhileStatement node);
	}
}