using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildForeachStatements : BaseCodeVisitor, IDecompilationStep
	{
		private bool insideTry;

		private bool foundEnumeratorAssignment;

		private bool foundWhile;

		private Expression foreachCollection;

		private VariableDefinition foreachVariable;

		private BlockStatement foreachBody;

		private VariableReference theEnumerator;

		private TryStatement theTry;

		private ForEachStatement foreach;

		private TypeReference foreachVariableType;

		private ExpressionStatement enumeratorAssignmentStatement;

		private bool shouldAdd;

		private bool isEnumeratorUsedInsideForEach;

		private MethodSpecificContext methodContext;

		private readonly HashSet<VariableDefinition> foreachVariables;

		private readonly List<Instruction> foreachVariableInstructions;

		private readonly List<Instruction> foreachCollectionInstructions;

		private IEnumerable<Instruction> foreachConditionInstructions;

		public RebuildForeachStatements()
		{
			this.foreachVariables = new HashSet<VariableDefinition>();
			this.foreachVariableInstructions = new List<Instruction>();
			this.foreachCollectionInstructions = new List<Instruction>();
			base();
			this.insideTry = false;
			this.foundEnumeratorAssignment = false;
			this.foundWhile = false;
			return;
		}

		private void AttachForeach()
		{
			this.GenerateForeachStatement();
			if (!this.isEnumeratorUsedInsideForEach)
			{
				V_0 = this.theTry.get_Parent() as BlockStatement;
				dummyVar0 = V_0.get_Statements().Remove(this.enumeratorAssignmentStatement);
				V_1 = V_0.get_Statements().IndexOf(this.theTry);
				V_0.get_Statements().RemoveAt(V_1);
				V_0.AddStatementAt(V_1, this.foreach);
				(new RebuildForeachStatements.YieldStateMachineCodeRemover(this.foreach, this.theEnumerator)).ProcessForEachStatement();
				this.CopyLabel();
				this.CheckVariable();
				this.ClearState();
				this.VisitForEachStatement(V_0.get_Statements().get_Item(V_1) as ForEachStatement);
			}
			return;
		}

		private bool CanContainForeach(TryStatement tryStatement)
		{
			if (tryStatement.get_CatchClauses().get_Count() != 0 || tryStatement.get_Try().get_Statements().get_Count() != 1 || tryStatement.get_Finally() == null || tryStatement.get_Finally().get_Body().get_Statements().get_Count() != 1 && tryStatement.get_Finally().get_Body().get_Statements().get_Count() != 2)
			{
				return false;
			}
			return this.IsValidFinally(tryStatement.get_Finally().get_Body());
		}

		private void CheckVariable()
		{
			if (this.foreachVariables.Contains(this.foreachVariable))
			{
				stackVariable11 = this.foreachVariable;
				this.foreachVariable = new VariableDefinition(this.foreachVariable.get_VariableType(), this.methodContext.get_Method());
				this.foreachVariableInstructions.Clear();
				this.methodContext.get_Variables().Add(this.foreachVariable);
				dummyVar0 = this.methodContext.get_VariablesToRename().Add(this.foreachVariable);
				(new RebuildForeachStatements.ForeachVariableChanger(stackVariable11, this.foreachVariable)).Visit(this.foreach);
			}
			dummyVar1 = this.foreachVariables.Add(this.foreachVariable);
			return;
		}

		private void ClearState()
		{
			this.insideTry = false;
			this.foundEnumeratorAssignment = false;
			this.foundWhile = false;
			this.foreachCollection = null;
			this.foreachVariable = null;
			this.foreachVariableInstructions.Clear();
			this.foreachCollectionInstructions.Clear();
			this.foreachBody = new BlockStatement();
			this.theEnumerator = null;
			this.theTry = null;
			this.foreach = null;
			this.enumeratorAssignmentStatement = null;
			this.foreachVariableType = null;
			this.isEnumeratorUsedInsideForEach = false;
			this.foreachConditionInstructions = null;
			return;
		}

		private void CopyLabel()
		{
			if (String.op_Inequality(this.theTry.get_Label(), String.Empty))
			{
				this.foreach.set_Label(this.theTry.get_Label());
				return;
			}
			if (String.op_Inequality(this.theTry.get_Try().get_Label(), String.Empty))
			{
				this.foreach.set_Label(this.theTry.get_Try().get_Label());
				return;
			}
			if (String.op_Inequality(this.theTry.get_Try().get_Statements().get_Item(0).get_Label(), String.Empty))
			{
				this.foreach.set_Label(this.theTry.get_Try().get_Statements().get_Item(0).get_Label());
			}
			return;
		}

		private void GenerateForeachStatement()
		{
			if (this.foreachVariable == null)
			{
				this.foreachVariable = new VariableDefinition(this.foreachVariableType, this.methodContext.get_Method());
				this.foreachVariableInstructions.Clear();
				dummyVar0 = this.methodContext.get_VariablesToRename().Add(this.foreachVariable);
			}
			V_0 = new VariableDeclarationExpression(this.foreachVariable, this.foreachVariableInstructions);
			V_1 = this.foreachCollection.CloneAndAttachInstructions(this.foreachCollectionInstructions);
			if (V_1 as BaseReferenceExpression != null)
			{
				V_1 = new ThisReferenceExpression(this.methodContext.get_Method().get_DeclaringType(), V_1.get_UnderlyingSameMethodInstructions());
			}
			this.foreach = new ForEachStatement(V_0, V_1, this.foreachBody, this.foreachConditionInstructions, this.theTry.get_Finally().get_UnderlyingSameMethodInstructions());
			dummyVar1 = (new RebuildForeachStatements.GetCurrentFixer(this.theEnumerator, this.foreachVariable)).Visit(this.foreach);
			V_2 = new RebuildForeachStatements.IsEnumeratorUsedVisitor(this.theEnumerator);
			V_2.Visit(this.foreach);
			this.isEnumeratorUsedInsideForEach = V_2.get_IsEnumeratorUsed();
			return;
		}

		private bool IsEnumeratorAssignment(Expression expression)
		{
			V_0 = expression as BinaryExpression;
			if (V_0 == null || !V_0.get_IsAssignmentExpression())
			{
				return false;
			}
			V_1 = V_0.get_Right();
			if (V_1 as MethodInvocationExpression != null)
			{
				V_2 = V_1 as MethodInvocationExpression;
				if (this.IsGetEnumerator(V_2))
				{
					if (V_0.get_Left() as VariableReferenceExpression == null)
					{
						return false;
					}
					this.foreachCollectionInstructions.Clear();
					this.foreachCollectionInstructions.AddRange(V_0.get_Left().get_UnderlyingSameMethodInstructions());
					this.foreachCollectionInstructions.AddRange(V_0.get_MappedInstructions());
					this.foreachCollectionInstructions.AddRange(V_2.get_InvocationInstructions());
					this.theEnumerator = (V_0.get_Left() as VariableReferenceExpression).get_Variable();
					return true;
				}
			}
			return false;
		}

		private bool IsEnumeratorDispose(MethodInvocationExpression methodInvocationExpression)
		{
			if (methodInvocationExpression == null)
			{
				return false;
			}
			if (String.op_Equality(methodInvocationExpression.get_MethodExpression().get_Method().get_Name(), "Dispose"))
			{
				return true;
			}
			return false;
		}

		private bool IsForeach(WhileStatement node)
		{
			if (node.get_Condition() as UnaryExpression != null)
			{
				V_0 = node.get_Condition() as UnaryExpression;
				if (V_0.get_Operator() == 11 && V_0.get_Operand() as MethodInvocationExpression != null)
				{
					return this.IsMoveNextCall(V_0.get_Operand() as MethodInvocationExpression);
				}
			}
			return false;
		}

		private bool IsForeachVariableAssignment(BinaryExpression assignment)
		{
			if (assignment == null)
			{
				return false;
			}
			if (assignment.get_Right() as ExplicitCastExpression == null)
			{
				if (assignment.get_Right() as MethodInvocationExpression == null && assignment.get_Right() as PropertyReferenceExpression == null)
				{
					return false;
				}
				V_0 = assignment.get_Right();
			}
			else
			{
				V_0 = (assignment.get_Right() as ExplicitCastExpression).get_Expression();
			}
			if (!this.IsGetCurrent(V_0))
			{
				return false;
			}
			V_1 = assignment.get_Left() as VariableReferenceExpression;
			if (V_1 == null)
			{
				return false;
			}
			this.foreachVariable = V_1.get_Variable().Resolve();
			this.foreachVariableInstructions.AddRange(assignment.get_UnderlyingSameMethodInstructions());
			this.foreachVariableType = assignment.get_Right().get_ExpressionType();
			return true;
		}

		private bool IsGetCurrent(Expression expression)
		{
			V_0 = expression as MethodInvocationExpression;
			if (V_0 == null)
			{
				V_1 = expression as PropertyReferenceExpression;
				if (V_1 == null)
				{
					return false;
				}
				return String.op_Equality(V_1.get_Property().get_Name(), "Current");
			}
			if (V_0.get_MethodExpression().get_Target() as VariableReferenceExpression == null || String.op_Inequality(V_0.get_MethodExpression().get_Method().get_Name(), "get_Current"))
			{
				return false;
			}
			if ((object)(V_0.get_MethodExpression().get_Target() as VariableReferenceExpression).get_Variable() != (object)this.theEnumerator)
			{
				return false;
			}
			return true;
		}

		private bool IsGetEnumerator(MethodInvocationExpression supposedGetEnumerator)
		{
			if (String.op_Inequality(supposedGetEnumerator.get_MethodExpression().get_Method().get_Name(), "GetEnumerator"))
			{
				return false;
			}
			if (supposedGetEnumerator.get_MethodExpression().get_Target() == null)
			{
				return false;
			}
			this.foreachCollection = supposedGetEnumerator.get_MethodExpression().get_Target();
			return true;
		}

		private bool IsMoveNextCall(MethodInvocationExpression invocation)
		{
			if (invocation == null)
			{
				return false;
			}
			if (String.op_Equality(invocation.get_MethodExpression().get_Method().get_Name(), "MoveNext"))
			{
				V_0 = invocation.get_MethodExpression().get_Target() as VariableReferenceExpression;
				if (V_0 != null && (object)V_0.get_Variable() == (object)this.theEnumerator)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsValidFinally(BlockStatement blockStatement)
		{
			if (blockStatement.get_Statements().get_Count() != 1)
			{
				V_0 = blockStatement.get_Statements().get_Item(1) as IfStatement;
			}
			else
			{
				V_0 = blockStatement.get_Statements().get_Item(0) as IfStatement;
				if (V_0 == null)
				{
					V_1 = blockStatement.get_Statements().get_Item(0) as ExpressionStatement;
					if (V_1 == null)
					{
						return false;
					}
					return this.IsEnumeratorDispose(V_1.get_Expression() as MethodInvocationExpression);
				}
			}
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_Then().get_Statements().get_Count() != 1)
			{
				return false;
			}
			V_2 = V_0.get_Then().get_Statements().get_Item(0) as ExpressionStatement;
			if (V_2 == null)
			{
				return false;
			}
			return this.IsEnumeratorDispose(V_2.get_Expression() as MethodInvocationExpression);
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.ClearState();
			this.foreachBody = new BlockStatement();
			this.Visit(body);
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count())
			{
				V_1 = node.get_Statements().get_Count();
				this.Visit(node.get_Statements().get_Item(V_0));
				V_2 = node.get_Statements().get_Count();
				if (V_1 > V_2)
				{
					V_0 = V_0 - V_1 - V_2;
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			if (this.foundWhile)
			{
				if (node.get_Expression() as BinaryExpression == null)
				{
					if (node.get_Expression() as MethodInvocationExpression != null || node.get_Expression() as PropertyReferenceExpression != null)
					{
						if (this.IsGetCurrent(node.get_Expression()))
						{
							this.foreachVariableType = node.get_Expression().get_ExpressionType();
							return;
						}
						if (this.shouldAdd)
						{
							this.foreachBody.AddStatement(node);
							return;
						}
					}
					else
					{
						if (node.get_Expression() as ExplicitCastExpression == null)
						{
							if (node.get_Expression() as BoxExpression == null)
							{
								if (this.shouldAdd)
								{
									this.foreachBody.AddStatement(node);
								}
							}
							else
							{
								V_1 = node.get_Expression() as BoxExpression;
								if (this.IsGetCurrent(V_1.get_BoxedExpression()))
								{
									this.foreachVariableType = V_1.get_BoxedAs();
									return;
								}
								if (this.shouldAdd)
								{
									this.foreachBody.AddStatement(node);
									return;
								}
							}
						}
						else
						{
							V_0 = node.get_Expression() as ExplicitCastExpression;
							if (this.IsGetCurrent(V_0.get_Expression()))
							{
								this.foreachVariableType = V_0.get_ExpressionType();
								return;
							}
							if (this.shouldAdd)
							{
								this.foreachBody.AddStatement(node);
								return;
							}
						}
					}
				}
				else
				{
					if (!this.IsForeachVariableAssignment(node.get_Expression() as BinaryExpression) && this.shouldAdd)
					{
						this.foreachBody.AddStatement(node);
						return;
					}
				}
			}
			else
			{
				if (this.IsEnumeratorAssignment(node.get_Expression()))
				{
					this.foundEnumeratorAssignment = true;
					this.enumeratorAssignmentStatement = node;
					return;
				}
			}
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (!this.foundWhile && this.IsGetCurrent(node) || this.IsMoveNextCall(node))
			{
				this.ClearState();
				return;
			}
			if (this.IsGetCurrent(node))
			{
				this.foreachVariableType = node.get_ExpressionType();
			}
			return;
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			if (!this.foundWhile && this.IsGetCurrent(node))
			{
				this.ClearState();
				return;
			}
			if (this.IsGetCurrent(node))
			{
				this.foreachVariableType = node.get_ExpressionType();
			}
			return;
		}

		public override void VisitTryStatement(TryStatement node)
		{
			if (!this.foundEnumeratorAssignment)
			{
				this.insideTry = false;
				this.VisitTryStatement(node);
				return;
			}
			if (this.CanContainForeach(node))
			{
				this.insideTry = true;
				this.theTry = node;
				this.VisitTryStatement(node);
			}
			this.insideTry = false;
			return;
		}

		public override void VisitWhileStatement(WhileStatement node)
		{
			if (!this.foundEnumeratorAssignment || !this.insideTry)
			{
				this.VisitWhileStatement(node);
			}
			else
			{
				if (!this.IsForeach(node))
				{
					this.ClearState();
					this.VisitWhileStatement(node);
					return;
				}
				V_0 = this.theTry.get_Parent() as BlockStatement;
				if (V_0 == null || V_0 != this.enumeratorAssignmentStatement.get_Parent() || V_0.get_Statements().IndexOf(this.enumeratorAssignmentStatement) + 1 != V_0.get_Statements().IndexOf(this.theTry))
				{
					this.ClearState();
					this.VisitWhileStatement(node);
					return;
				}
				this.foreachConditionInstructions = node.get_Condition().get_UnderlyingSameMethodInstructions();
				this.foundWhile = true;
				this.shouldAdd = true;
				V_1 = node.get_Body().get_Statements().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (V_2 as ExpressionStatement != null)
						{
							this.VisitExpressionStatement(V_2 as ExpressionStatement);
						}
						else
						{
							this.foreachBody.AddStatement(V_2);
						}
						if (this.foreachVariableType != null)
						{
							continue;
						}
						V_3 = new RebuildForeachStatements.ForeachElementTypeFinder(this.theEnumerator);
						V_3.Visit(V_2);
						this.foreachVariableType = V_3.get_ResultingType();
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
				if (this.foreachVariableType == null)
				{
					this.ClearState();
					this.VisitWhileStatement(node);
					return;
				}
				this.AttachForeach();
				if (this.isEnumeratorUsedInsideForEach)
				{
					this.ClearState();
					this.VisitWhileStatement(node);
					return;
				}
			}
			return;
		}

		private class ForeachElementTypeFinder : BaseCodeVisitor
		{
			private readonly VariableReference theEnumerator;

			public TypeReference ResultingType
			{
				get;
				set;
			}

			public ForeachElementTypeFinder(VariableReference theEnumerator)
			{
				base();
				this.theEnumerator = theEnumerator;
				return;
			}

			private bool IsGetCurrent(Expression expression)
			{
				V_0 = expression as MethodInvocationExpression;
				if (V_0 == null)
				{
					V_1 = expression as PropertyReferenceExpression;
					if (V_1 == null)
					{
						return false;
					}
					return String.op_Equality(V_1.get_Property().get_Name(), "Current");
				}
				if (V_0.get_MethodExpression().get_Target() as VariableReferenceExpression == null || String.op_Inequality(V_0.get_MethodExpression().get_Method().get_Name(), "get_Current"))
				{
					return false;
				}
				if ((object)(V_0.get_MethodExpression().get_Target() as VariableReferenceExpression).get_Variable() != (object)this.theEnumerator)
				{
					return false;
				}
				return true;
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (this.IsGetCurrent(node))
				{
					this.set_ResultingType(node.get_ExpressionType());
				}
				this.VisitMethodInvocationExpression(node);
				return;
			}

			public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (this.IsGetCurrent(node))
				{
					this.set_ResultingType(node.get_ExpressionType());
				}
				this.VisitPropertyReferenceExpression(node);
				return;
			}

			public override void VisitTryStatement(TryStatement node)
			{
				return;
			}
		}

		private class ForeachVariableChanger : BaseCodeVisitor
		{
			private readonly VariableDefinition oldVariable;

			private readonly VariableDefinition newVariable;

			public ForeachVariableChanger(VariableDefinition oldVariable, VariableDefinition newVariable)
			{
				base();
				this.oldVariable = oldVariable;
				this.newVariable = newVariable;
				return;
			}

			public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
			{
				if ((object)node.get_Variable() == (object)this.oldVariable)
				{
					node.set_Variable(this.newVariable);
				}
				return;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if (node.get_Variable() == this.oldVariable)
				{
					node.set_Variable(this.newVariable);
				}
				return;
			}
		}

		private class GetCurrentFixer : BaseCodeTransformer
		{
			private readonly VariableReference enumerator;

			private readonly VariableReference foreachVariable;

			public GetCurrentFixer(VariableReference enumerator, VariableReference foreachVariable)
			{
				base();
				this.enumerator = enumerator;
				this.foreachVariable = foreachVariable;
				return;
			}

			public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (node.get_MethodExpression().get_Target() as VariableReferenceExpression == null || (object)(node.get_MethodExpression().get_Target() as VariableReferenceExpression).get_Variable() != (object)this.enumerator || !String.op_Equality(node.get_MethodExpression().get_Method().get_Name(), "get_Current"))
				{
					return this.VisitMethodInvocationExpression(node);
				}
				return new VariableReferenceExpression(this.foreachVariable, null);
			}

			public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (String.op_Equality(node.get_Property().get_Name(), "Current"))
				{
					V_0 = node.get_Target() as VariableReferenceExpression;
					if (V_0 != null && (object)V_0.get_Variable() == (object)this.enumerator)
					{
						return new VariableReferenceExpression(this.foreachVariable, null);
					}
				}
				return this.VisitPropertyReferenceExpression(node);
			}
		}

		private class IsEnumeratorUsedVisitor : BaseCodeVisitor
		{
			private readonly VariableReference enumerator;

			public bool IsEnumeratorUsed
			{
				get;
				set;
			}

			public IsEnumeratorUsedVisitor(VariableReference enumerator)
			{
				base();
				this.enumerator = enumerator;
				this.set_IsEnumeratorUsed(false);
				return;
			}

			public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
			{
				if (node.get_MethodExpression().get_Target() as VariableReferenceExpression != null && (object)(node.get_MethodExpression().get_Target() as VariableReferenceExpression).get_Variable() == (object)this.enumerator && String.op_Inequality(node.get_MethodExpression().get_Method().get_Name(), "get_Current"))
				{
					this.set_IsEnumeratorUsed(true);
				}
				this.VisitMethodInvocationExpression(node);
				return;
			}

			public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
			{
				if (String.op_Inequality(node.get_Property().get_Name(), "Current"))
				{
					V_0 = node.get_Target() as VariableReferenceExpression;
					if (V_0 != null && (object)V_0.get_Variable() == (object)this.enumerator)
					{
						this.set_IsEnumeratorUsed(true);
					}
				}
				this.VisitPropertyReferenceExpression(node);
				return;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				if ((object)node.get_Variable() == (object)this.enumerator)
				{
					this.set_IsEnumeratorUsed(true);
				}
				this.VisitVariableReferenceExpression(node);
				return;
			}
		}

		private class YieldStateMachineCodeRemover
		{
			private ForEachStatement foreach;

			private VariableReference enumeratorVariable;

			public YieldStateMachineCodeRemover(ForEachStatement foreach, VariableReference enumeratorVariable)
			{
				base();
				this.foreach = foreach;
				this.enumeratorVariable = enumeratorVariable;
				return;
			}

			public void ProcessForEachStatement()
			{
				this.RemoveLastForeachStatementIfNeeded();
				this.RemoveFirstSuccessorIfNeeded();
				return;
			}

			private void RemoveFirstSuccessorIfNeeded()
			{
				V_0 = this.foreach.get_Parent() as BlockStatement;
				V_1 = V_0.get_Statements().IndexOf(this.foreach);
				if (V_0.get_Statements().get_Count() <= V_1 + 1)
				{
					return;
				}
				V_2 = V_0.get_Statements().get_Item(V_1 + 1) as ExpressionStatement;
				if (V_2 == null)
				{
					return;
				}
				V_3 = V_2.get_Expression() as BinaryExpression;
				if (V_3 == null)
				{
					return;
				}
				V_4 = V_3.get_Left() as VariableReferenceExpression;
				if (V_4 == null)
				{
					return;
				}
				if ((object)V_4.get_Variable() != (object)this.enumeratorVariable)
				{
					return;
				}
				V_5 = V_3.get_Right() as LiteralExpression;
				if (V_5 != null && V_5.get_Value() == null)
				{
					this.RemoveStatement(V_0.get_Statements(), V_2);
				}
				if (V_3.get_Right() as ObjectCreationExpression != null)
				{
					this.RemoveStatement(V_0.get_Statements(), V_2);
				}
				return;
			}

			private void RemoveLastForeachStatementIfNeeded()
			{
				if (this.foreach.get_Body().get_Statements().get_Count() == 0)
				{
					return;
				}
				V_0 = this.foreach.get_Body().get_Statements().Last<Statement>() as ExpressionStatement;
				if (V_0 == null)
				{
					return;
				}
				V_1 = V_0.get_Expression() as BinaryExpression;
				if (V_1 == null)
				{
					return;
				}
				if (!V_1.get_IsAssignmentExpression())
				{
					return;
				}
				V_2 = V_1.get_Left() as VariableReferenceExpression;
				if (V_2 == null || V_2.get_Variable() != this.foreach.get_Variable().get_Variable())
				{
					return;
				}
				V_3 = V_1.get_Right() as LiteralExpression;
				if (V_3 != null && V_3.get_Value() == null)
				{
					this.RemoveStatement(this.foreach.get_Body().get_Statements(), V_0);
					return;
				}
				if (V_1.get_Right() as DefaultObjectExpression == null)
				{
					return;
				}
				this.RemoveStatement(this.foreach.get_Body().get_Statements(), V_0);
				return;
			}

			private void RemoveStatement(StatementCollection collection, Statement statement)
			{
				dummyVar0 = collection.Remove(statement);
				return;
			}
		}
	}
}