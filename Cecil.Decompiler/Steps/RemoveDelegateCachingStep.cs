using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
    class RemoveDelegateCachingStep : BaseCodeTransformer, IDecompilationStep
    {
        protected DecompilationContext context;
        protected Dictionary<FieldDefinition, Expression> fieldToReplacingExpressionMap;
        protected Dictionary<VariableReference, Expression> variableToReplacingExpressionMap;
        protected Dictionary<VariableReference, Statement> initializationsToRemove;
        private DelegateCachingVersion cachingVersion;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.context = context;
            this.fieldToReplacingExpressionMap = new Dictionary<FieldDefinition, Expression>();
            this.variableToReplacingExpressionMap = new Dictionary<VariableReference, Expression>();
            this.initializationsToRemove = new Dictionary<VariableReference, Statement>();
            BlockStatement result = (BlockStatement)Visit(body);
            ProcessInitializations();
            return result;
        }

        protected virtual void ProcessInitializations()
        {
            RemoveInitializations();
        }

        protected void RemoveInitializations()
        {
            foreach (KeyValuePair<VariableReference, Statement> pair in initializationsToRemove)
            {
                if (!variableToReplacingExpressionMap.ContainsKey(pair.Key))
                {
                    continue;
                }

                BlockStatement parentBlock = pair.Value.Parent as BlockStatement;
                if (parentBlock == null)
                {
                    throw new Exception("Invalid parent statement.");
                }

                this.context.MethodContext.Variables.Remove(pair.Key.Resolve());
                parentBlock.Statements.Remove(pair.Value);
            }
        }

        public override ICodeNode VisitIfStatement(IfStatement node)
        {
            if (CheckIfStatement(node))
            {
                return GetIfSubstitution(node);
            }

            return base.VisitIfStatement(node);
        }

        protected virtual ICodeNode GetIfSubstitution(IfStatement node)
        {
            return null;
        }

        private bool CheckIfStatement(IfStatement theIf)
        {
            if (!CheckIfStatementStructure(theIf))
            {
                return false;
            }

            BinaryExpression theCondition = theIf.Condition as BinaryExpression;

            if (theCondition.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
            {
                return CheckFieldCaching(theIf);
            }
            else if (theCondition.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
            {
                return CheckVariableCaching(theIf);
            }

            return false;
        }

        private bool CheckIfStatementStructure(IfStatement theIf)
        {
            if (theIf.Else == null && theIf.Condition.CodeNodeType == CodeNodeType.BinaryExpression)
            {
                if (theIf.Then.Statements.Count == 1 && theIf.Then.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement)
                {
                    this.cachingVersion = DelegateCachingVersion.V1;
                    return true;
                }
                else if (theIf.Then.Statements.Count == 3)
                {
                    ExpressionStatement first = theIf.Then.Statements[0] as ExpressionStatement;
                    if (first == null)
                    {
                        return false;
                    }

                    BinaryExpression firstBinary = first.Expression as BinaryExpression;
                    if (firstBinary == null)
                    {
                        return false;
                    }

                    if (!firstBinary.IsAssignmentExpression ||
                        firstBinary.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                        firstBinary.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                        !initializationsToRemove.ContainsKey((firstBinary.Right as VariableReferenceExpression).Variable))
                    {
                        return false;
                    }

                    if (theIf.Then.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement)
                    {
                        return false;
                    }

                    ExpressionStatement third = theIf.Then.Statements[2] as ExpressionStatement;
                    if (third == null)
                    {
                        return false;
                    }

                    BinaryExpression thirdBinary = third.Expression as BinaryExpression;
                    if (thirdBinary == null)
                    {
                        return false;
                    }

                    if (!thirdBinary.IsAssignmentExpression ||
                        thirdBinary.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                        thirdBinary.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                        !initializationsToRemove.ContainsKey((thirdBinary.Right as VariableReferenceExpression).Variable))
                    {
                        return false;
                    }

                    this.cachingVersion = DelegateCachingVersion.V2;
                    return true;
                }
            }

            return false;
        }

        private bool CheckFieldCaching(IfStatement theIf)
        {
            BinaryExpression theCondition = theIf.Condition as BinaryExpression;

            if (theCondition.Operator != BinaryOperator.ValueEquality || theCondition.Right.CodeNodeType != CodeNodeType.LiteralExpression ||
                (theCondition.Right as LiteralExpression).Value != null)
            {
                return false;
            }

            FieldDefinition theFieldDef = (theCondition.Left as FieldReferenceExpression).Field.Resolve();
            if (theFieldDef == null || !theFieldDef.IsStatic || !theFieldDef.IsPrivate)
            {
                return false;
            }

            BinaryExpression theAssignExpression = (theIf.Then.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
            if (theAssignExpression == null || !theAssignExpression.IsAssignmentExpression ||
                theAssignExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression ||
                (theAssignExpression.Left as FieldReferenceExpression).Field.Resolve() != theFieldDef)
            {
                return false;
            }

            if (fieldToReplacingExpressionMap.ContainsKey(theFieldDef))
            {
                throw new Exception("A caching field cannot be assigned more than once.");
            }

            //Slow checks
            if (!theFieldDef.IsCompilerGenerated())
            {
                return false;
            }

            TypeDefinition fieldTypeDef = theFieldDef.FieldType.Resolve();
            if (fieldTypeDef == null || fieldTypeDef.BaseType == null || fieldTypeDef.BaseType.FullName != "System.MulticastDelegate")
            {
                return false;
            }

            fieldToReplacingExpressionMap[theFieldDef] = theAssignExpression.Right;
            return true;
        }

        private bool CheckVariableCaching(IfStatement theIf)
        {
            BinaryExpression theCondition = theIf.Condition as BinaryExpression;

            if (theCondition.Operator != BinaryOperator.ValueEquality || theCondition.Right.CodeNodeType != CodeNodeType.LiteralExpression ||
                (theCondition.Right as LiteralExpression).Value != null)
            {
                return false;
            }

            VariableReference theVariable = (theCondition.Left as VariableReferenceExpression).Variable;
            if (!initializationsToRemove.ContainsKey(theVariable))
            {
                return false;
            }

            int theAssignExpressionIndex = this.cachingVersion == DelegateCachingVersion.V1 ? 0 : 1;
            BinaryExpression theAssignExpression = (theIf.Then.Statements[theAssignExpressionIndex] as ExpressionStatement).Expression as BinaryExpression;
            if (theAssignExpression == null || !theAssignExpression.IsAssignmentExpression ||
                theAssignExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                (theAssignExpression.Left as VariableReferenceExpression).Variable != theVariable)
            {
                return false;
            }

            if (variableToReplacingExpressionMap.ContainsKey(theVariable))
            {
                throw new Exception("A caching variable cannot be assigned more than once.");
            }

            //Slow checks
            TypeDefinition variableTypeDef = theVariable.VariableType.Resolve();
            if (variableTypeDef == null || variableTypeDef.BaseType == null || variableTypeDef.BaseType.FullName != "System.MulticastDelegate")
            {
                return false;
            }

            variableToReplacingExpressionMap[theVariable] = theAssignExpression.Right;
            return true;
        }

        public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
        {
            Expression fieldValue;
            FieldDefinition theFieldDef = node.Field.Resolve();
            if (theFieldDef != null && fieldToReplacingExpressionMap.TryGetValue(theFieldDef, out fieldValue))
            {
                return fieldValue.CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
            }

            return base.VisitFieldReferenceExpression(node);
        }

        public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            Expression variableValue;
            if (variableToReplacingExpressionMap.TryGetValue(node.Variable, out variableValue))
            {
                return variableValue.CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
            }

            return base.VisitVariableReferenceExpression(node);
        }

        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            if (node.IsAssignmentExpression)
            {
                if (node.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
                {
                    FieldDefinition theFieldDef = (node.Left as FieldReferenceExpression).Field.Resolve();
                    if (theFieldDef != null && fieldToReplacingExpressionMap.ContainsKey(theFieldDef))
                    {
                        throw new Exception("A caching field cannot be assigned more than once.");
                    }
                }
                else if (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    if (variableToReplacingExpressionMap.ContainsKey((node.Left as VariableReferenceExpression).Variable))
                    {
                        throw new Exception("A caching variable cannot be assigned more than once.");
                    }
                }
            }

            return base.VisitBinaryExpression(node);
        }

        public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
        {
            if (CheckVariableInitialization(node))
            {
                return node;
            }
            return base.VisitExpressionStatement(node);
        }

        private bool CheckVariableInitialization(ExpressionStatement node)
        {
            if (!node.IsAssignmentStatement())
            {
                return false;
            }

            BinaryExpression theAssignExpression = node.Expression as BinaryExpression;

            if (theAssignExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
            {
                return false;
            }

            Expression value = theAssignExpression.Right;
            if (value.CodeNodeType == CodeNodeType.ExplicitCastExpression)
            {
                value = (value as ExplicitCastExpression).Expression;
            }

            if ((value.CodeNodeType != CodeNodeType.LiteralExpression || (value as LiteralExpression).Value != null) &&
                (value.CodeNodeType != CodeNodeType.FieldReferenceExpression))
            {
                return false;
            }

            if (value.CodeNodeType == CodeNodeType.FieldReferenceExpression)
            {
                FieldReferenceExpression fieldReferenceExpression = value as FieldReferenceExpression;
                
                TypeDefinition fieldType = fieldReferenceExpression.ExpressionType.Resolve();
                if (fieldType == null || fieldType.BaseType == null || fieldType.BaseType.FullName != "System.MulticastDelegate")
                {
                    return false;
                }

                // Slow checks
                FieldDefinition fieldDef = fieldReferenceExpression.Field.Resolve();
                if ((fieldDef.DeclaringType != this.context.MethodContext.Method.DeclaringType &&
                    !fieldDef.DeclaringType.IsNestedIn(this.context.MethodContext.Method.DeclaringType)) ||
                    !fieldDef.DeclaringType.IsCompilerGenerated())
                {
                    return false;
                }
            }

            initializationsToRemove[(theAssignExpression.Left as VariableReferenceExpression).Variable] = node;
            return true;
        }

        private enum DelegateCachingVersion
        {
            V1, // Before C# 6.0
            V2  // C# 6.0
        }
    }
}
