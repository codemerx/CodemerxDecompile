using System;
using System.Linq;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using System.Threading;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Statements;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Steps
{
    internal class RebuildLockStatements : BaseCodeVisitor, IDecompilationStep
    {
        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            Visit(body);
            return body;
        }

        public override void VisitBlockStatement(BlockStatement node)
        {
            for (int i = 0; i < node.Statements.Count; i++)
            {
                if (!Match(node.Statements, i))
                    continue;

                // repalce try with lock
                node.Statements.RemoveAt(i);
                node.AddStatementAt(i, Lock);

                //RemoveFlagVariable(i - 1, node, theFlagVariable);

                if (this.lockType == LockType.Simple)
                {
                    node.Statements.RemoveAt(i + 1); //the try
                    node.Statements.RemoveAt(--i); // the second assign
                    node.Statements.RemoveAt(--i); // the first assign
                }
                else // LockType.WithFlag (V1, V2 or V3)
                {
                    int numberOfStatementsToRemove;
                    if (lockType == LockType.WithFlagV1)
                    {
                        numberOfStatementsToRemove = 3;
                    }
                    else if (lockType == LockType.WithFlagV2)
                    {
                        numberOfStatementsToRemove = 1;
                    }
                    else // V3
                    {
                        numberOfStatementsToRemove = 2;
                    }

                    for (int j = 0; j < numberOfStatementsToRemove; j++)
                    {
                        Lock.Body.Statements.RemoveAt(0);
                    }

                    if(i > 0)
                    {
                        node.Statements.RemoveAt(--i);
                        if (lockType == LockType.WithFlagV2)
                        {
                            node.Statements.RemoveAt(--i);
                        }
                    }
                }
            }

            Visit(node.Statements);
        }

        private TryStatement @try;
        private BlockStatement body;
        private LockStatement @lock;
        private VariableReference theFlagVariable;
        private LockType lockType;
        private Expression lockObjectExpression;
        private VariableReference theLocalVariable;
        private VariableReference thePhiVariable;
        private readonly List<Instruction> lockingInstructions = new List<Instruction>();

        private StatementCollection statements;
        private int statementIndex;
        
        private LockStatement Lock
        {
            get
            {
                @lock = @lock ?? new LockStatement(lockObjectExpression.CloneAndAttachInstructions(lockingInstructions), body, @try.Finally.UnderlyingSameMethodInstructions);
                return @lock;
            }
        }

        private bool Match(StatementCollection statements, int statementIndex)
        {
            PrepareMatcher(statements, statementIndex);

            Statement currentStatement = statements[statementIndex];
            if (currentStatement.CodeNodeType == CodeNodeType.TryStatement)
            {
                @try = currentStatement as TryStatement;
                if (!DetermineWithFlagLockTypeVersion(@try))
                {
                    return false;
                }

                if (this.lockType == LockType.WithFlagV2)
                {
                    if (statementIndex - 2 < 0 || !CheckLockVariableAssignmentExpression(statements[statementIndex - 2]))
                    {
                        return false;
                    }
                }
                else if (this.lockType == LockType.WithFlagV3)
                {
                    // In V3 the flag is assigned before the try and the lock variable is assigned in the try
                    if (statementIndex - 1 < 0)
                    {
                        return false;
                    }
                }
            }
            else if (currentStatement.CodeNodeType == CodeNodeType.ExpressionStatement &&
                CheckTheMethodInvocation((currentStatement as ExpressionStatement).Expression as MethodInvocationExpression, "Enter"))
            {
                MethodReference methodReference = ((currentStatement as ExpressionStatement).Expression as MethodInvocationExpression).MethodExpression.Method;
                if(methodReference.Parameters.Count != 1)
                {
                    return false;
                }

                if (statementIndex + 1 >= statements.Count || statementIndex - 2 < 0)
                {
                    return false;
                }
                @try = statements[statementIndex + 1] as TryStatement;
                lockType = LockType.Simple;
                if(!CheckTheAssignExpressions(statements[statementIndex - 2], statements[statementIndex - 1]))
                {
                    return false;
                }
                lockingInstructions.AddRange(currentStatement.UnderlyingSameMethodInstructions);
            }

            if (@try == null)
                return false;

            if (!IsLockStatement(@try))
                return false;
            
            this.body = @try.Try;
            
            return true;
        }

        /// <summary>
        /// Checks the structure of the given statement, and if it matches the pattern for lock variable assignment,
        /// assigns the theLocalVariable and the lockObjectExpression with the coersponding values.
        /// </summary>
        /// <param name="statement">The statement that needs to be checked.</param>
        /// <returns>True if the given statement match the pattern, otherwise - false.</returns>
        /// <remarks>
        /// This method works only for lock variable assignment for LockType.WithFlagV2 and LockType.WithFlagV3 and it should
        /// be used only when this lock type is found. Otherwise the CheckTheAssignExpressions method should be used.
        /// </remarks>
        private bool CheckLockVariableAssignmentExpression(Statement statement)
        {
            if (statement.CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return false;
            }

            ExpressionStatement expressionStatement = statement as ExpressionStatement;
            if (expressionStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
            {
                return false;
            }

            BinaryExpression assignment = expressionStatement.Expression as BinaryExpression;
            if (!assignment.IsAssignmentExpression || assignment.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
            {
                return false;
            }

            this.theLocalVariable = (assignment.Left as VariableReferenceExpression).Variable;
            this.lockObjectExpression = assignment.Right;

            return true;
        }

        private bool DetermineWithFlagLockTypeVersion(TryStatement @try)
        {
            if (@try == null || @try.Try == null || @try.Try.Statements.Count == 0)
            {
                return false;
            }

            if (@try.Try.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return false;
            }

            ExpressionStatement firstStatement = @try.Try.Statements[0] as ExpressionStatement;
            if (firstStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression)
            {
                if (@try.Try.Statements.Count > 1 &&
                    @try.Try.Statements[1].CodeNodeType == CodeNodeType.ExpressionStatement &&
                    (@try.Try.Statements[1] as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.BinaryExpression)
                {
                    lockType = LockType.WithFlagV1;
                }
                else
                {
                    lockType = LockType.WithFlagV3;
                }
            }
            else if (firstStatement.Expression.CodeNodeType == CodeNodeType.MethodInvocationExpression)
            {
                lockType = LockType.WithFlagV2;
            }
            else
            {
                return false;
            }

            return true;
        }

        bool IsLockStatement(TryStatement @try)
        {
            if(@try == null)
            {
                return false;
            }

            //If the lock is with a flag and it's V1 then the statements in the try should be at least 3: 2 assignments and the method invocation
            // If the lock is with a flag and it's V2 the statement in the try should be at least 1: the method invocation.
            // If the lock is with a flag and it's V3 the statements in the try should be at least 2: the lock variable assignment and the method invocation
            bool result = (this.lockType == LockType.WithFlagV1 && @try.Try.Statements.Count > 2) ||
                          this.lockType == LockType.Simple ||
                          (this.lockType == LockType.WithFlagV2 && @try.Try.Statements.Count > 0) ||
                          (this.lockType == LockType.WithFlagV3 && @try.Try.Statements.Count > 1);
            result &= @try.CatchClauses.Count == 0 && @try.Finally != null;
   
            if (result)
            {
                if(lockType == LockType.WithFlagV1 || lockType == LockType.WithFlagV2 || lockType == LockType.WithFlagV3)
                {
                    int enterMethodInvocationIndex;
                    if (lockType == LockType.WithFlagV1)
                    {
                        result &= CheckTheAssignExpressions(@try.Try.Statements[0], @try.Try.Statements[1]);
                        enterMethodInvocationIndex = 2;
                    }
                    else if (lockType == LockType.WithFlagV2)
                    {
                        enterMethodInvocationIndex = 0;
                    }
                    else // lockType == LockType.WithFlagV3
                    {
                        result &= CheckLockVariableAssignmentExpression(@try.Try.Statements[0]);
                        enterMethodInvocationIndex = 1;
                    }

                    //The index 3 is because of the generated PhiVariable, if this variable is cleand before this step the matching will fail
                    result &= @try.Try.Statements[enterMethodInvocationIndex].CodeNodeType == CodeNodeType.ExpressionStatement;

                    if (result)
                    {
                        ExpressionStatement expressionStmt = @try.Try.Statements[enterMethodInvocationIndex] as ExpressionStatement;
                        result &= expressionStmt.Expression.CodeNodeType == CodeNodeType.MethodInvocationExpression;
           
                        MethodInvocationExpression methodInvocation = expressionStmt.Expression as MethodInvocationExpression;
                        result &= CheckTheMethodInvocation(methodInvocation, "Enter") && methodInvocation.MethodExpression.Method.Parameters.Count == 2;
                        if (result)
                        {
                            lockingInstructions.AddRange(methodInvocation.UnderlyingSameMethodInstructions);
                            this.theFlagVariable = GetTheFlagVariable(methodInvocation);
                        }
                    }
                }

                result &= CheckTheFinallyClause(@try.Finally.Body);
            }

            return result;
        }

        private bool CheckTheAssignExpressions(Statement firstAssign, Statement secondAssign)
        {
            VariableReferenceExpression thePhiVariableExpression = null;

            if (firstAssign.CodeNodeType == CodeNodeType.ExpressionStatement)
            {
                BinaryExpression firstAssignExpression = (firstAssign as ExpressionStatement).Expression as BinaryExpression;
                if (firstAssignExpression == null || !firstAssignExpression.IsAssignmentExpression)
                {
                    return false;
                }

                thePhiVariableExpression = firstAssignExpression.Left as VariableReferenceExpression;
                this.lockObjectExpression = firstAssignExpression.Right;

                lockingInstructions.AddRange(firstAssignExpression.Left.UnderlyingSameMethodInstructions);
                lockingInstructions.AddRange(firstAssignExpression.MappedInstructions);
            }

            if (thePhiVariableExpression != null && secondAssign.CodeNodeType == CodeNodeType.ExpressionStatement)
            {
                BinaryExpression secondAssignExpression = (secondAssign as ExpressionStatement).Expression as BinaryExpression;
                if(secondAssignExpression == null || !secondAssignExpression.IsAssignmentExpression ||
                    secondAssignExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                    secondAssignExpression.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression)
                {
                    return false;
                }

                this.theLocalVariable = (secondAssignExpression.Left as VariableReferenceExpression).Variable;
                if ((secondAssignExpression.Right as VariableReferenceExpression).Variable != thePhiVariableExpression.Variable)
                {
                    return false;
                }
            }

            lockingInstructions.AddRange(secondAssign.UnderlyingSameMethodInstructions);
            if (thePhiVariableExpression != null)
            {
                this.thePhiVariable = thePhiVariableExpression.Variable;
            }

            return true;
        }

        private bool CheckTheFinallyClause(BlockStatement theFinally)
        {
            //At most the finally can be: bool cond = flag != false; if(cond) { Monitor.Exit(...); }
            int statementsCount = theFinally.Statements.Count;
            if(statementsCount > 2 && statementsCount == 0)
            {
                return false;
            }

            MethodInvocationExpression theMethodInvocation = null;
            if(this.lockType == LockType.WithFlagV1 || this.lockType == LockType.WithFlagV2 || this.lockType == LockType.WithFlagV3)
            {
                VariableReference theConditionVariable = null;
                if (statementsCount == 2)
                {
                    if (theFinally.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
                    {
                        return false;
                    }

                    BinaryExpression theConditionAssignExpression = (theFinally.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
                    if (theConditionAssignExpression == null || !theConditionAssignExpression.IsAssignmentExpression ||
                        theConditionAssignExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
                    {
                        return false;
                    }

                    theConditionVariable = (theConditionAssignExpression.Left as VariableReferenceExpression).Variable;
                }

                if(statementsCount == 0)
                {
                    return false;
                }

                IfStatement theIf = theFinally.Statements[statementsCount - 1] as IfStatement;
                if(theIf == null)
                {
                    return false;
                }

                if(theConditionVariable != null && 
                    (theIf.Condition.CodeNodeType != CodeNodeType.UnaryExpression ||
                        (theIf.Condition as UnaryExpression).Operator != UnaryOperator.LogicalNot ||
                        (theIf.Condition as UnaryExpression).Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                        ((theIf.Condition as UnaryExpression).Operand as VariableReferenceExpression).Variable != theConditionVariable))
                {
                    return false;
                }

                return CheckTheIfStatement(theIf);
            }
            else //LockType.Simple
            {
                if(statementsCount > 1 || theFinally.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
                {
                    return false;
                }

                theMethodInvocation = (theFinally.Statements[0] as ExpressionStatement).Expression as MethodInvocationExpression;
                return CheckTheMethodInvocation(theMethodInvocation, "Exit");
            }
        }

        private bool CheckTheIfStatement(IfStatement theIf)
        {
            if(theIf == null || theIf.Else != null || theIf.Then == null || theIf.Then.Statements == null ||
                theIf.Then.Statements.Count != 1 || theIf.Then.Statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return false;
            }

            MethodInvocationExpression theMethodInvocation = (theIf.Then.Statements[0] as ExpressionStatement).Expression as MethodInvocationExpression;

            return CheckTheMethodInvocation(theMethodInvocation, "Exit");
        }

        private bool CheckTheMethodInvocation(MethodInvocationExpression theMethodInvocation, string methodName)
        {
            if (theMethodInvocation == null || theMethodInvocation.MethodExpression.CodeNodeType != CodeNodeType.MethodReferenceExpression)
            {
                return false;
            }

            MethodReference theMethodReference = theMethodInvocation.MethodExpression.Method;

            return theMethodReference.DeclaringType.FullName == typeof(Monitor).FullName && theMethodReference.Name == methodName;
        }

        private VariableReference GetTheFlagVariable(MethodInvocationExpression methodInvocation)
        {
            Expression theArgumen = methodInvocation.Arguments[1];
            VariableReferenceExpression theFlagExpression = null;
            if (theArgumen.CodeNodeType == CodeNodeType.UnaryExpression && (theArgumen as UnaryExpression).Operator == UnaryOperator.AddressReference)
            {
                theFlagExpression = (theArgumen as UnaryExpression).Operand as VariableReferenceExpression;
            }
            else
            {
                theFlagExpression = theArgumen as VariableReferenceExpression;
            }

            return theFlagExpression == null ? null : theFlagExpression.Variable;
        }

        private void PrepareMatcher(StatementCollection statements, int statementIndex)
        {
            this.statements = statements;
            this.statementIndex = statementIndex;
            @try = null;
            this.body = null;
            this.@lock = null;
            this.lockObjectExpression = null;
            this.theLocalVariable = null;
            this.thePhiVariable = null;
            this.lockingInstructions.Clear();
        }

        private enum LockType
        {
            Simple,
            WithFlagV1,
            WithFlagV2,
            WithFlagV3
        }
    }
}
