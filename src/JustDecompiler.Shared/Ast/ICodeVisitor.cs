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

using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
namespace Telerik.JustDecompiler.Ast {

	public interface ICodeVisitor {
		void VisitBlockStatement (BlockStatement node);
        void VisitReturnExpression(ReturnExpression node);
        void VisitGotoStatement(GotoStatement node);
		void VisitIfStatement (IfStatement node);
        void VisitIfElseIfStatement(IfElseIfStatement node);
		void VisitExpressionStatement (ExpressionStatement node);
		void VisitThrowExpression (ThrowExpression node);
		void VisitWhileStatement (WhileStatement node);
		void VisitDoWhileStatement (DoWhileStatement node);
		void VisitBreakStatement (BreakStatement node);
		void VisitContinueStatement (ContinueStatement node);
		void VisitForStatement (ForStatement node);
		void VisitForEachStatement (ForEachStatement node);
		void VisitConditionCase (ConditionCase node);
		void VisitDefaultCase (DefaultCase node);
		void VisitSwitchStatement (SwitchStatement node);
		void VisitCatchClause (CatchClause node);
		void VisitTryStatement (TryStatement node);
		void VisitBlockExpression (BlockExpression node);
		void VisitMethodInvocationExpression (MethodInvocationExpression node);
		void VisitMethodReferenceExpression (MethodReferenceExpression node);
		void VisitDelegateCreationExpression (DelegateCreationExpression node);
		void VisitLiteralExpression (LiteralExpression node);
		void VisitUnaryExpression (UnaryExpression node);
		void VisitBinaryExpression (BinaryExpression node);
		void VisitArgumentReferenceExpression (ArgumentReferenceExpression node);
		void VisitVariableReferenceExpression (VariableReferenceExpression node);
		void VisitVariableDeclarationExpression (VariableDeclarationExpression node);
		void VisitThisReferenceExpression (ThisReferenceExpression node);
		void VisitBaseReferenceExpression (BaseReferenceExpression node);
		void VisitFieldReferenceExpression (FieldReferenceExpression node);
		void VisitExplicitCastExpression(ExplicitCastExpression node);
		void VisitImplicitCastExpression (ImplicitCastExpression node);
        void VisitSafeCastExpression (SafeCastExpression node);
		void VisitCanCastExpression (CanCastExpression node);
		void VisitTypeOfExpression (TypeOfExpression node);
		void VisitConditionExpression (ConditionExpression node);
		void VisitArrayCreationExpression (ArrayCreationExpression node);
		void VisitArrayIndexerExpression (ArrayIndexerExpression node);
		void VisitObjectCreationExpression (ObjectCreationExpression node);
        void VisitDefaultObjectExpression(DefaultObjectExpression node);
		void VisitPropertyReferenceExpression (PropertyReferenceExpression node);
		void VisitTypeReferenceExpression (TypeReferenceExpression node);
        void VisitEnumExpression(EnumExpression node);
        void VisitUnsafeBlockStatement(UnsafeBlockStatement node);
        void VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node);
        void VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node);
        void VisitDynamicIndexerExpression(DynamicIndexerExpression node);
        void VisitEventReferenceExpression(EventReferenceExpression node);
		void VisitBoxExpression(BoxExpression node);
		void VisitLambdaParameterExpression(LambdaParameterExpression node);
		void VisitArrayLengthExpression(ArrayLengthExpression node);
        void VisitAwaitExpression(AwaitExpression node);
		void VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node);
		void VisitCaseGotoStatement(CaseGotoStatement node);
        void VisitFinallyClause(FinallyClause node);
        void VisitFromClause(FromClause node);
        void VisitWhereClause(WhereClause node);
        void VisitSelectClause(SelectClause node);
        void VisitOrderByClause(OrderByClause node);
        void VisitJoinClause(JoinClause node);
        void VisitGroupClause(GroupClause node);
        void VisitLetClause(LetClause node);
        void VisitIntoClause(IntoClause node);
		void VisitLinqQueryExpression(LinqQueryExpression node);
		void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node);
		void VisitPropertyInitializerExpression(PropertyInitializerExpression node);
		void VisitFieldInitializerExpression(FieldInitializerExpression node);
		void VisitParenthesesExpression(ParenthesesExpression node);
		void VisitInitializerExpression(InitializerExpression node);
        void VisitCheckedExpression(CheckedExpression node);
        void VisitMemberHandleExpression(MemberHandleExpression node);
        void VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node);
        void VisitRaiseEventExpression(RaiseEventExpression node);
        void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node);
        void VisitRefReturnExpression(RefReturnExpression node);
    }
}
