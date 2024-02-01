using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildAsyncStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private readonly Dictionary<FieldDefinition, Expression> parameterMappings = new Dictionary<FieldDefinition, Expression>();

		private DecompilationContext context;

		private MethodSpecificContext methodContext;

		private TypeDefinition stateMachineTypeDef;

		private FieldDefinition builderField;

		private RebuildAsyncStatementsStep.MatcherState matcherState;

		private StatementCollection asyncStatements;

		private StatementCollection originalStatements;

		private AsyncData asyncData;

		private VariableReference currentAwaiterVariable;

		private Expression awaitedExpression;

		public RebuildAsyncStatementsStep()
		{
		}

		private bool GetBuilderField()
		{
			bool flag;
			MethodDefinition stateMachineMethod = this.GetStateMachineMethod("SetStateMachine") ?? this.GetStateMachineMethod("System.Runtime.CompilerServices.IAsyncStateMachine.SetStateMachine");
			if (stateMachineMethod == null || stateMachineMethod.get_Body() == null)
			{
				return false;
			}
			if (stateMachineMethod.get_Body().get_Instructions().get_Count() <= 1)
			{
				Mono.Collections.Generic.Collection<FieldDefinition>.Enumerator enumerator = this.stateMachineTypeDef.get_Fields().GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						FieldDefinition current = enumerator.get_Current();
						if (!(current.get_FieldType().get_Name() == "AsyncVoidMethodBuilder") && !(current.get_FieldType().get_Name() == "AsyncTaskMethodBuilder") && !(current.get_FieldType().get_Name() == "AsyncTaskMethodBuilder`1"))
						{
							continue;
						}
						this.builderField = current;
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					enumerator.Dispose();
				}
			}
			else
			{
				Mono.Collections.Generic.Collection<Instruction>.Enumerator enumerator1 = stateMachineMethod.get_Body().get_Instructions().GetEnumerator();
				try
				{
					while (enumerator1.MoveNext())
					{
						Instruction instruction = enumerator1.get_Current();
						if (instruction.get_OpCode().get_Code() != 121)
						{
							continue;
						}
						this.builderField = ((FieldReference)instruction.get_Operand()).Resolve();
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					enumerator1.Dispose();
				}
			}
			return flag;
		}

		private string GetFriendlyName(string name)
		{
			int num = name.LastIndexOf('\u005F');
			if (num != -1 && num + 1 < name.Length)
			{
				name = name.Substring(num + 1);
			}
			return name;
		}

		private StatementCollection GetMoveNextStatements()
		{
			MethodDefinition stateMachineMethod = this.GetStateMachineMethod("MoveNext");
			if (stateMachineMethod == null || stateMachineMethod.get_Body() == null)
			{
				return null;
			}
			BlockStatement blockStatement = stateMachineMethod.get_Body().DecompileAsyncStateMachine(this.context, out this.asyncData);
			if (blockStatement == null)
			{
				return null;
			}
			return blockStatement.Statements;
		}

		private MethodDefinition GetStateMachineMethod(string name)
		{
			MethodDefinition methodDefinition;
			Mono.Collections.Generic.Collection<MethodDefinition>.Enumerator enumerator = this.stateMachineTypeDef.get_Methods().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					MethodDefinition current = enumerator.get_Current();
					if (current.get_Name() != name)
					{
						continue;
					}
					methodDefinition = current;
					return methodDefinition;
				}
				return null;
			}
			finally
			{
				enumerator.Dispose();
			}
			return methodDefinition;
		}

		private bool IsAsyncFirstAssignmentStatement(Statement statement, out TypeDefinition asyncStateMachineType)
		{
			asyncStateMachineType = null;
			if (statement is ExpressionStatement)
			{
				ExpressionStatement expressionStatement = statement as ExpressionStatement;
				if (expressionStatement.Expression is BinaryExpression)
				{
					BinaryExpression expression = expressionStatement.Expression as BinaryExpression;
					if (expression.Right is ThisReferenceExpression && expression.Left is FieldReferenceExpression)
					{
						TypeReference declaringType = (expression.Left as FieldReferenceExpression).Field.get_DeclaringType();
						if (declaringType == null)
						{
							return false;
						}
						TypeDefinition typeDefinition = declaringType.Resolve();
						if (typeDefinition == null || (object)typeDefinition.get_DeclaringType() != (object)this.methodContext.Method.get_DeclaringType() || !typeDefinition.IsAsyncStateMachine())
						{
							return false;
						}
						asyncStateMachineType = typeDefinition;
						return true;
					}
				}
			}
			return false;
		}

		private bool Match()
		{
			if (this.originalStatements.Count == 0)
			{
				return false;
			}
			if (!this.methodContext.Method.IsAsync(out this.stateMachineTypeDef) && (!this.methodContext.Method.HasAsyncAttributes() || !this.IsAsyncFirstAssignmentStatement(this.originalStatements[0], out this.stateMachineTypeDef) || !this.methodContext.Method.HasAsyncStateMachineVariable()))
			{
				return false;
			}
			if (!this.GetBuilderField())
			{
				return false;
			}
			this.asyncStatements = this.GetMoveNextStatements();
			if (this.asyncStatements == null || !this.TryRemoveOuterTryCatch(this.asyncStatements))
			{
				return false;
			}
			this.SetParameterMappings(this.originalStatements);
			this.matcherState = RebuildAsyncStatementsStep.MatcherState.FindAwaitExpression;
			this.asyncStatements = (StatementCollection)this.Visit(this.asyncStatements);
			if (this.matcherState == RebuildAsyncStatementsStep.MatcherState.FindAwaitExpression)
			{
				return true;
			}
			return this.matcherState == RebuildAsyncStatementsStep.MatcherState.FindInitObj;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.methodContext = context.MethodContext;
			this.originalStatements = body.Statements;
			if (this.Match())
			{
				body.Statements = this.asyncStatements;
			}
			return body;
		}

		private void SetParameterMappings(StatementCollection statements)
		{
			for (int i = 0; i < statements.Count; i++)
			{
				if (statements[i].IsAssignmentStatement())
				{
					BinaryExpression expression = (statements[i] as ExpressionStatement).Expression as BinaryExpression;
					if (expression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
					{
						FieldReference field = (expression.Left as FieldReferenceExpression).Field;
						if ((object)field.get_DeclaringType().Resolve() == (object)this.stateMachineTypeDef)
						{
							this.parameterMappings[field.Resolve()] = expression.Right;
						}
					}
				}
			}
		}

		private bool TryGetStateField(BlockStatement theCatch)
		{
			bool flag;
			using (IEnumerator<Statement> enumerator = theCatch.Statements.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Statement current = enumerator.Current;
					if (!current.IsAssignmentStatement())
					{
						continue;
					}
					BinaryExpression expression = (current as ExpressionStatement).Expression as BinaryExpression;
					if (expression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || expression.Right.CodeNodeType != CodeNodeType.LiteralExpression)
					{
						continue;
					}
					FieldDefinition fieldDefinition = (expression.Left as FieldReferenceExpression).Field.Resolve();
					if (fieldDefinition == null || fieldDefinition.get_DeclaringType() == null)
					{
						flag = false;
						return flag;
					}
					else if ((object)fieldDefinition.get_DeclaringType().Resolve() == (object)this.stateMachineTypeDef)
					{
						this.asyncData.StateField = fieldDefinition;
						flag = true;
						return flag;
					}
					else
					{
						flag = false;
						return flag;
					}
				}
				return false;
			}
			return flag;
		}

		private bool TryRemoveOuterTryCatch(StatementCollection statements)
		{
			int i;
			for (i = 0; i < statements.Count; i++)
			{
				Statement item = statements[i];
				if (item.CodeNodeType == CodeNodeType.TryStatement && (item as TryStatement).CatchClauses.Count == 1)
				{
					break;
				}
			}
			if (i == statements.Count)
			{
				return false;
			}
			TryStatement tryStatement = statements[i] as TryStatement;
			StatementCollection statementCollection = tryStatement.Try.Statements;
			if (this.asyncData.StateField == null && !this.TryGetStateField(tryStatement.CatchClauses[0].Body))
			{
				return false;
			}
			statements.RemoveAt(i);
			for (int j = 0; j < statementCollection.Count; j++)
			{
				statements.Insert(i + j, statementCollection[j]);
			}
			return true;
		}

		public override ICodeNode Visit(ICodeNode node)
		{
			if (this.matcherState == RebuildAsyncStatementsStep.MatcherState.Stopped)
			{
				return node;
			}
			return base.Visit(node);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.Operator == BinaryOperator.Assign)
			{
				if (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression && this.asyncData.AwaiterVariables.Contains((node.Left as VariableReferenceExpression).Variable))
				{
					VariableReference variable = (node.Left as VariableReferenceExpression).Variable;
					if (node.Right.CodeNodeType == CodeNodeType.MethodInvocationExpression && (node.Right as MethodInvocationExpression).MethodExpression.Method.get_Name() == "GetAwaiter")
					{
						Expression expression = null;
						MethodInvocationExpression right = node.Right as MethodInvocationExpression;
						if (right.MethodExpression.Target != null)
						{
							if (right.Arguments.Count == 0)
							{
								expression = (Expression)this.Visit(right.MethodExpression.Target);
							}
						}
						else if (right.Arguments.Count == 1)
						{
							expression = (Expression)this.Visit(right.Arguments[0]);
						}
						if (expression != null && (this.matcherState == RebuildAsyncStatementsStep.MatcherState.FindAwaitExpression || this.matcherState == RebuildAsyncStatementsStep.MatcherState.FindInitObj))
						{
							this.currentAwaiterVariable = variable;
							this.awaitedExpression = expression;
							this.matcherState = RebuildAsyncStatementsStep.MatcherState.FindIsCompletedInvoke;
							return null;
						}
					}
					else if ((node.Right.CodeNodeType == CodeNodeType.ObjectCreationExpression || node.Right.CodeNodeType == CodeNodeType.LiteralExpression && (node.Right as LiteralExpression).Value == null) && (this.matcherState & RebuildAsyncStatementsStep.MatcherState.FindInitObj) == RebuildAsyncStatementsStep.MatcherState.FindInitObj && (object)this.currentAwaiterVariable == (object)variable)
					{
						this.matcherState ^= RebuildAsyncStatementsStep.MatcherState.FindInitObj;
						return null;
					}
					this.matcherState = RebuildAsyncStatementsStep.MatcherState.Stopped;
					return node;
				}
				if (node.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression && (object)(node.Left as FieldReferenceExpression).Field.Resolve() == (object)this.asyncData.StateField || node.Right.CodeNodeType == CodeNodeType.ThisReferenceExpression)
				{
					return null;
				}
			}
			return base.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			node.Expression = (Expression)this.Visit(node.Expression);
			if (node.Expression != null)
			{
				return node;
			}
			if (node.Label != null && node.Label != String.Empty)
			{
				Statement nextStatement = node.GetNextStatement();
				if (nextStatement == null || nextStatement.Label != null && nextStatement.Label != String.Empty)
				{
					EmptyStatement emptyStatement = new EmptyStatement()
					{
						Label = node.Label
					};
					this.methodContext.GotoLabels[node.Label] = emptyStatement;
					return emptyStatement;
				}
				nextStatement.Label = node.Label;
				this.methodContext.GotoLabels[node.Label] = nextStatement;
			}
			return null;
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if ((object)node.Field.get_DeclaringType().Resolve() != (object)this.stateMachineTypeDef)
			{
				return base.VisitFieldReferenceExpression(node);
			}
			FieldDefinition fieldDefinition = node.Field.Resolve();
			if (this.parameterMappings.ContainsKey(fieldDefinition))
			{
				return this.parameterMappings[fieldDefinition].CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
			}
			VariableDefinition variableDefinition = new VariableDefinition(this.GetFriendlyName(fieldDefinition.get_Name()), fieldDefinition.get_FieldType(), this.methodContext.Method);
			this.methodContext.Variables.Add(variableDefinition);
			this.methodContext.VariableAssignmentData.Add(variableDefinition, this.asyncData.FieldAssignmentData[fieldDefinition]);
			this.methodContext.VariablesToRename.Add(variableDefinition);
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(variableDefinition, node.UnderlyingSameMethodInstructions);
			this.parameterMappings[fieldDefinition] = variableReferenceExpression;
			return variableReferenceExpression;
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			Expression item;
			MethodReferenceExpression methodExpression = node.MethodExpression;
			if (methodExpression.Target != null)
			{
				if (methodExpression.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression && this.asyncData.AwaiterVariables.Contains((methodExpression.Target as VariableReferenceExpression).Variable))
				{
					VariableReference variable = (methodExpression.Target as VariableReferenceExpression).Variable;
					if ((object)this.currentAwaiterVariable == (object)variable)
					{
						if (methodExpression.Method.get_Name() == "get_IsCompleted")
						{
							if (this.matcherState == RebuildAsyncStatementsStep.MatcherState.FindIsCompletedInvoke)
							{
								this.matcherState = RebuildAsyncStatementsStep.MatcherState.FindGetResultInvoke | RebuildAsyncStatementsStep.MatcherState.FindInitObj;
								return null;
							}
						}
						else if (methodExpression.Method.get_Name() == "GetResult" && (this.matcherState & RebuildAsyncStatementsStep.MatcherState.FindGetResultInvoke) == RebuildAsyncStatementsStep.MatcherState.FindGetResultInvoke)
						{
							this.matcherState ^= RebuildAsyncStatementsStep.MatcherState.FindGetResultInvoke;
							return new AwaitExpression((Expression)this.Visit(this.awaitedExpression), methodExpression.Method.get_ReturnType(), node.UnderlyingSameMethodInstructions);
						}
					}
					this.matcherState = RebuildAsyncStatementsStep.MatcherState.Stopped;
					return node;
				}
				if (methodExpression.Target.CodeNodeType == CodeNodeType.FieldReferenceExpression && (object)(methodExpression.Target as FieldReferenceExpression).Field.Resolve() == (object)this.builderField && methodExpression.Method.get_Name() == "SetResult")
				{
					if (node.Arguments.Count > 0)
					{
						item = node.Arguments[0];
					}
					else
					{
						item = null;
					}
					return new ReturnExpression(item, methodExpression.UnderlyingSameMethodInstructions);
				}
			}
			return base.VisitMethodInvocationExpression(node);
		}

		public override ICodeNode VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			MethodReferenceExpression methodExpression = node.MethodExpression;
			if (methodExpression.Target == null || methodExpression.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression || !this.asyncData.AwaiterVariables.Contains((methodExpression.Target as VariableReferenceExpression).Variable))
			{
				return base.VisitPropertyReferenceExpression(node);
			}
			VariableReference variable = (methodExpression.Target as VariableReferenceExpression).Variable;
			if ((object)this.currentAwaiterVariable == (object)variable && methodExpression.Method.get_Name() == "get_IsCompleted" && this.matcherState == RebuildAsyncStatementsStep.MatcherState.FindIsCompletedInvoke)
			{
				this.matcherState = RebuildAsyncStatementsStep.MatcherState.FindGetResultInvoke | RebuildAsyncStatementsStep.MatcherState.FindInitObj;
				return null;
			}
			this.matcherState = RebuildAsyncStatementsStep.MatcherState.Stopped;
			return node;
		}

		public override ICodeNode VisitUnaryExpression(UnaryExpression node)
		{
			node.Operand = (Expression)this.Visit(node.Operand);
			if (node.Operand == null)
			{
				return null;
			}
			return node;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.asyncData.VariableToFieldMap.ContainsKey(node.Variable))
			{
				FieldDefinition fieldDefinition = this.asyncData.VariableToFieldMap[node.Variable].Resolve();
				if (fieldDefinition != null && this.parameterMappings.ContainsKey(fieldDefinition))
				{
					return this.parameterMappings[fieldDefinition].CloneExpressionOnly();
				}
			}
			this.methodContext.VariablesToRename.Add(node.Variable.Resolve());
			return base.VisitVariableReferenceExpression(node);
		}

		private enum MatcherState
		{
			FindAwaitExpression = 0,
			FindIsCompletedInvoke = 1,
			FindGetResultInvoke = 2,
			FindInitObj = 4,
			Stopped = 8
		}
	}
}