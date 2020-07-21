using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler
{
	public static class Extensions
	{
		public static BlockStatement Decompile(this MethodBody body, ILanguage language, TypeSpecificContext typeContext = null)
		{
			return body.Decompile(language, out V_0, typeContext);
		}

		public static BlockStatement Decompile(this MethodBody body, ILanguage language, out DecompilationContext context, TypeSpecificContext typeContext = null)
		{
			if (typeContext == null)
			{
				V_0 = language.CreatePipeline();
			}
			else
			{
				V_0 = language.CreatePipeline(new DecompilationContext(new MethodSpecificContext(body), typeContext, language));
			}
			return Extensions.RunPipeline(V_0, language, body, out context);
		}

		public static BlockStatement Decompile(this MethodBody body, ILanguage language, DecompilationContext context)
		{
			if (body != null)
			{
				dummyVar0 = body.get_Method();
			}
			return Extensions.RunPipeline(language.CreatePipeline(context), language, body, out context);
		}

		internal static BlockStatement DecompileAsyncStateMachine(this MethodBody body, DecompilationContext enclosingContext, out AsyncData asyncData)
		{
			stackVariable0 = body;
			stackVariable1 = enclosingContext;
			stackVariable2 = new RemoveAsyncStateMachineStep();
			stackVariable3 = Extensions.u003cu003ec.u003cu003e9__8_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<DecompilationContext, IStateMachineData>(Extensions.u003cu003ec.u003cu003e9.u003cDecompileAsyncStateMachineu003eb__8_0);
				Extensions.u003cu003ec.u003cu003e9__8_0 = stackVariable3;
			}
			stackVariable5 = stackVariable0.DecompileStateMachine(stackVariable1, stackVariable2, stackVariable3, out V_0);
			asyncData = V_0.get_MethodContext().get_AsyncData();
			return stackVariable5;
		}

		internal static BlockStatement DecompileLambda(this MethodBody body, ILanguage language, DecompilationContext context)
		{
			if (body != null)
			{
				dummyVar0 = body.get_Method();
			}
			return Extensions.RunPipeline(language.CreateLambdaPipeline(context), language, body, out context);
		}

		private static BlockStatement DecompileStateMachine(this MethodBody body, DecompilationContext enclosingContext, BaseStateMachineRemoverStep removeStateMachineStep, Func<DecompilationContext, IStateMachineData> stateMachineDataSelector, out DecompilationContext decompilationContext)
		{
			V_0 = Extensions.GetStateMachineRemovalPipeline(removeStateMachineStep, stateMachineDataSelector);
			decompilationContext = V_0.Run(body, enclosingContext.get_Language());
			enclosingContext.get_MethodContext().get_Variables().AddRange(decompilationContext.get_MethodContext().get_Variables());
			enclosingContext.get_MethodContext().get_VariableDefinitionToNameMap().AddRange<VariableDefinition, string>(decompilationContext.get_MethodContext().get_VariableDefinitionToNameMap());
			enclosingContext.get_MethodContext().AddInnerMethodParametersToContext(decompilationContext.get_MethodContext());
			enclosingContext.get_MethodContext().get_VariableAssignmentData().AddRange<VariableDefinition, AssignmentType>(decompilationContext.get_MethodContext().get_VariableAssignmentData());
			enclosingContext.get_MethodContext().get_GotoLabels().AddRange<string, Statement>(decompilationContext.get_MethodContext().get_GotoLabels());
			enclosingContext.get_MethodContext().get_GotoStatements().AddRange(decompilationContext.get_MethodContext().get_GotoStatements());
			return V_0.get_Body();
		}

		internal static BlockStatement DecompileYieldStateMachine(this MethodBody body, DecompilationContext enclosingContext, out YieldData yieldData)
		{
			stackVariable0 = body;
			stackVariable1 = enclosingContext;
			stackVariable2 = new RemoveYieldStateMachineStep();
			stackVariable3 = Extensions.u003cu003ec.u003cu003e9__7_0;
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				stackVariable3 = new Func<DecompilationContext, IStateMachineData>(Extensions.u003cu003ec.u003cu003e9.u003cDecompileYieldStateMachineu003eb__7_0);
				Extensions.u003cu003ec.u003cu003e9__7_0 = stackVariable3;
			}
			stackVariable5 = stackVariable0.DecompileStateMachine(stackVariable1, stackVariable2, stackVariable3, out V_0);
			yieldData = V_0.get_MethodContext().get_YieldData();
			return stackVariable5;
		}

		internal static TElement First<TElement>(this IList<TElement> list)
		{
			return list.get_Item(0);
		}

		private static DecompilationPipeline GetStateMachineRemovalPipeline(BaseStateMachineRemoverStep removeStateMachineStep, Func<DecompilationContext, IStateMachineData> stateMachineDataSelector)
		{
			stackVariable0 = BaseLanguage.get_IntermediateRepresenationPipeline();
			V_0 = new List<IDecompilationStep>();
			V_0.Add(removeStateMachineStep);
			V_1 = stackVariable0.get_Steps().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.Add(V_2);
					if (V_2 as VariableAssignmentAnalysisStep == null)
					{
						continue;
					}
					V_0.Add(new FieldAssignmentAnalysisStep(stateMachineDataSelector));
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return new DecompilationPipeline(V_0);
		}

		internal static bool IsArgumentReferenceToRefParameter(this Expression expression)
		{
			if (expression.get_CodeNodeType() != 23)
			{
				return false;
			}
			V_0 = expression as UnaryExpression;
			if (V_0.get_Operator() != 8 || V_0.get_Operand().get_CodeNodeType() != 25)
			{
				return false;
			}
			if (!(V_0.get_Operand() as ArgumentReferenceExpression).get_Parameter().get_ParameterType().get_IsByReference())
			{
				return false;
			}
			return true;
		}

		internal static TElement Last<TElement>(this IList<TElement> list)
		{
			return list.get_Item(list.get_Count() - 1);
		}

		private static BlockStatement RunPipeline(DecompilationPipeline pipeline, ILanguage language, MethodBody body, out DecompilationContext context)
		{
			context = pipeline.Run(body, language);
			return pipeline.get_Body();
		}
	}
}