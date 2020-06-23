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
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildAnonymousDelegatesStep : BaseCodeVisitor, IDecompilationStep
	{
		private RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder theRebuilder;

		public RebuildAnonymousDelegatesStep()
		{
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.theRebuilder = new RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder(context, body);
			this.VisitBlockStatement(body);
			this.theRebuilder.CleanUpVariableCopyAssignments();
			return body;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			for (int i = 0; i < node.Statements.Count - 1; i++)
			{
				this.theRebuilder.Match(node, i);
			}
			base.VisitBlockStatement(node);
		}

		private class AnonymousDelegateRebuilder : BaseCodeTransformer
		{
			private VariableReference delegateVariableReference;

			private readonly DecompilationContext context;

			private Dictionary<FieldDefinition, Expression> fieldDefToAssignedValueMap;

			private readonly BlockStatement methodBodyBlock;

			private RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State state;

			private int startIndex;

			private readonly RebuildAnonymousDelegatesStep.FieldReferenceVisitor fieldVisitor;

			private TypeDefinition delegateTypeDef;

			private HashSet<Statement> statementsToKeep;

			private readonly RebuildAnonymousDelegatesStep.VariableCopyFinder variableCopyFinder;

			private HashSet<VariableReference> delegateCopies;

			private readonly HashSet<Statement> assignmentsToRemove;

			private readonly Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>> closuresArchive;

			private List<BlockStatement> delegatesFound;

			public AnonymousDelegateRebuilder(DecompilationContext context, BlockStatement methodBodyBlock)
			{
				this.context = context;
				this.methodBodyBlock = methodBodyBlock;
				if (context.MethodContext.FieldToExpression == null)
				{
					context.MethodContext.FieldToExpression = new Dictionary<FieldDefinition, Expression>();
				}
				this.fieldDefToAssignedValueMap = context.MethodContext.FieldToExpression;
				this.fieldVisitor = new RebuildAnonymousDelegatesStep.FieldReferenceVisitor(methodBodyBlock);
				this.variableCopyFinder = new RebuildAnonymousDelegatesStep.VariableCopyFinder(methodBodyBlock);
				this.assignmentsToRemove = new HashSet<Statement>();
				this.closuresArchive = context.MethodContext.ClosureVariableToFieldValue;
			}

			private bool CheckAssignExpression(ExpressionStatement theStatement)
			{
				if (theStatement == null || theStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
				{
					return false;
				}
				BinaryExpression expression = theStatement.Expression as BinaryExpression;
				if (!expression.IsAssignmentExpression)
				{
					return false;
				}
				RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State state = this.state;
				if (state == RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.DelegateCreate)
				{
					return this.CheckDelegateCreation(expression);
				}
				if (state != RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.FieldAssign)
				{
					return false;
				}
				return this.CheckFieldAssignment(expression, theStatement);
			}

			private bool CheckDelegateCreation(BinaryExpression theAssignExpression)
			{
				if (theAssignExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || theAssignExpression.Right.CodeNodeType != CodeNodeType.ObjectCreationExpression)
				{
					return false;
				}
				ObjectCreationExpression right = theAssignExpression.Right as ObjectCreationExpression;
				if (right.Arguments.Count != 0)
				{
					return false;
				}
				TypeDefinition typeDefinition = right.ExpressionType.Resolve();
				if (typeDefinition == null || !this.CheckTypeForCompilerGeneratedAttribute(typeDefinition))
				{
					return false;
				}
				this.delegateTypeDef = typeDefinition;
				this.delegateVariableReference = (theAssignExpression.Left as VariableReferenceExpression).Variable;
				return true;
			}

			private bool CheckFieldAssignment(BinaryExpression theAssignExpression, ExpressionStatement theStatement)
			{
				if (theAssignExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression)
				{
					return false;
				}
				FieldReferenceExpression left = theAssignExpression.Left as FieldReferenceExpression;
				if (left.Target == null || left.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression || (left.Target as VariableReferenceExpression).Variable != this.delegateVariableReference)
				{
					return false;
				}
				FieldDefinition right = left.Field.Resolve();
				if ((theAssignExpression.Right.CodeNodeType == CodeNodeType.ThisReferenceExpression || theAssignExpression.Right.CodeNodeType == CodeNodeType.ArgumentReferenceExpression || this.IsClosureVariableReference(theAssignExpression.Right as VariableReferenceExpression)) && !this.fieldVisitor.CheckForAnotherAssignment(right, left, this.delegateCopies))
				{
					this.fieldDefToAssignedValueMap[right] = theAssignExpression.Right;
				}
				else
				{
					this.statementsToKeep.Add(theStatement);
				}
				return true;
			}

			private bool CheckTypeForCompilerGeneratedAttribute(TypeDefinition typeDefinition)
			{
				while (typeDefinition.IsNested)
				{
					if (typeDefinition.HasCompilerGeneratedAttribute())
					{
						return true;
					}
					typeDefinition = typeDefinition.DeclaringType;
				}
				return false;
			}

			public void CleanUpVariableCopyAssignments()
			{
				foreach (Statement statement in new List<Statement>(this.assignmentsToRemove))
				{
					BlockStatement parent = statement.Parent as BlockStatement;
					if (parent == null)
					{
						continue;
					}
					parent.Statements.Remove(statement);
				}
			}

			private bool IsClosureVariableReference(VariableReferenceExpression varRefExpression)
			{
				if (varRefExpression == null)
				{
					return false;
				}
				return this.closuresArchive.ContainsKey(varRefExpression.Variable);
			}

			private void MapTheRestOfTheFieldsToVariables()
			{
				foreach (FieldDefinition field in this.delegateTypeDef.Fields)
				{
					if (this.fieldDefToAssignedValueMap.ContainsKey(field))
					{
						continue;
					}
					MethodSpecificContext methodContext = this.context.MethodContext;
					int lambdaVariablesCount = methodContext.LambdaVariablesCount;
					methodContext.LambdaVariablesCount = lambdaVariablesCount + 1;
					VariableDefinition variableDefinition = new VariableDefinition(String.Concat("lambdaVar", lambdaVariablesCount.ToString()), field.FieldType, this.context.MethodContext.Method);
					this.context.MethodContext.Variables.Add(variableDefinition);
					this.context.MethodContext.VariablesToRename.Add(variableDefinition);
					this.fieldDefToAssignedValueMap[field] = new VariableReferenceExpression(variableDefinition, null);
				}
			}

			public void Match(BlockStatement theBlock, int index)
			{
				int num;
				this.delegatesFound = new List<BlockStatement>();
				this.statementsToKeep = new HashSet<Statement>();
				this.delegateCopies = new HashSet<VariableReference>();
				this.fieldDefToAssignedValueMap = new Dictionary<FieldDefinition, Expression>();
				this.state = RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.DelegateCreate;
				if (!this.CheckAssignExpression(theBlock.Statements[index] as ExpressionStatement))
				{
					return;
				}
				this.delegateCopies.Add(this.delegateVariableReference);
				this.variableCopyFinder.FindCopiesOfDelegate(this.delegateCopies, this.assignmentsToRemove);
				this.startIndex = index;
				this.state = RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.FieldAssign;
				do
				{
					num = index + 1;
					index = num;
				}
				while (num < theBlock.Statements.Count && this.CheckAssignExpression(theBlock.Statements[index] as ExpressionStatement));
				if (index == theBlock.Statements.Count)
				{
					return;
				}
				this.MapTheRestOfTheFieldsToVariables();
				this.RemoveFieldAssignments(theBlock.Statements, index);
				this.CleanUpVariableCopyAssignments();
				this.state = RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.ReplaceFields;
				this.VisitBlockStatement(this.methodBodyBlock);
				this.state = RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.ReplaceDelegate;
				for (int i = this.startIndex; i < theBlock.Statements.Count; i++)
				{
					theBlock.Statements[i] = (Statement)this.Visit(theBlock.Statements[i]);
					this.state = RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.ReplaceFields;
					foreach (BlockStatement blockStatement in this.delegatesFound)
					{
						this.VisitBlockStatement(blockStatement);
					}
					this.state = RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.ReplaceDelegate;
					this.delegatesFound = new List<BlockStatement>();
				}
				this.SaveClosureToArchive();
			}

			private void RemoveFieldAssignments(StatementCollection theCollection, int currentIndex)
			{
				for (int i = this.startIndex; i < currentIndex; i++)
				{
					if (this.statementsToKeep.Contains(theCollection[i]))
					{
						int item = this.startIndex;
						this.startIndex = item + 1;
						theCollection[item] = theCollection[i];
					}
				}
				this.RemoveRange(theCollection, this.startIndex, currentIndex);
			}

			private void RemoveRange(StatementCollection theCollection, int from, int to)
			{
				while (to < theCollection.Count)
				{
					theCollection[from] = theCollection[to];
					from++;
					to++;
				}
				int count = theCollection.Count;
				while (from < count)
				{
					int num = count - 1;
					count = num;
					theCollection.RemoveAt(num);
				}
			}

			private void SaveClosureToArchive()
			{
				foreach (VariableReference delegateCopy in this.delegateCopies)
				{
					this.closuresArchive.Add(delegateCopy, this.fieldDefToAssignedValueMap);
				}
			}

			public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
			{
				Expression expression;
				Dictionary<FieldDefinition, Expression> fieldDefinitions;
				if (this.state != RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.ReplaceFields)
				{
					return base.VisitFieldReferenceExpression(node);
				}
				FieldDefinition fieldDefinition = node.Field.Resolve();
				if (fieldDefinition == null)
				{
					return base.VisitFieldReferenceExpression(node);
				}
				if (this.fieldDefToAssignedValueMap.TryGetValue(fieldDefinition, out expression))
				{
					return expression.CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
				}
				base.VisitFieldReferenceExpression(node);
				if (node.Target == null || node.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
				{
					return node;
				}
				VariableReference variable = (node.Target as VariableReferenceExpression).Variable;
				if (!this.closuresArchive.TryGetValue(variable, out fieldDefinitions) || !fieldDefinitions.TryGetValue(fieldDefinition, out expression))
				{
					return node;
				}
				return expression.CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
			}

			public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
			{
				if (this.state != RebuildAnonymousDelegatesStep.AnonymousDelegateRebuilder.State.ReplaceDelegate || node.Arguments == null || node.Arguments.Count != 2 || node.Arguments[0].CodeNodeType != CodeNodeType.VariableReferenceExpression || node.Arguments[1].CodeNodeType != CodeNodeType.MethodReferenceExpression || !this.delegateCopies.Contains((node.Arguments[0] as VariableReferenceExpression).Variable))
				{
					return base.VisitObjectCreationExpression(node);
				}
				TypeDefinition typeDefinition = node.Constructor.DeclaringType.Resolve();
				if (typeDefinition == null || typeDefinition.BaseType == null || typeDefinition.BaseType.FullName != "System.MulticastDelegate")
				{
					return base.VisitObjectCreationExpression(node);
				}
				MethodReference method = (node.Arguments[1] as MethodReferenceExpression).Method;
				MethodDefinition methodDefinition = (node.Arguments[1] as MethodReferenceExpression).MethodDefinition;
				MethodSpecificContext methodSpecificContext = new MethodSpecificContext(methodDefinition.Body);
				DecompilationContext decompilationContext = new DecompilationContext(methodSpecificContext, this.context.TypeContext, this.context.ModuleContext, this.context.AssemblyContext, this.context.Language);
				methodSpecificContext.FieldToExpression = this.fieldDefToAssignedValueMap;
				BlockStatement blockStatement = methodDefinition.Body.DecompileLambda(this.context.Language, decompilationContext);
				if (blockStatement.Statements.Count == 1 && blockStatement.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement && (blockStatement.Statements[0] as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.ReturnExpression)
				{
					ReturnExpression expression = (blockStatement.Statements[0] as ExpressionStatement).Expression as ReturnExpression;
					ShortFormReturnExpression shortFormReturnExpression = new ShortFormReturnExpression(expression.Value, expression.MappedInstructions);
					blockStatement = new BlockStatement();
					blockStatement.Statements.Add(new ExpressionStatement(shortFormReturnExpression));
				}
				this.context.MethodContext.VariableDefinitionToNameMap.AddRange<VariableDefinition, string>(decompilationContext.MethodContext.VariableDefinitionToNameMap);
				this.context.MethodContext.VariableNamesCollection.UnionWith(decompilationContext.MethodContext.VariableNamesCollection);
				this.context.MethodContext.AddInnerMethodParametersToContext(decompilationContext.MethodContext);
				this.context.MethodContext.GotoStatements.AddRange(decompilationContext.MethodContext.GotoStatements);
				this.context.MethodContext.GotoLabels.AddRange<string, Statement>(decompilationContext.MethodContext.GotoLabels);
				ExpressionCollection expressionCollection = new ExpressionCollection();
				bool flag = LambdaExpressionsHelper.HasAnonymousParameter(methodDefinition.Parameters);
				foreach (ParameterDefinition parameter in methodDefinition.Parameters)
				{
					expressionCollection.Add(new LambdaParameterExpression(parameter, !flag, null));
				}
				this.delegatesFound.Add(blockStatement);
				LambdaExpression lambdaExpression = new LambdaExpression(expressionCollection, blockStatement, methodDefinition.IsAsync(), methodDefinition.IsFunction(), method.Parameters, false, node.Arguments[1].MappedInstructions)
				{
					ExpressionType = typeDefinition
				};
				return new DelegateCreationExpression(node.Constructor.DeclaringType, lambdaExpression, node.Arguments[0], node.MappedInstructions);
			}

			private enum State
			{
				DelegateCreate,
				FieldAssign,
				ReplaceDelegate,
				ReplaceFields
			}
		}

		private class FieldReferenceVisitor : BaseCodeVisitor
		{
			private readonly BlockStatement theBlockStatement;

			private FieldDefinition fieldDef;

			private FieldReferenceExpression assignedReference;

			private bool foundUsage;

			private HashSet<VariableReference> delegateVariableCopies;

			public FieldReferenceVisitor(BlockStatement theBlockStatement)
			{
				this.theBlockStatement = theBlockStatement;
			}

			private void CheckFieldReference(FieldReferenceExpression node)
			{
				if (node != this.assignedReference && node.Target != null && node.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression && this.delegateVariableCopies.Contains((node.Target as VariableReferenceExpression).Variable) && node.Field.Resolve() == this.fieldDef)
				{
					this.foundUsage = true;
				}
			}

			public bool CheckForAnotherAssignment(FieldDefinition fieldDef, FieldReferenceExpression assignedReference, HashSet<VariableReference> delegateVariableCopies)
			{
				this.fieldDef = fieldDef;
				this.assignedReference = assignedReference;
				this.delegateVariableCopies = delegateVariableCopies;
				this.foundUsage = false;
				base.Visit(this.theBlockStatement);
				return this.foundUsage;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.foundUsage)
				{
					base.Visit(node);
				}
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (this.foundUsage)
				{
					return;
				}
				if (node.IsAssignmentExpression && node.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					this.CheckFieldReference(node.Left as FieldReferenceExpression);
					if (this.foundUsage)
					{
						return;
					}
				}
				base.VisitBinaryExpression(node);
			}
		}

		private class VariableCopyFinder : BaseCodeVisitor
		{
			private readonly BlockStatement theBlock;

			private HashSet<VariableReference> delegateVariablesSet;

			private HashSet<Statement> statementsToRemove;

			public VariableCopyFinder(BlockStatement theBlock)
			{
				this.theBlock = theBlock;
			}

			private bool CheckBinaryExpression(BinaryExpression node)
			{
				if (node.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || node.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression || !this.delegateVariablesSet.Contains((node.Right as VariableReferenceExpression).Variable))
				{
					return false;
				}
				this.delegateVariablesSet.Add((node.Left as VariableReferenceExpression).Variable);
				return true;
			}

			public void FindCopiesOfDelegate(HashSet<VariableReference> delegateVariablesSet, HashSet<Statement> statementsToRemove)
			{
				this.delegateVariablesSet = delegateVariablesSet;
				this.statementsToRemove = statementsToRemove;
				this.Visit(this.theBlock);
			}

			public override void VisitExpressionStatement(ExpressionStatement node)
			{
				if (!node.IsAssignmentStatement() || !this.CheckBinaryExpression(node.Expression as BinaryExpression))
				{
					base.VisitExpressionStatement(node);
					return;
				}
				this.statementsToRemove.Add(node);
			}
		}
	}
}