using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	class MultiAssignPattern : CommonPatterns, ICodePattern
	{
        private readonly HashSet<VariableDefinition> variablesToRemove = new HashSet<VariableDefinition>();
        private readonly MethodSpecificContext methodContext;

		public MultiAssignPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext) : base(patternsContext, methodContext.Method.Module.TypeSystem)
        {
            this.methodContext = methodContext;
        }

        public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
        {
            result = null;
            replacedStatementsCount = 0;
            startIndex = 0;

            for (startIndex = 0; startIndex < statements.Count; startIndex++)
            {
                VariableDefinition variable;
                if (TryMatchInternal(statements, startIndex, out result, out replacedStatementsCount, out variable))
                {
                    FixContext(variable, 0, replacedStatementsCount - 1, (ExpressionStatement)result);
                    RemoveFromContext();
                    return true;
                }
            }

            variablesToRemove.Clear();
            return false;
        }

		//a0 = a1 = a2 = ... = aN;
		//
		//==
		//
		//x = aN;
		//aN-1 = x;
		//...
		//a1 = x;
		//a0 = x;
		//
		//x - phi variable
		//a(0 - N-1) - phi variables or parameters
		//aN - expression
		//result -> x = a0 = a1 = a2 = ... = aN
		//it will be used for inlining in a method invocation
		private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount, out VariableDefinition xVariableDef)
		{
            result = null;
            replacedStatementsCount = 0;
            xVariableDef = null;

			if (statements.Count < 1|| statements[startIndex].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			
			VariableReference xVariableReference;
			Expression theAssignedExpression;
			
			BinaryExpression valueToXAssignExpression = (statements[startIndex] as ExpressionStatement).Expression as BinaryExpression;
			if (!IsAssignToVariableExpression(valueToXAssignExpression, out xVariableReference) ||
                !this.methodContext.StackData.VariableToDefineUseInfo.ContainsKey(xVariableReference.Resolve()))
			{
				return false;
			}
			
			theAssignedExpression = valueToXAssignExpression.Right;

            int currentIndex = startIndex + 1;
			for (; currentIndex < statements.Count; currentIndex++)
			{
				Statement currentStatement = statements[currentIndex];
				if (currentStatement.CodeNodeType != CodeNodeType.ExpressionStatement || !string.IsNullOrEmpty(currentStatement.Label))
				{
					break;
				}
				
				BinaryExpression xToVarAssignExpression = (currentStatement as ExpressionStatement).Expression as BinaryExpression;
				
				if (xToVarAssignExpression == null || !xToVarAssignExpression.IsAssignmentExpression ||
					xToVarAssignExpression.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
					(xToVarAssignExpression.Right as VariableReferenceExpression).Variable != xVariableReference)
				{
					break;
				}

				if (xToVarAssignExpression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					return false;
				}

                if (xToVarAssignExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    VariableDefinition assignedVariable = (xToVarAssignExpression.Left as VariableReferenceExpression).Variable.Resolve();

                    if (assignedVariable == xVariableReference)
                    {
                        return false;
                    }

                    variablesToRemove.Add(assignedVariable);
                }

				theAssignedExpression = new BinaryExpression(BinaryOperator.Assign, xToVarAssignExpression.Left, theAssignedExpression, typeSystem, null);
			}
			
            replacedStatementsCount = currentIndex - startIndex;
			if (replacedStatementsCount == 1)
			{
				return false;
			}
			
			BinaryExpression inlinedAssign = new BinaryExpression(BinaryOperator.Assign,
				new VariableReferenceExpression(xVariableReference, null), theAssignedExpression, typeSystem, null);

            result = new ExpressionStatement(inlinedAssign) { Parent = statements[startIndex].Parent };
            xVariableDef = xVariableReference.Resolve();

			return true;
		}

        private void RemoveFromContext()
        {
            foreach (VariableDefinition variable in variablesToRemove)
            {
                if (patternsContext.VariableToDefineUseCountContext.Remove(variable))
                {
                    patternsContext.VariableToSingleAssignmentMap.Remove(variable);
                }
            }

            variablesToRemove.Clear();
        }
	}
}
