using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler
{
	public static class Extensions
	{
		public static BlockStatement Decompile(this MethodBody body, ILanguage language, TypeSpecificContext typeContext = null)
		{
			DecompilationContext decompilationContext;
			return body.Decompile(language, out decompilationContext, typeContext);
		}

		public static BlockStatement Decompile(this MethodBody body, ILanguage language, out DecompilationContext context, TypeSpecificContext typeContext = null)
		{
			DecompilationPipeline decompilationPipeline;
			decompilationPipeline = (typeContext == null ? language.CreatePipeline() : language.CreatePipeline(new DecompilationContext(new MethodSpecificContext(body), typeContext, language)));
			return Telerik.JustDecompiler.Decompiler.Extensions.RunPipeline(decompilationPipeline, language, body, out context);
		}

		public static BlockStatement Decompile(this MethodBody body, ILanguage language, DecompilationContext context)
		{
			if (body != null)
			{
				body.get_Method();
			}
			return Telerik.JustDecompiler.Decompiler.Extensions.RunPipeline(language.CreatePipeline(context), language, body, out context);
		}

		internal static BlockStatement DecompileAsyncStateMachine(this MethodBody body, DecompilationContext enclosingContext, out AsyncData asyncData)
		{
			DecompilationContext decompilationContext;
			BlockStatement blockStatement = body.DecompileStateMachine(enclosingContext, new RemoveAsyncStateMachineStep(), (DecompilationContext context) => context.MethodContext.AsyncData, out decompilationContext);
			asyncData = decompilationContext.MethodContext.AsyncData;
			return blockStatement;
		}

		internal static BlockStatement DecompileLambda(this MethodBody body, ILanguage language, DecompilationContext context)
		{
			if (body != null)
			{
				body.get_Method();
			}
			return Telerik.JustDecompiler.Decompiler.Extensions.RunPipeline(language.CreateLambdaPipeline(context), language, body, out context);
		}

		private static BlockStatement DecompileStateMachine(this MethodBody body, DecompilationContext enclosingContext, BaseStateMachineRemoverStep removeStateMachineStep, Func<DecompilationContext, IStateMachineData> stateMachineDataSelector, out DecompilationContext decompilationContext)
		{
			DecompilationPipeline stateMachineRemovalPipeline = Telerik.JustDecompiler.Decompiler.Extensions.GetStateMachineRemovalPipeline(removeStateMachineStep, stateMachineDataSelector);
			decompilationContext = stateMachineRemovalPipeline.Run(body, enclosingContext.Language);
			enclosingContext.MethodContext.Variables.AddRange(decompilationContext.MethodContext.Variables);
			enclosingContext.MethodContext.VariableDefinitionToNameMap.AddRange<VariableDefinition, string>(decompilationContext.MethodContext.VariableDefinitionToNameMap);
			enclosingContext.MethodContext.AddInnerMethodParametersToContext(decompilationContext.MethodContext);
			enclosingContext.MethodContext.VariableAssignmentData.AddRange<VariableDefinition, AssignmentType>(decompilationContext.MethodContext.VariableAssignmentData);
			enclosingContext.MethodContext.GotoLabels.AddRange<string, Statement>(decompilationContext.MethodContext.GotoLabels);
			enclosingContext.MethodContext.GotoStatements.AddRange(decompilationContext.MethodContext.GotoStatements);
			return stateMachineRemovalPipeline.Body;
		}

		internal static BlockStatement DecompileYieldStateMachine(this MethodBody body, DecompilationContext enclosingContext, out YieldData yieldData)
		{
			DecompilationContext decompilationContext;
			BlockStatement blockStatement = body.DecompileStateMachine(enclosingContext, new RemoveYieldStateMachineStep(), (DecompilationContext context) => context.MethodContext.YieldData, out decompilationContext);
			yieldData = decompilationContext.MethodContext.YieldData;
			return blockStatement;
		}

		internal static TElement First<TElement>(this IList<TElement> list)
		{
			return list[0];
		}

		private static DecompilationPipeline GetStateMachineRemovalPipeline(BaseStateMachineRemoverStep removeStateMachineStep, Func<DecompilationContext, IStateMachineData> stateMachineDataSelector)
		{
			DecompilationPipeline intermediateRepresenationPipeline = BaseLanguage.IntermediateRepresenationPipeline;
			List<IDecompilationStep> decompilationSteps = new List<IDecompilationStep>()
			{
				removeStateMachineStep
			};
			foreach (IDecompilationStep step in intermediateRepresenationPipeline.Steps)
			{
				decompilationSteps.Add(step);
				if (!(step is VariableAssignmentAnalysisStep))
				{
					continue;
				}
				decompilationSteps.Add(new FieldAssignmentAnalysisStep(stateMachineDataSelector));
			}
			return new DecompilationPipeline(decompilationSteps);
		}

		internal static bool IsArgumentReferenceToRefParameter(this Expression expression)
		{
			if (expression.CodeNodeType != CodeNodeType.UnaryExpression)
			{
				return false;
			}
			UnaryExpression unaryExpression = expression as UnaryExpression;
			if (unaryExpression.Operator != UnaryOperator.AddressDereference || unaryExpression.Operand.CodeNodeType != CodeNodeType.ArgumentReferenceExpression)
			{
				return false;
			}
			if (!(unaryExpression.Operand as ArgumentReferenceExpression).Parameter.get_ParameterType().get_IsByReference())
			{
				return false;
			}
			return true;
		}

		internal static TElement Last<TElement>(this IList<TElement> list)
		{
			return list[list.Count - 1];
		}

		private static BlockStatement RunPipeline(DecompilationPipeline pipeline, ILanguage language, MethodBody body, out DecompilationContext context)
		{
			context = pipeline.Run(body, language);
			return pipeline.Body;
		}
	}
}