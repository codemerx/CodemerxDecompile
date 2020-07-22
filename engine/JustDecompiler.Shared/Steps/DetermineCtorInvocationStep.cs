using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil;
using Telerik.JustDecompiler.Steps.CodePatterns;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Steps
{
    class DetermineCtorInvocationStep : IDecompilationStep
    {
        private MethodSpecificContext methodContext;
        private BlockStatement methodBodyBlock;
        private TypeSystem typeSystem;
        private DecompilationContext context;
        private CodePatternsContext patternsContext;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            if (!context.MethodContext.Method.IsConstructor || body.Statements.Count == 0)
            {
                return body;
            }

            this.methodContext = context.MethodContext;
            this.typeSystem = methodContext.Method.Module.TypeSystem;
            this.methodBodyBlock = body;
            this.context = context;

            ProcessCtorInvocation();

            return body;
        }

        private void ProcessCtorInvocation()
        {
            int startIndex, endIndex;
            bool isBaseCtor;
            StatementCollection statements = GetStatementsForInvocation(out endIndex, out startIndex, out isBaseCtor);

            if (statements == null)
            {
                return;
            }

            this.patternsContext = new CodePatternsContext(statements);
            
            List<ICodePattern> patternArray = new List<ICodePattern>(new ICodePattern[] 
												{   new NullCoalescingPattern(patternsContext, methodContext), new TernaryConditionPatternAgressive(patternsContext, typeSystem),
                                                    new ArrayInitialisationPattern(patternsContext, typeSystem), new VariableInliningPatternAggressive(patternsContext, methodContext, this.context.Language.VariablesToNotInlineFinder),
                                                    new MultiAssignPattern(patternsContext, methodContext)});

            if (isBaseCtor)
            {
                patternArray.Add(new InitializationPattern(patternsContext, context));
            }

            if (!ProcessStatementCollection(statements, patternArray))
            {
                return;
            }

            if (statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return;
            }

            MethodInvocationExpression theCtorInvokeExpression = (statements[0] as ExpressionStatement).Expression as MethodInvocationExpression;
            if (theCtorInvokeExpression.CodeNodeType != CodeNodeType.BaseCtorExpression &&
                theCtorInvokeExpression.CodeNodeType != CodeNodeType.ThisCtorExpression)
            {
                return;
            }

            methodContext.CtorInvokeExpression = theCtorInvokeExpression;
        }

        private bool ProcessStatementCollection(StatementCollection statements, IEnumerable<ICodePattern> patternInvokeArray)
        {
            for (int i = 0; i < statements.Count; i++)
            {
                if (statements[i].CodeNodeType == CodeNodeType.IfStatement)
                {
                    IfStatement theIf = statements[i] as IfStatement;
                    ProcessStatementCollection(theIf.Then.Statements, patternInvokeArray);
                    if (theIf.Else != null)
                    {
                        ProcessStatementCollection(theIf.Else.Statements, patternInvokeArray);
                    }
                }
            }

            bool didTransform;
            do
            {
                didTransform = false;

                foreach (ICodePattern pattern in patternInvokeArray)
                {
                    int replacedStatementsCount = -1;
                    Statement resultingStatement;
                    int startIndex;
                    bool currentTransformation = pattern.TryMatch(statements, out startIndex, out resultingStatement, out replacedStatementsCount);
                    didTransform |= currentTransformation;
                    if (currentTransformation)
                    {
                        if (resultingStatement != null)
                        {
                            RemoveRangeAndInsert(statements, startIndex, replacedStatementsCount, resultingStatement);
                        }
                        else
                        {
                            RemoveRange(statements, startIndex, replacedStatementsCount);
                        }
                        break;
                    }
                }
            } while (didTransform);

            return statements.Count == 1;
        }

        private StatementCollection GetStatementsForInvocation(out int endIndex, out int startIndex, out bool isBaseCtor)
        {
            startIndex = 0;
            StatementCollection bodyStatements = methodBodyBlock.Statements;
            StatementCollection statements = new StatementCollection();
            isBaseCtor = false;
            int index = 0;
            for (; index < bodyStatements.Count; index++)
            {
                if (IsVariableDeclaration(bodyStatements[index]) && statements.Count == 0)
                {
                    startIndex++;
                    continue;
                }
                statements.Add(bodyStatements[index].Clone());
                if (IsCtorInvocation(bodyStatements[index], out isBaseCtor))
                {
                    if (isBaseCtor)
                    {
                        methodContext.IsBaseConstructorInvokingConstructor = true;
                    }
                    break;
                }
            }

            endIndex = index;

            if (index == bodyStatements.Count)
            {
                endIndex = -1;
                statements = null;
            }

            return statements;
        }

        private bool IsVariableDeclaration(Statement statements)
        {
            if (statements.CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return false;
            }
            if (((ExpressionStatement)statements).Expression.CodeNodeType != CodeNodeType.VariableDeclarationExpression)
            {
                return false;
            }
            return true;
        }

        private bool IsCtorInvocation(Statement statement, out bool isBaseCtor)
        {
            isBaseCtor = false;
            if (statement.CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return false;
            }

            MethodInvocationExpression theMethodInvokeExpression = (statement as ExpressionStatement).Expression as MethodInvocationExpression;
            if (theMethodInvokeExpression == null || theMethodInvokeExpression.CodeNodeType != CodeNodeType.BaseCtorExpression &&
                theMethodInvokeExpression.CodeNodeType != CodeNodeType.ThisCtorExpression)
            {
                return false;
            }
            isBaseCtor = theMethodInvokeExpression.CodeNodeType == CodeNodeType.BaseCtorExpression;

            return true;
        }

        //Removes statements from index to index + length - 1
        //Inserts newStatement at index
        private void RemoveRangeAndInsert(StatementCollection statements, int startIndex, int length, Statement newStatement)
        {
            statements[startIndex] = newStatement;
            RemoveRange(statements, startIndex + 1, length - 1);
        }

        private void RemoveRange(StatementCollection statements, int startIndex, int length)
        {
            if (length == 0)
            {
                return;
            }
            int count = statements.Count;
            for (int i = startIndex; i + length < count; i++)
            {
                statements[i] = statements[i + length];
            }

            while (length > 0)
            {
                statements.RemoveAt(--count);
                length--;
            }
        }

    }
}
