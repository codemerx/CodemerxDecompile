using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
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
        public MethodSpecificContext(MethodBody body)
        {
            this.Body = body;
            this.Variables = new Collection<VariableDefinition>(body.Variables);
            this.ControlFlowGraph = ControlFlowGraph.Create(body.Method);
            this.GotoLabels = new Dictionary<string, Statement>();
            this.GotoStatements = new List<GotoStatement>();
            this.IsMethodBodyChanged = false;
            this.VariableDefinitionToNameMap = new Dictionary<VariableDefinition, string>();
			this.VariableNamesCollection = new HashSet<string>();
            this.ParameterDefinitionToNameMap = new Dictionary<ParameterDefinition, string>();
            this.VariablesToRename = GetMethodVariablesToRename();
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

        public MethodSpecificContext(MethodBody body,
            Dictionary<VariableDefinition, string> variableDefinitionToNameMap, Dictionary<ParameterDefinition, string> parameterDefinitionTonameMap,
            MethodInvocationExpression ctorInvokeExpression)
            : this(body)
        {
            this.VariableDefinitionToNameMap = variableDefinitionToNameMap;
            this.ParameterDefinitionToNameMap = parameterDefinitionTonameMap;
            this.CtorInvokeExpression = ctorInvokeExpression;
        }

        internal MethodSpecificContext(DecompilationAnalysisResults analysisResults, YieldData yieldData, AsyncData asyncData,
            bool isMethodBodyChanged, Dictionary<string, Statement> gotoLabels, List<GotoStatement> gotoStatements,
            StackUsageData stackData, bool isBaseConstructorInvokingConstructor, bool enableEventAnalysis,
            MethodBody body, Collection<VariableDefinition> variables, ControlFlowGraph controlFlowGraph,
            ExpressionDecompilerData expressions, BlockLogicalConstruct logicalConstructsTree, LogicalFlowBuilderContext logicalConstructsContext,
            MethodInvocationExpression ctorInvokeExpression, Dictionary<Statement, ILogicalConstruct> statementToLogicalConstruct,
            Dictionary<ILogicalConstruct, List<Statement>> logicalConstructToStatements, Dictionary<VariableDefinition, string> variableDefinitionToNameMap,
            HashSet<string> variableNamesCollection, Dictionary<ParameterDefinition, string> parameterDefinitionToNameMap,
            HashSet<VariableDefinition> variablesToRename, Dictionary<FieldDefinition, Expression> fieldToExpression,
            int lambdaVariablesCount, Dictionary<VariableDefinition, AssignmentType> variableAssignmentData, List<ParameterDefinition> outParametersToAssign,
            bool isDestructor, BlockStatement destructorStatements, HashSet<VariableDefinition> undeclaredLinqVariables,
            Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>> closureVariableToFieldValue,
            HashSet<VariableDefinition> variablesToNotDeclare, CompilerOptimizedSwitchByStringData switchByStringData)
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

        public DecompilationAnalysisResults AnalysisResults { get; set; }

        internal YieldData YieldData { get; set; }

        internal AsyncData AsyncData { get; set; }

        internal bool IsMethodBodyChanged { get; set; }

        internal Dictionary<string, Statement> GotoLabels { get; private set; }

        internal List<GotoStatement> GotoStatements { get; private set; }

        internal StackUsageData StackData { get; set; }

        public bool IsBaseConstructorInvokingConstructor { get; set; }

        bool enableEventAnalysis;
        internal bool EnableEventAnalysis
        {
            get
            {
                return enableEventAnalysis && !this.Method.IsAddOn && !this.Method.IsRemoveOn/* && !this.Method.IsFire*/;
            }

            set
            {
                enableEventAnalysis = value;
            }
        }

        internal MethodDefinition Method
        {
            get
            {
                return Body.Method;
            }
        }

        internal MethodBody Body { get; private set; }

        internal Collection<VariableDefinition> Variables { get; private set; }

        internal ControlFlowGraph ControlFlowGraph { get; private set; }

        //internal VariableDefineUseAnalysisData DefineUseAnalysisData { get; set; }

        internal ExpressionDecompilerData Expressions { get; set; }

        internal BlockLogicalConstruct LogicalConstructsTree { get; set; }

        internal LogicalFlowBuilderContext LogicalConstructsContext { get; set; }

        public MethodInvocationExpression CtorInvokeExpression { get; set; }

        internal Dictionary<Statement, ILogicalConstruct> StatementToLogicalConstruct { get; set; }

        internal Dictionary<ILogicalConstruct, List<Statement>> LogicalConstructToStatements { get; set; }

        internal void RemoveVariable(VariableReference reference)
        {
            RemoveVariable(reference.Resolve());
        }

        internal void RemoveVariable(VariableDefinition variable)
        {
            int index = Variables.IndexOf(variable);
            if (index == -1)
            {
                return;
            }

            Variables.RemoveAt(index);
        }

        internal Dictionary<VariableDefinition, string> VariableDefinitionToNameMap { get; set; }
		internal HashSet<string> VariableNamesCollection { get; set; }
        internal Dictionary<ParameterDefinition, string> ParameterDefinitionToNameMap { get; set; }

        internal void AddInnerMethodParametersToContext(MethodSpecificContext innerMethodContext)
        {
            this.ParameterDefinitionToNameMap.AddRange(innerMethodContext.ParameterDefinitionToNameMap);
            foreach (ParameterDefinition parameter in innerMethodContext.Method.Parameters)
            {
                this.ParameterDefinitionToNameMap[parameter] = parameter.Name;
            }
        }

        internal HashSet<VariableDefinition> VariablesToRename { get; private set; }

        private HashSet<VariableDefinition> GetMethodVariablesToRename()
        {
            HashSet<VariableDefinition> result = new HashSet<VariableDefinition>();
            foreach (VariableDefinition variable in this.Variables)
            {
                if (!variable.VariableNameChanged)
                {
                    result.Add(variable);
                }
            }
            return result;
        }

        /// <summary>
        /// This set is used by <see cref="RebuildAnnonymousDelegatesStep"/> to pass the map between a field name and its assigned expression
        /// between different instances of decompilation, as nested anonymous delegates invoke new decompilation cycle.
        /// </summary>
        public Dictionary<FieldDefinition, Expression> FieldToExpression { get; set; }

        public int LambdaVariablesCount { get; set; }

        internal Dictionary<VariableDefinition, AssignmentType> VariableAssignmentData { get; set; }

        internal List<ParameterDefinition> OutParametersToAssign { get; private set; }

		public bool IsDestructor { get; set; }

		public BlockStatement DestructorStatements { get; set; }

        public HashSet<VariableDefinition> UndeclaredLinqVariables { get; private set; }

        public Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>> ClosureVariableToFieldValue { get; private set; }

        public HashSet<VariableDefinition> VariablesToNotDeclare { get; private set; }

        public CompilerOptimizedSwitchByStringData SwitchByStringData { get; set; }
    }
}
