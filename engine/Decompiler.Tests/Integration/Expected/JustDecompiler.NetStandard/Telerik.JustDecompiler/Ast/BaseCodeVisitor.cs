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
			base();
			return;
		}

		public virtual void Visit(ICodeNode node)
		{
			this.visitsOnStack = this.visitsOnStack + (long)1;
			if (this.visitsOnStack == (long)0x258)
			{
				this.visitsOnStack = (long)0;
				throw new Exception("Stack overflow while traversing code tree in visit.");
			}
			if (node == null)
			{
				this.visitsOnStack = this.visitsOnStack - (long)1;
				return;
			}
			switch (node.get_CodeNodeType())
			{
				case 0:
				{
					this.VisitBlockStatement((BlockStatement)node);
					break;
				}
				case 1:
				{
					this.VisitUnsafeBlockStatement((UnsafeBlockStatement)node);
					break;
				}
				case 2:
				{
					this.VisitGotoStatement((GotoStatement)node);
					break;
				}
				case 3:
				{
					this.VisitIfStatement((IfStatement)node);
					break;
				}
				case 4:
				{
					this.VisitIfElseIfStatement((IfElseIfStatement)node);
					break;
				}
				case 5:
				{
					this.VisitExpressionStatement((ExpressionStatement)node);
					break;
				}
				case 6:
				{
					this.VisitThrowExpression((ThrowExpression)node);
					break;
				}
				case 7:
				{
					this.VisitWhileStatement((WhileStatement)node);
					break;
				}
				case 8:
				{
					this.VisitDoWhileStatement((DoWhileStatement)node);
					break;
				}
				case 9:
				{
					this.VisitBreakStatement((BreakStatement)node);
					break;
				}
				case 10:
				{
					this.VisitContinueStatement((ContinueStatement)node);
					break;
				}
				case 11:
				{
					this.VisitForStatement((ForStatement)node);
					break;
				}
				case 12:
				{
					this.VisitForEachStatement((ForEachStatement)node);
					break;
				}
				case 13:
				{
					this.VisitConditionCase((ConditionCase)node);
					break;
				}
				case 14:
				{
					this.VisitDefaultCase((DefaultCase)node);
					break;
				}
				case 15:
				{
					this.VisitSwitchStatement((SwitchStatement)node);
					break;
				}
				case 16:
				{
					this.VisitCatchClause((CatchClause)node);
					break;
				}
				case 17:
				{
					this.VisitTryStatement((TryStatement)node);
					break;
				}
				case 18:
				{
					this.VisitBlockExpression((BlockExpression)node);
					break;
				}
				case 19:
				{
					this.VisitMethodInvocationExpression((MethodInvocationExpression)node);
					break;
				}
				case 20:
				{
					this.VisitMethodReferenceExpression((MethodReferenceExpression)node);
					break;
				}
				case 21:
				{
					this.VisitDelegateCreationExpression((DelegateCreationExpression)node);
					break;
				}
				case 22:
				{
					this.VisitLiteralExpression((LiteralExpression)node);
					break;
				}
				case 23:
				{
					this.VisitUnaryExpression((UnaryExpression)node);
					break;
				}
				case 24:
				{
					this.VisitBinaryExpression((BinaryExpression)node);
					break;
				}
				case 25:
				{
					this.VisitArgumentReferenceExpression((ArgumentReferenceExpression)node);
					break;
				}
				case 26:
				{
					this.VisitVariableReferenceExpression((VariableReferenceExpression)node);
					break;
				}
				case 27:
				{
					this.VisitVariableDeclarationExpression((VariableDeclarationExpression)node);
					break;
				}
				case 28:
				{
					this.VisitThisReferenceExpression((ThisReferenceExpression)node);
					break;
				}
				case 29:
				{
					this.VisitBaseReferenceExpression((BaseReferenceExpression)node);
					break;
				}
				case 30:
				{
					this.VisitFieldReferenceExpression((FieldReferenceExpression)node);
					break;
				}
				case 31:
				{
					this.VisitExplicitCastExpression((ExplicitCastExpression)node);
					break;
				}
				case 32:
				{
					this.VisitImplicitCastExpression((ImplicitCastExpression)node);
					break;
				}
				case 33:
				{
					this.VisitSafeCastExpression((SafeCastExpression)node);
					break;
				}
				case 34:
				{
					this.VisitCanCastExpression((CanCastExpression)node);
					break;
				}
				case 35:
				{
					this.VisitTypeOfExpression((TypeOfExpression)node);
					break;
				}
				case 36:
				{
					this.VisitConditionExpression((ConditionExpression)node);
					break;
				}
				case 37:
				{
					this.VisitFixedStatement((FixedStatement)node);
					break;
				}
				case 38:
				{
					this.VisitArrayCreationExpression((ArrayCreationExpression)node);
					break;
				}
				case 39:
				{
					this.VisitArrayIndexerExpression((ArrayIndexerExpression)node);
					break;
				}
				case 40:
				{
					this.VisitObjectCreationExpression((ObjectCreationExpression)node);
					break;
				}
				case 41:
				{
					this.VisitDefaultObjectExpression((DefaultObjectExpression)node);
					break;
				}
				case 42:
				{
					this.VisitPropertyReferenceExpression((PropertyReferenceExpression)node);
					break;
				}
				case 43:
				{
					this.VisitTypeReferenceExpression((TypeReferenceExpression)node);
					break;
				}
				case 44:
				{
					this.VisitUsingStatement((UsingStatement)node);
					break;
				}
				case 45:
				{
					this.VisitStackAllocExpression((StackAllocExpression)node);
					break;
				}
				case 46:
				{
					this.VisitSizeOfExpression((SizeOfExpression)node);
					break;
				}
				case 47:
				{
					this.VisitMakeRefExpression((MakeRefExpression)node);
					break;
				}
				case 48:
				{
					this.VisitEventReferenceExpression((EventReferenceExpression)node);
					break;
				}
				case 49:
				{
					this.VisitEnumExpression((EnumExpression)node);
					break;
				}
				case 50:
				{
					this.VisitLambdaExpression((LambdaExpression)node);
					break;
				}
				case 51:
				{
					this.VisitDelegateInvokeExpression((DelegateInvokeExpression)node);
					break;
				}
				case 52:
				{
					this.VisitBaseCtorExpression((BaseCtorExpression)node);
					break;
				}
				case 53:
				{
					this.VisitThisCtorExpression((ThisCtorExpression)node);
					break;
				}
				case 54:
				{
					this.VisitYieldReturnExpression((YieldReturnExpression)node);
					break;
				}
				case 55:
				{
					this.VisitYieldBreakExpression((YieldBreakExpression)node);
					break;
				}
				case 56:
				{
					this.VisitLockStatement((LockStatement)node);
					break;
				}
				case 57:
				{
					this.VisitReturnExpression((ReturnExpression)node);
					break;
				}
				case 58:
				{
					this.VisitEmptyStatement((EmptyStatement)node);
					break;
				}
				case 59:
				{
					this.VisitDynamicMemberReferenceExpression((DynamicMemberReferenceExpression)node);
					break;
				}
				case 60:
				{
					this.VisitDynamicConstructorInvocationExpression((DynamicConstructorInvocationExpression)node);
					break;
				}
				case 61:
				{
					this.VisitDynamicIndexerExpression((DynamicIndexerExpression)node);
					break;
				}
				case 62:
				{
					this.VisitBoxExpression((BoxExpression)node);
					break;
				}
				case 63:
				{
					this.VisitAnonymousPropertyInitializerExpression((AnonymousPropertyInitializerExpression)node);
					break;
				}
				case 64:
				{
					this.VisitLambdaParameterExpression((LambdaParameterExpression)node);
					break;
				}
				case 65:
				{
					this.VisitAwaitExpression((AwaitExpression)node);
					break;
				}
				case 66:
				{
					this.VisitArrayLengthExpression((ArrayLengthExpression)node);
					break;
				}
				case 67:
				{
					this.VisitExceptionStatement((ExceptionStatement)node);
					break;
				}
				case 68:
				{
					this.VisitBreakSwitchCaseStatement((BreakSwitchCaseStatement)node);
					break;
				}
				case 69:
				{
					this.VisitCaseGotoStatement((CaseGotoStatement)node);
					break;
				}
				case 70:
				{
					this.VisitFinallyClause((FinallyClause)node);
					break;
				}
				case 71:
				{
					this.VisitShortFormReturnExpression((ShortFormReturnExpression)node);
					break;
				}
				case 72:
				{
					this.VisitAnonymousObjectCreationExpression((AnonymousObjectCreationExpression)node);
					break;
				}
				case 73:
				{
					this.VisitFromClause((FromClause)node);
					break;
				}
				case 74:
				{
					this.VisitSelectClause((SelectClause)node);
					break;
				}
				case 75:
				{
					this.VisitWhereClause((WhereClause)node);
					break;
				}
				case 76:
				{
					this.VisitGroupClause((GroupClause)node);
					break;
				}
				case 77:
				{
					this.VisitOrderByClause((OrderByClause)node);
					break;
				}
				case 78:
				{
					this.VisitJoinClause((JoinClause)node);
					break;
				}
				case 79:
				{
					this.VisitLetClause((LetClause)node);
					break;
				}
				case 80:
				{
					this.VisitIntoClause((IntoClause)node);
					break;
				}
				case 81:
				{
					this.VisitLinqQueryExpression((LinqQueryExpression)node);
					break;
				}
				case 82:
				{
					this.VisitArrayVariableDeclarationExpression((ArrayVariableDeclarationExpression)node);
					break;
				}
				case 83:
				{
					this.VisitArrayAssignmentVariableReferenceExpression((ArrayVariableReferenceExpression)node);
					break;
				}
				case 84:
				{
					this.VisitArrayAssignmentFieldReferenceExpression((ArrayAssignmentFieldReferenceExpression)node);
					break;
				}
				case 85:
				{
					this.VisitPropertyInitializerExpression((PropertyInitializerExpression)node);
					break;
				}
				case 86:
				{
					this.VisitFieldInitializerExpression((FieldInitializerExpression)node);
					break;
				}
				case 87:
				{
					this.VisitParenthesesExpression((ParenthesesExpression)node);
					break;
				}
				case 88:
				{
					this.VisitInitializerExpression((InitializerExpression)node);
					break;
				}
				case 89:
				{
					this.VisitCheckedExpression((CheckedExpression)node);
					break;
				}
				case 90:
				{
					this.VisitMemberHandleExpression((MemberHandleExpression)node);
					break;
				}
				case 91:
				{
					this.VisitAutoPropertyConstructorInitializerExpression((AutoPropertyConstructorInitializerExpression)node);
					break;
				}
				case 92:
				{
					this.VisitRaiseEventExpression((RaiseEventExpression)node);
					break;
				}
				case 93:
				{
					this.VisitRefVariableDeclarationExpression((RefVariableDeclarationExpression)node);
					break;
				}
				case 94:
				{
					this.VisitRefReturnExpression((RefReturnExpression)node);
					break;
				}
				default:
				{
					goto Label0;
				}
			}
			this.visitsOnStack = this.visitsOnStack - (long)1;
			return;
		Label0:
			throw new ArgumentException();
		}

		public virtual void Visit(IEnumerable collection)
		{
			V_0 = collection.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = (ICodeNode)V_0.get_Current();
					this.Visit(V_1);
				}
			}
			finally
			{
				V_2 = V_0 as IDisposable;
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
			return;
		}

		public virtual void VisitAnonymousObjectCreationExpression(AnonymousObjectCreationExpression node)
		{
			this.Visit(node.get_Initializer());
			return;
		}

		public virtual void VisitAnonymousPropertyInitializerExpression(AnonymousPropertyInitializerExpression node)
		{
			return;
		}

		public virtual void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			return;
		}

		public virtual void VisitArrayAssignmentFieldReferenceExpression(ArrayAssignmentFieldReferenceExpression node)
		{
			this.Visit(node.get_Field());
			this.Visit(node.get_Dimensions());
			return;
		}

		public virtual void VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			this.Visit(node.get_Variable());
			this.Visit(node.get_Dimensions());
			return;
		}

		public virtual void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			this.Visit(node.get_Dimensions());
			this.Visit(node.get_Initializer());
			return;
		}

		public virtual void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			this.VisitIIndexerExpression(node);
			return;
		}

		public virtual void VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			this.Visit(node.get_Target());
			return;
		}

		public virtual void VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			this.Visit(node.get_Variable());
			this.Visit(node.get_Dimensions());
			return;
		}

		public virtual void VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node)
		{
			return;
		}

		public virtual void VisitAwaitExpression(AwaitExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.Visit(node.get_Arguments());
			return;
		}

		public virtual void VisitBaseReferenceExpression(BaseReferenceExpression node)
		{
			return;
		}

		public virtual void VisitBinaryExpression(BinaryExpression node)
		{
			this.Visit(node.get_Left());
			this.Visit(node.get_Right());
			return;
		}

		public virtual void VisitBlockExpression(BlockExpression node)
		{
			this.Visit(node.get_Expressions());
			return;
		}

		public virtual void VisitBlockStatement(BlockStatement node)
		{
			this.Visit(node.get_Statements());
			return;
		}

		public virtual void VisitBoxExpression(BoxExpression node)
		{
			this.Visit(node.get_BoxedExpression());
			return;
		}

		public virtual void VisitBreakStatement(BreakStatement node)
		{
			return;
		}

		public virtual void VisitBreakSwitchCaseStatement(BreakSwitchCaseStatement node)
		{
			return;
		}

		public virtual void VisitCanCastExpression(CanCastExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitCaseGotoStatement(CaseGotoStatement node)
		{
			return;
		}

		public virtual void VisitCatchClause(CatchClause node)
		{
			this.Visit(node.get_Body());
			this.Visit(node.get_Variable());
			this.Visit(node.get_Filter());
			return;
		}

		public virtual void VisitCheckedExpression(CheckedExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitConditionCase(ConditionCase node)
		{
			this.Visit(node.get_Condition());
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitConditionExpression(ConditionExpression node)
		{
			this.Visit(node.get_Condition());
			this.Visit(node.get_Then());
			this.Visit(node.get_Else());
			return;
		}

		public virtual void VisitContinueStatement(ContinueStatement node)
		{
			return;
		}

		public virtual void VisitDefaultCase(DefaultCase node)
		{
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
			return;
		}

		public virtual void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			this.Visit(node.get_Target());
			this.Visit(node.get_MethodExpression());
			return;
		}

		public virtual void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			this.Visit(node.get_Target());
			this.Visit(node.get_Arguments());
			return;
		}

		public virtual void VisitDoWhileStatement(DoWhileStatement node)
		{
			this.Visit(node.get_Body());
			this.Visit(node.get_Condition());
			return;
		}

		public virtual void VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
		{
			this.Visit(node.get_Arguments());
			return;
		}

		public virtual void VisitDynamicIndexerExpression(DynamicIndexerExpression node)
		{
			this.VisitIIndexerExpression(node);
			return;
		}

		public virtual void VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
		{
			this.Visit(node.get_Target());
			if (node.get_IsMethodInvocation())
			{
				this.Visit(node.get_InvocationArguments());
			}
			return;
		}

		public virtual void VisitEmptyStatement(EmptyStatement node)
		{
			return;
		}

		public virtual void VisitEnumExpression(EnumExpression node)
		{
			return;
		}

		public virtual void VisitEventReferenceExpression(EventReferenceExpression node)
		{
			this.Visit(node.get_Target());
			return;
		}

		public virtual void VisitExceptionStatement(ExceptionStatement node)
		{
			return;
		}

		public virtual void VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitExpressionStatement(ExpressionStatement node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
			return;
		}

		public virtual void VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			this.Visit(node.get_Target());
			return;
		}

		public void VisitFinallyClause(FinallyClause node)
		{
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitFixedStatement(FixedStatement node)
		{
			this.Visit(node.get_Expression());
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitForEachStatement(ForEachStatement node)
		{
			this.Visit(node.get_Variable());
			this.Visit(node.get_Collection());
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitForStatement(ForStatement node)
		{
			this.Visit(node.get_Initializer());
			this.Visit(node.get_Condition());
			this.Visit(node.get_Increment());
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitFromClause(FromClause node)
		{
			this.Visit(node.get_Identifier());
			this.Visit(node.get_Collection());
			return;
		}

		public virtual void VisitGotoStatement(GotoStatement node)
		{
			return;
		}

		public virtual void VisitGroupClause(GroupClause node)
		{
			this.Visit(node.get_Expression());
			this.Visit(node.get_GroupKey());
			return;
		}

		public virtual void VisitIfElseIfStatement(IfElseIfStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_ConditionBlocks().get_Count())
			{
				V_1 = node.get_ConditionBlocks().get_Item(V_0);
				this.Visit(V_1.get_Key());
				this.Visit(V_1.get_Value());
				V_0 = V_0 + 1;
			}
			this.Visit(node.get_Else());
			return;
		}

		public virtual void VisitIfStatement(IfStatement node)
		{
			this.Visit(node.get_Condition());
			this.Visit(node.get_Then());
			this.Visit(node.get_Else());
			return;
		}

		protected virtual void VisitIIndexerExpression(IIndexerExpression node)
		{
			this.Visit(node.get_Target());
			this.Visit(node.get_Indices());
			return;
		}

		public virtual void VisitImplicitCastExpression(ImplicitCastExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitInitializerExpression(InitializerExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitIntoClause(IntoClause node)
		{
			this.Visit(node.get_Identifier());
			return;
		}

		public virtual void VisitJoinClause(JoinClause node)
		{
			this.Visit(node.get_InnerIdentifier());
			this.Visit(node.get_InnerCollection());
			this.Visit(node.get_OuterKey());
			this.Visit(node.get_InnerKey());
			return;
		}

		public virtual void VisitLambdaExpression(LambdaExpression node)
		{
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitLambdaParameterExpression(LambdaParameterExpression node)
		{
			return;
		}

		public virtual void VisitLetClause(LetClause node)
		{
			this.Visit(node.get_Identifier());
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitLinqQueryExpression(LinqQueryExpression node)
		{
			V_0 = node.get_Clauses().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.Visit(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public virtual void VisitLiteralExpression(LiteralExpression node)
		{
			return;
		}

		public virtual void VisitLockStatement(LockStatement node)
		{
			this.Visit(node.get_Expression());
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitMakeRefExpression(MakeRefExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitMemberHandleExpression(MemberHandleExpression node)
		{
			return;
		}

		public virtual void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.Visit(node.get_MethodExpression());
			this.Visit(node.get_Arguments());
			return;
		}

		public virtual void VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			this.Visit(node.get_Target());
			return;
		}

		public virtual void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			this.Visit(node.get_Arguments());
			this.Visit(node.get_Initializer());
			return;
		}

		public virtual void VisitOrderByClause(OrderByClause node)
		{
			V_0 = node.get_ExpressionToOrderDirectionMap().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.Visit(V_1.get_Key());
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public virtual void VisitParenthesesExpression(ParenthesesExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitPropertyInitializerExpression(PropertyInitializerExpression node)
		{
			return;
		}

		public virtual void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			this.Visit(node.get_Target());
			this.Visit(node.get_Arguments());
			return;
		}

		public virtual void VisitRaiseEventExpression(RaiseEventExpression node)
		{
			this.Visit(node.get_Arguments());
			return;
		}

		public virtual void VisitRefReturnExpression(RefReturnExpression node)
		{
			this.Visit(node.get_Value());
			return;
		}

		public virtual void VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
		{
			return;
		}

		public virtual void VisitReturnExpression(ReturnExpression node)
		{
			this.Visit(node.get_Value());
			return;
		}

		public virtual void VisitSafeCastExpression(SafeCastExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitSelectClause(SelectClause node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitShortFormReturnExpression(ShortFormReturnExpression node)
		{
			this.Visit(node.get_Value());
			return;
		}

		public virtual void VisitSizeOfExpression(SizeOfExpression node)
		{
			return;
		}

		public virtual void VisitStackAllocExpression(StackAllocExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitSwitchStatement(SwitchStatement node)
		{
			this.Visit(node.get_Condition());
			this.Visit(node.get_Cases());
			return;
		}

		public virtual void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.Visit(node.get_Arguments());
			return;
		}

		public virtual void VisitThisReferenceExpression(ThisReferenceExpression node)
		{
			return;
		}

		public virtual void VisitThrowExpression(ThrowExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}

		public virtual void VisitTryStatement(TryStatement node)
		{
			this.Visit(node.get_Try());
			this.Visit(node.get_CatchClauses());
			this.Visit(node.get_Fault());
			this.Visit(node.get_Finally());
			return;
		}

		public virtual void VisitTypeOfExpression(TypeOfExpression node)
		{
			return;
		}

		public virtual void VisitTypeReferenceExpression(TypeReferenceExpression node)
		{
			return;
		}

		public virtual void VisitUnaryExpression(UnaryExpression node)
		{
			this.Visit(node.get_Operand());
			return;
		}

		public virtual void VisitUnsafeBlockStatement(UnsafeBlockStatement unsafeBlock)
		{
			this.Visit(unsafeBlock.get_Statements());
			return;
		}

		public virtual void VisitUsingStatement(UsingStatement node)
		{
			this.Visit(node.get_Expression());
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			return;
		}

		public virtual void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			return;
		}

		public virtual void VisitWhereClause(WhereClause node)
		{
			this.Visit(node.get_Condition());
			return;
		}

		public virtual void VisitWhileStatement(WhileStatement node)
		{
			this.Visit(node.get_Condition());
			this.Visit(node.get_Body());
			return;
		}

		public virtual void VisitYieldBreakExpression(YieldBreakExpression node)
		{
			return;
		}

		public virtual void VisitYieldReturnExpression(YieldReturnExpression node)
		{
			this.Visit(node.get_Expression());
			return;
		}
	}
}