using System;
using System.Collections.Generic;
using Mono.Cecil;
using System.Linq;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
    public class HandleVirtualMethodInvocations
    {
        private MethodDefinition method;

        public HandleVirtualMethodInvocations(MethodDefinition method)
        {
            this.method = method;
        }

        public void VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            if (node.VirtualCall)
            {
                //Targets of virtual method calls should not be changed
                MethodReferenceExpression method = node.MethodExpression;

                if (!method.Target.HasType)
                {
                    /// The target expression has no type.
                    /// This will result in bad navigation when clicking in the produced method call.
                    return;
                }

                if (method.Target.ExpressionType.FullName == method.Method.DeclaringType.FullName)
                {
                    return;
                }
                MethodReference overridingMethod = method.Method;
                TypeDefinition targetType = method.Target.ExpressionType.Resolve();
                if (targetType == null)
                {
                    // Generics
                    return;
                }
                List<TypeDefinition> inheritanceChain = GetInheritanceChain(targetType, method.Method.DeclaringType.FullName);

                foreach (TypeDefinition type in inheritanceChain)
                {
                    MethodDefinition overrider = type.Methods.FirstOrDefault(x => x.Name == method.Method.Name && x.HasSameSignatureWith(method.Method));
                    if (overrider != null)
                    {
                        overridingMethod = overrider;
                    }
                }

                node.MethodExpression = new MethodReferenceExpression(node.MethodExpression.Target, overridingMethod, node.MethodExpression.MappedInstructions);

                return;
            }

            MethodReferenceExpression methodReferenceExpression = node.MethodExpression;
            if (methodReferenceExpression == null)
            {
                return;
            }

            if (methodReferenceExpression.Target is ThisReferenceExpression)
            {
                TypeDefinition decompiledMethodDeclaringType = this.method.DeclaringType.Resolve();
                if (decompiledMethodDeclaringType != null &&
                    decompiledMethodDeclaringType != methodReferenceExpression.Method.DeclaringType.Resolve())
                {
                    TypeReference baseType = this.method.DeclaringType.BaseType;

                    // does not have a base class.
                    if (baseType == null || baseType.FullName == typeof(System.Object).FullName)
                    {
                        return;
                    }

                    methodReferenceExpression.Target = new BaseReferenceExpression(methodReferenceExpression.Method.DeclaringType,
                        (methodReferenceExpression.Target as ThisReferenceExpression).MappedInstructions);
                }
            }
        }
  
        private List<TypeDefinition> GetInheritanceChain(TypeDefinition targetType, string definingTypeFullName)
        {
            List<TypeDefinition> allParents = new List<TypeDefinition>();
            List<int> inheritorIndex = new List<int>();

            /// Add the lowest type.
            /// It has no inheritors in the chain.
            allParents.Add(targetType);
            inheritorIndex.Add(-1);

            int definingTypeIndex= - 1;

            for (int i = 0; i < allParents.Count; i++)
            {
                TypeDefinition currentType = allParents[i];
                if (currentType == null || currentType.FullName == definingTypeFullName)
                {
                    definingTypeIndex = i;
                    break;
                }
                TypeReference baseType = currentType.BaseType;
                if (baseType != null)
                {
                    TypeDefinition baseDef = baseType.Resolve();
                    /// Add baseDef to the inheritance chains
                    /// mark class i as the decendant it came from
                    if (baseDef != null)
                    {
                        allParents.Add(baseDef);
                        inheritorIndex.Add(i);
                    }
                }
                foreach (TypeReference @interface in currentType.Interfaces)
                {
                    if (@interface != null)
                    {
                        TypeDefinition interfaceDef = @interface.Resolve();
                        if (interfaceDef != null)
                        {
                            allParents.Add(interfaceDef);
                            inheritorIndex.Add(i);
                        }
                    }
                }
            }

            List<TypeDefinition> result = new List<TypeDefinition>();
            while (definingTypeIndex != -1)
            {
                result.Add(allParents[definingTypeIndex]);
                definingTypeIndex = inheritorIndex[definingTypeIndex];
            }

            return result;
        }
    }
}