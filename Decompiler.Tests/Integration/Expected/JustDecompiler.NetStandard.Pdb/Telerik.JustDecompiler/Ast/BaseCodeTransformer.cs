using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Ast
{
	public class BaseCodeTransformer : ICodeTransformer
	{
		private long visitsOnStack;

		public BaseCodeTransformer()
		{
			base();
			return;
		}

		private ICodeNode DoVisit(ICodeNode node)
		{
			if (node == null)
			{
				return null;
			}
			switch (node.get_CodeNodeType())
			{
				case 0:
				{
					return this.VisitBlockStatement((BlockStatement)node);
				}
				case 1:
				{
				Label0:
					throw new ArgumentException();
				}
				case 2:
				{
					return this.VisitGotoStatement((GotoStatement)node);
				}
				case 3:
				{
					return this.VisitIfStatement((IfStatement)node);
				}
				case 4:
				{
					return this.VisitIfElseIfStatement((IfElseIfStatement)node);
				}
				case 5:
				{
					return this.VisitExpressionStatement((ExpressionStatement)node);
				}
				case 6:
				{
					return this.VisitThrowExpression((ThrowExpression)node);
				}
				case 7:
				{
					return this.VisitWhileStatement((WhileStatement)node);
				}
				case 8:
				{
					return this.VisitDoWhileStatement((DoWhileStatement)node);
				}
				case 9:
				{
					return this.VisitBreakStatement((BreakStatement)node);
				}
				case 10:
				{
					return this.VisitContinueStatement((ContinueStatement)node);
				}
				case 11:
				{
					return this.VisitForStatement((ForStatement)node);
				}
				case 12:
				{
					return this.VisitForEachStatement((ForEachStatement)node);
				}
				case 13:
				{
					return this.VisitConditionCase((ConditionCase)node);
				}
				case 14:
				{
					return this.VisitDefaultCase((DefaultCase)node);
				}
				case 15:
				{
					return this.VisitSwitchStatement((SwitchStatement)node);
				}
				case 16:
				{
					return this.VisitCatchClause((CatchClause)node);
				}
				case 17:
				{
					return this.VisitTryStatement((TryStatement)node);
				}
				case 18:
				{
					return this.VisitBlockExpression((BlockExpression)node);
				}
				case 19:
				{
					return this.VisitMethodInvocationExpression((MethodInvocationExpression)node);
				}
				case 20:
				{
					return this.VisitMethodReferenceExpression((MethodReferenceExpression)node);
				}
				case 21:
				{
					return this.VisitDelegateCreationExpression((DelegateCreationExpression)node);
				}
				case 22:
				{
					return this.VisitLiteralExpression((LiteralExpression)node);
				}
				case 23:
				{
					return this.VisitUnaryExpression((UnaryExpression)node);
				}
				case 24:
				{
					return this.VisitBinaryExpression((BinaryExpression)node);
				}
				case 25:
				{
					return this.VisitArgumentReferenceExpression((ArgumentReferenceExpression)node);
				}
				case 26:
				{
					return this.VisitVariableReferenceExpression((VariableReferenceExpression)node);
				}
				case 27:
				{
					return this.VisitVariableDeclarationExpression((VariableDeclarationExpression)node);
				}
				case 28:
				{
					return this.VisitThisReferenceExpression((ThisReferenceExpression)node);
				}
				case 29:
				{
					return this.VisitBaseReferenceExpression((BaseReferenceExpression)node);
				}
				case 30:
				{
					return this.VisitFieldReferenceExpression((FieldReferenceExpression)node);
				}
				case 31:
				{
					return this.VisitExplicitCastExpression((ExplicitCastExpression)node);
				}
				case 32:
				{
					return this.VisitImplicitCastExpression((ImplicitCastExpression)node);
				}
				case 33:
				{
					return this.VisitSafeCastExpression((SafeCastExpression)node);
				}
				case 34:
				{
					return this.VisitCanCastExpression((CanCastExpression)node);
				}
				case 35:
				{
					return this.VisitTypeOfExpression((TypeOfExpression)node);
				}
				case 36:
				{
					return this.VisitConditionExpression((ConditionExpression)node);
				}
				case 37:
				{
					return this.VisitFixedStatement((FixedStatement)node);
				}
				case 38:
				{
					return this.VisitArrayCreationExpression((ArrayCreationExpression)node);
				}
				case 39:
				{
					return this.VisitArrayIndexerExpression((ArrayIndexerExpression)node);
				}
				case 40:
				{
					return this.VisitObjectCreationExpression((ObjectCreationExpression)node);
				}
				case 41:
				{
					return this.VisitDefaultObjectExpression((DefaultObjectExpression)node);
				}
				case 42:
				{
					return this.VisitPropertyReferenceExpression((PropertyReferenceExpression)node);
				}
				case 43:
				{
					return this.VisitTypeReferenceExpression((TypeReferenceExpression)node);
				}
				case 44:
				{
					return this.VisitUsingStatement((UsingStatement)node);
				}
				case 45:
				{
					return this.VisitStackAllocExpression((StackAllocExpression)node);
				}
				case 46:
				{
					return this.VisitSizeOfExpression((SizeOfExpression)node);
				}
				case 47:
				{
					return this.VisitMakeRefExpression((MakeRefExpression)node);
				}
				case 48:
				{
					return this.VisitEventReferenceExpression((EventReferenceExpression)node);
				}
				case 49:
				{
					return this.VisitEnumExpression((EnumExpression)node);
				}
				case 50:
				{
					return this.VisitLambdaExpression((LambdaExpression)node);
				}
				case 51:
				{
					return this.VisitDelegateInvokeExpression((DelegateInvokeExpression)node);
				}
				case 52:
				{
					return this.VisitBaseCtorExpression((BaseCtorExpression)node);
				}
				case 53:
				{
					return this.VisitThisCtorExpression((ThisCtorExpression)node);
				}
				case 54:
				{
					return this.VisitYieldReturnExpression((YieldReturnExpression)node);
				}
				case 55:
				{
					return this.VisitYieldBreakExpression((YieldBreakExpression)node);
				}
				case 56:
				{
					return this.VisitLockStatement((LockStatement)node);
				}
				case 57:
				{
					return this.VisitReturnExpression((ReturnExpression)node);
				}
				case 58:
				{
					return this.VisitEmptyStatement((EmptyStatement)node);
				}
				case 59:
				{
					return this.VisitDynamicMemberReferenceExpression((DynamicMemberReferenceExpression)node);
				}
				case 60:
				{
					return this.VisitDynamicConstructorInvocationExpression((DynamicConstructorInvocationExpression)node);
				}
				case 61:
				{
					return this.VisitDynamicIndexerExpression((DynamicIndexerExpression)node);
				}
				case 62:
				{
					return this.VisitBoxExpression((BoxExpression)node);
				}
				case 63:
				{
					return this.VisitAnonymousPropertyInitializerExpression((AnonymousPropertyInitializerExpression)node);
				}
				case 64:
				{
					return this.VisitLambdaParameterExpression((LambdaParameterExpression)node);
				}
				case 65:
				{
					return this.VisitAwaitExpression((AwaitExpression)node);
				}
				case 66:
				{
					return this.VisitArrayLengthExpression((ArrayLengthExpression)node);
				}
				case 67:
				{
					return this.VisitExceptionStatement((ExceptionStatement)node);
				}
				case 68:
				{
					return this.VisitBreakSwitchCaseStatement((BreakSwitchCaseStatement)node);
				}
				case 69:
				{
					return this.VisitCaseGotoStatement((CaseGotoStatement)node);
				}
				case 70:
				{
					return this.VisitFinallyClause((FinallyClause)node);
				}
				case 71:
				{
					return this.VisitShortFormReturnExpression((ShortFormReturnExpression)node);
				}
				case 72:
				{
					return this.VisitAnonymousObjectCreationExpression((AnonymousObjectCreationExpression)node);
				}
				case 73:
				{
					return this.VisitFromClause((FromClause)node);
				}
				case 74:
				{
					return this.VisitSelectClause((SelectClause)node);
				}
				case 75:
				{
					return this.VisitWhereClause((WhereClause)node);
				}
				case 76:
				{
					return this.VisitGroupClause((GroupClause)node);
				}
				case 77:
				{
					return this.VisitOrderByClause((OrderByClause)node);
				}
				case 78:
				{
					return this.VisitJoinClause((JoinClause)node);
				}
				case 79:
				{
					return this.VisitLetClause((LetClause)node);
				}
				case 80:
				{
					return this.VisitIntoClause((IntoClause)node);
				}
				case 81:
				{
					return this.VisitLinqQueryExpression((LinqQueryExpression)node);
				}
				case 82:
				{
					return this.VisitArrayVariableDeclarationExpression((ArrayVariableDeclarationExpression)node);
				}
				case 83:
				{
					return this.VisitArrayAssignmentVariableReferenceExpression((ArrayVariableReferenceExpression)node);
				}
				case 84:
				{
					return this.VisitArrayAssignmentFieldReferenceExpression((ArrayAssignmentFieldReferenceExpression)node);
				}
				case 85:
				{
					return this.VisitPropertyInitializerExpression((PropertyInitializerExpression)node);
				}
				case 86:
				{
					return this.VisitFieldInitializerExpression((FieldInitializerExpression)node);
				}
				case 87:
				{
					return this.VisitParenthesesExpression((ParenthesesExpression)node);
				}
				case 88:
				{
					return this.VisitInitializerExpression((InitializerExpression)node);
				}
				case 89:
				{
					return this.VisitCheckedExpression((CheckedExpression)node);
				}
				case 90:
				{
					return this.VisitMemberHandleExpression((MemberHandleExpression)node);
				}
				case 91:
				{
					return this.VisitAutoPropertyConstructorInitializerExpression((AutoPropertyConstructorInitializerExpression)node);
				}
				case 92:
				{
					return this.VisitRaiseEventExpression((RaiseEventExpression)node);
				}
				case 93:
				{
					return this.VisitRefVariableDeclarationExpression((RefVariableDeclarationExpression)node);
				}
				case 94:
				{
					return this.VisitRefReturnExpression((RefReturnExpression)node);
				}
				default:
				{
					goto Label0;
				}
			}
		}

		public virtual ICodeNode Visit(ICodeNode node)
		{
			this.visitsOnStack = this.visitsOnStack + (long)1;
			if (this.visitsOnStack == (long)0x24e)
			{
				this.visitsOnStack = (long)0;
				throw new Exception("Stack overflow while traversing code tree in transform.");
			}
			stackVariable12 = this.DoVisit(node);
			this.visitsOnStack = this.visitsOnStack - (long)1;
			return stackVariable12;
		}

		protected virtual TCollection Visit<TCollection, TElement>(TCollection original)
		where TCollection : class, IList<TElement>, new()
		where TElement : class, ICodeNode
		{
			V_0 = default(TCollection);
			V_1 = 0;
			while (V_1 < original.get_Count())
			{
				V_2 = (TElement)this.Visit(original.get_Item(V_1));
				if (V_0 == null)
				{
					if ((object)V_2 != (object)original.get_Item(V_1))
					{
						V_0 = Activator.CreateInstance<TCollection>();
						V_3 = 0;
						while (V_3 < V_1)
						{
							V_0.Add(original.get_Item(V_3));
							V_3 = V_3 + 1;
						}
						if (V_2 != null)
						{
							V_0.Add(V_2);
						}
					}
				}
				else
				{
					if (V_2 != null)
					{
						V_0.Add(V_2);
					}
				}
				V_1 = V_1 + 1;
			}
			stackVariable49 = V_0;
			if (stackVariable49 == null)
			{
				dummyVar0 = stackVariable49;
				stackVariable49 = original;
			}
			return stackVariable49;
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
			node.set_Initializer((InitializerExpression)this.Visit(node.get_Initializer()));
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
			node.set_Field((FieldReferenceExpression)this.Visit(node.get_Field()));
			node.set_Dimensions((ExpressionCollection)this.Visit(node.get_Dimensions()));
			return node;
		}

		public virtual ICodeNode VisitArrayAssignmentVariableReferenceExpression(ArrayVariableReferenceExpression node)
		{
			node.set_Variable((VariableReferenceExpression)this.Visit(node.get_Variable()));
			node.set_Dimensions((ExpressionCollection)this.Visit(node.get_Dimensions()));
			return node;
		}

		public virtual ICodeNode VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			node.set_Dimensions((ExpressionCollection)this.Visit(node.get_Dimensions()));
			node.set_Initializer((InitializerExpression)this.Visit(node.get_Initializer()));
			return node;
		}

		public virtual ICodeNode VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			return this.VisitIIndexerExpression(node);
		}

		public virtual ICodeNode VisitArrayLengthExpression(ArrayLengthExpression node)
		{
			node.set_Target((Expression)this.Visit(node.get_Target()));
			return node;
		}

		public virtual ICodeNode VisitArrayVariableDeclarationExpression(ArrayVariableDeclarationExpression node)
		{
			node.set_Variable((VariableDeclarationExpression)this.Visit(node.get_Variable()));
			node.set_Dimensions((ExpressionCollection)this.Visit(node.get_Dimensions()));
			return node;
		}

		public virtual ICodeNode VisitAutoPropertyConstructorInitializerExpression(AutoPropertyConstructorInitializerExpression node)
		{
			return node;
		}

		public ICodeNode VisitAwaitExpression(AwaitExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
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
			node.set_Left((Expression)this.Visit(node.get_Left()));
			node.set_Right((Expression)this.Visit(node.get_Right()));
			return node;
		}

		public virtual ICodeNode VisitBlockExpression(BlockExpression node)
		{
			node.set_Expressions((ExpressionCollection)this.Visit(node.get_Expressions()));
			return node;
		}

		public virtual ICodeNode VisitBlockStatement(BlockStatement node)
		{
			node.set_Statements((StatementCollection)this.Visit(node.get_Statements()));
			return node;
		}

		public virtual ICodeNode VisitBoxExpression(BoxExpression node)
		{
			node.set_BoxedExpression((Expression)this.Visit(node.get_BoxedExpression()));
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
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitCaseGotoStatement(CaseGotoStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitCatchClause(CatchClause node)
		{
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			node.set_Variable((VariableDeclarationExpression)this.Visit(node.get_Variable()));
			node.set_Filter((Statement)this.Visit(node.get_Filter()));
			return node;
		}

		public virtual ICodeNode VisitCheckedExpression(CheckedExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitConditionCase(ConditionCase node)
		{
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public virtual ICodeNode VisitConditionExpression(ConditionExpression node)
		{
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			node.set_Then((Expression)this.Visit(node.get_Then()));
			node.set_Else((Expression)this.Visit(node.get_Else()));
			return node;
		}

		public virtual ICodeNode VisitContinueStatement(ContinueStatement node)
		{
			return node;
		}

		private ICodeNode VisitCtorExpression(MethodInvocationExpression node)
		{
			node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
			return node;
		}

		public virtual ICodeNode VisitDefaultCase(DefaultCase node)
		{
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public virtual ICodeNode VisitDefaultObjectExpression(DefaultObjectExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			node.set_Target((Expression)this.Visit(node.get_Target()));
			node.set_MethodExpression((Expression)this.Visit(node.get_MethodExpression()));
			return node;
		}

		public virtual ICodeNode VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			node.set_Target((Expression)this.Visit(node.get_Target()));
			node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
			return node;
		}

		public virtual ICodeNode VisitDoWhileStatement(DoWhileStatement node)
		{
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			return node;
		}

		public virtual ICodeNode VisitDynamicConstructorInvocationExpression(DynamicConstructorInvocationExpression node)
		{
			node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
			return node;
		}

		public virtual ICodeNode VisitDynamicIndexerExpression(DynamicIndexerExpression node)
		{
			return this.VisitIIndexerExpression(node);
		}

		public virtual ICodeNode VisitDynamicMemberReferenceExpression(DynamicMemberReferenceExpression node)
		{
			node.set_Target((Expression)this.Visit(node.get_Target()));
			if (node.get_IsMethodInvocation())
			{
				node.set_InvocationArguments((ExpressionCollection)this.Visit(node.get_InvocationArguments()));
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
			node.set_Target((Expression)this.Visit(node.get_Target()));
			return node;
		}

		public virtual ICodeNode VisitExceptionStatement(ExceptionStatement node)
		{
			return node;
		}

		public virtual ICodeNode VisitExplicitCastExpression(ExplicitCastExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public ICodeNode VisitFieldInitializerExpression(FieldInitializerExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			node.set_Target((Expression)this.Visit(node.get_Target()));
			return node;
		}

		public ICodeNode VisitFinallyClause(FinallyClause node)
		{
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public virtual ICodeNode VisitFixedStatement(FixedStatement node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public virtual ICodeNode VisitForEachStatement(ForEachStatement node)
		{
			node.set_Variable((VariableDeclarationExpression)this.Visit(node.get_Variable()));
			node.set_Collection((Expression)this.Visit(node.get_Collection()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public virtual ICodeNode VisitForStatement(ForStatement node)
		{
			node.set_Initializer((Expression)this.Visit(node.get_Initializer()));
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			node.set_Increment((Expression)this.Visit(node.get_Increment()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public ICodeNode VisitFromClause(FromClause node)
		{
			node.set_Identifier((Expression)this.Visit(node.get_Identifier()));
			node.set_Collection((Expression)this.Visit(node.get_Collection()));
			return node;
		}

		public virtual ICodeNode VisitGotoStatement(GotoStatement node)
		{
			return node;
		}

		public ICodeNode VisitGroupClause(GroupClause node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			node.set_GroupKey((Expression)this.Visit(node.get_GroupKey()));
			return node;
		}

		public virtual ICodeNode VisitIfElseIfStatement(IfElseIfStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_ConditionBlocks().get_Count())
			{
				V_3 = node.get_ConditionBlocks().get_Item(V_0);
				V_1 = (Expression)this.Visit(V_3.get_Key());
				V_3 = node.get_ConditionBlocks().get_Item(V_0);
				V_2 = (BlockStatement)this.Visit(V_3.get_Value());
				V_2.set_Parent(node);
				node.get_ConditionBlocks().set_Item(V_0, new KeyValuePair<Expression, BlockStatement>(V_1, V_2));
				V_0 = V_0 + 1;
			}
			node.set_Else((BlockStatement)this.Visit(node.get_Else()));
			return node;
		}

		public virtual ICodeNode VisitIfStatement(IfStatement node)
		{
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			node.set_Then((BlockStatement)this.Visit(node.get_Then()));
			node.set_Else((BlockStatement)this.Visit(node.get_Else()));
			return node;
		}

		private ICodeNode VisitIIndexerExpression(IIndexerExpression node)
		{
			node.set_Target((Expression)this.Visit(node.get_Target()));
			node.set_Indices((ExpressionCollection)this.Visit(node.get_Indices()));
			return (ICodeNode)node;
		}

		public virtual ICodeNode VisitImplicitCastExpression(ImplicitCastExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitInitializerExpression(InitializerExpression node)
		{
			node.set_Expression((BlockExpression)this.Visit(node.get_Expression()));
			return node;
		}

		public ICodeNode VisitIntoClause(IntoClause node)
		{
			node.set_Identifier((VariableReferenceExpression)this.Visit(node.get_Identifier()));
			return node;
		}

		public ICodeNode VisitJoinClause(JoinClause node)
		{
			node.set_InnerIdentifier((Expression)this.Visit(node.get_InnerIdentifier()));
			node.set_InnerCollection((Expression)this.Visit(node.get_InnerCollection()));
			node.set_OuterKey((Expression)this.Visit(node.get_OuterKey()));
			node.set_InnerKey((Expression)this.Visit(node.get_InnerKey()));
			return node;
		}

		public virtual ICodeNode VisitLambdaExpression(LambdaExpression node)
		{
			node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public ICodeNode VisitLambdaParameterExpression(LambdaParameterExpression node)
		{
			return node;
		}

		public ICodeNode VisitLetClause(LetClause node)
		{
			node.set_Identifier((VariableReferenceExpression)this.Visit(node.get_Identifier()));
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitLinqQueryExpression(LinqQueryExpression node)
		{
			V_0 = 0;
			while (V_0 < node.get_Clauses().get_Count())
			{
				node.get_Clauses().set_Item(V_0, (QueryClause)this.Visit(node.get_Clauses().get_Item(V_0)));
				V_0 = V_0 + 1;
			}
			return node;
		}

		public virtual ICodeNode VisitLiteralExpression(LiteralExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitLockStatement(LockStatement node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public virtual ICodeNode VisitMakeRefExpression(MakeRefExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitMemberHandleExpression(MemberHandleExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			node.set_MethodExpression((MethodReferenceExpression)this.Visit(node.get_MethodExpression()));
			node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
			return node;
		}

		public virtual ICodeNode VisitMethodReferenceExpression(MethodReferenceExpression node)
		{
			node.set_Target((Expression)this.Visit(node.get_Target()));
			return node;
		}

		public virtual ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
			node.set_Initializer((InitializerExpression)this.Visit(node.get_Initializer()));
			return node;
		}

		public ICodeNode VisitOrderByClause(OrderByClause node)
		{
			V_0 = 0;
			while (V_0 < node.get_ExpressionToOrderDirectionMap().get_Count())
			{
				V_1 = node.get_ExpressionToOrderDirectionMap().get_Item(V_0);
				node.get_ExpressionToOrderDirectionMap().set_Item(V_0, new KeyValuePair<Expression, OrderDirection>((Expression)this.Visit(V_1.get_Key()), V_1.get_Value()));
				V_0 = V_0 + 1;
			}
			return node;
		}

		public virtual ICodeNode VisitParenthesesExpression(ParenthesesExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public ICodeNode VisitPropertyInitializerExpression(PropertyInitializerExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			node.set_Target((Expression)this.Visit(node.get_Target()));
			node.set_Arguments((ExpressionCollection)this.Visit(node.get_Arguments()));
			return node;
		}

		public virtual ICodeNode VisitRaiseEventExpression(RaiseEventExpression node)
		{
			dummyVar0 = this.Visit(node.get_Arguments());
			return node;
		}

		public virtual ICodeNode VisitRefReturnExpression(RefReturnExpression node)
		{
			node.set_Value((Expression)this.Visit(node.get_Value()));
			return node;
		}

		public virtual ICodeNode VisitRefVariableDeclarationExpression(RefVariableDeclarationExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitReturnExpression(ReturnExpression node)
		{
			node.set_Value((Expression)this.Visit(node.get_Value()));
			return node;
		}

		public virtual ICodeNode VisitSafeCastExpression(SafeCastExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public ICodeNode VisitSelectClause(SelectClause node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitShortFormReturnExpression(ShortFormReturnExpression node)
		{
			node.set_Value((Expression)this.Visit(node.get_Value()));
			return node;
		}

		public virtual ICodeNode VisitSizeOfExpression(SizeOfExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitStackAllocExpression(StackAllocExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitSwitchStatement(SwitchStatement node)
		{
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			V_0 = new SwitchCaseCollection();
			V_1 = node.get_Cases().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.Add(V_2);
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			node.set_Cases((SwitchCaseCollection)this.Visit(V_0));
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
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}

		public virtual ICodeNode VisitTryStatement(TryStatement node)
		{
			node.set_Try((BlockStatement)this.Visit(node.get_Try()));
			node.set_CatchClauses((CatchClauseCollection)this.Visit(node.get_CatchClauses()));
			node.set_Fault((BlockStatement)this.Visit(node.get_Fault()));
			node.set_Finally((FinallyClause)this.Visit(node.get_Finally()));
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
			node.set_Operand((Expression)this.Visit(node.get_Operand()));
			return node;
		}

		public virtual ICodeNode VisitUsingStatement(UsingStatement node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
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
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			return node;
		}

		public virtual ICodeNode VisitWhileStatement(WhileStatement node)
		{
			node.set_Condition((Expression)this.Visit(node.get_Condition()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public virtual ICodeNode VisitYieldBreakExpression(YieldBreakExpression node)
		{
			return node;
		}

		public virtual ICodeNode VisitYieldReturnExpression(YieldReturnExpression node)
		{
			node.set_Expression((Expression)this.Visit(node.get_Expression()));
			return node;
		}
	}
}