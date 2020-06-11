#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Languages.CSharp;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler
{
    public static class Extensions
    {
        public static BlockStatement Decompile(this MethodBody body, ILanguage language, TypeSpecificContext typeContext = null)
        {
            DecompilationContext dc;
            return body.Decompile(language, out dc, typeContext);
        }

        public static BlockStatement Decompile(this MethodBody body, ILanguage language, out DecompilationContext context, TypeSpecificContext typeContext = null)
        {
            DecompilationPipeline pipeline;
            if (typeContext != null)
            {
                pipeline = language.CreatePipeline(new DecompilationContext(new MethodSpecificContext(body), typeContext, language));
            }
            else
            {
                pipeline = language.CreatePipeline();
            }

            return RunPipeline(pipeline, language, body, out context);
        }

        internal static BlockStatement DecompileLambda(this MethodBody body, ILanguage language, DecompilationContext context)
        {
            MethodDefinition method = null;
            if (body != null)
            {
                method = body.Method;
            }
            DecompilationPipeline pipeline = language.CreateLambdaPipeline(context);
            return RunPipeline(pipeline, language, body, out context);
        }

        /// <summary>
        /// Decompiles <paramref name="body"/>, using the preset <paramref name="context"/>.
        /// </summary>
        /// <param name="body">The body of the method, to be decompiled.</param>
        /// <param name="language">The language to which is decompiled.</param>
        /// <param name="context">The context for the decompilation.</param>
        /// <returns>Returns the expression-statement tree, representing the decompiled method.</returns>
        public static BlockStatement Decompile(this MethodBody body, ILanguage language, DecompilationContext context)
        {
            MethodDefinition method = null;
            if (body != null)
            {
                method = body.Method;
            }
            DecompilationPipeline pipeline = language.CreatePipeline(context);
            return RunPipeline(pipeline, language, body, out context);
        }

        static BlockStatement RunPipeline(DecompilationPipeline pipeline, ILanguage language, MethodBody body, out DecompilationContext context)
        {
            context = pipeline.Run(body, language);
            return pipeline.Body;
        }

        private static BlockStatement DecompileStateMachine(this MethodBody body, DecompilationContext enclosingContext,
            BaseStateMachineRemoverStep removeStateMachineStep, Func<DecompilationContext, IStateMachineData> stateMachineDataSelector,
            out DecompilationContext decompilationContext)
        {
            DecompilationPipeline thePipeline = GetStateMachineRemovalPipeline(removeStateMachineStep, stateMachineDataSelector);
            decompilationContext = thePipeline.Run(body, enclosingContext.Language);

            enclosingContext.MethodContext.Variables.AddRange(decompilationContext.MethodContext.Variables);
            enclosingContext.MethodContext.VariableDefinitionToNameMap.AddRange(decompilationContext.MethodContext.VariableDefinitionToNameMap);
            enclosingContext.MethodContext.AddInnerMethodParametersToContext(decompilationContext.MethodContext);
            enclosingContext.MethodContext.VariableAssignmentData.AddRange(decompilationContext.MethodContext.VariableAssignmentData);
            enclosingContext.MethodContext.GotoLabels.AddRange(decompilationContext.MethodContext.GotoLabels);
            enclosingContext.MethodContext.GotoStatements.AddRange(decompilationContext.MethodContext.GotoStatements);
            BlockStatement theBlockStatement = thePipeline.Body;
            return theBlockStatement;
        }

        private static DecompilationPipeline GetStateMachineRemovalPipeline(BaseStateMachineRemoverStep removeStateMachineStep,
            Func<DecompilationContext, IStateMachineData> stateMachineDataSelector)
        {
            DecompilationPipeline intermediatePipeline = BaseLanguage.IntermediateRepresenationPipeline;
            List<IDecompilationStep> newSteps = new List<IDecompilationStep>();

            newSteps.Add(removeStateMachineStep);
            foreach (IDecompilationStep step in intermediatePipeline.Steps)
            {
                newSteps.Add(step);
                if (step is VariableAssignmentAnalysisStep)
                {
                    newSteps.Add(new FieldAssignmentAnalysisStep(stateMachineDataSelector));
                }
            }

            return new DecompilationPipeline(newSteps);
        }

        internal static BlockStatement DecompileYieldStateMachine(this MethodBody body, DecompilationContext enclosingContext,
            out YieldData yieldData)
        {
            DecompilationContext decompilationContext;
            BlockStatement theBlockStatement = body.DecompileStateMachine(enclosingContext, new RemoveYieldStateMachineStep(),
                (DecompilationContext context) => context.MethodContext.YieldData, out decompilationContext);

            yieldData = decompilationContext.MethodContext.YieldData;
            return theBlockStatement;
        }

        internal static BlockStatement DecompileAsyncStateMachine(this MethodBody body, DecompilationContext enclosingContext,
            out AsyncData asyncData)
        {
            DecompilationContext decompilationContext;
            BlockStatement theBlockStatement = body.DecompileStateMachine(enclosingContext, new RemoveAsyncStateMachineStep(),
                (DecompilationContext context) => context.MethodContext.AsyncData, out decompilationContext);

            asyncData = decompilationContext.MethodContext.AsyncData;
            return theBlockStatement;
        }

        internal static TElement First<TElement>(this IList<TElement> list)
        {
            return list[0];
        }

        internal static TElement Last<TElement>(this IList<TElement> list)
        {
            return list[list.Count - 1];
        }

        internal static bool IsArgumentReferenceToRefParameter(this Expression expression)
        {
            if (expression.CodeNodeType != CodeNodeType.UnaryExpression)
            {
                return false;
            }

            UnaryExpression unary = expression as UnaryExpression;
            if (unary.Operator != UnaryOperator.AddressDereference ||
                unary.Operand.CodeNodeType != CodeNodeType.ArgumentReferenceExpression)
            {
                return false;
            }

            ArgumentReferenceExpression argumentReference = unary.Operand as ArgumentReferenceExpression;
            if (!argumentReference.Parameter.ParameterType.IsByReference)
            {
                return false;
            }

            return true;
        }
    }
}