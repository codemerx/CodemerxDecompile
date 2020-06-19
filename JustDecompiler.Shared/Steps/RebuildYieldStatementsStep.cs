using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Steps
{
    class RebuildYieldStatementsStep : BaseCodeTransformer, IDecompilationStep
    {
        private DecompilationContext decompilationContext;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.decompilationContext = context;
            if (Match(body.Statements))
            {
                body.Statements = newStatements;
                body = (BlockStatement)Visit(body);
                RemoveLastIfYieldBreak(body.Statements);
                return body;
            }
            return body;
        }

        TypeDefinition yieldDeclaringType;
        StatementCollection statements;
        YieldData yieldData;
        private readonly Dictionary<FieldDefinition, Expression> parameterMappings = new Dictionary<FieldDefinition, Expression>();

        StatementCollection newStatements;

        private bool Match(StatementCollection statements)
        {
            this.statements = statements;

            yieldDeclaringType = GetGeneratedType();

            if (yieldDeclaringType == null)
            {
                return false;
            }

            IEnumerable<Statement> moveNextMethodStatements = GetStatements();
            if (moveNextMethodStatements == null || yieldData == null)
            {
                return false;
            }

            if (statements.Count > 2)
            {
                SetParameterMappings();
                StatementCollection getEnumeratorStatements = GetEnumeratorStatements();
                if (getEnumeratorStatements != null)
                {
                    PostProcessMappings(getEnumeratorStatements);
                }
            }

            newStatements = new StatementCollection();
            foreach (Statement statement in moveNextMethodStatements)
            {
                newStatements.Add(statement);
            }

            return true;
        }

        private TypeDefinition GetGeneratedType()
        {
            if (statements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return null;
            }

            ExpressionStatement expressionStatement = statements[0] as ExpressionStatement;

            ObjectCreationExpression objectCreationExpression;
            if (expressionStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression)
            {
                if (!(expressionStatement.Expression as BinaryExpression).IsAssignmentExpression)
                {
                    return null;
                }

                BinaryExpression assingExpression = expressionStatement.Expression as BinaryExpression;

                if (assingExpression.Right.CodeNodeType != CodeNodeType.ObjectCreationExpression)
                {
                    return null;
                }

                objectCreationExpression = assingExpression.Right as ObjectCreationExpression;
            }
            else if (expressionStatement.Expression.CodeNodeType == CodeNodeType.ReturnExpression)
            {
                ReturnExpression returnExpression = expressionStatement.Expression as ReturnExpression;
                if (returnExpression.Value == null || returnExpression.Value.CodeNodeType != CodeNodeType.ObjectCreationExpression)
                {
                    return null;
                }

                objectCreationExpression = returnExpression.Value as ObjectCreationExpression;
            }
            else
            {
                return null;
            }

            MethodReference constructor = objectCreationExpression.Constructor;

            if (constructor == null || constructor.DeclaringType == null)
            {
                return null;
            }

            TypeDefinition declaringType = constructor.DeclaringType.Resolve();

            if (declaringType == null || !declaringType.IsNestedPrivate || !declaringType.HasCompilerGeneratedAttribute())
            {
                return null;
            }

            return declaringType;
        }

        private IEnumerable<Statement> GetStatements()
        {
            MethodDefinition moveNextMethod = null;
            foreach (MethodDefinition methodDef in yieldDeclaringType.Methods)
            {
                if (methodDef.Name == "MoveNext")
                {
                    moveNextMethod = methodDef;
                    break;
                }
            }

            if (moveNextMethod == null || moveNextMethod.Body == null)
            {
                return null;
            }

            BlockStatement moveNextBody = moveNextMethod.Body.DecompileYieldStateMachine(decompilationContext, out yieldData);

            return moveNextBody != null ? GetStatements(moveNextBody) : null;
        }

        private IEnumerable<Statement> GetStatements(BlockStatement moveNextBody)
        {
            List<Statement> statements = new List<Statement>();

            for (int i = 0; i < moveNextBody.Statements.Count; i++)
            {
                Statement currentStatement = moveNextBody.Statements[i];

                TryStatement tryStatement = currentStatement as TryStatement;
                if (yieldData.StateMachineVersion == YieldStateMachineVersion.V1 && tryStatement != null && tryStatement.Fault != null ||
                    yieldData.StateMachineVersion == YieldStateMachineVersion.V2 && tryStatement != null && tryStatement.CatchClauses.Count == 1)
                {
                    statements.AddRange(tryStatement.Try.Statements);
                }
                else
                {
                    statements.Add(currentStatement);
                }
            }

            return statements;
        }

        private StatementCollection GetEnumeratorStatements()
        {
            MethodDefinition getEnumeratorMethod = null;
            foreach (MethodDefinition methodDef in yieldDeclaringType.Methods)
            {
                if (methodDef.Name.EndsWith(".GetEnumerator"))
                {
                    getEnumeratorMethod = methodDef;
                    break;
                }
            }

            if (getEnumeratorMethod == null)
            {
                return null;
            }

            BlockStatement getEnumeratorBody = getEnumeratorMethod.Body != null ? getEnumeratorMethod.Body.Decompile(this.decompilationContext.Language) : null;

            return getEnumeratorBody.Statements;
        }

        private void SetParameterMappings()
        {
            for (int i = 1; i < statements.Count; i++)
            {
                if (statements[i].CodeNodeType == CodeNodeType.ExpressionStatement)
                {
                    ExpressionStatement expressionStatement = statements[i] as ExpressionStatement;
                    if (expressionStatement.Expression.CodeNodeType == CodeNodeType.BinaryExpression &&
                        (expressionStatement.Expression as BinaryExpression).IsAssignmentExpression)
                    {
                        BinaryExpression assignExpression = expressionStatement.Expression as BinaryExpression;
                        if (assignExpression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
                        {
                            FieldReference fieldRef = (assignExpression.Left as FieldReferenceExpression).Field;
                            if (fieldRef.DeclaringType.Resolve() == yieldDeclaringType)
                            {
                                parameterMappings[fieldRef.Resolve()] = assignExpression.Right;
                            }
                        }
                    }
                }
            }
        }

        private void PostProcessMappings(StatementCollection getEnumeratorStatements)
        {
            foreach (Statement statement in getEnumeratorStatements)
            {
                if (statement.CodeNodeType == CodeNodeType.ExpressionStatement &&
                    (statement as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.BinaryExpression)
                {
                    BinaryExpression binaryExpression = (statement as ExpressionStatement).Expression as BinaryExpression;
                    if (binaryExpression.IsAssignmentExpression && binaryExpression.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression &&
                        binaryExpression.Right.CodeNodeType == CodeNodeType.FieldReferenceExpression)
                    {
                        FieldDefinition leftFieldDef = (binaryExpression.Left as FieldReferenceExpression).Field.Resolve();
                        FieldDefinition rightFieldDef = (binaryExpression.Right as FieldReferenceExpression).Field.Resolve();

                        Expression fieldValue;
                        if (parameterMappings.TryGetValue(rightFieldDef, out fieldValue))
                        {
                            parameterMappings.Remove(rightFieldDef);
                            parameterMappings[leftFieldDef] = fieldValue;
                        }
                    }
                }
            }
        }

        private void RemoveLastIfYieldBreak(StatementCollection collection)
        {
            int lastIndex = collection.Count - 1;
            Statement lastElement = collection[lastIndex];

            if ((lastElement.CodeNodeType == CodeNodeType.ExpressionStatement &&
                (lastElement as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.YieldBreakExpression &&
                string.IsNullOrEmpty(lastElement.Label) && (yieldData.YieldBreaks.Count != 1 || yieldData.YieldReturns.Count != 0)))
            {
                collection.RemoveAt(lastIndex);
            }
        }

        public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
        {
            Expression result = (Expression)Visit(node.Expression);
            if (result == null)
            {
                if (!string.IsNullOrEmpty(node.Label))
                {
                    Statement nextStatement = node.GetNextStatement();
                    if (nextStatement == null || !string.IsNullOrEmpty(nextStatement.Label))
                    {
                        Statement newStatement = new EmptyStatement();
                        newStatement.Label = node.Label;
                        return newStatement;
                    }

                    nextStatement.Label = node.Label;
                }
                return null;
            }

            node.Expression = result;
            return node;
        }

        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            if (node.IsAssignmentExpression)
            {
                if (CheckFieldReference(node.Left) || CheckFieldReference(node.Right) || CheckVariableReference(node.Left) || CheckVariableReference(node.Right) ||
                    node.Right.CodeNodeType == CodeNodeType.ThisReferenceExpression)
                {
                    return null;
                }
            }

            return base.VisitBinaryExpression(node);
        }

        private bool CheckFieldReference(Expression expression)
        {
            if (expression.CodeNodeType == CodeNodeType.FieldReferenceExpression)
            {
                FieldDefinition fieldDef = (expression as FieldReferenceExpression).Field.Resolve();
                if (fieldDef == yieldData.FieldsInfo.CurrentItemField || fieldDef == yieldData.FieldsInfo.StateHolderField)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckVariableReference(Expression expression)
        {
            VariableReference variableReference = null;
            if (expression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
            {
                variableReference = (expression as VariableReferenceExpression).Variable;
            }
            else if (expression.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
            {
                variableReference = (expression as VariableDeclarationExpression).Variable;
            }

            if (variableReference != null && variableReference == yieldData.FieldsInfo.ReturnFlagVariable)
            {
                return true;
            }

            return false;
        }

        public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
        {
            if (node.Field.DeclaringType.Resolve() != yieldDeclaringType)
            {
                return base.VisitFieldReferenceExpression(node);
            }

            FieldDefinition fieldDef = node.Field.Resolve();
            if (parameterMappings.ContainsKey(fieldDef))
            {
                return parameterMappings[fieldDef].CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
            }

            VariableDefinition variableDefinition = new VariableDefinition(GetFriendlyName(fieldDef.Name), fieldDef.FieldType, this.decompilationContext.MethodContext.Method);
            this.decompilationContext.MethodContext.Variables.Add(variableDefinition);
            this.decompilationContext.MethodContext.VariableAssignmentData.Add(variableDefinition, yieldData.FieldAssignmentData[fieldDef]);
            this.decompilationContext.MethodContext.VariablesToRename.Add(variableDefinition);

            VariableReferenceExpression variableReferenceExpression = new VariableReferenceExpression(variableDefinition, node.UnderlyingSameMethodInstructions);
            parameterMappings[fieldDef] = variableReferenceExpression;
            return variableReferenceExpression;
        }

        public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            if (node.MethodExpression.Target != null && node.MethodExpression.Target.CodeNodeType == CodeNodeType.ThisReferenceExpression)
            {
                return null;
            }

            return base.VisitMethodInvocationExpression(node);
        }

        private string GetFriendlyName(string fieldName)
        {
            if (fieldName[0] == '<')
            {
                int index = fieldName.IndexOf('>');
                if (index > 1)
                {
                    return fieldName.Substring(1, index - 1);
                }
            }

            return fieldName;
        }

        public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
            this.decompilationContext.MethodContext.VariablesToRename.Add(node.Variable);
            return base.VisitVariableDeclarationExpression(node);
        }

        public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            this.decompilationContext.MethodContext.VariablesToRename.Add(node.Variable.Resolve());
            return base.VisitVariableReferenceExpression(node);
        }

        public override ICodeNode VisitTryStatement(TryStatement node)
        {
            if (node.Finally != null && node.Finally.Body.Statements.Count == 1 && node.Finally.Body.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement)
            {
                ExpressionStatement theExpressionStatement = node.Finally.Body.Statements[0] as ExpressionStatement;
                if (theExpressionStatement.Expression.CodeNodeType == CodeNodeType.MethodInvocationExpression)
                {
                    MethodReferenceExpression theMethodReferenceExpression = (theExpressionStatement.Expression as MethodInvocationExpression).MethodExpression;
                    if (theMethodReferenceExpression != null && theMethodReferenceExpression.Method != null &&
                        theMethodReferenceExpression.Method.DeclaringType != null)
                    {
                        TypeDefinition theDeclaringTypeDef = theMethodReferenceExpression.Method.DeclaringType.Resolve();
                        if (theDeclaringTypeDef == yieldDeclaringType)
                        {
                            node.Finally = new FinallyClause(theMethodReferenceExpression.Method.Resolve().Body.Decompile(this.decompilationContext.Language), node.Finally.UnderlyingSameMethodInstructions);
                        }
                    }
                }
            }
            return base.VisitTryStatement(node);
        }
    }
}
