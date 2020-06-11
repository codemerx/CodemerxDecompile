using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Steps.DynamicVariables;
using Mono.Cecil.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
    internal class ResolveDynamicVariablesStep : BaseCodeVisitor, IDecompilationStep
    {
        private const int UseCompileTimeType = 1;
        private const string CallSiteInstanceTypeName = "System.Runtime.CompilerServices.CallSite<!0>";
        private const string InvalidStatementExceptionString = "Invalid statement.";
        private const string IEnumerableOfCSharpArgumentInfo = "System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>";
        private const string IEnumerableOfSystemType = "System.Collections.Generic.IEnumerable<System.Type>";

        private readonly Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap = new Dictionary<FieldDefinition, CallSiteInfo>();
        private readonly Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap = new Dictionary<VariableReference, CallSiteInfo>();
        private readonly HashSet<Statement> statementsToRemove = new HashSet<Statement>();

        private MethodSpecificContext methodContext;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            methodContext = context.MethodContext;
            Visit(body);
            body = CallSiteInvocationReplacer.ReplaceInvocations(body, fieldToCallSiteInfoMap, variableToCallSiteInfoMap, statementsToRemove,
                methodContext.Method.Module.TypeSystem);
            RemoveStatements();
            return body;
        }

        public override void VisitIfStatement(IfStatement node)
        {
            if(node.Else == null && node.Condition.CodeNodeType == CodeNodeType.BinaryExpression)
            {
                BinaryExpression ifCondition = node.Condition as BinaryExpression;
                if(ifCondition.Operator == BinaryOperator.ValueEquality && ifCondition.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression &&
                    ifCondition.Right.CodeNodeType == CodeNodeType.LiteralExpression && (ifCondition.Right as LiteralExpression).Value == null)
                {
                    FieldReferenceExpression callSiteField = ifCondition.Left as FieldReferenceExpression;
                    FieldDefinition resolvedField = callSiteField.Field.Resolve();
                    if (callSiteField.Field.FieldType.GetElementType().GetFriendlyFullName(null) == CallSiteInstanceTypeName &&
                        CheckFieldDefinition(resolvedField))
                    {
                        ProcessCallSiteCaching(node, resolvedField);
                        statementsToRemove.Add(node);
                        return;
                    }
                }
            }
            base.VisitIfStatement(node);
        }

        private bool CheckFieldDefinition(FieldDefinition callSiteFieldDefinition)
        {
            return callSiteFieldDefinition != null && callSiteFieldDefinition.IsStatic && callSiteFieldDefinition.DeclaringType != null &&
                callSiteFieldDefinition.DeclaringType.HasCompilerGeneratedAttribute() &&
                callSiteFieldDefinition.DeclaringType.DeclaringType == methodContext.Method.DeclaringType;
        }

        public override void VisitExpressionStatement(ExpressionStatement node)
        {
            if (!node.IsAssignmentStatement())
            {
                return;
            }

            BinaryExpression assignExpression = node.Expression as BinaryExpression;
            if(assignExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                assignExpression.Right.CodeNodeType != CodeNodeType.FieldReferenceExpression)
            {
                return;
            }

            FieldReferenceExpression targetFieldRefExpression = assignExpression.Right as FieldReferenceExpression;
            if(targetFieldRefExpression.Target == null ||
                targetFieldRefExpression.Target.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                targetFieldRefExpression.Field.Name != "Target")
            {
                return;
            }

            FieldDefinition callSiteField = (targetFieldRefExpression.Target as FieldReferenceExpression).Field.Resolve();
            CallSiteInfo callSiteInfo;
            if(callSiteField == null || !fieldToCallSiteInfoMap.TryGetValue(callSiteField, out callSiteInfo))
            {
                return;
            }

            variableToCallSiteInfoMap.Add((assignExpression.Left as VariableReferenceExpression).Variable, callSiteInfo);
            statementsToRemove.Add(node);
        }

        private void ProcessCallSiteCaching(IfStatement theIf, FieldDefinition callSiteField)
        {
            MethodInvocationExpression binderMethodInvocation = GetBinderMethodInvocation(theIf.Then.Statements.Last() as ExpressionStatement, callSiteField);
            CallSiteInfo callSiteInfo = new CallSiteInfo(callSiteField, binderMethodInvocation.MethodExpression.Method.Name);

            int index = 0;

            if(callSiteInfo.BinderType == CallSiteBinderType.GetMember || callSiteInfo.BinderType == CallSiteBinderType.InvokeMember ||
                callSiteInfo.BinderType == CallSiteBinderType.SetMember || callSiteInfo.BinderType == CallSiteBinderType.IsEvent)
            {
                GetMemberNameArgument(binderMethodInvocation, callSiteInfo);
            }

            if (callSiteInfo.BinderType == CallSiteBinderType.BinaryOperation || callSiteInfo.BinderType == CallSiteBinderType.UnaryOperation)
            {
                GetOperatorArgument(binderMethodInvocation, callSiteInfo);
            }

            if (callSiteInfo.BinderType == CallSiteBinderType.Convert)
            {
                GetConvertTypeArgument(binderMethodInvocation, callSiteInfo);
            }

            if(callSiteInfo.BinderType == CallSiteBinderType.InvokeMember)
            {
                VariableReference typeArrayVariable = GetTypeArrayVariable(binderMethodInvocation);
                if(typeArrayVariable != null)
                {
                    callSiteInfo.GenericTypeArguments = new List<TypeReference>();
                    ProcessArgumentArray(theIf.Then.Statements, ref index, typeArrayVariable, callSiteInfo, GetGenericTypeArgument);
                }
            }

            if (callSiteInfo.BinderType != CallSiteBinderType.Convert && callSiteInfo.BinderType != CallSiteBinderType.IsEvent)
            {
                ProcessArgumentArray(theIf.Then.Statements, ref index, GetArgumentArrayVariable(binderMethodInvocation), callSiteInfo, GetDynamicArgument);
            }
            else
            {
                callSiteInfo.DynamicArgumentIndices.Add(0);
            }

            fieldToCallSiteInfoMap.Add(callSiteField, callSiteInfo);
        }

        private void GetMemberNameArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
        {
            int memberNameIndex = 1;

            if (binderInvocation.Arguments[memberNameIndex].CodeNodeType == CodeNodeType.LiteralExpression)
            {
                callSiteInfo.MemberName = (binderInvocation.Arguments[memberNameIndex] as LiteralExpression).Value as string;
                if (callSiteInfo.MemberName != null)
                {
                    return;
                }
            }
            throw new Exception("Invalid argument: member name.");
        }

        private void GetOperatorArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
        {
            int operatorIndex = 1;

            if (binderInvocation.Arguments[operatorIndex].CodeNodeType != CodeNodeType.LiteralExpression)
            {
                throw new Exception("Invalid argument: operator.");
            }

            callSiteInfo.Operator = (ExpressionType)Convert.ToInt32((binderInvocation.Arguments[operatorIndex] as LiteralExpression).Value);
        }

        private void GetConvertTypeArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
        {
            int typeArgumentIndex = 1;
            TypeReference typeRef;
            if(binderInvocation.Arguments[typeArgumentIndex].CodeNodeType != CodeNodeType.MethodInvocationExpression ||
                !(binderInvocation.Arguments[typeArgumentIndex] as MethodInvocationExpression).IsTypeOfExpression(out typeRef))
            {
                throw new Exception("Invalid argument: convert type.");
            }

            callSiteInfo.ConvertType = typeRef;
        }

        private MethodInvocationExpression GetBinderMethodInvocation(ExpressionStatement callSiteCreationStatement, FieldDefinition callSiteField)
        {
            if (callSiteCreationStatement == null || callSiteCreationStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression ||
                (callSiteCreationStatement.Expression as BinaryExpression).Operator != BinaryOperator.Assign ||
                (callSiteCreationStatement.Expression as BinaryExpression).Left.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                ((callSiteCreationStatement.Expression as BinaryExpression).Left as FieldReferenceExpression).Field.Resolve() != callSiteField)
            {
                throw new Exception("Last statement is not CallSite field assignment.");
            }

            MethodInvocationExpression creationInvocation = (callSiteCreationStatement.Expression as BinaryExpression).Right as MethodInvocationExpression;
            if (creationInvocation.MethodExpression.Method.DeclaringType.GetElementType().GetFriendlyFullName(null) != CallSiteInstanceTypeName ||
                creationInvocation.MethodExpression.Target != null || creationInvocation.MethodExpression.Method.Name != "Create" ||
                creationInvocation.Arguments[0].CodeNodeType != CodeNodeType.MethodInvocationExpression)
            {
                throw new Exception("Invalid CallSite field assignment.");
            }

            MethodInvocationExpression binderCreationInvocation = creationInvocation.Arguments[0] as MethodInvocationExpression;
            if(binderCreationInvocation.MethodExpression.Target != null ||
                binderCreationInvocation.MethodExpression.Method.DeclaringType.GetFriendlyFullName(null) != "Microsoft.CSharp.RuntimeBinder.Binder")
            {
                throw new Exception("Invalid CallSite creation argument.");
            }

            return binderCreationInvocation;
        }

        private void ProcessArgumentArray(StatementCollection statements, ref int index, VariableReference typesArray, CallSiteInfo callSiteInfo,
            Action<MethodInvocationExpression, int, CallSiteInfo> action)
        {
			if ((statements[index] is ExpressionStatement) && ((statements[index] as ExpressionStatement).Expression is BinaryExpression))
			{
				BinaryExpression binaryExpression = (statements[index] as ExpressionStatement).Expression as BinaryExpression;
				if (binaryExpression.IsAssignmentExpression && (binaryExpression.Left is VariableReferenceExpression) && (binaryExpression.Right is MethodInvocationExpression))
				{
					index++;
				}
			}

            int arraySize = CheckNewArrayInitializationAndSize(statements[index++], typesArray);

            for (int i = 0; i < arraySize; i++)
            {
                if (!statements[index].IsAssignmentStatement())
                {
                    throw new Exception(InvalidStatementExceptionString);
                }

                BinaryExpression elementAssignment = (statements[index++] as ExpressionStatement).Expression as BinaryExpression;
                if(elementAssignment.Left.CodeNodeType != CodeNodeType.ArrayIndexerExpression ||
                    !CheckArrayIndexerExpression(elementAssignment.Left as ArrayIndexerExpression, typesArray, i) ||
                    elementAssignment.Right.CodeNodeType != CodeNodeType.MethodInvocationExpression)
                {
                    throw new Exception(InvalidStatementExceptionString);
                }

                action(elementAssignment.Right as MethodInvocationExpression, i, callSiteInfo);
            }
        }

        private void GetGenericTypeArgument(MethodInvocationExpression expression, int index, CallSiteInfo callSiteInfo)
        {
            TypeReference typeRef;
            if (!expression.IsTypeOfExpression(out typeRef))
            {
                throw new Exception(InvalidStatementExceptionString);
            }

            callSiteInfo.GenericTypeArguments.Add(typeRef);
        }

        private void GetDynamicArgument(MethodInvocationExpression expression, int index, CallSiteInfo callSiteInfo)
        {
            if (expression.MethodExpression.Method.Name != "Create" ||
                expression.MethodExpression.Target != null ||
                expression.MethodExpression.Method.DeclaringType.GetFriendlyFullName(null) != "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" ||
                expression.Arguments.Count != 2 || expression.Arguments[0].CodeNodeType != CodeNodeType.LiteralExpression)
            {
                throw new Exception(InvalidStatementExceptionString);
            }

            int argumentInfoFlag = Convert.ToInt32((expression.Arguments[0] as LiteralExpression).Value);

            if((argumentInfoFlag & UseCompileTimeType) == 0)
            {
                callSiteInfo.DynamicArgumentIndices.Add(index);
            }
        }

        private int CheckNewArrayInitializationAndSize(Statement statement, VariableReference arrayVariable)
        {
            if (!statement.IsAssignmentStatement())
            {
                throw new Exception(InvalidStatementExceptionString);
            }

            BinaryExpression arrayInitialization = (statement as ExpressionStatement).Expression as BinaryExpression;

            if (arrayInitialization.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                (arrayInitialization.Left as VariableReferenceExpression).Variable != arrayVariable)
            {
                throw new Exception(InvalidStatementExceptionString);
            }

            if (arrayInitialization.Right.CodeNodeType != CodeNodeType.ArrayCreationExpression ||
                (arrayInitialization.Right as ArrayCreationExpression).Dimensions.Count != 1 ||
                (arrayInitialization.Right as ArrayCreationExpression).Dimensions[0].CodeNodeType != CodeNodeType.LiteralExpression)
            {
                throw new Exception(InvalidStatementExceptionString);
            }

            return Convert.ToInt32(((arrayInitialization.Right as ArrayCreationExpression).Dimensions[0] as LiteralExpression).Value);
        }

        private bool CheckArrayIndexerExpression(ArrayIndexerExpression expression, VariableReference arrayVariable, int index)
        {
            return expression.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    (expression.Target as VariableReferenceExpression).Variable == arrayVariable &&
                    expression.Indices.Count == 1 &&
                    expression.Indices[0].CodeNodeType == CodeNodeType.LiteralExpression &&
                    Convert.ToInt32((expression.Indices[0] as LiteralExpression).Value) == index;
        }

        private VariableReference GetTypeArrayVariable(MethodInvocationExpression binderMethodInvocation)
        {
            int typeArrayArgumentIndex = 2;
            Expression typeArrayArgument = binderMethodInvocation.Arguments[typeArrayArgumentIndex];
            if (typeArrayArgument.CodeNodeType == CodeNodeType.LiteralExpression &&
                (typeArrayArgument as LiteralExpression).Value == null)
            {
                return null;
            }

            VariableReferenceExpression typeArrayVariableReference = null;
            if (typeArrayArgument.CodeNodeType == CodeNodeType.VariableReferenceExpression)
            {
                typeArrayVariableReference = typeArrayArgument as VariableReferenceExpression;
            }
            else if (typeArrayArgument.CodeNodeType == CodeNodeType.ExplicitCastExpression)
            {
                ExplicitCastExpression cast = typeArrayArgument as ExplicitCastExpression;
                if (cast.ExpressionType.GetFriendlyFullName(null) == IEnumerableOfSystemType &&
                    cast.Expression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    typeArrayVariableReference = cast.Expression as VariableReferenceExpression;
                }
            }

            if (typeArrayVariableReference == null)
            {
                throw new Exception("Invalid argument: typeArguments.");
            }

            return typeArrayVariableReference.Variable;
        }

        private VariableReference GetArgumentArrayVariable(MethodInvocationExpression binderMethodInvocation)
        {
            int argumentArrayIndex = binderMethodInvocation.Arguments.Count - 1;
            Expression argumentArrayExpression = binderMethodInvocation.Arguments[argumentArrayIndex];
            VariableReferenceExpression argumentArrayVariableReference = null;
            if (argumentArrayExpression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
            {
                argumentArrayVariableReference = argumentArrayExpression as VariableReferenceExpression;
            }
            else if (argumentArrayExpression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
            {
                ExplicitCastExpression cast = argumentArrayExpression as ExplicitCastExpression;
                if (cast.ExpressionType.GetFriendlyFullName(null) == IEnumerableOfCSharpArgumentInfo &&
                    cast.Expression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    argumentArrayVariableReference = cast.Expression as VariableReferenceExpression;
                }
            }

            if (argumentArrayVariableReference == null)
            {
                throw new Exception("Invalid argument: argumentInfo.");
            }

            return argumentArrayVariableReference.Variable;
        }

        private void RemoveStatements()
        {
            foreach (Statement statement in statementsToRemove)
            {
                (statement.Parent as BlockStatement).Statements.Remove(statement);
            }
        }
    }
}
