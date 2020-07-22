using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    class VisualBasicVariablesToNotInlineFinder : BaseCodeVisitor, IVariablesToNotInlineFinder
    {
        private Dictionary<VariableReference, CodeNodeType> variableToAssignedCodeNodeTypeMap;
        private HashSet<VariableDefinition> variablesToNotInline;
        private ILanguage language;

        public VisualBasicVariablesToNotInlineFinder(ILanguage language)
        {
            this.language = language;
        }

        public HashSet<VariableDefinition> Find(Dictionary<int, IList<Expression>> blockExpressions)
        {
            ResetInternalState();

            foreach (List<Expression> blockExpression in blockExpressions.Values)
            {
                foreach (Expression expression in blockExpression)
                {
                    ProcessExpression(expression);
                }
            }

            return this.variablesToNotInline;
        }

        public HashSet<VariableDefinition> Find(StatementCollection statements)
        {
            ResetInternalState();

            this.Visit(statements);

            return this.variablesToNotInline;
        }

        public override void VisitExpressionStatement(ExpressionStatement node)
        {
            ProcessExpression(node.Expression);
        }

        private void ProcessExpression(Expression expression)
        {
            if (expression.CodeNodeType == CodeNodeType.BinaryExpression)
            {
                ProcessBinaryExpression(expression as BinaryExpression);
            }
            else if (expression.CodeNodeType == CodeNodeType.MethodInvocationExpression)
            {
                ProcessMethodInvocation(expression as MethodInvocationExpression);
            }
            else if (expression.CodeNodeType == CodeNodeType.DelegateInvokeExpression)
            {
                ProcessDelegateInvokeExpression(expression as DelegateInvokeExpression);
            }
        }

        private void ResetInternalState()
        {
            this.variableToAssignedCodeNodeTypeMap = new Dictionary<VariableReference, CodeNodeType>();
            this.variablesToNotInline = new HashSet<VariableDefinition>();
        }

        private void ProcessBinaryExpression(BinaryExpression binaryExpression)
        {
            if (!binaryExpression.IsAssignmentExpression)
            {
                return;
            }

            if (binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
            {
                return;
            }

            CodeNodeType rightCodeNodeType;
            if (binaryExpression.Right.IsArgumentReferenceToRefParameter())
            {
                rightCodeNodeType = CodeNodeType.ArgumentReferenceExpression;
            }
            else
            {
                rightCodeNodeType = binaryExpression.Right.CodeNodeType;
            }

            VariableReference variableReference = (binaryExpression.Left as VariableReferenceExpression).Variable;
            if (this.variableToAssignedCodeNodeTypeMap.ContainsKey(variableReference))
            {
                this.variableToAssignedCodeNodeTypeMap[variableReference] = rightCodeNodeType;
            }
            else
            {
                this.variableToAssignedCodeNodeTypeMap.Add(variableReference, rightCodeNodeType);
            }
        }

        private void ProcessMethodInvocation(MethodInvocationExpression methodInvocationExpression)
        {
            Expression methodTarget = methodInvocationExpression.GetTarget();
            if (methodTarget == null)
            {
                return;
            }

            VariableReferenceExpression variableReference;
            if (methodTarget.CodeNodeType == CodeNodeType.VariableReferenceExpression)
            {
                variableReference = methodTarget as VariableReferenceExpression;
            }
            // This handles the case where an method is called on a primitive type.
            else if (methodTarget.CodeNodeType == CodeNodeType.UnaryExpression)
            {
                UnaryExpression outerUnary = methodTarget as UnaryExpression;
                if (outerUnary.Operator != UnaryOperator.AddressDereference ||
                    outerUnary.Operand.CodeNodeType != CodeNodeType.UnaryExpression)
                {
                    return;
                }

                UnaryExpression innerUnary = outerUnary.Operand as UnaryExpression;
                if (innerUnary.Operator != UnaryOperator.AddressReference ||
                    innerUnary.Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression)
                {
                    return;
                }

                variableReference = innerUnary.Operand as VariableReferenceExpression;
            }
            else
            {
                return;
            }
            
            ProcessVariableReferenceExpression(variableReference);
        }

        private void ProcessVariableReferenceExpression(VariableReferenceExpression variableReferenceExpression)
        {
            VariableReference variableReference = variableReferenceExpression.Variable;
            if (this.variableToAssignedCodeNodeTypeMap.ContainsKey(variableReference))
            {
                CodeNodeType variableAssignmentType = this.variableToAssignedCodeNodeTypeMap[variableReference];
                VariableDefinition variable = variableReference.Resolve();
                if (!this.variablesToNotInline.Contains(variable))
                {
                    if (!this.language.IsValidLineStarter(variableAssignmentType))
                    {
                        this.variablesToNotInline.Add(variable);
                    }
                }
            }
        }

        private void ProcessDelegateInvokeExpression(DelegateInvokeExpression delegateInvokeExpression)
        {
            if (delegateInvokeExpression.Target == null ||
                delegateInvokeExpression.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
            {
                return;
            }
            
            ProcessVariableReferenceExpression(delegateInvokeExpression.Target as VariableReferenceExpression);
        }
    }
}
