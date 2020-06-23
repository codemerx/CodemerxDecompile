using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps.CodePatterns;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildCatchClausesFilterStep : BaseCodeTransformer, IDecompilationStep
	{
		private const uint MaxRID = 0xffffff;

		private DecompilationContext context;

		private CatchClause currentCatchClause;

		private HashSet<VariableDefinition> variablesUsedOutsideFilters;

		private Dictionary<CatchClause, HashSet<VariableDefinition>> catchClausesUsedVariablesMap;

		private Dictionary<CatchClause, HashSet<ParameterDefinition>> catchClausesUsedParametersMap;

		private Dictionary<CatchClause, Dictionary<VariableDefinition, ParameterDefinition>> catchClausesVariablesToParametersMap;

		private List<FilterMethodToBeDecompiled> methodsToBeDecompiled;

		public RebuildCatchClausesFilterStep()
		{
			this.currentCatchClause = null;
			this.variablesUsedOutsideFilters = new HashSet<VariableDefinition>();
			this.catchClausesUsedVariablesMap = new Dictionary<CatchClause, HashSet<VariableDefinition>>();
			this.catchClausesUsedParametersMap = new Dictionary<CatchClause, HashSet<ParameterDefinition>>();
			this.catchClausesVariablesToParametersMap = new Dictionary<CatchClause, Dictionary<VariableDefinition, ParameterDefinition>>();
			this.methodsToBeDecompiled = new List<FilterMethodToBeDecompiled>();
		}

		private List<Expression> AddAllParameters(CatchClause catchClause, MethodDefinition method, VariableDeclarationExpression variable)
		{
			string name;
			List<Expression> expressions = new List<Expression>();
			method.Parameters.Add(this.CreateParameter("JustDecompileGenerated_Exception", variable.Variable.VariableType));
			expressions.Add(new UnaryExpression(UnaryOperator.AddressReference, new VariableReferenceExpression(variable.Variable, null), null));
			if (this.catchClausesUsedVariablesMap.ContainsKey(catchClause))
			{
				foreach (VariableDefinition item in this.catchClausesUsedVariablesMap[catchClause])
				{
					if (!this.variablesUsedOutsideFilters.Contains(item) || item == variable.Variable)
					{
						continue;
					}
					if (this.context.MethodContext.VariablesToRename.Contains(item))
					{
						name = null;
					}
					else
					{
						name = item.Name;
					}
					string str = name;
					ParameterDefinition parameterDefinition = this.CreateParameter(str, item.VariableType);
					if (str == null)
					{
						if (!this.catchClausesVariablesToParametersMap.ContainsKey(catchClause))
						{
							this.catchClausesVariablesToParametersMap.Add(catchClause, new Dictionary<VariableDefinition, ParameterDefinition>());
						}
						this.catchClausesVariablesToParametersMap[catchClause].Add(item, parameterDefinition);
					}
					method.Parameters.Add(parameterDefinition);
					expressions.Add(new UnaryExpression(UnaryOperator.AddressReference, new VariableReferenceExpression(item, null), null));
				}
			}
			if (this.catchClausesUsedParametersMap.ContainsKey(catchClause))
			{
				foreach (ParameterDefinition item1 in this.catchClausesUsedParametersMap[catchClause])
				{
					method.Parameters.Add(this.CreateParameter(item1.Name, item1.ParameterType));
					expressions.Add(new UnaryExpression(UnaryOperator.AddressReference, new ArgumentReferenceExpression(item1, null), null));
				}
			}
			return expressions;
		}

		private void AddReferencedParameter(ParameterDefinition parameterDefinition)
		{
			if (this.currentCatchClause != null)
			{
				if (!this.catchClausesUsedParametersMap.ContainsKey(this.currentCatchClause))
				{
					this.catchClausesUsedParametersMap.Add(this.currentCatchClause, new HashSet<ParameterDefinition>());
				}
				this.catchClausesUsedParametersMap[this.currentCatchClause].Add(parameterDefinition);
			}
		}

		private void AddReferencedVariable(VariableDefinition variableDefinition)
		{
			if (this.currentCatchClause == null)
			{
				this.variablesUsedOutsideFilters.Add(variableDefinition);
				return;
			}
			if (!this.catchClausesUsedVariablesMap.ContainsKey(this.currentCatchClause))
			{
				this.catchClausesUsedVariablesMap.Add(this.currentCatchClause, new HashSet<VariableDefinition>());
			}
			this.catchClausesUsedVariablesMap[this.currentCatchClause].Add(variableDefinition);
		}

		private void AddVariablesToNotDeclare(DecompilationContext context, CatchClause currentCatch)
		{
			foreach (VariableDefinition variable in context.MethodContext.Variables)
			{
				if (this.variablesUsedOutsideFilters.Contains(variable))
				{
					context.MethodContext.VariablesToNotDeclare.Add(variable);
				}
				foreach (KeyValuePair<CatchClause, HashSet<VariableDefinition>> keyValuePair in this.catchClausesUsedVariablesMap)
				{
					if (keyValuePair.Key.Equals(currentCatch) || !keyValuePair.Value.Contains(variable))
					{
						continue;
					}
					context.MethodContext.VariablesToNotDeclare.Add(variable);
				}
			}
		}

		private MethodSpecificContext CloneAndReplaceMethodBody(MethodSpecificContext context, MethodBody methodBody)
		{
			return new MethodSpecificContext(context.AnalysisResults, context.YieldData, context.AsyncData, context.IsMethodBodyChanged, new Dictionary<string, Statement>(context.GotoLabels), new List<GotoStatement>(context.GotoStatements), context.StackData, context.IsBaseConstructorInvokingConstructor, context.EnableEventAnalysis, methodBody, new Mono.Collections.Generic.Collection<VariableDefinition>(context.Variables), context.ControlFlowGraph, context.Expressions, context.LogicalConstructsTree, context.LogicalConstructsContext, context.CtorInvokeExpression, new Dictionary<Statement, ILogicalConstruct>(context.StatementToLogicalConstruct), new Dictionary<ILogicalConstruct, List<Statement>>(context.LogicalConstructToStatements), new Dictionary<VariableDefinition, string>(context.VariableDefinitionToNameMap), new HashSet<string>(context.VariableNamesCollection), new Dictionary<ParameterDefinition, string>(context.ParameterDefinitionToNameMap), new HashSet<VariableDefinition>(context.VariablesToRename), new Dictionary<FieldDefinition, Expression>(context.FieldToExpression), context.LambdaVariablesCount, new Dictionary<VariableDefinition, AssignmentType>(context.VariableAssignmentData), new List<ParameterDefinition>(context.OutParametersToAssign), context.IsDestructor, context.DestructorStatements, new HashSet<VariableDefinition>(context.UndeclaredLinqVariables), new Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>>(context.ClosureVariableToFieldValue), new HashSet<VariableDefinition>(context.VariablesToNotDeclare), context.SwitchByStringData.Clone() as CompilerOptimizedSwitchByStringData);
		}

		private void CreateMethod(CatchClause catchClause, VariableDeclarationExpression variable, out Expression methodInvocationExpression)
		{
			methodInvocationExpression = null;
			BlockStatement filter = catchClause.Filter as BlockStatement;
			VariableReferenceExpression expression = (((filter.Statements.First<Statement>() as ExpressionStatement).Expression as BinaryExpression).Right as SafeCastExpression).Expression as VariableReferenceExpression;
			expression.Variable = variable.Variable;
			ExpressionStatement returnExpression = filter.Statements.Last<Statement>() as ExpressionStatement;
			returnExpression.Expression = new ReturnExpression(returnExpression.Expression, returnExpression.Expression.MappedInstructions);
			int count = this.context.TypeContext.GeneratedFilterMethods.Count + this.methodsToBeDecompiled.Count;
			MethodDefinition methodDefinition = new MethodDefinition(String.Format("JustDecompileGenerated_Filter_{0}", count), MethodAttributes.Private, this.context.MethodContext.Method.Module.TypeSystem.Boolean)
			{
				Body = new MethodBody(methodDefinition),
				MetadataToken = new MetadataToken(Mono.Cecil.TokenType.Method, (uint)(0xffffff - count)),
				IsStatic = this.context.MethodContext.Method.IsStatic,
				HasThis = !methodDefinition.IsStatic,
				DeclaringType = this.context.MethodContext.Method.DeclaringType,
				SemanticsAttributes = MethodSemanticsAttributes.None,
				IsJustDecompileGenerated = true
			};
			DecompilationContext decompilationContext = new DecompilationContext(this.CloneAndReplaceMethodBody(this.context.MethodContext, methodDefinition.Body), this.context.TypeContext, this.context.Language);
			VariableDefinition variableDefinition = expression.Variable.Resolve();
			if (decompilationContext.MethodContext.VariableDefinitionToNameMap.ContainsKey(variableDefinition))
			{
				decompilationContext.MethodContext.VariableDefinitionToNameMap.Add(variableDefinition, "JustDecompileGenerated_Exception");
			}
			else
			{
				decompilationContext.MethodContext.VariableDefinitionToNameMap[variableDefinition] = "JustDecompileGenerated_Exception";
			}
			decompilationContext.MethodContext.VariablesToNotDeclare.Add(variable.Variable);
			this.methodsToBeDecompiled.Add(new FilterMethodToBeDecompiled(methodDefinition, catchClause, decompilationContext, catchClause.Filter as BlockStatement));
			List<Expression> expressions = this.AddAllParameters(catchClause, methodDefinition, variable);
			methodInvocationExpression = this.CreateMethodInvocation(methodDefinition, expressions);
		}

		private MethodInvocationExpression CreateMethodInvocation(MethodDefinition method, List<Expression> arguments)
		{
			Expression thisReferenceExpression;
			if (method.IsStatic)
			{
				thisReferenceExpression = null;
			}
			else
			{
				thisReferenceExpression = new ThisReferenceExpression(method.DeclaringType, null);
			}
			MethodInvocationExpression methodInvocationExpression = new MethodInvocationExpression(new MethodReferenceExpression(thisReferenceExpression, method, null), null);
			foreach (Expression argument in arguments)
			{
				methodInvocationExpression.Arguments.Add(argument);
			}
			return methodInvocationExpression;
		}

		private ParameterDefinition CreateParameter(string name, TypeReference type)
		{
			return new ParameterDefinition(name, ParameterAttributes.None, new ByReferenceType(type));
		}

		private void DecompileMethods()
		{
			foreach (FilterMethodToBeDecompiled filterMethodToBeDecompiled in this.methodsToBeDecompiled)
			{
				this.AddVariablesToNotDeclare(filterMethodToBeDecompiled.Context, filterMethodToBeDecompiled.CatchClause);
				BlockDecompilationPipeline blockDecompilationPipeline = this.context.Language.CreateFilterMethodPipeline(filterMethodToBeDecompiled.Context);
				DecompilationContext decompilationContext = blockDecompilationPipeline.Run(filterMethodToBeDecompiled.Method.Body, filterMethodToBeDecompiled.Block, this.context.Language);
				this.context.TypeContext.GeneratedFilterMethods.Add(new GeneratedMethod(filterMethodToBeDecompiled.Method, blockDecompilationPipeline.Body, decompilationContext.MethodContext));
				this.context.TypeContext.GeneratedMethodDefinitionToNameMap.Add(filterMethodToBeDecompiled.Method, filterMethodToBeDecompiled.Method.Name);
				this.FixVariablesNames(decompilationContext, filterMethodToBeDecompiled.CatchClause);
			}
		}

		private void FixVariablesNames(DecompilationContext innerContext, CatchClause currentCatch)
		{
			if (this.catchClausesVariablesToParametersMap.ContainsKey(currentCatch))
			{
				foreach (KeyValuePair<VariableDefinition, ParameterDefinition> item in this.catchClausesVariablesToParametersMap[currentCatch])
				{
					innerContext.MethodContext.VariableDefinitionToNameMap[item.Key] = innerContext.MethodContext.ParameterDefinitionToNameMap[item.Value];
				}
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			if (!context.MethodContext.Body.ExceptionHandlers.Any<ExceptionHandler>((ExceptionHandler eh) => eh.HandlerType == ExceptionHandlerType.Filter))
			{
				return body;
			}
			this.context = context;
			body = (BlockStatement)this.Visit(body);
			this.RemoveVariablesUsedOnlyInFilters();
			this.DecompileMethods();
			return body;
		}

		private void RemoveVariablesUsedOnlyInFilters()
		{
			foreach (VariableDefinition variable in this.context.MethodContext.Variables)
			{
				if (this.variablesUsedOutsideFilters.Contains(variable) || this.context.MethodContext.VariablesToNotDeclare.Contains(variable))
				{
					continue;
				}
				this.context.MethodContext.VariablesToNotDeclare.Add(variable);
			}
		}

		public override ICodeNode VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			this.AddReferencedParameter(node.Parameter.Resolve());
			return node;
		}

		public override ICodeNode VisitCatchClause(CatchClause node)
		{
			VariableDeclarationExpression variableDeclarationExpression;
			Expression expression;
			node.Body = (BlockStatement)base.Visit(node.Body);
			if (node.Filter == null || !(node.Filter is BlockStatement))
			{
				return node;
			}
			this.currentCatchClause = node;
			node.Filter = (Statement)base.Visit(node.Filter);
			this.currentCatchClause = null;
			bool flag = CatchClausesFilterPattern.TryMatch(node.Filter as BlockStatement, out variableDeclarationExpression, out expression);
			if (!flag)
			{
				if (variableDeclarationExpression == null || variableDeclarationExpression.ExpressionType.FullName == "System.Object" || !CatchClausesFilterPattern.TryMatchMethodStructure(node.Filter as BlockStatement))
				{
					throw new NotSupportedException("Unsupported structure of filter clause.");
				}
				this.CreateMethod(node, variableDeclarationExpression, out expression);
			}
			this.context.MethodContext.VariablesToNotDeclare.Add(variableDeclarationExpression.Variable);
			if (!flag)
			{
				node.Variable = variableDeclarationExpression.CloneExpressionOnly() as VariableDeclarationExpression;
			}
			else
			{
				node.Variable = variableDeclarationExpression;
			}
			node.Type = variableDeclarationExpression.ExpressionType;
			node.Filter = new ExpressionStatement(expression);
			return node;
		}

		public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.AddReferencedVariable(node.Variable.Resolve());
			return node;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			this.AddReferencedVariable(node.Variable.Resolve());
			return node;
		}
	}
}