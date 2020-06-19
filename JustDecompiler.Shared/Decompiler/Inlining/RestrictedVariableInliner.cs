using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    class RestrictedVariableInliner : SimpleVariableInliner
    {
        public RestrictedVariableInliner(TypeSystem typeSystem)
            : base(typeSystem)
        {
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            node.MethodExpression = (MethodReferenceExpression)Visit(node.MethodExpression);
            if (status != InliningResult.NotFound)
            {
                return node;
            }

            MethodReference methodRef = node.MethodExpression.Method;

            for (int i = 0; i < node.Arguments.Count; i++)
            {
                if (!methodRef.Parameters[i].ParameterType.IsByReference)
                {
                    node.Arguments[i] = (Expression)Visit(node.Arguments[i]);

                    if (status != InliningResult.NotFound)
                    {
                        return node;
                    }
                }
                else if (valueHasSideEffects)
                {
                    SideEffectsFinder sideEffectsFinder = new SideEffectsFinder();
                    if (sideEffectsFinder.HasSideEffectsRecursive(node.Arguments[i]))
                    {
                        status = InliningResult.Abort;
                        return node;
                    }
                }
            }

            return node;
        }

        protected override ICodeNode GetNewValue(VariableReferenceExpression node)
        {
            return value.CloneAndAttachInstructions(node.UnderlyingSameMethodInstructions);
        }
    }
}
