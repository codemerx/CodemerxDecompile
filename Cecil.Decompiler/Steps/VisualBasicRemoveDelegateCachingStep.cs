using Mono.Cecil.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;
using System;

namespace Telerik.JustDecompiler.Steps
{
    class VisualBasicRemoveDelegateCachingStep : RemoveDelegateCachingStep
    {
        private HashSet<VariableReference> variablesToNotInline;
        private Dictionary<VariableReference, Statement> initializationsToFix;

        public VisualBasicRemoveDelegateCachingStep()
        {
            this.variablesToNotInline = new HashSet<VariableReference>();
            this.initializationsToFix = new Dictionary<VariableReference, Statement>();
        }
        
        protected override void ProcessInitializations()
        {
            foreach (KeyValuePair<VariableReference, Statement> pair in this.initializationsToFix)
            {
                if (this.variableToReplacingExpressionMap.ContainsKey(pair.Key))
                {
                    ExpressionStatement statement = pair.Value as ExpressionStatement;
                    BinaryExpression binary = statement.Expression as BinaryExpression;
                    binary.Right = this.variableToReplacingExpressionMap[pair.Key];
                }
            }

            RemoveInitializations();
        }

        public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            if (this.variableToReplacingExpressionMap.ContainsKey(node.Variable))
            {
                if (this.variablesToNotInline.Contains(node.Variable))
                {
                    this.initializationsToFix.Add(node.Variable, this.initializationsToRemove[node.Variable]);
                    this.initializationsToRemove.Remove(node.Variable);

                    return node;
                }
            }

            return base.VisitVariableReferenceExpression(node);
        }

        public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
        {
            ICodeNode result = base.VisitFieldReferenceExpression(node);
            if (result.CodeNodeType == CodeNodeType.VariableReferenceExpression)
            {
                return Visit(result);
            }

            return result;
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            Expression methodTarget = node.GetTarget();
            if (methodTarget != null)
            {
                if (methodTarget.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    VariableReference variable = (methodTarget as VariableReferenceExpression).Variable;
                    if (this.variableToReplacingExpressionMap.ContainsKey(variable))
                    {
                        this.variablesToNotInline.Add(variable);
                    }
                }
                else if (methodTarget.CodeNodeType == CodeNodeType.FieldReferenceExpression)
                {
                    FieldDefinition field = (methodTarget as FieldReferenceExpression).Field.Resolve();
                    if (this.fieldToReplacingExpressionMap.ContainsKey(field))
                    {
                        VariableReference variableToNotInline = (this.fieldToReplacingExpressionMap[field] as VariableReferenceExpression).Variable;
                        this.variablesToNotInline.Add(variableToNotInline);
                    }
                }
            }

            return base.VisitMethodInvocationExpression(node);
        }

        protected override ICodeNode GetIfSubstitution(IfStatement node)
        {
            BinaryExpression condition = node.Condition as BinaryExpression;
            if (condition.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression)
            {
                return null;
            }

            FieldReferenceExpression fieldReference = condition.Left as FieldReferenceExpression;
            FieldDefinition fieldDefinition = fieldReference.Field.Resolve();
            if (!this.fieldToReplacingExpressionMap.ContainsKey(fieldDefinition))
            {
                throw new Exception("Caching field not found.");
            }

            VariableDefinition newVariable = new VariableDefinition(fieldDefinition.FieldType, this.context.MethodContext.Method);
            VariableReferenceExpression newVariableReference = new VariableReferenceExpression(newVariable, null);
            BinaryExpression newAssignment = new BinaryExpression(BinaryOperator.Assign, newVariableReference, this.fieldToReplacingExpressionMap[fieldDefinition],
                                                                  this.context.MethodContext.Method.Module.TypeSystem, null);
            ExpressionStatement newStatement = new ExpressionStatement(newAssignment);

            this.initializationsToRemove.Add(newVariable, newStatement);

            this.variableToReplacingExpressionMap.Add(newVariable, this.fieldToReplacingExpressionMap[fieldDefinition]);
            this.fieldToReplacingExpressionMap[fieldDefinition] = newVariableReference;

            this.context.MethodContext.Variables.Add(newVariable);
            this.context.MethodContext.VariablesToRename.Add(newVariable);

            return newStatement;
        }
    }
}
