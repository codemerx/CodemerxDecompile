using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
    class UsedAsTypeHelper
    {
        private readonly TypeSystem typeSystem;
        private readonly MethodSpecificContext methodContext;

        public UsedAsTypeHelper(MethodSpecificContext methodContext)
        {
            this.methodContext = methodContext;
            this.typeSystem = methodContext.Method.Module.TypeSystem;
        }

        /// <summary>
        /// Determines the type of the <paramref name="variable"/> based on its usage.
        /// </summary>
        /// <param name="instruction">The instruction that uses the variable.</param>
        /// <param name="variable">Tha variable.</param>
        /// <returns>Returns the ClassHierarchyNode for the found type.</returns>
        public TypeReference GetUseExpressionTypeNode(Instruction instruction, Expression instructionExpression, VariableReference variable)
        {
            Code instrOpCode = instruction.OpCode.Code;
            if (instrOpCode == Code.Ldobj)
            {
                TypeReference tr = instruction.Operand as TypeReference;
                return tr;
            }
            if (IsConditionalBranch(instrOpCode))
            {
                return typeSystem.Boolean;
            }
            if (instrOpCode == Code.Pop)
            {
                return null;
            }

            return GetUseExpressionTypeNode(instructionExpression, variable);
        }

        /// <summary>
        /// Checks if <paramref name="instructionOpCode"/> is code of conditional jump instruction. For more information see
        /// <see cref="Ecma-355.pdf"/>.
        /// </summary>
        /// <param name="instructionOpCode">The operation code of the instruction.</param>
        /// <returns>Returns true, if the operation conde is for conditional jump.</returns>
        private static bool IsConditionalBranch(Code instructionOpCode)
        {
            return instructionOpCode == Code.Brtrue || instructionOpCode == Code.Brtrue_S || instructionOpCode == Code.Brfalse || instructionOpCode == Code.Brfalse_S;
        }

        /// <summary>
        /// Determines the type which the <paramref name="variable"/> should have, according to its usage in the <paramref name="expression"/>.
        /// </summary>
        /// <param name="expression">The expression that uses the variable.</param>
        /// <param name="variable">The variable whose type is being infered.</param>
        /// <returns>Returns the ClassHierarchyNode coresponding to the infered type.</returns>
        private TypeReference GetUseExpressionTypeNode(Expression expression, VariableReference variable)
        {
            switch (expression.CodeNodeType)
            {
                case CodeNodeType.BaseCtorExpression:
                case CodeNodeType.ThisCtorExpression:
                case CodeNodeType.MethodInvocationExpression:
                case CodeNodeType.PropertyReferenceExpression:
                    return GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
                case CodeNodeType.BinaryExpression:
                    return GetUseInBinaryExpression(expression as BinaryExpression, variable);
                case CodeNodeType.VariableDeclarationExpression:
                    VariableDeclarationExpression vdEx = expression as VariableDeclarationExpression;
                    return vdEx.Variable.VariableType;
                case CodeNodeType.VariableReferenceExpression:
                    VariableReferenceExpression vrEx = expression as VariableReferenceExpression;
                    return vrEx.Variable == variable ? vrEx.Variable.VariableType : null;
                case CodeNodeType.ExplicitCastExpression:
                case CodeNodeType.SafeCastExpression:
                    return typeSystem.Object;
                case CodeNodeType.ArrayIndexerExpression:
                    return GetUseInArrayIndexer(expression as ArrayIndexerExpression, variable);
                case CodeNodeType.ObjectCreationExpression:
                    return GetUseInObjectCreation(expression as ObjectCreationExpression, variable);
                case CodeNodeType.ReturnExpression:
                    return methodContext.Method.FixedReturnType;
                case CodeNodeType.ArrayCreationExpression:
                    return GetUseInArrayCreation(expression as ArrayCreationExpression, variable);
                case CodeNodeType.UnaryExpression:
                    return GetUseExpressionTypeNode((expression as UnaryExpression).Operand, variable);
                case CodeNodeType.BoxExpression:
                    return (expression as BoxExpression).BoxedAs;
                case CodeNodeType.ThrowExpression:
                    return null;
                case CodeNodeType.FieldReferenceExpression:
                    return (expression as FieldReferenceExpression).Field.DeclaringType;
                default:
                    throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
            }
        }

        /// <summary>
        /// Resolves the type of usage of <paramref name="variable"/>, when the expression it's used in is ArrayCreationExpression
        /// </summary>
        /// <param name="arrayCreationExpression">The expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode coresponding go the infered type.</returns>
        private TypeReference GetUseInArrayCreation(ArrayCreationExpression arrayCreationExpression, VariableReference variable)
        {
            foreach (Expression expr in arrayCreationExpression.Dimensions)
            {
                if (expr is VariableReferenceExpression && (expr as VariableReferenceExpression).Variable == variable)
                {
                    ///If the variable is used as an index, then it's type is Int32.
                    return typeSystem.Int32;
                }
            }
            foreach (Expression ex in arrayCreationExpression.Initializer.Expressions)
            {
                if (ex is VariableReferenceExpression && (ex as VariableReferenceExpression).Variable == variable)
                {
                    ///If the variable is directly referenced in the Initializer, then its type is the element type of the array.
                    return arrayCreationExpression.ElementType;
                }
            }
            throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
        }

        /// <summary>
        /// Determines the use type of <paramref name="variable"/> in <paramref name="objectCreationExpression"/>.
        /// </summary>
        /// <param name="objectCreationExpression">The object creataion expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode corresponding to the infered type.</returns>
        private TypeReference GetUseInObjectCreation(ObjectCreationExpression objectCreationExpression, VariableReference variable)
        {
            Expression arg = null;
            foreach (Expression expr in objectCreationExpression.Arguments)
            {
                if (expr is VariableReferenceExpression && (expr as VariableReferenceExpression).Variable == variable)
                {
                    arg = expr;
                }
            }
            return objectCreationExpression.Constructor.Parameters[objectCreationExpression.Arguments.IndexOf(arg)].ResolveParameterType(objectCreationExpression.Constructor);
        }

        /// <summary>
        /// Determines the use type of <paramref name="variable"/> in <paramref name="arrayIndexerExpression"/>.
        /// </summary>
        /// <param name="arrayIndexerExpression">The array indexer expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode corresponding to the infered type.</returns>
        private TypeReference GetUseInArrayIndexer(ArrayIndexerExpression arrayIndexerExpression, VariableReference variable)
        {
            foreach (Expression expr in arrayIndexerExpression.Indices)
            {
                if (expr is VariableReferenceExpression && (expr as VariableReferenceExpression).Variable == variable)
                {
                    return typeSystem.Int32;
                }
            }

            ///If the variable is not indexer, it might be of any array type.
            ///System.Array is returned, since it's the parent type of every array type.
            TypeReference result = new TypeReference("System", "Array", typeSystem.Object.Module, typeSystem.Object.Scope);
            return result;
        }

        /// <summary>
        /// Determines the use type of <paramref name="variable"/> in <paramref name="binaryExpression"/>.
        /// </summary>
        /// <param name="binaryExpression">The binary expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode corresponding to the infered type.</returns>
        private TypeReference GetUseInBinaryExpression(BinaryExpression binaryExpression, VariableReference variable)
        {
            if (binaryExpression.Right.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                (binaryExpression.Right as VariableReferenceExpression).Variable == variable)
            {
                return binaryExpression.Left.ExpressionType;
            }
            else if (binaryExpression.Left is VariableReferenceExpression && (binaryExpression.Left as VariableReferenceExpression).Variable == variable)
            {
                return binaryExpression.Right.ExpressionType;
            }

            return GetUseExpressionTypeNode(binaryExpression.Left, variable) ?? GetUseExpressionTypeNode(binaryExpression.Right, variable);
        }

        /// <summary>
        /// Determines the use type of <paramref name="variable"/> in <paramref name="methodInvocationExpression"/>.
        /// </summary>
        /// <param name="methodInvocationExpression">The method invocation expression.</param>
        /// <param name="variable">The variable.</param>
        /// <returns>Returns the ClassHierarchyNode corresponding to the infered type.</returns>
        private TypeReference GetUseInMethodInvocation(MethodInvocationExpression methodInvocationExpression, VariableReference variable)
        {
            Expression ex = null;
            foreach (Expression expr in methodInvocationExpression.Arguments)
            {
                //Make a better check if the variable is actually taking place in the argument.
                //Expressions different from VariableReferenceExpression can also be part of method's arguments
                //Sample test: RayTracer.StartRender
                if (expr is VariableReferenceExpression && (expr as VariableReferenceExpression).Variable == variable)
                {
                    ex = expr;
                }
            }
            if (ex != null)
            {
                //check if variable is one of method's arguments
                int index = methodInvocationExpression.Arguments.IndexOf(ex);
                MethodReference method = methodInvocationExpression.MethodExpression.Method;
                return method.Parameters[index].ResolveParameterType(method);
            }
            else
            {
                //variable should be the method target
                Expression target = (methodInvocationExpression.MethodExpression as MemberReferenceExpresion).Target;
                if ((target as VariableReferenceExpression).Variable == variable)
                {
                    //the target should be of the method's declared type
                    //not sure how this handles extension methods
                    return (methodInvocationExpression.MethodExpression as MemberReferenceExpresion).Member.DeclaringType;
                }
            }
            return null;
        }
    }
}
