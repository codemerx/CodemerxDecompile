using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast
{
	public interface ICodeTransformer
	{
		ICodeNode VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node);

		ICodeNode VisitArgumentReferenceExpression(ArgumentReferenceExpression node);

		ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node);

		ICodeNode VisitArrayIndexerExpression(ArrayIndexerExpression node);

		ICodeNode VisitArrayLengthExpression(ArrayLengthExpression node);

		ICodeNode VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node);

		ICodeNode VisitAwaitExpression(AwaitExpression node);

		ICodeNode VisitBaseCtorExpression(BaseCtorExpression node);

		ICodeNode VisitBaseReferenceExpression(BaseReferenceExpression node);

		ICodeNode VisitBinaryExpression(BinaryExpression node);

		ICodeNode VisitBlockExpression(BlockExpression node);

		ICodeNode VisitBlockStatement(BlockStatement node);

		ICodeNode VisitBoxExpression(BoxExpression node);

		ICodeNode VisitBreakStatement(BreakStatement node);

		ICodeNode VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node);

		ICodeNode VisitCanCastExpression(CanCastExpression node);

		ICodeNode VisitCaseGotoStatement(CaseGotoStatement node);

		ICodeNode VisitCatchClause(CatchClause node);

		ICodeNode VisitCheckedExpression(CheckedExpression node);

		ICodeNode VisitConditionCase(ConditionCase node);

		ICodeNode VisitConditionExpression(ConditionExpression node);

		ICodeNode VisitContinueStatement(ContinueStatement node);

		ICodeNode VisitDefaultCase(DefaultCase node);

		ICodeNode VisitDefaultObjectExpression(DefaultObjectExpression node);

		ICodeNode VisitDelegateCreationExpression(DelegateCreationExpression node);

		ICodeNode VisitDoWhileStatement(DoWhileStatement node);

		ICodeNode VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node);

		ICodeNode VisitDynamicIndexerExpression(DynamicIndexerExpression node);

		ICodeNode VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node);

		ICodeNode VisitEnumExpression(EnumExpression node);

		ICodeNode VisitEventReferenceExpression(EventReferenceExpression node);

		ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node);

		ICodeNode VisitExpressionStatement(ExpressionStatement node);

		ICodeNode VisitFieldInitializerExpression(FieldInitializerExpression node);

		ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node);

		ICodeNode VisitFinallyClause(FinallyClause node);

		ICodeNode VisitFixedStatement(FixedStatement node);

		ICodeNode VisitForEachStatement(ForEachStatement node);

		ICodeNode VisitForStatement(ForStatement node);

		ICodeNode VisitFromClause(FromClause node);

		ICodeNode VisitGotoStatement(GotoStatement node);

		ICodeNode VisitGroupClause(GroupClause node);

		ICodeNode VisitIfElseIfStatement(IfElseIfStatement node);

		ICodeNode VisitIfStatement(IfStatement node);

		ICodeNode VisitImplicitCastExpression(ImplicitCastExpression node);

		ICodeNode VisitInitializerExpression(InitializerExpression node);

		ICodeNode VisitIntoClause(IntoClause node);

		ICodeNode VisitJoinClause(JoinClause node);

		ICodeNode VisitLambdaParameterExpression(LambdaParameterExpression node);

		ICodeNode VisitLetClause(LetClause node);

		ICodeNode VisitLinqQueryExpression(LinqQueryExpression node);

		ICodeNode VisitLiteralExpression(LiteralExpression node);

		ICodeNode VisitMemberHandleExpression(MemberHandleExpression node);

		ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node);

		ICodeNode VisitMethodReferenceExpression(MethodReferenceExpression node);

		ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node);

		ICodeNode VisitOrderByClause(OrderByClause node);

		ICodeNode VisitParenthesesExpression(ParenthesesExpression node);

		ICodeNode VisitPropertyInitializerExpression(PropertyInitializerExpression node);

		ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node);

		ICodeNode VisitRaiseEventExpression(RaiseEventExpression node);

		ICodeNode VisitRefReturnExpression(RefReturnExpression node);

		ICodeNode VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node);

		ICodeNode VisitReturnExpression(ReturnExpression node);

		ICodeNode VisitSafeCastExpression(SafeCastExpression node);

		ICodeNode VisitSelectClause(SelectClause node);

		ICodeNode VisitSwitchStatement(SwitchStatement node);

		ICodeNode VisitThisCtorExpression(ThisCtorExpression node);

		ICodeNode VisitThisReferenceExpression(ThisReferenceExpression node);

		ICodeNode VisitThrowExpression(ThrowExpression node);

		ICodeNode VisitTryStatement(TryStatement node);

		ICodeNode VisitTypeOfExpression(TypeOfExpression node);

		ICodeNode VisitTypeReferenceExpression(TypeReferenceExpression node);

		ICodeNode VisitUnaryExpression(UnaryExpression node);

		ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node);

		ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node);

		ICodeNode VisitWhereClause(WhereClause node);

		ICodeNode VisitWhileStatement(WhileStatement node);

		ICodeNode VisitYieldReturnExpression(YieldReturnExpression node);
	}
}