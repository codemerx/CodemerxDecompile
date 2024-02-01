using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildLockStatements : BaseCodeVisitor, IDecompilationStep
	{
		private TryStatement @try;

		private BlockStatement body;

		private LockStatement @lock;

		private VariableReference theFlagVariable;

		private RebuildLockStatements.LockType lockType;

		private Expression lockObjectExpression;

		private VariableReference theLocalVariable;

		private VariableReference thePhiVariable;

		private readonly List<Instruction> lockingInstructions = new List<Instruction>();

		private StatementCollection statements;

		private int statementIndex;

		private LockStatement Lock
		{
			get
			{
				this.@lock = this.@lock ?? new LockStatement(this.lockObjectExpression.CloneAndAttachInstructions(this.lockingInstructions), this.body, this.@try.Finally.UnderlyingSameMethodInstructions);
				return this.@lock;
			}
		}

		public RebuildLockStatements()
		{
		}

		private bool CheckLockVariableAssignmentExpression(Statement statement)
		{
			if (statement.CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			ExpressionStatement expressionStatement = statement as ExpressionStatement;
			if (expressionStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression expression = expressionStatement.Expression as BinaryExpression;
			if (!expression.IsAssignmentExpression || expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
			{
				return false;
			}
			this.theLocalVariable = (expression.Left as VariableReferenceExpression).Variable;
			this.lockObjectExpression = expression.Right;
			return true;
		}

		private bool CheckTheAssignExpressions(Statement firstAssign, Statement secondAssign)
		{
			VariableReferenceExpression left = null;
			if (firstAssign.CodeNodeType == CodeNodeType.ExpressionStatement)
			{
				BinaryExpression expression = (firstAssign as ExpressionStatement).Expression as BinaryExpression;
				if (expression == null || !expression.IsAssignmentExpression)
				{
					return false;
				}
				left = expression.Left as VariableReferenceExpression;
				this.lockObjectExpression = expression.Right;
				this.lockingInstructions.AddRange(expression.Left.UnderlyingSameMethodInstructions);
				this.lockingInstructions.AddRange(expression.MappedInstructions);
			}
			if (left != null && secondAssign.CodeNodeType == CodeNodeType.ExpressionStatement)
			{
				BinaryExpression binaryExpression = (secondAssign as ExpressionStatement).Expression as BinaryExpression;
				if (binaryExpression == null || !binaryExpression.IsAssignmentExpression || binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || binaryExpression.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression)
				{
					return false;
				}
				this.theLocalVariable = (binaryExpression.Left as VariableReferenceExpression).Variable;
				if ((object)(binaryExpression.Right as VariableReferenceExpression).Variable != (object)left.Variable)
				{
					return false;
				}
			}
			this.lockingInstructions.AddRange(secondAssign.UnderlyingSameMethodInstructions);
			if (left != null)
			{
				this.thePhiVariable = left.Variable;
			}
			return true;
		}

		private bool CheckTheFinallyClause(BlockStatement theFinally)
		{
			int count = theFinally.Statements.Count;
			if (count > 2 && count == 0)
			{
				return false;
			}
			MethodInvocationExpression expression = null;
			if (this.lockType != RebuildLockStatements.LockType.WithFlagV1 && this.lockType != RebuildLockStatements.LockType.WithFlagV2 && this.lockType != RebuildLockStatements.LockType.WithFlagV3)
			{
				if (count > 1 || theFinally.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
				{
					return false;
				}
				expression = (theFinally.Statements[0] as ExpressionStatement).Expression as MethodInvocationExpression;
				return this.CheckTheMethodInvocation(expression, "Exit");
			}
			VariableReference variable = null;
			if (count == 2)
			{
				if (theFinally.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
				{
					return false;
				}
				BinaryExpression binaryExpression = (theFinally.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
				if (binaryExpression == null || !binaryExpression.IsAssignmentExpression || binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
				{
					return false;
				}
				variable = (binaryExpression.Left as VariableReferenceExpression).Variable;
			}
			if (count == 0)
			{
				return false;
			}
			IfStatement item = theFinally.Statements[count - 1] as IfStatement;
			if (item == null)
			{
				return false;
			}
			if (variable != null && (item.Condition.CodeNodeType != CodeNodeType.UnaryExpression || (item.Condition as UnaryExpression).Operator != UnaryOperator.LogicalNot || (item.Condition as UnaryExpression).Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)((item.Condition as UnaryExpression).Operand as VariableReferenceExpression).Variable != (object)variable))
			{
				return false;
			}
			return this.CheckTheIfStatement(item);
		}

		private bool CheckTheIfStatement(IfStatement theIf)
		{
			if (theIf == null || theIf.Else != null || theIf.Then == null || theIf.Then.Statements == null || theIf.Then.Statements.Count != 1 || theIf.Then.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			MethodInvocationExpression expression = (theIf.Then.Statements[0] as ExpressionStatement).Expression as MethodInvocationExpression;
			return this.CheckTheMethodInvocation(expression, "Exit");
		}

		private bool CheckTheMethodInvocation(MethodInvocationExpression theMethodInvocation, string methodName)
		{
			if (theMethodInvocation == null || theMethodInvocation.MethodExpression.CodeNodeType != CodeNodeType.MethodReferenceExpression)
			{
				return false;
			}
			MethodReference method = theMethodInvocation.MethodExpression.Method;
			if (method.get_DeclaringType().get_FullName() != typeof(Monitor).FullName)
			{
				return false;
			}
			return method.get_Name() == methodName;
		}

		private bool DetermineWithFlagLockTypeVersion(TryStatement @try)
		{
			if (@try == null || @try.Try == null || @try.Try.Statements.Count == 0)
			{
				return false;
			}
			if (@try.Try.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			ExpressionStatement item = @try.Try.Statements[0] as ExpressionStatement;
			if (item.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				if (item.Expression.CodeNodeType != CodeNodeType.MethodInvocationExpression)
				{
					return false;
				}
				this.lockType = RebuildLockStatements.LockType.WithFlagV2;
			}
			else if (@try.Try.Statements.Count <= 1 || @try.Try.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement || (@try.Try.Statements[1] as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				this.lockType = RebuildLockStatements.LockType.WithFlagV3;
			}
			else
			{
				this.lockType = RebuildLockStatements.LockType.WithFlagV1;
			}
			return true;
		}

		private VariableReference GetTheFlagVariable(MethodInvocationExpression methodInvocation)
		{
			Expression item = methodInvocation.Arguments[1];
			VariableReferenceExpression variableReferenceExpression = null;
			variableReferenceExpression = (item.CodeNodeType != CodeNodeType.UnaryExpression || (item as UnaryExpression).Operator != UnaryOperator.AddressReference ? item as VariableReferenceExpression : (item as UnaryExpression).Operand as VariableReferenceExpression);
			if (variableReferenceExpression == null)
			{
				return null;
			}
			return variableReferenceExpression.Variable;
		}

		private bool IsLockStatement(TryStatement @try)
		{
			int num;
			bool flag;
			if (@try == null)
			{
				return false;
			}
			if ((this.lockType != RebuildLockStatements.LockType.WithFlagV1 || @try.Try.Statements.Count <= 2) && this.lockType != RebuildLockStatements.LockType.Simple && (this.lockType != RebuildLockStatements.LockType.WithFlagV2 || @try.Try.Statements.Count <= 0))
			{
				flag = (this.lockType != RebuildLockStatements.LockType.WithFlagV3 ? false : @try.Try.Statements.Count > 1);
			}
			else
			{
				flag = true;
			}
			bool codeNodeType = flag;
			codeNodeType = codeNodeType & (@try.CatchClauses.Count != 0 ? false : @try.Finally != null);
			if (codeNodeType)
			{
				if (this.lockType == RebuildLockStatements.LockType.WithFlagV1 || this.lockType == RebuildLockStatements.LockType.WithFlagV2 || this.lockType == RebuildLockStatements.LockType.WithFlagV3)
				{
					if (this.lockType == RebuildLockStatements.LockType.WithFlagV1)
					{
						codeNodeType &= this.CheckTheAssignExpressions(@try.Try.Statements[0], @try.Try.Statements[1]);
						num = 2;
					}
					else if (this.lockType != RebuildLockStatements.LockType.WithFlagV2)
					{
						codeNodeType &= this.CheckLockVariableAssignmentExpression(@try.Try.Statements[0]);
						num = 1;
					}
					else
					{
						num = 0;
					}
					codeNodeType = codeNodeType & @try.Try.Statements[num].CodeNodeType == CodeNodeType.ExpressionStatement;
					if (codeNodeType)
					{
						ExpressionStatement item = @try.Try.Statements[num] as ExpressionStatement;
						codeNodeType = codeNodeType & item.Expression.CodeNodeType == CodeNodeType.MethodInvocationExpression;
						MethodInvocationExpression expression = item.Expression as MethodInvocationExpression;
						codeNodeType = codeNodeType & (!this.CheckTheMethodInvocation(expression, "Enter") ? false : expression.MethodExpression.Method.get_Parameters().get_Count() == 2);
						if (codeNodeType)
						{
							this.lockingInstructions.AddRange(expression.UnderlyingSameMethodInstructions);
							this.theFlagVariable = this.GetTheFlagVariable(expression);
						}
					}
				}
				codeNodeType &= this.CheckTheFinallyClause(@try.Finally.Body);
			}
			return codeNodeType;
		}

		private bool Match(StatementCollection statements, int statementIndex)
		{
			this.PrepareMatcher(statements, statementIndex);
			Statement item = statements[statementIndex];
			if (item.CodeNodeType == CodeNodeType.TryStatement)
			{
				this.@try = item as TryStatement;
				if (!this.DetermineWithFlagLockTypeVersion(this.@try))
				{
					return false;
				}
				if (this.lockType == RebuildLockStatements.LockType.WithFlagV2)
				{
					if (statementIndex - 2 < 0 || !this.CheckLockVariableAssignmentExpression(statements[statementIndex - 2]))
					{
						return false;
					}
				}
				else if (this.lockType == RebuildLockStatements.LockType.WithFlagV3 && statementIndex - 1 < 0)
				{
					return false;
				}
			}
			else if (item.CodeNodeType == CodeNodeType.ExpressionStatement && this.CheckTheMethodInvocation((item as ExpressionStatement).Expression as MethodInvocationExpression, "Enter"))
			{
				if (((item as ExpressionStatement).Expression as MethodInvocationExpression).MethodExpression.Method.get_Parameters().get_Count() != 1)
				{
					return false;
				}
				if (statementIndex + 1 >= statements.Count || statementIndex - 2 < 0)
				{
					return false;
				}
				this.@try = statements[statementIndex + 1] as TryStatement;
				this.lockType = RebuildLockStatements.LockType.Simple;
				if (!this.CheckTheAssignExpressions(statements[statementIndex - 2], statements[statementIndex - 1]))
				{
					return false;
				}
				this.lockingInstructions.AddRange(item.UnderlyingSameMethodInstructions);
			}
			if (this.@try == null)
			{
				return false;
			}
			if (!this.IsLockStatement(this.@try))
			{
				return false;
			}
			this.body = this.@try.Try;
			return true;
		}

		private void PrepareMatcher(StatementCollection statements, int statementIndex)
		{
			this.statements = statements;
			this.statementIndex = statementIndex;
			this.@try = null;
			this.body = null;
			this.@lock = null;
			this.lockObjectExpression = null;
			this.theLocalVariable = null;
			this.thePhiVariable = null;
			this.lockingInstructions.Clear();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			int num;
			for (int i = 0; i < node.Statements.Count; i++)
			{
				if (this.Match(node.Statements, i))
				{
					node.Statements.RemoveAt(i);
					node.AddStatementAt(i, this.Lock);
					if (this.lockType != RebuildLockStatements.LockType.Simple)
					{
						if (this.lockType != RebuildLockStatements.LockType.WithFlagV1)
						{
							num = (this.lockType != RebuildLockStatements.LockType.WithFlagV2 ? 2 : 1);
						}
						else
						{
							num = 3;
						}
						for (int j = 0; j < num; j++)
						{
							this.Lock.Body.Statements.RemoveAt(0);
						}
						if (i > 0)
						{
							int num1 = i - 1;
							i = num1;
							node.Statements.RemoveAt(num1);
							if (this.lockType == RebuildLockStatements.LockType.WithFlagV2)
							{
								int num2 = i - 1;
								i = num2;
								node.Statements.RemoveAt(num2);
							}
						}
					}
					else
					{
						node.Statements.RemoveAt(i + 1);
						int num3 = i - 1;
						i = num3;
						node.Statements.RemoveAt(num3);
						int num4 = i - 1;
						i = num4;
						node.Statements.RemoveAt(num4);
					}
				}
			}
			this.Visit(node.Statements);
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