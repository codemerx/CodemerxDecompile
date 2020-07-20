using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
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
				if (!this.enableEventAnalysis || this.get_Method().get_IsAddOn())
				{
					return false;
				}
				return !this.get_Method().get_IsRemoveOn();
			}
			set
			{
				this.enableEventAnalysis = value;
				return;
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
				return this.get_Body().get_Method();
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
			base();
			this.set_Body(body);
			this.set_Variables(new Collection<VariableDefinition>(body.get_Variables()));
			this.set_ControlFlowGraph(Telerik.JustDecompiler.Cil.ControlFlowGraph.Create(body.get_Method()));
			this.set_GotoLabels(new Dictionary<string, Statement>());
			this.set_GotoStatements(new List<GotoStatement>());
			this.set_IsMethodBodyChanged(false);
			this.set_VariableDefinitionToNameMap(new Dictionary<VariableDefinition, string>());
			this.set_VariableNamesCollection(new HashSet<string>());
			this.set_ParameterDefinitionToNameMap(new Dictionary<ParameterDefinition, string>());
			this.set_VariablesToRename(this.GetMethodVariablesToRename());
			this.set_LambdaVariablesCount(0);
			this.set_AnalysisResults(new DecompilationAnalysisResults());
			this.set_VariableAssignmentData(new Dictionary<VariableDefinition, AssignmentType>());
			this.set_OutParametersToAssign(new List<ParameterDefinition>());
			this.set_IsBaseConstructorInvokingConstructor(false);
			this.enableEventAnalysis = true;
			this.set_IsDestructor(false);
			this.set_UndeclaredLinqVariables(new HashSet<VariableDefinition>());
			this.set_ClosureVariableToFieldValue(new Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>>());
			this.set_VariablesToNotDeclare(new HashSet<VariableDefinition>());
			this.set_SwitchByStringData(new CompilerOptimizedSwitchByStringData());
			return;
		}

		public MethodSpecificContext(MethodBody body, Dictionary<VariableDefinition, string> variableDefinitionToNameMap, Dictionary<ParameterDefinition, string> parameterDefinitionTonameMap, MethodInvocationExpression ctorInvokeExpression)
		{
			this(body);
			this.set_VariableDefinitionToNameMap(variableDefinitionToNameMap);
			this.set_ParameterDefinitionToNameMap(parameterDefinitionTonameMap);
			this.set_CtorInvokeExpression(ctorInvokeExpression);
			return;
		}

		internal MethodSpecificContext(DecompilationAnalysisResults analysisResults, Telerik.JustDecompiler.Decompiler.YieldData yieldData, Telerik.JustDecompiler.Decompiler.AsyncData asyncData, bool isMethodBodyChanged, Dictionary<string, Statement> gotoLabels, List<GotoStatement> gotoStatements, StackUsageData stackData, bool isBaseConstructorInvokingConstructor, bool enableEventAnalysis, MethodBody body, Collection<VariableDefinition> variables, Telerik.JustDecompiler.Cil.ControlFlowGraph controlFlowGraph, ExpressionDecompilerData expressions, BlockLogicalConstruct logicalConstructsTree, LogicalFlowBuilderContext logicalConstructsContext, MethodInvocationExpression ctorInvokeExpression, Dictionary<Statement, ILogicalConstruct> statementToLogicalConstruct, Dictionary<ILogicalConstruct, List<Statement>> logicalConstructToStatements, Dictionary<VariableDefinition, string> variableDefinitionToNameMap, HashSet<string> variableNamesCollection, Dictionary<ParameterDefinition, string> parameterDefinitionToNameMap, HashSet<VariableDefinition> variablesToRename, Dictionary<FieldDefinition, Expression> fieldToExpression, int lambdaVariablesCount, Dictionary<VariableDefinition, AssignmentType> variableAssignmentData, List<ParameterDefinition> outParametersToAssign, bool isDestructor, BlockStatement destructorStatements, HashSet<VariableDefinition> undeclaredLinqVariables, Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>> closureVariableToFieldValue, HashSet<VariableDefinition> variablesToNotDeclare, CompilerOptimizedSwitchByStringData switchByStringData)
		{
			base();
			this.set_AnalysisResults(analysisResults);
			this.set_YieldData(yieldData);
			this.set_AsyncData(asyncData);
			this.set_IsMethodBodyChanged(isMethodBodyChanged);
			this.set_GotoLabels(gotoLabels);
			this.set_GotoStatements(gotoStatements);
			this.set_StackData(stackData);
			this.set_IsBaseConstructorInvokingConstructor(isBaseConstructorInvokingConstructor);
			this.set_EnableEventAnalysis(enableEventAnalysis);
			this.set_Body(body);
			this.set_Variables(variables);
			this.set_ControlFlowGraph(controlFlowGraph);
			this.set_Expressions(expressions);
			this.set_LogicalConstructsTree(logicalConstructsTree);
			this.set_LogicalConstructsContext(logicalConstructsContext);
			this.set_CtorInvokeExpression(ctorInvokeExpression);
			this.set_StatementToLogicalConstruct(statementToLogicalConstruct);
			this.set_LogicalConstructToStatements(logicalConstructToStatements);
			this.set_VariableDefinitionToNameMap(variableDefinitionToNameMap);
			this.set_VariableNamesCollection(variableNamesCollection);
			this.set_ParameterDefinitionToNameMap(parameterDefinitionToNameMap);
			this.set_VariablesToRename(variablesToRename);
			this.set_FieldToExpression(fieldToExpression);
			this.set_LambdaVariablesCount(lambdaVariablesCount);
			this.set_VariableAssignmentData(variableAssignmentData);
			this.set_OutParametersToAssign(outParametersToAssign);
			this.set_IsDestructor(isDestructor);
			this.set_DestructorStatements(destructorStatements);
			this.set_UndeclaredLinqVariables(undeclaredLinqVariables);
			this.set_ClosureVariableToFieldValue(closureVariableToFieldValue);
			this.set_VariablesToNotDeclare(variablesToNotDeclare);
			this.set_SwitchByStringData(switchByStringData);
			return;
		}

		internal void AddInnerMethodParametersToContext(MethodSpecificContext innerMethodContext)
		{
			this.get_ParameterDefinitionToNameMap().AddRange<ParameterDefinition, string>(innerMethodContext.get_ParameterDefinitionToNameMap());
			V_0 = innerMethodContext.get_Method().get_Parameters().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.get_ParameterDefinitionToNameMap().set_Item(V_1, V_1.get_Name());
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private HashSet<VariableDefinition> GetMethodVariablesToRename()
		{
			V_0 = new HashSet<VariableDefinition>();
			V_1 = this.get_Variables().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_VariableNameChanged())
					{
						continue;
					}
					dummyVar0 = V_0.Add(V_2);
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0;
		}

		internal void RemoveVariable(VariableReference reference)
		{
			this.RemoveVariable(reference.Resolve());
			return;
		}

		internal void RemoveVariable(VariableDefinition variable)
		{
			V_0 = this.get_Variables().IndexOf(variable);
			if (V_0 == -1)
			{
				return;
			}
			this.get_Variables().RemoveAt(V_0);
			return;
		}
	}
}