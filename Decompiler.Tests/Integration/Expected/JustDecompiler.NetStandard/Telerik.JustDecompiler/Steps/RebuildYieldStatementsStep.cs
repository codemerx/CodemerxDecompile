using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildYieldStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext decompilationContext;

		private TypeDefinition yieldDeclaringType;

		private StatementCollection statements;

		private YieldData yieldData;

		private readonly Dictionary<FieldDefinition, Expression> parameterMappings = new Dictionary<FieldDefinition, Expression>();

		private StatementCollection newStatements;

		public RebuildYieldStatementsStep()
		{
		}

		private bool CheckFieldReference(Expression expression)
		{
			if (expression.CodeNodeType == CodeNodeType.FieldReferenceExpression)
			{
				FieldDefinition fieldDefinition = (expression as FieldReferenceExpression).Field.Resolve();
				if (fieldDefinition == this.yieldData.FieldsInfo.CurrentItemField || fieldDefinition == this.yieldData.FieldsInfo.StateHolderField)
				{
					return true;
				}
			}
			return false;
		}

		private bool CheckVariableReference(Expression expression)
		{
			VariableReference variable = null;
			if (expression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				variable = (expression as VariableReferenceExpression).Variable;
			}
			else if (expression.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
			{
				variable = (expression as VariableDeclarationExpression).Variable;
			}
			if (variable != null && variable == this.yieldData.FieldsInfo.ReturnFlagVariable)
			{
				return true;
			}
			return false;
		}

		private StatementCollection GetEnumeratorStatements()
		{
			BlockStatement blockStatement;
			MethodDefinition methodDefinition = null;
			foreach (MethodDefinition method in this.yieldDeclaringType.Methods)
			{
				if (!method.Name.EndsWith(".GetEnumerator"))
				{
					continue;
				}
				methodDefinition = method;
				if (methodDefinition == null)
				{
					return null;
				}
				if (methodDefinition.Body != null)
				{
					blockStatement = methodDefinition.Body.Decompile(this.decompilationContext.Language, (TypeSpecificContext)null);
				}
				else
				{
					blockStatement = null;
				}
				return blockStatement.Statements;
			}
			if (methodDefinition == null)
			{
				return null;
			}
			if (methodDefinition.Body != null)
			{
				blockStatement = methodDefinition.Body.Decompile(this.decompilationContext.Language, (TypeSpecificContext)null);
			}
			else
			{
				blockStatement = null;
			}
			return blockStatement.Statements;
		}

		private string GetFriendlyName(string fieldName)
		{
			if (fieldName[0] == '<')
			{
				int num = fieldName.IndexOf('>');
				if (num > 1)
				{
					return fieldName.Substring(1, num - 1);
				}
			}
			return fieldName;
		}

		private TypeDefinition GetGeneratedType()
		{
			ObjectCreationExpression value;
			if (this.statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return null;
			}
			ExpressionStatement item = this.statements[0] as ExpressionStatement;
			if (item.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				if (item.Expression.CodeNodeType != CodeNodeType.ReturnExpression)
				{
					return null;
				}
				ReturnExpression expression = item.Expression as ReturnExpression;
				if (expression.Value == null || expression.Value.CodeNodeType != CodeNodeType.ObjectCreationExpression)
				{
					return null;
				}
				value = expression.Value as ObjectCreationExpression;
			}
			else
			{
				if (!(item.Expression as BinaryExpression).IsAssignmentExpression)
				{
					return null;
				}
				BinaryExpression binaryExpression = item.Expression as BinaryExpression;
				if (binaryExpression.Right.CodeNodeType != CodeNodeType.ObjectCreationExpression)
				{
					return null;
				}
				value = binaryExpression.Right as ObjectCreationExpression;
			}
			MethodReference constructor = value.Constructor;
			if (constructor == null || constructor.DeclaringType == null)
			{
				return null;
			}
			TypeDefinition typeDefinition = constructor.DeclaringType.Resolve();
			if (typeDefinition != null && typeDefinition.IsNestedPrivate && typeDefinition.HasCompilerGeneratedAttribute())
			{
				return typeDefinition;
			}
			return null;
		}

		private IEnumerable<Statement> GetStatements()
		{
			BlockStatement blockStatement;
			MethodDefinition methodDefinition = null;
			foreach (MethodDefinition method in this.yieldDeclaringType.Methods)
			{
				if (method.Name != "MoveNext")
				{
					continue;
				}
				methodDefinition = method;
				if (methodDefinition == null || methodDefinition.Body == null)
				{
					return null;
				}
				blockStatement = methodDefinition.Body.DecompileYieldStateMachine(this.decompilationContext, out this.yieldData);
				if (blockStatement == null)
				{
					return null;
				}
				return this.GetStatements(blockStatement);
			}
			if (methodDefinition == null || methodDefinition.Body == null)
			{
				return null;
			}
			blockStatement = methodDefinition.Body.DecompileYieldStateMachine(this.decompilationContext, out this.yieldData);
			if (blockStatement == null)
			{
				return null;
			}
			return this.GetStatements(blockStatement);
		}

		private IEnumerable<Statement> GetStatements(BlockStatement moveNextBody)
		{
			List<Statement> statements = new List<Statement>();
			for (int i = 0; i < moveNextBody.Statements.Count; i++)
			{
				Statement item = moveNextBody.Statements[i];
				TryStatement tryStatement = item as TryStatement;
				if ((this.yieldData.StateMachineVersion != YieldStateMachineVersion.V1 || tryStatement == null || tryStatement.Fault == null) && (this.yieldData.StateMachineVersion != YieldStateMachineVersion.V2 || tryStatement == null || tryStatement.CatchClauses.Count != 1))
				{
					statements.Add(item);
				}
				else
				{
					statements.AddRange(tryStatement.Try.Statements);
				}
			}
			return statements;
		}

		private bool Match(StatementCollection statements)
		{
			this.statements = statements;
			this.yieldDeclaringType = this.GetGeneratedType();
			if (this.yieldDeclaringType == null)
			{
				return false;
			}
			IEnumerable<Statement> statements1 = this.GetStatements();
			if (statements1 == null || this.yieldData == null)
			{
				return false;
			}
			if (statements.Count > 2)
			{
				this.SetParameterMappings();
				StatementCollection enumeratorStatements = this.GetEnumeratorStatements();
				if (enumeratorStatements != null)
				{
					this.PostProcessMappings(enumeratorStatements);
				}
			}
			this.newStatements = new StatementCollection();
			foreach (Statement statement in statements1)
			{
				this.newStatements.Add(statement);
			}
			return true;
		}

		private void PostProcessMappings(StatementCollection getEnumeratorStatements)
		{
			Expression expression;
			foreach (Statement getEnumeratorStatement in getEnumeratorStatements)
			{
				if (getEnumeratorStatement.CodeNodeType != CodeNodeType.ExpressionStatement || (getEnumeratorStatement as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.BinaryExpression)
				{
					continue;
				}
				BinaryExpression binaryExpression = (getEnumeratorStatement as ExpressionStatement).Expression as BinaryExpression;
				if (!binaryExpression.IsAssignmentExpression || binaryExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || binaryExpression.Right.CodeNodeType != CodeNodeType.FieldReferenceExpression)
				{
					continue;
				}
				FieldDefinition fieldDefinition = (binaryExpression.Left as FieldReferenceExpression).Field.Resolve();
				FieldDefinition fieldDefinition1 = (binaryExpression.Right as FieldReferenceExpression).Field.Resolve();
				if (!this.parameterMappings.TryGetValue(fieldDefinition1, out expression))
				{
					continue;
				}
				this.parameterMappings.Remove(fieldDefinition1);
				this.parameterMappings[fieldDefinition] = expression;
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.decompilationContext = context;
			if (!this.Match(body.Statements))
			{
				return body;
			}
			body.Statements = this.newStatements;
			body = (BlockStatement)this.Visit(body);
			this.RemoveLastIfYieldBreak(body.Statements);
			return body;
		}

		private void RemoveLastIfYieldBreak(StatementCollection collection)
		{
			int count = collection.Count - 1;
			Statement item = collection[count];
			if (item.CodeNodeType == CodeNodeType.ExpressionStatement && (item as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.YieldBreakExpression && String.IsNullOrEmpty(item.Label) && (this.yieldData.YieldBreaks.Count != 1 || this.yieldData.YieldReturns.Count != 0))
			{
				collection.RemoveAt(count);
			}
		}

		private void SetParameterMappings()
		{
			for (int i = 1; i < this.statements.Count; i++)
			{
				if (this.statements[i].CodeNodeType == CodeNodeType.ExpressionStatement)
				{
					ExpressionStatement item = this.statements[i] as ExpressionStatement;
					if (item.Expression.CodeNodeType == CodeNodeType.BinaryExpression && (item.Expression as BinaryExpression).IsAssignmentExpression)
					{
						BinaryExpression expression = item.Expression as BinaryExpression;
						if (expression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
						{
							FieldReference field = (expression.Left as FieldReferenceExpression).Field;
							if (field.DeclaringType.Resolve() == this.yieldDeclaringType)
							{
								this.parameterMappings[field.Resolve()] = expression.Right;
							}
						}
					}
				}
			}
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.IsAssignmentExpression && (this.CheckFieldReference(node.Left) || this.CheckFieldReference(node.Right) || this.CheckVariableReference(node.Left) || this.CheckVariableReference(node.Right) || node.Right.CodeNodeType == CodeNodeType.ThisReferenceExpression))
			{
				return null;
			}
			return base.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			Expression expression = (Expression)this.Visit(node.Expression);
			if (expression != null)
			{
				node.Expression = expression;
				return node;
			}
			if (!String.IsNullOrEmpty(node.Label))
			{
				Statement nextStatement = node.GetNextStatement();
				if (nextStatement == null || !String.IsNullOrEmpty(nextStatement.Label))
				{
					return new EmptyStatement()
					{
						Label = node.Label
					};
				}
				nextStatement.Label = node.Label;
			}
			return null;
		}

		public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
		{
			if (node.Field.DeclaringType.Resolve() != this.yieldDeclaringType)
			{
				return base.VisitFieldReferenceExpression(node);
			}
			FieldDefinition fieldDefinition = node.Field.Resolve();
			if (this.parameterMappings.ContainsKey(fieldDefinition))
			{
				return this.parameterMappings[fieldDefinition].CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
			}
			VariableDefinition variableDefinition = new VariableDefinition(this.GetFriendlyName(fieldDefinition.Name), fieldDefinition.FieldType, this.decompilationContext.MethodContext.Method);
			this.decompilationContext.MethodContext.Variables.Add(variableDefinition);
			this.decompilationContext.MethodContext.VariableAssignmentData.Add(variableDefinition, this.yieldData.FieldAssignmentData[fieldDefinition]);
			this.decompilationContext.MethodContext.VariablesToRename.Add(variableDefinition);
			VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(variableDefinition, node.UnderlyingSameMethodInstructions);
			this.parameterMappings[fieldDefinition] = variableReferenceExpression;
			return variableReferenceExpression;
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (node.MethodExpression.Target != null && node.MethodExpression.Target.CodeNodeType == CodeNodeType.ThisReferenceExpression)
			{
				return null;
			}
			return base.VisitMethodInvocationExpression(node);
		}

		public override ICodeNode VisitTryStatement(TryStatement node)
		{
			if (node.Finally != null && node.Finally.Body.Statements.Count == 1 && node.Finally.Body.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement)
			{
				ExpressionStatement item = node.Finally.Body.Statements[0] as ExpressionStatement;
				if (item.Expression.CodeNodeType == CodeNodeType.MethodInvocationExpression)
				{
					MethodReferenceExpression methodExpression = (item.Expression as MethodInvocationExpression).MethodExpression;
					if (methodExpression != null && methodExpression.Method != null && methodExpression.Method.DeclaringType != null && methodExpression.Method.DeclaringType.Resolve() == this.yieldDeclaringType)
					{
						node.Finally = new FinallyClause(methodExpression.Method.Resolve().Body.Decompile(this.decompilationContext.Language, (TypeSpecificContext)null), node.Finally.UnderlyingSameMethodInstructions);
					}
				}
			}
			return base.VisitTryStatement(node);
		}

		public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.decompilationContext.MethodContext.VariablesToRename.Add(node.Variable);
			return base.VisitVariableDeclarationExpression(node);
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			this.decompilationContext.MethodContext.VariablesToRename.Add(node.Variable.Resolve());
			return base.VisitVariableReferenceExpression(node);
		}
	}
}