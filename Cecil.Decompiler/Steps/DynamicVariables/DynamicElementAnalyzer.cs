using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Common;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
    class DynamicElementAnalyzer
    {
        private const string InconsistentCountOfFlags = "Inconsistent count of positioning flags.";

        public static bool Analyze(Expression expression)
        {
            DynamicElementAnalyzer analyzer = new DynamicElementAnalyzer();
            return analyzer.AnalyzeExpression(expression);
        }

        private DynamicElementAnalyzer()
        {
            dynamicPositioningFlags.AddFirst(true);
        }

        private readonly LinkedList<bool> dynamicPositioningFlags = new LinkedList<bool>();

        private bool AnalyzeExpression(Expression expression)
        {
            GenericParameter genericParam = null;
            Expression target = null;

            if(expression.CodeNodeType == CodeNodeType.ArgumentReferenceExpression)
            {
                return CheckParameter((expression as ArgumentReferenceExpression).Parameter.Resolve());
            }
            else if (expression.CodeNodeType == CodeNodeType.VariableReferenceExpression || expression.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
            {
                VariableDefinition varDef = expression.CodeNodeType == CodeNodeType.VariableReferenceExpression ?
                    (expression as VariableReferenceExpression).Variable.Resolve() :
                    (expression as VariableDeclarationExpression).Variable;

                return FixDynamicFlags(varDef);
            }
            else if(expression.CodeNodeType == CodeNodeType.MethodInvocationExpression || 
					expression.CodeNodeType == CodeNodeType.PropertyReferenceExpression)
            {
                MethodInvocationExpression methodInvocation = expression as MethodInvocationExpression;
                genericParam = methodInvocation.MethodExpression.Method.GenericParameterReturnType;
                target = methodInvocation.MethodExpression.Target;
            }
            else if (expression.CodeNodeType == CodeNodeType.FieldReferenceExpression)
            {
                FieldReferenceExpression fieldReferenceExpression = expression as FieldReferenceExpression;
                genericParam = fieldReferenceExpression.Field.FieldType as GenericParameter;
                target = fieldReferenceExpression.Target;
            }
            else if(expression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
            {
                return FixDynamicFlags(expression as ExplicitCastExpression);
            }
            else if(expression.CodeNodeType == CodeNodeType.ArrayIndexerExpression)
            {
                dynamicPositioningFlags.AddFirst(false);
                return AnalyzeExpression((expression as ArrayIndexerExpression).Target);
            }
            else if(expression.CodeNodeType == CodeNodeType.UnaryExpression &&
                ((expression as UnaryExpression).Operator == UnaryOperator.AddressDereference ||
                (expression as UnaryExpression).Operator == UnaryOperator.AddressOf ||
                (expression as UnaryExpression).Operator == UnaryOperator.AddressReference))
            {
                dynamicPositioningFlags.AddFirst(false);
                return AnalyzeExpression((expression as UnaryExpression).Operand);
            }
            else
            {
                return false;
            }

            if (target == null || genericParam == null)
            {
                return false;
            }

            if (genericParam.Owner is TypeReference && target.ExpressionType.IsGenericInstance && genericParam.Name[0] == '!'
                && genericParam.Name[1] != '!')
            {
                GenericInstanceType genericInstance = target.ExpressionType as GenericInstanceType;
                int paramIndex = int.Parse(genericParam.Name.Substring(1));
                dynamicPositioningFlags.AddFirst(false);
                for (int i = 0; i < genericInstance.GenericArguments.Count; i++)
                {
                    if (i == paramIndex)
                    {
                        continue;
                    }

                    int count = 0;
                    CountTypeTokens(genericInstance.GenericArguments[i], ref count);

                    for (int j = 0; j < count; j++)
                    {
                        if (i < paramIndex)
                        {
                            dynamicPositioningFlags.AddFirst(false);
                        }
                        else
                        {
                            dynamicPositioningFlags.AddLast(false);
                        }
                    }
                }

                return AnalyzeExpression(target);
            }

            return false;
        }

        private bool CheckParameter(ParameterDefinition paramDef)
        {
            CustomAttribute dynamicAttribute;
            if(!paramDef.TryGetDynamicAttribute(out dynamicAttribute))
            {
                return false;
            }

            bool[] dynamicPositioningFlags = DynamicHelper.GetDynamicPositioningFlags(dynamicAttribute);
            if(dynamicPositioningFlags.Length != this.dynamicPositioningFlags.Count)
            {
                return false;
            }

            LinkedListNode<bool> currentNode = this.dynamicPositioningFlags.First;
            foreach (bool flag in dynamicPositioningFlags)
            {
                if(flag != currentNode.Value)
                {
                    return false;
                }

                currentNode = currentNode.Next;
            }

            return true;
        }

        private bool FixDynamicFlags(IDynamicTypeContainer dynamicTypeContainer)
        {
            if (dynamicTypeContainer.IsDynamic)
            {
                if (dynamicTypeContainer.DynamicPositioningFlags.Length != this.dynamicPositioningFlags.Count)
                {
                    throw new Exception(InconsistentCountOfFlags);
                }
            }
            else
            {
                int count = 0;
                CountTypeTokens(dynamicTypeContainer.DynamicContainingType, ref count);

                if (count != this.dynamicPositioningFlags.Count)
                {
                    //throw new Exception(InconsistentCountOfFlags);
                    return false;
                }

                dynamicTypeContainer.DynamicPositioningFlags = new bool[count];
            }

            LinkedListNode<bool> currentNode = this.dynamicPositioningFlags.First;
            for (int i = 0; i < dynamicTypeContainer.DynamicPositioningFlags.Length; i++)
            {
                dynamicTypeContainer.DynamicPositioningFlags[i] |= currentNode.Value;
                currentNode = currentNode.Next;
            }

            return true;
        }

        private void CountTypeTokens(TypeReference typeRef, ref int count)
        {
            count++;
            if (typeRef is GenericInstanceType)
            {
                GenericInstanceType genericInstance = typeRef as GenericInstanceType;
                foreach (TypeReference argument in genericInstance.GenericArguments)
                {
                    CountTypeTokens(argument, ref count);
                }
            }
            else if (typeRef is TypeSpecification)
            {
                CountTypeTokens((typeRef as TypeSpecification).ElementType, ref count);
            }
        }
    }
}
