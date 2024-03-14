using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler
{
	public class MethodSpecificContext
	{
		private bool enableEventAnalysis;

		public DecompilationAnalysisResults AnalysisResults
		{
			get;
			set;
		}

		internal Telerik.JustDecompiler.Decompiler.AsyncData AsyncData
		{
			get;
			set;
		}

		internal MethodBody Body
		{
			get;
			private set;
		}

		public Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>> ClosureVariableToFieldValue
		{
			get;
			private set;
		}

		internal Telerik.JustDecompiler.Cil.ControlFlowGraph ControlFlowGraph
		{
			get;
			private set;
		}

		public MethodInvocationExpression CtorInvokeExpression
		{
			get;
			set;
		}

		public BlockStatement DestructorStatements
		{
			get;
			set;
		}

		internal bool EnableEventAnalysis
		{
			get
			{
				if (!this.enableEventAnalysis || this.Method.get_IsAddOn())
				{
					return false;
				}
				return !this.Method.get_IsRemoveOn();
			}
			set
			{
				this.enableEventAnalysis = value;
			}
		}

		internal ExpressionDecompilerData Expressions
		{
			get;
			set;
		}

		public Dictionary<FieldDefinition, Expression> FieldToExpression
		{
			get;
			set;
		}

		internal Dictionary<string, Statement> GotoLabels
		{
			get;
			private set;
		}

		internal List<GotoStatement> GotoStatements
		{
			get;
			private set;
		}

		public bool IsBaseConstructorInvokingConstructor
		{
			get;
			set;
		}

		public bool IsDestructor
		{
			get;
			set;
		}

		internal bool IsMethodBodyChanged
		{
			get;
			set;
		}

		public int LambdaVariablesCount
		{
			get;
			set;
		}

		internal LogicalFlowBuilderContext LogicalConstructsContext
		{
			get;
			set;
		}

		internal BlockLogicalConstruct LogicalConstructsTree
		{
			get;
			set;
		}

		internal Dictionary<ILogicalConstruct, List<Statement>> LogicalConstructToStatements
		{
			get;
			set;
		}

		internal MethodDefinition Method
		{
			get
			{
				return this.Body.get_Method();
			}
		}

		internal List<ParameterDefinition> OutParametersToAssign
		{
			get;
			private set;
		}

		internal Dictionary<ParameterDefinition, string> ParameterDefinitionToNameMap
		{
			get;
			set;
		}

		internal StackUsageData StackData
		{
			get;
			set;
		}

		internal Dictionary<Statement, ILogicalConstruct> StatementToLogicalConstruct
		{
			get;
			set;
		}

		public CompilerOptimizedSwitchByStringData SwitchByStringData
		{
			get;
			set;
		}

		public HashSet<VariableDefinition> UndeclaredLinqVariables
		{
			get;
			private set;
		}

		internal Dictionary<VariableDefinition, AssignmentType> VariableAssignmentData
		{
			get;
			set;
		}

		internal Dictionary<VariableDefinition, string> VariableDefinitionToNameMap
		{
			get;
			set;
		}

		internal HashSet<string> VariableNamesCollection
		{
			get;
			set;
		}

		internal Collection<VariableDefinition> Variables
		{
			get;
			private set;
		}

		public HashSet<VariableDefinition> VariablesToNotDeclare
		{
			get;
			private set;
		}

		internal HashSet<VariableDefinition> VariablesToRename
		{
			get;
			private set;
		}

		internal Telerik.JustDecompiler.Decompiler.YieldData YieldData
		{
			get;
			set;
		}

		public MethodSpecificContext(MethodBody body)
		{
			this.Body = body;
			this.Variables = new Collection<VariableDefinition>(body.get_Variables());
			this.ControlFlowGraph = Telerik.JustDecompiler.Cil.ControlFlowGraph.Create(body.get_Method());
			this.GotoLabels = new Dictionary<string, Statement>();
			this.GotoStatements = new List<GotoStatement>();
			this.IsMethodBodyChanged = false;
			this.VariableDefinitionToNameMap = new Dictionary<VariableDefinition, string>();
			this.VariableNamesCollection = new HashSet<string>();
			this.ParameterDefinitionToNameMap = new Dictionary<ParameterDefinition, string>();
			this.VariablesToRename = this.GetMethodVariablesToRename();
			this.LambdaVariablesCount = 0;
			this.AnalysisResults = new DecompilationAnalysisResults();
			this.VariableAssignmentData = new Dictionary<VariableDefinition, AssignmentType>();
			this.OutParametersToAssign = new List<ParameterDefinition>();
			this.IsBaseConstructorInvokingConstructor = false;
			this.enableEventAnalysis = true;
			this.IsDestructor = false;
			this.UndeclaredLinqVariables = new HashSet<VariableDefinition>();
			this.ClosureVariableToFieldValue = new Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>>();
			this.VariablesToNotDeclare = new HashSet<VariableDefinition>();
			this.SwitchByStringData = new CompilerOptimizedSwitchByStringData();
		}

		public MethodSpecificContext(MethodBody body, Dictionary<VariableDefinition, string> variableDefinitionToNameMap, Dictionary<ParameterDefinition, string> parameterDefinitionTonameMap, MethodInvocationExpression ctorInvokeExpression) : this(body)
		{
			this.VariableDefinitionToNameMap = variableDefinitionToNameMap;
			this.ParameterDefinitionToNameMap = parameterDefinitionTonameMap;
			this.CtorInvokeExpression = ctorInvokeExpression;
		}

		internal MethodSpecificContext(DecompilationAnalysisResults analysisResults, Telerik.JustDecompiler.Decompiler.YieldData yieldData, Telerik.JustDecompiler.Decompiler.AsyncData asyncData, bool isMethodBodyChanged, Dictionary<string, Statement> gotoLabels, List<GotoStatement> gotoStatements, StackUsageData stackData, bool isBaseConstructorInvokingConstructor, bool enableEventAnalysis, MethodBody body, Collection<VariableDefinition> variables, Telerik.JustDecompiler.Cil.ControlFlowGraph controlFlowGraph, ExpressionDecompilerData expressions, BlockLogicalConstruct logicalConstructsTree, LogicalFlowBuilderContext logicalConstructsContext, MethodInvocationExpression ctorInvokeExpression, Dictionary<Statement, ILogicalConstruct> statementToLogicalConstruct, Dictionary<ILogicalConstruct, List<Statement>> logicalConstructToStatements, Dictionary<VariableDefinition, string> variableDefinitionToNameMap, HashSet<string> variableNamesCollection, Dictionary<ParameterDefinition, string> parameterDefinitionToNameMap, HashSet<VariableDefinition> variablesToRename, Dictionary<FieldDefinition, Expression> fieldToExpression, int lambdaVariablesCount, Dictionary<VariableDefinition, AssignmentType> variableAssignmentData, List<ParameterDefinition> outParametersToAssign, bool isDestructor, BlockStatement destructorStatements, HashSet<VariableDefinition> undeclaredLinqVariables, Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>> closureVariableToFieldValue, HashSet<VariableDefinition> variablesToNotDeclare, CompilerOptimizedSwitchByStringData switchByStringData)
		{
			this.AnalysisResults = analysisResults;
			this.YieldData = yieldData;
			this.AsyncData = asyncData;
			this.IsMethodBodyChanged = isMethodBodyChanged;
			this.GotoLabels = gotoLabels;
			this.GotoStatements = gotoStatements;
			this.StackData = stackData;
			this.IsBaseConstructorInvokingConstructor = isBaseConstructorInvokingConstructor;
			this.EnableEventAnalysis = enableEventAnalysis;
			this.Body = body;
			this.Variables = variables;
			this.ControlFlowGraph = controlFlowGraph;
			this.Expressions = expressions;
			this.LogicalConstructsTree = logicalConstructsTree;
			this.LogicalConstructsContext = logicalConstructsContext;
			this.CtorInvokeExpression = ctorInvokeExpression;
			this.StatementToLogicalConstruct = statementToLogicalConstruct;
			this.LogicalConstructToStatements = logicalConstructToStatements;
			this.VariableDefinitionToNameMap = variableDefinitionToNameMap;
			this.VariableNamesCollection = variableNamesCollection;
			this.ParameterDefinitionToNameMap = parameterDefinitionToNameMap;
			this.VariablesToRename = variablesToRename;
			this.FieldToExpression = fieldToExpression;
			this.LambdaVariablesCount = lambdaVariablesCount;
			this.VariableAssignmentData = variableAssignmentData;
			this.OutParametersToAssign = outParametersToAssign;
			this.IsDestructor = isDestructor;
			this.DestructorStatements = destructorStatements;
			this.UndeclaredLinqVariables = undeclaredLinqVariables;
			this.ClosureVariableToFieldValue = closureVariableToFieldValue;
			this.VariablesToNotDeclare = variablesToNotDeclare;
			this.SwitchByStringData = switchByStringData;
		}

		internal void AddInnerMethodParametersToContext(MethodSpecificContext innerMethodContext)
		{
			this.ParameterDefinitionToNameMap.AddRange<ParameterDefinition, string>(innerMethodContext.ParameterDefinitionToNameMap);
			foreach (ParameterDefinition parameter in innerMethodContext.Method.get_Parameters())
			{
				this.ParameterDefinitionToNameMap[parameter] = parameter.get_Name();
			}
		}

		private HashSet<VariableDefinition> GetMethodVariablesToRename()
		{
			HashSet<VariableDefinition> variableDefinitions = new HashSet<VariableDefinition>();
			foreach (VariableDefinition variable in this.Variables)
			{
				if (variable.get_VariableNameChanged())
				{
					continue;
				}
				variableDefinitions.Add(variable);
			}
			return variableDefinitions;
		}

		internal void RemoveVariable(VariableReference reference)
		{
			this.RemoveVariable(reference.Resolve());
		}

		internal void RemoveVariable(VariableDefinition variable)
		{
			int num = this.Variables.IndexOf(variable);
			if (num == -1)
			{
				return;
			}
			this.Variables.RemoveAt(num);
		}
	}
}