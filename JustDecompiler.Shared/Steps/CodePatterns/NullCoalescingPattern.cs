using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
    class NullCoalescingPattern : CommonPatterns, ICodePattern
    {
        private readonly MethodSpecificContext methodContext;

        public NullCoalescingPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext)
            : base(patternsContext, methodContext.Method.Module.TypeSystem)
        {
            this.methodContext = methodContext;
        }

        public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
        {
            result = null;
            replacedStatementsCount = 2;
            for (startIndex = 0; startIndex + 1 < statements.Count; startIndex++)
            {
                bool currentTransform = TryMatchInternal(statements, startIndex, out result);
                if (currentTransform)
                {
                    return true;
                }
            }
            return false;
        }

        //x = y ?? z;
        //
        //==
        //
        //x = y;
        //if(x == null)
        //{
        //      dummyVar = x;
        //      x = z;
        //}
        //
        //x - stack variable
        //y, z - expressions
        //dummyVar - dummy variable(this assignment might be missing)
        private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result)
        {
            result = null;
            if (statements.Count < startIndex + 2)
            {
                return false;
            }

            VariableReference xVariableReference;
            Expression yExpressionValue;
            Expression zExpressionValue;

            if (statements[startIndex].CodeNodeType != CodeNodeType.ExpressionStatement ||
                statements[startIndex + 1].CodeNodeType != CodeNodeType.IfStatement)
            {
                return false;
            }
            if (!string.IsNullOrEmpty(statements[startIndex + 1].Label))
            {
                return false;
            }

            BinaryExpression yToXAssingExpression = (statements[startIndex] as ExpressionStatement).Expression as BinaryExpression;
            if (!IsAssignToVariableExpression(yToXAssingExpression, out xVariableReference))
            {
                return false;
            }

            yExpressionValue = yToXAssingExpression.Right;

            IfStatement theIfStatement = statements[startIndex + 1] as IfStatement;
            int zToXAssignIndex = ContainsDummyAssignment(theIfStatement.Then, xVariableReference) ? 1 : 0;
            if (theIfStatement.Else != null || theIfStatement.Then.Statements.Count != 1 + zToXAssignIndex ||
                theIfStatement.Then.Statements[zToXAssignIndex].CodeNodeType != CodeNodeType.ExpressionStatement
                || !string.IsNullOrEmpty(theIfStatement.Then.Statements[zToXAssignIndex].Label))
            {
                return false;
            }

            BinaryExpression theCondition = theIfStatement.Condition as BinaryExpression;
            if (theCondition == null || theCondition.Operator != BinaryOperator.ValueEquality ||
                theCondition.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                (theCondition.Left as VariableReferenceExpression).Variable != xVariableReference ||
                theCondition.Right.CodeNodeType != CodeNodeType.LiteralExpression ||
                (theCondition.Right as LiteralExpression).Value != null)
            {
                return false;
            }

            BinaryExpression zToXAssignExpression = (theIfStatement.Then.Statements[zToXAssignIndex] as ExpressionStatement).Expression as BinaryExpression;
            VariableReference zVariable;
            if (zToXAssignExpression == null || !IsAssignToVariableExpression(zToXAssignExpression, out zVariable) ||
                zVariable != xVariableReference)
            {
                return false;
            }

            zExpressionValue = zToXAssignExpression.Right;

            if (!yExpressionValue.HasType || !zExpressionValue.HasType || yExpressionValue.ExpressionType.FullName != zExpressionValue.ExpressionType.FullName)
            {
                return false;
            }
            

            BinaryExpression nullCoalescingEpxression = new BinaryExpression(BinaryOperator.NullCoalesce, yExpressionValue, zExpressionValue, typeSystem, null);
            BinaryExpression nullCoalescingAssign = new BinaryExpression(BinaryOperator.Assign,
                new VariableReferenceExpression(xVariableReference, null), nullCoalescingEpxression, typeSystem, null);

            result = new ExpressionStatement(nullCoalescingAssign) { Parent = statements[startIndex].Parent };

            FixContext(xVariableReference.Resolve(), 1, zToXAssignIndex + 1, result as ExpressionStatement);

            return true;
        }

        private bool ContainsDummyAssignment(BlockStatement theThen, VariableReference xVariableReference)
        {
            if (theThen.Statements.Count == 0 || theThen.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement || !string.IsNullOrEmpty(theThen.Statements[0].Label))
            {
                return false;
            }
            
            BinaryExpression theAssign = (theThen.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
            if (theAssign == null || theAssign.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || theAssign.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                (theAssign.Right as VariableReferenceExpression).Variable != xVariableReference)
            {
                return false;
            }

            StackVariableDefineUseInfo defineUseInfo;
            return methodContext.StackData.VariableToDefineUseInfo.TryGetValue((theAssign.Left as VariableReferenceExpression).Variable.Resolve(), out defineUseInfo) &&
                defineUseInfo.UsedAt.Count == 0;
        }
    }
}
