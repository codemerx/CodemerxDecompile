using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildLockStatements : BaseCodeVisitor, IDecompilationStep
	{
		private TryStatement try;

		private BlockStatement body;

		private LockStatement lock;

		private VariableReference theFlagVariable;

		private RebuildLockStatements.LockType lockType;

		private Expression lockObjectExpression;

		private VariableReference theLocalVariable;

		private VariableReference thePhiVariable;

		private readonly List<Instruction> lockingInstructions;

		private StatementCollection statements;

		private int statementIndex;

		private LockStatement Lock
		{
			get
			{
				stackVariable2 = this.lock;
				if (stackVariable2 == null)
				{
					dummyVar0 = stackVariable2;
					stackVariable2 = new LockStatement(this.lockObjectExpression.CloneAndAttachInstructions(this.lockingInstructions), this.body, this.try.get_Finally().get_UnderlyingSameMethodInstructions());
				}
				this.lock = stackVariable2;
				return this.lock;
			}
		}

		public RebuildLockStatements()
		{
			this.lockingInstructions = new List<Instruction>();
			base();
			return;
		}

		private bool CheckLockVariableAssignmentExpression(Statement statement)
		{
			if (statement.get_CodeNodeType() != 5)
			{
				return false;
			}
			V_0 = statement as ExpressionStatement;
			if (V_0.get_Expression().get_CodeNodeType() != 24)
			{
				return false;
			}
			V_1 = V_0.get_Expression() as BinaryExpression;
			if (!V_1.get_IsAssignmentExpression() || V_1.get_Left().get_CodeNodeType() != 26)
			{
				return false;
			}
			this.theLocalVariable = (V_1.get_Left() as VariableReferenceExpression).get_Variable();
			this.lockObjectExpression = V_1.get_Right();
			return true;
		}

		private bool CheckTheAssignExpressions(Statement firstAssign, Statement secondAssign)
		{
			V_0 = null;
			if (firstAssign.get_CodeNodeType() == 5)
			{
				V_1 = (firstAssign as ExpressionStatement).get_Expression() as BinaryExpression;
				if (V_1 == null || !V_1.get_IsAssignmentExpression())
				{
					return false;
				}
				V_0 = V_1.get_Left() as VariableReferenceExpression;
				this.lockObjectExpression = V_1.get_Right();
				this.lockingInstructions.AddRange(V_1.get_Left().get_UnderlyingSameMethodInstructions());
				this.lockingInstructions.AddRange(V_1.get_MappedInstructions());
			}
			if (V_0 != null && secondAssign.get_CodeNodeType() == 5)
			{
				V_2 = (secondAssign as ExpressionStatement).get_Expression() as BinaryExpression;
				if (V_2 == null || !V_2.get_IsAssignmentExpression() || V_2.get_Left().get_CodeNodeType() != 26 || V_2.get_Right().get_CodeNodeType() != 26)
				{
					return false;
				}
				this.theLocalVariable = (V_2.get_Left() as VariableReferenceExpression).get_Variable();
				if ((object)(V_2.get_Right() as VariableReferenceExpression).get_Variable() != (object)V_0.get_Variable())
				{
					return false;
				}
			}
			this.lockingInstructions.AddRange(secondAssign.get_UnderlyingSameMethodInstructions());
			if (V_0 != null)
			{
				this.thePhiVariable = V_0.get_Variable();
			}
			return true;
		}

		private bool CheckTheFinallyClause(BlockStatement theFinally)
		{
			V_0 = theFinally.get_Statements().get_Count();
			if (V_0 > 2 && V_0 == 0)
			{
				return false;
			}
			V_1 = null;
			if (this.lockType != 1 && this.lockType != 2 && this.lockType != 3)
			{
				if (V_0 > 1 || theFinally.get_Statements().get_Item(0).get_CodeNodeType() != 5)
				{
					return false;
				}
				V_1 = (theFinally.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as MethodInvocationExpression;
				return this.CheckTheMethodInvocation(V_1, "Exit");
			}
			V_2 = null;
			if (V_0 == 2)
			{
				if (theFinally.get_Statements().get_Item(0).get_CodeNodeType() != 5)
				{
					return false;
				}
				V_4 = (theFinally.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
				if (V_4 == null || !V_4.get_IsAssignmentExpression() || V_4.get_Left().get_CodeNodeType() != 26)
				{
					return false;
				}
				V_2 = (V_4.get_Left() as VariableReferenceExpression).get_Variable();
			}
			if (V_0 == 0)
			{
				return false;
			}
			V_3 = theFinally.get_Statements().get_Item(V_0 - 1) as IfStatement;
			if (V_3 == null)
			{
				return false;
			}
			if (V_2 != null && V_3.get_Condition().get_CodeNodeType() != 23 || (V_3.get_Condition() as UnaryExpression).get_Operator() != 1 || (V_3.get_Condition() as UnaryExpression).get_Operand().get_CodeNodeType() != 26 || (object)((V_3.get_Condition() as UnaryExpression).get_Operand() as VariableReferenceExpression).get_Variable() != (object)V_2)
			{
				return false;
			}
			return this.CheckTheIfStatement(V_3);
		}

		private bool CheckTheIfStatement(IfStatement theIf)
		{
			if (theIf == null || theIf.get_Else() != null || theIf.get_Then() == null || theIf.get_Then().get_Statements() == null || theIf.get_Then().get_Statements().get_Count() != 1 || theIf.get_Then().get_Statements().get_Item(0).get_CodeNodeType() != 5)
			{
				return false;
			}
			V_0 = (theIf.get_Then().get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as MethodInvocationExpression;
			return this.CheckTheMethodInvocation(V_0, "Exit");
		}

		private bool CheckTheMethodInvocation(MethodInvocationExpression theMethodInvocation, string methodName)
		{
			if (theMethodInvocation == null || theMethodInvocation.get_MethodExpression().get_CodeNodeType() != 20)
			{
				return false;
			}
			V_0 = theMethodInvocation.get_MethodExpression().get_Method();
			if (!String.op_Equality(V_0.get_DeclaringType().get_FullName(), Type.GetTypeFromHandle(// 
			// Current member / type: System.Boolean Telerik.JustDecompiler.Steps.RebuildLockStatements::CheckTheMethodInvocation(Telerik.JustDecompiler.Ast.Expressions.MethodInvocationExpression,System.String)
			// Exception in: System.Boolean CheckTheMethodInvocation(Telerik.JustDecompiler.Ast.Expressions.MethodInvocationExpression,System.String)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private bool DetermineWithFlagLockTypeVersion(TryStatement try)
		{
			if (try == null || try.get_Try() == null || try.get_Try().get_Statements().get_Count() == 0)
			{
				return false;
			}
			if (try.get_Try().get_Statements().get_Item(0).get_CodeNodeType() != 5)
			{
				return false;
			}
			V_0 = try.get_Try().get_Statements().get_Item(0) as ExpressionStatement;
			if (V_0.get_Expression().get_CodeNodeType() != 24)
			{
				if (V_0.get_Expression().get_CodeNodeType() != 19)
				{
					return false;
				}
				this.lockType = 2;
			}
			else
			{
				if (try.get_Try().get_Statements().get_Count() <= 1 || try.get_Try().get_Statements().get_Item(1).get_CodeNodeType() != 5 || (try.get_Try().get_Statements().get_Item(1) as ExpressionStatement).get_Expression().get_CodeNodeType() != 24)
				{
					this.lockType = 3;
				}
				else
				{
					this.lockType = 1;
				}
			}
			return true;
		}

		private VariableReference GetTheFlagVariable(MethodInvocationExpression methodInvocation)
		{
			V_0 = methodInvocation.get_Arguments().get_Item(1);
			V_1 = null;
			if (V_0.get_CodeNodeType() != 23 || (V_0 as UnaryExpression).get_Operator() != 7)
			{
				V_1 = V_0 as VariableReferenceExpression;
			}
			else
			{
				V_1 = (V_0 as UnaryExpression).get_Operand() as VariableReferenceExpression;
			}
			if (V_1 == null)
			{
				return null;
			}
			return V_1.get_Variable();
		}

		private bool IsLockStatement(TryStatement try)
		{
			if (try == null)
			{
				return false;
			}
			if (this.lockType != 1 || try.get_Try().get_Statements().get_Count() <= 2 && this.lockType != RebuildLockStatements.LockType.Simple && this.lockType != 2 || try.get_Try().get_Statements().get_Count() <= 0)
			{
				if (this.lockType != 3)
				{
					stackVariable6 = false;
				}
				else
				{
					stackVariable6 = try.get_Try().get_Statements().get_Count() > 1;
				}
			}
			else
			{
				stackVariable6 = true;
			}
			V_0 = stackVariable6;
			stackVariable7 = V_0;
			if (try.get_CatchClauses().get_Count() != 0)
			{
				stackVariable11 = false;
			}
			else
			{
				stackVariable11 = try.get_Finally() != null;
			}
			V_0 = stackVariable7 & stackVariable11;
			if (V_0)
			{
				if (this.lockType == 1 || this.lockType == 2 || this.lockType == 3)
				{
					if (this.lockType != 1)
					{
						if (this.lockType != 2)
						{
							V_0 = V_0 & this.CheckLockVariableAssignmentExpression(try.get_Try().get_Statements().get_Item(0));
							V_1 = 1;
						}
						else
						{
							V_1 = 0;
						}
					}
					else
					{
						V_0 = V_0 & this.CheckTheAssignExpressions(try.get_Try().get_Statements().get_Item(0), try.get_Try().get_Statements().get_Item(1));
						V_1 = 2;
					}
					V_0 = V_0 & try.get_Try().get_Statements().get_Item(V_1).get_CodeNodeType() == 5;
					if (V_0)
					{
						V_2 = try.get_Try().get_Statements().get_Item(V_1) as ExpressionStatement;
						V_0 = V_0 & V_2.get_Expression().get_CodeNodeType() == 19;
						V_3 = V_2.get_Expression() as MethodInvocationExpression;
						stackVariable68 = V_0;
						if (!this.CheckTheMethodInvocation(V_3, "Enter"))
						{
							stackVariable73 = false;
						}
						else
						{
							stackVariable73 = V_3.get_MethodExpression().get_Method().get_Parameters().get_Count() == 2;
						}
						V_0 = stackVariable68 & stackVariable73;
						if (V_0)
						{
							this.lockingInstructions.AddRange(V_3.get_UnderlyingSameMethodInstructions());
							this.theFlagVariable = this.GetTheFlagVariable(V_3);
						}
					}
				}
				V_0 = V_0 & this.CheckTheFinallyClause(try.get_Finally().get_Body());
			}
			return V_0;
		}

		private bool Match(StatementCollection statements, int statementIndex)
		{
			this.PrepareMatcher(statements, statementIndex);
			V_0 = statements.get_Item(statementIndex);
			if (V_0.get_CodeNodeType() != 17)
			{
				if (V_0.get_CodeNodeType() == 5 && this.CheckTheMethodInvocation((V_0 as ExpressionStatement).get_Expression() as MethodInvocationExpression, "Enter"))
				{
					if (((V_0 as ExpressionStatement).get_Expression() as MethodInvocationExpression).get_MethodExpression().get_Method().get_Parameters().get_Count() != 1)
					{
						return false;
					}
					if (statementIndex + 1 >= statements.get_Count() || statementIndex - 2 < 0)
					{
						return false;
					}
					this.try = statements.get_Item(statementIndex + 1) as TryStatement;
					this.lockType = 0;
					if (!this.CheckTheAssignExpressions(statements.get_Item(statementIndex - 2), statements.get_Item(statementIndex - 1)))
					{
						return false;
					}
					this.lockingInstructions.AddRange(V_0.get_UnderlyingSameMethodInstructions());
				}
			}
			else
			{
				this.try = V_0 as TryStatement;
				if (!this.DetermineWithFlagLockTypeVersion(this.try))
				{
					return false;
				}
				if (this.lockType != 2)
				{
					if (this.lockType == 3 && statementIndex - 1 < 0)
					{
						return false;
					}
				}
				else
				{
					if (statementIndex - 2 < 0 || !this.CheckLockVariableAssignmentExpression(statements.get_Item(statementIndex - 2)))
					{
						return false;
					}
				}
			}
			if (this.try == null)
			{
				return false;
			}
			if (!this.IsLockStatement(this.try))
			{
				return false;
			}
			this.body = this.try.get_Try();
			return true;
		}

		private void PrepareMatcher(StatementCollection statements, int statementIndex)
		{
			this.statements = statements;
			this.statementIndex = statementIndex;
			this.try = null;
			this.body = null;
			this.lock = null;
			this.lockObjectExpression = null;
			this.theLocalVariable = null;
			this.thePhiVariable = null;
			this.lockingInstructions.Clear();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count())
			{
				if (this.Match(node.get_Statements(), V_0))
				{
					node.get_Statements().RemoveAt(V_0);
					node.AddStatementAt(V_0, this.get_Lock());
					if (this.lockType != RebuildLockStatements.LockType.Simple)
					{
						if (this.lockType != 1)
						{
							if (this.lockType != 2)
							{
								V_1 = 2;
							}
							else
							{
								V_1 = 1;
							}
						}
						else
						{
							V_1 = 3;
						}
						V_2 = 0;
						while (V_2 < V_1)
						{
							this.get_Lock().get_Body().get_Statements().RemoveAt(0);
							V_2 = V_2 + 1;
						}
						if (V_0 > 0)
						{
							stackVariable46 = V_0 - 1;
							V_0 = stackVariable46;
							node.get_Statements().RemoveAt(stackVariable46);
							if (this.lockType == 2)
							{
								stackVariable54 = V_0 - 1;
								V_0 = stackVariable54;
								node.get_Statements().RemoveAt(stackVariable54);
							}
						}
					}
					else
					{
						node.get_Statements().RemoveAt(V_0 + 1);
						stackVariable66 = V_0 - 1;
						V_0 = stackVariable66;
						node.get_Statements().RemoveAt(stackVariable66);
						stackVariable71 = V_0 - 1;
						V_0 = stackVariable71;
						node.get_Statements().RemoveAt(stackVariable71);
					}
				}
				V_0 = V_0 + 1;
			}
			this.Visit(node.get_Statements());
			return;
		}

		private enum LockType
		{
			Simple,
			WithFlagV1,
			WithFlagV2,
			WithFlagV3
		}
	}
}