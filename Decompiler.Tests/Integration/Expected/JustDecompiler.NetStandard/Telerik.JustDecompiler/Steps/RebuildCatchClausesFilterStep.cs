using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

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
			base();
			this.currentCatchClause = null;
			this.variablesUsedOutsideFilters = new HashSet<VariableDefinition>();
			this.catchClausesUsedVariablesMap = new Dictionary<CatchClause, HashSet<VariableDefinition>>();
			this.catchClausesUsedParametersMap = new Dictionary<CatchClause, HashSet<ParameterDefinition>>();
			this.catchClausesVariablesToParametersMap = new Dictionary<CatchClause, Dictionary<VariableDefinition, ParameterDefinition>>();
			this.methodsToBeDecompiled = new List<FilterMethodToBeDecompiled>();
			return;
		}

		private List<Expression> AddAllParameters(CatchClause catchClause, MethodDefinition method, VariableDeclarationExpression variable)
		{
			V_0 = new List<Expression>();
			method.get_Parameters().Add(this.CreateParameter("JustDecompileGenerated_Exception", variable.get_Variable().get_VariableType()));
			V_0.Add(new UnaryExpression(7, new VariableReferenceExpression(variable.get_Variable(), null), null));
			if (this.catchClausesUsedVariablesMap.ContainsKey(catchClause))
			{
				V_1 = this.catchClausesUsedVariablesMap.get_Item(catchClause).GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (!this.variablesUsedOutsideFilters.Contains(V_2) || (object)V_2 == (object)variable.get_Variable())
						{
							continue;
						}
						if (this.context.get_MethodContext().get_VariablesToRename().Contains(V_2))
						{
							stackVariable72 = null;
						}
						else
						{
							stackVariable72 = V_2.get_Name();
						}
						V_3 = stackVariable72;
						V_4 = this.CreateParameter(V_3, V_2.get_VariableType());
						if (V_3 == null)
						{
							if (!this.catchClausesVariablesToParametersMap.ContainsKey(catchClause))
							{
								this.catchClausesVariablesToParametersMap.Add(catchClause, new Dictionary<VariableDefinition, ParameterDefinition>());
							}
							this.catchClausesVariablesToParametersMap.get_Item(catchClause).Add(V_2, V_4);
						}
						method.get_Parameters().Add(V_4);
						V_0.Add(new UnaryExpression(7, new VariableReferenceExpression(V_2, null), null));
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
			}
			if (this.catchClausesUsedParametersMap.ContainsKey(catchClause))
			{
				V_5 = this.catchClausesUsedParametersMap.get_Item(catchClause).GetEnumerator();
				try
				{
					while (V_5.MoveNext())
					{
						V_6 = V_5.get_Current();
						method.get_Parameters().Add(this.CreateParameter(V_6.get_Name(), V_6.get_ParameterType()));
						V_0.Add(new UnaryExpression(7, new ArgumentReferenceExpression(V_6, null), null));
					}
				}
				finally
				{
					((IDisposable)V_5).Dispose();
				}
			}
			return V_0;
		}

		private void AddReferencedParameter(ParameterDefinition parameterDefinition)
		{
			if (this.currentCatchClause != null)
			{
				if (!this.catchClausesUsedParametersMap.ContainsKey(this.currentCatchClause))
				{
					this.catchClausesUsedParametersMap.Add(this.currentCatchClause, new HashSet<ParameterDefinition>());
				}
				dummyVar0 = this.catchClausesUsedParametersMap.get_Item(this.currentCatchClause).Add(parameterDefinition);
			}
			return;
		}

		private void AddReferencedVariable(VariableDefinition variableDefinition)
		{
			if (this.currentCatchClause == null)
			{
				dummyVar1 = this.variablesUsedOutsideFilters.Add(variableDefinition);
				return;
			}
			if (!this.catchClausesUsedVariablesMap.ContainsKey(this.currentCatchClause))
			{
				this.catchClausesUsedVariablesMap.Add(this.currentCatchClause, new HashSet<VariableDefinition>());
			}
			dummyVar0 = this.catchClausesUsedVariablesMap.get_Item(this.currentCatchClause).Add(variableDefinition);
			return;
		}

		private void AddVariablesToNotDeclare(DecompilationContext context, CatchClause currentCatch)
		{
			V_0 = context.get_MethodContext().get_Variables().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.variablesUsedOutsideFilters.Contains(V_1))
					{
						dummyVar0 = context.get_MethodContext().get_VariablesToNotDeclare().Add(V_1);
					}
					V_2 = this.catchClausesUsedVariablesMap.GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							if (V_3.get_Key().Equals(currentCatch) || !V_3.get_Value().Contains(V_1))
							{
								continue;
							}
							dummyVar1 = context.get_MethodContext().get_VariablesToNotDeclare().Add(V_1);
						}
					}
					finally
					{
						((IDisposable)V_2).Dispose();
					}
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		private MethodSpecificContext CloneAndReplaceMethodBody(MethodSpecificContext context, MethodBody methodBody)
		{
			return new MethodSpecificContext(context.get_AnalysisResults(), context.get_YieldData(), context.get_AsyncData(), context.get_IsMethodBodyChanged(), new Dictionary<string, Statement>(context.get_GotoLabels()), new List<GotoStatement>(context.get_GotoStatements()), context.get_StackData(), context.get_IsBaseConstructorInvokingConstructor(), context.get_EnableEventAnalysis(), methodBody, new Collection<VariableDefinition>(context.get_Variables()), context.get_ControlFlowGraph(), context.get_Expressions(), context.get_LogicalConstructsTree(), context.get_LogicalConstructsContext(), context.get_CtorInvokeExpression(), new Dictionary<Statement, ILogicalConstruct>(context.get_StatementToLogicalConstruct()), new Dictionary<ILogicalConstruct, List<Statement>>(context.get_LogicalConstructToStatements()), new Dictionary<VariableDefinition, string>(context.get_VariableDefinitionToNameMap()), new HashSet<string>(context.get_VariableNamesCollection()), new Dictionary<ParameterDefinition, string>(context.get_ParameterDefinitionToNameMap()), new HashSet<VariableDefinition>(context.get_VariablesToRename()), new Dictionary<FieldDefinition, Expression>(context.get_FieldToExpression()), context.get_LambdaVariablesCount(), new Dictionary<VariableDefinition, AssignmentType>(context.get_VariableAssignmentData()), new List<ParameterDefinition>(context.get_OutParametersToAssign()), context.get_IsDestructor(), context.get_DestructorStatements(), new HashSet<VariableDefinition>(context.get_UndeclaredLinqVariables()), new Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>>(context.get_ClosureVariableToFieldValue()), new HashSet<VariableDefinition>(context.get_VariablesToNotDeclare()), context.get_SwitchByStringData().Clone() as CompilerOptimizedSwitchByStringData);
		}

		private void CreateMethod(CatchClause catchClause, VariableDeclarationExpression variable, out Expression methodInvocationExpression)
		{
			methodInvocationExpression = null;
			stackVariable4 = catchClause.get_Filter() as BlockStatement;
			V_0 = (((stackVariable4.get_Statements().First<Statement>() as ExpressionStatement).get_Expression() as BinaryExpression).get_Right() as SafeCastExpression).get_Expression() as VariableReferenceExpression;
			V_0.set_Variable(variable.get_Variable());
			V_1 = stackVariable4.get_Statements().Last<Statement>() as ExpressionStatement;
			V_1.set_Expression(new ReturnExpression(V_1.get_Expression(), V_1.get_Expression().get_MappedInstructions()));
			V_2 = this.context.get_TypeContext().get_GeneratedFilterMethods().get_Count() + this.methodsToBeDecompiled.get_Count();
			V_3 = new MethodDefinition(String.Format("JustDecompileGenerated_Filter_{0}", V_2), 1, this.context.get_MethodContext().get_Method().get_Module().get_TypeSystem().get_Boolean());
			V_3.set_Body(new MethodBody(V_3));
			V_3.set_MetadataToken(new MetadataToken(0x6000000, 0xffffff - V_2));
			V_3.set_IsStatic(this.context.get_MethodContext().get_Method().get_IsStatic());
			V_3.set_HasThis(!V_3.get_IsStatic());
			V_3.set_DeclaringType(this.context.get_MethodContext().get_Method().get_DeclaringType());
			V_3.set_SemanticsAttributes(0);
			V_3.set_IsJustDecompileGenerated(true);
			V_4 = new DecompilationContext(this.CloneAndReplaceMethodBody(this.context.get_MethodContext(), V_3.get_Body()), this.context.get_TypeContext(), this.context.get_Language());
			V_5 = V_0.get_Variable().Resolve();
			if (V_4.get_MethodContext().get_VariableDefinitionToNameMap().ContainsKey(V_5))
			{
				V_4.get_MethodContext().get_VariableDefinitionToNameMap().Add(V_5, "JustDecompileGenerated_Exception");
			}
			else
			{
				V_4.get_MethodContext().get_VariableDefinitionToNameMap().set_Item(V_5, "JustDecompileGenerated_Exception");
			}
			dummyVar0 = V_4.get_MethodContext().get_VariablesToNotDeclare().Add(variable.get_Variable());
			this.methodsToBeDecompiled.Add(new FilterMethodToBeDecompiled(V_3, catchClause, V_4, catchClause.get_Filter() as BlockStatement));
			V_6 = this.AddAllParameters(catchClause, V_3, variable);
			methodInvocationExpression = this.CreateMethodInvocation(V_3, V_6);
			return;
		}

		private MethodInvocationExpression CreateMethodInvocation(MethodDefinition method, List<Expression> arguments)
		{
			if (method.get_IsStatic())
			{
				stackVariable2 = null;
			}
			else
			{
				stackVariable2 = new ThisReferenceExpression(method.get_DeclaringType(), null);
			}
			V_0 = new MethodInvocationExpression(new MethodReferenceExpression(stackVariable2, method, null), null);
			V_1 = arguments.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.get_Arguments().Add(V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		private ParameterDefinition CreateParameter(string name, TypeReference type)
		{
			return new ParameterDefinition(name, 0, new ByReferenceType(type));
		}

		private void DecompileMethods()
		{
			V_0 = this.methodsToBeDecompiled.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.AddVariablesToNotDeclare(V_1.get_Context(), V_1.get_CatchClause());
					V_2 = this.context.get_Language().CreateFilterMethodPipeline(V_1.get_Context());
					V_3 = V_2.Run(V_1.get_Method().get_Body(), V_1.get_Block(), this.context.get_Language());
					this.context.get_TypeContext().get_GeneratedFilterMethods().Add(new GeneratedMethod(V_1.get_Method(), V_2.get_Body(), V_3.get_MethodContext()));
					this.context.get_TypeContext().get_GeneratedMethodDefinitionToNameMap().Add(V_1.get_Method(), V_1.get_Method().get_Name());
					this.FixVariablesNames(V_3, V_1.get_CatchClause());
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		private void FixVariablesNames(DecompilationContext innerContext, CatchClause currentCatch)
		{
			if (this.catchClausesVariablesToParametersMap.ContainsKey(currentCatch))
			{
				V_0 = this.catchClausesVariablesToParametersMap.get_Item(currentCatch).GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						innerContext.get_MethodContext().get_VariableDefinitionToNameMap().set_Item(V_1.get_Key(), innerContext.get_MethodContext().get_ParameterDefinitionToNameMap().get_Item(V_1.get_Value()));
					}
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
			}
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			stackVariable3 = context.get_MethodContext().get_Body().get_ExceptionHandlers();
			stackVariable4 = RebuildCatchClausesFilterStep.u003cu003ec.u003cu003e9__9_0;
			if (stackVariable4 == null)
			{
				dummyVar0 = stackVariable4;
				stackVariable4 = new Func<ExceptionHandler, bool>(RebuildCatchClausesFilterStep.u003cu003ec.u003cu003e9.u003cProcessu003eb__9_0);
				RebuildCatchClausesFilterStep.u003cu003ec.u003cu003e9__9_0 = stackVariable4;
			}
			if (!stackVariable3.Any<ExceptionHandler>(stackVariable4))
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
			V_0 = this.context.get_MethodContext().get_Variables().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.variablesUsedOutsideFilters.Contains(V_1) || this.context.get_MethodContext().get_VariablesToNotDeclare().Contains(V_1))
					{
						continue;
					}
					dummyVar0 = this.context.get_MethodContext().get_VariablesToNotDeclare().Add(V_1);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		public override ICodeNode VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
		{
			this.AddReferencedParameter(node.get_Parameter().Resolve());
			return node;
		}

		public override ICodeNode VisitCatchClause(CatchClause node)
		{
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			if (node.get_Filter() == null || node.get_Filter() as BlockStatement == null)
			{
				return node;
			}
			this.currentCatchClause = node;
			node.set_Filter((Statement)this.Visit(node.get_Filter()));
			this.currentCatchClause = null;
			stackVariable27 = CatchClausesFilterPattern.TryMatch(node.get_Filter() as BlockStatement, out V_0, out V_1);
			if (!stackVariable27)
			{
				if (V_0 == null || String.op_Equality(V_0.get_ExpressionType().get_FullName(), "System.Object") || !CatchClausesFilterPattern.TryMatchMethodStructure(node.get_Filter() as BlockStatement))
				{
					throw new NotSupportedException("Unsupported structure of filter clause.");
				}
				this.CreateMethod(node, V_0, out V_1);
			}
			dummyVar0 = this.context.get_MethodContext().get_VariablesToNotDeclare().Add(V_0.get_Variable());
			if (!stackVariable27)
			{
				node.set_Variable(V_0.CloneExpressionOnly() as VariableDeclarationExpression);
			}
			else
			{
				node.set_Variable(V_0);
			}
			node.set_Type(V_0.get_ExpressionType());
			node.set_Filter(new ExpressionStatement(V_1));
			return node;
		}

		public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			this.AddReferencedVariable(node.get_Variable().Resolve());
			return node;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			this.AddReferencedVariable(node.get_Variable().Resolve());
			return node;
		}
	}
}