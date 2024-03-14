using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
    class RebuildAnonymousDelegatesStep : BaseCodeVisitor, IDecompilationStep
    {
        private AnonymousDelegateRebuilder theRebuilder;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            theRebuilder = new AnonymousDelegateRebuilder(context, body);
            VisitBlockStatement(body);
            theRebuilder.CleanUpVariableCopyAssignments();
            return body;
        }

        public override void VisitBlockStatement(BlockStatement node)
        {
            for (int i = 0; i < node.Statements.Count - 1; i++)
            {
                theRebuilder.Match(node, i);
            }

            base.VisitBlockStatement(node);
        }

        private class AnonymousDelegateRebuilder : BaseCodeTransformer
        {
			//private int variableCount;
            private VariableReference delegateVariableReference = null;
            private readonly DecompilationContext context;
            private Dictionary<FieldDefinition, Expression> fieldDefToAssignedValueMap;
            private readonly BlockStatement methodBodyBlock;
            private State state;
            private int startIndex;
            private readonly FieldReferenceVisitor fieldVisitor;
            private TypeDefinition delegateTypeDef;
            private HashSet<Statement> statementsToKeep;
            private readonly VariableCopyFinder variableCopyFinder;
            private HashSet<VariableReference> delegateCopies;
            private readonly HashSet<Statement> assignmentsToRemove;
            private readonly Dictionary<VariableReference, Dictionary<FieldDefinition, Expression>> closuresArchive;
            private List<BlockStatement> delegatesFound;

            public AnonymousDelegateRebuilder(DecompilationContext context, BlockStatement methodBodyBlock)
            {
                this.context = context;
                this.methodBodyBlock = methodBodyBlock;
				//this.variableCount = context.MethodContext.LambdaVariablesCount;
                if (context.MethodContext.FieldToExpression == null)
                {
                    /// Create the map, when first needed.
                    context.MethodContext.FieldToExpression = new Dictionary<FieldDefinition, Expression>();
                }
                /// Use the passed by upper level map.
                this.fieldDefToAssignedValueMap = context.MethodContext.FieldToExpression;
                this.fieldVisitor = new FieldReferenceVisitor(methodBodyBlock);
                this.variableCopyFinder = new VariableCopyFinder(methodBodyBlock);
                this.assignmentsToRemove = new HashSet<Statement>();
                this.closuresArchive = context.MethodContext.ClosureVariableToFieldValue;
            }

            public void Match(BlockStatement theBlock, int index)
            {
                delegatesFound = new List<BlockStatement>();
                statementsToKeep = new HashSet<Statement>();
                delegateCopies = new HashSet<VariableReference>();
                fieldDefToAssignedValueMap = new Dictionary<FieldDefinition, Expression>();

                state = State.DelegateCreate;
                if (!CheckAssignExpression(theBlock.Statements[index] as ExpressionStatement))
                {
                    return;
                }

                delegateCopies.Add(delegateVariableReference);
                variableCopyFinder.FindCopiesOfDelegate(delegateCopies, assignmentsToRemove);

                startIndex = index;
                state = State.FieldAssign;
                for (; ++index < theBlock.Statements.Count;)
                {
                    if (!CheckAssignExpression(theBlock.Statements[index] as ExpressionStatement))
                    {
                        break;
                    }
                }

                if (index == theBlock.Statements.Count)
                {
                    return;
                }

                MapTheRestOfTheFieldsToVariables();

                RemoveFieldAssignments(theBlock.Statements, index);
                CleanUpVariableCopyAssignments();

                state = State.ReplaceFields;
                VisitBlockStatement(methodBodyBlock);

                state = State.ReplaceDelegate;
                for (int i = startIndex; i < theBlock.Statements.Count; i++)
                {
                    theBlock.Statements[i] = (Statement)Visit(theBlock.Statements[i]);

                    state = State.ReplaceFields;
                    foreach (BlockStatement delegateBody in delegatesFound)
                    {
                        VisitBlockStatement(delegateBody);
                    }
                    state = State.ReplaceDelegate;

                    delegatesFound = new List<BlockStatement>();
                }

                SaveClosureToArchive();
            }

            private void RemoveFieldAssignments(StatementCollection theCollection, int currentIndex)
            {
                for (int i = startIndex; i < currentIndex; i++)
                {
                    if (statementsToKeep.Contains(theCollection[i]))
                    {
                        theCollection[startIndex++] = theCollection[i];
                    }
                }

                RemoveRange(theCollection, startIndex, currentIndex);
            }

            public void CleanUpVariableCopyAssignments()
            {
                List<Statement> statements = new List<Statement>(this.assignmentsToRemove);
                foreach (Statement statement in statements)
                {
                    BlockStatement parent = statement.Parent as BlockStatement;
                    if (parent != null)
                    {
                        parent.Statements.Remove(statement);
                    }
                }
            }

            private void RemoveRange(StatementCollection theCollection, int from, int to)
            {
                for (; to < theCollection.Count; from++, to++)
                {
                    theCollection[from] = theCollection[to];
                }

                int count = theCollection.Count;
                while (from < count)
                {
                    theCollection.RemoveAt(--count);
                }
            }

            private void MapTheRestOfTheFieldsToVariables()
            {
                foreach (FieldDefinition fieldDef in this.delegateTypeDef.Fields)
                {
                    if(!this.fieldDefToAssignedValueMap.ContainsKey(fieldDef))
                    {
                        VariableDefinition newVar = new VariableDefinition("lambdaVar" + context.MethodContext.LambdaVariablesCount++, fieldDef.FieldType, this.context.MethodContext.Method);
                        this.context.MethodContext.Variables.Add(newVar);
                        this.context.MethodContext.VariablesToRename.Add(newVar);
                        fieldDefToAssignedValueMap[fieldDef] = new VariableReferenceExpression(newVar, null);
                    }
                }
            }

            private void SaveClosureToArchive()
            {
                foreach (VariableReference delegateVariable in delegateCopies)
                {
                    closuresArchive.Add(delegateVariable, fieldDefToAssignedValueMap);
                }
            }

            private bool CheckAssignExpression(ExpressionStatement theStatement)
            {
                if (theStatement == null || theStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
                {
                    return false;
                }

                BinaryExpression theAssignExpression = theStatement.Expression as BinaryExpression;
                if (!theAssignExpression.IsAssignmentExpression)
                {
                    return false;
                }

                switch (state)
                {
                    case State.DelegateCreate:
                        return CheckDelegateCreation(theAssignExpression);
                    case State.FieldAssign:
                        return CheckFieldAssignment(theAssignExpression, theStatement);
                    default:
                        return false;
                }
            }

            private bool CheckDelegateCreation(BinaryExpression theAssignExpression)
            {
                if (theAssignExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                    theAssignExpression.Right.CodeNodeType != CodeNodeType.ObjectCreationExpression)
                {
                    return false;
                }

                ObjectCreationExpression theObjectCreation = theAssignExpression.Right as ObjectCreationExpression;
                if (theObjectCreation.Arguments.Count != 0)
                {
                    return false;
                }

                TypeDefinition createdObjectType = theObjectCreation.ExpressionType.Resolve();
                if (createdObjectType == null || !CheckTypeForCompilerGeneratedAttribute(createdObjectType))
                {
                    return false;
                }

                this.delegateTypeDef = createdObjectType;
                delegateVariableReference = (theAssignExpression.Left as VariableReferenceExpression).Variable;
                return true;
            }
  
            private bool CheckTypeForCompilerGeneratedAttribute(TypeDefinition typeDefinition)
            {
                while (typeDefinition.IsNested)
                {
                    if (typeDefinition.HasCompilerGeneratedAttribute())
                    {
                        return true;
                    }

                    typeDefinition = typeDefinition.DeclaringType;
                }

                return false;
            }

            private bool CheckFieldAssignment(BinaryExpression theAssignExpression, ExpressionStatement theStatement)
            {
                if (theAssignExpression.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression)
                {
                    return false;
                }

                FieldReferenceExpression theFieldReferenceExpression = theAssignExpression.Left as FieldReferenceExpression;
                if (theFieldReferenceExpression.Target == null ||
                    theFieldReferenceExpression.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                    (theFieldReferenceExpression.Target as VariableReferenceExpression).Variable != delegateVariableReference)
                {
                    return false;
                }

                FieldDefinition fieldDef = theFieldReferenceExpression.Field.Resolve();

                if ((theAssignExpression.Right.CodeNodeType == CodeNodeType.ThisReferenceExpression ||
                    theAssignExpression.Right.CodeNodeType == CodeNodeType.ArgumentReferenceExpression ||
                    IsClosureVariableReference(theAssignExpression.Right as VariableReferenceExpression))
                    && !fieldVisitor.CheckForAnotherAssignment(fieldDef, theFieldReferenceExpression, delegateCopies))
                {
                    fieldDefToAssignedValueMap[fieldDef] = theAssignExpression.Right;
                }
                else
                {
                    this.statementsToKeep.Add(theStatement);
                }
                return true;
            }

            private bool IsClosureVariableReference(VariableReferenceExpression varRefExpression)
            {
                if (varRefExpression == null)
                {
                    return false;
                }

                return closuresArchive.ContainsKey(varRefExpression.Variable);
            }

            public override ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
            {
				if (state == State.ReplaceDelegate && node.Arguments != null && node.Arguments.Count == 2 &&
                    node.Arguments[0].CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    node.Arguments[1].CodeNodeType == CodeNodeType.MethodReferenceExpression &&
                    delegateCopies.Contains((node.Arguments[0] as VariableReferenceExpression).Variable))
				{
                    //final check inserted here for optimization
                    TypeDefinition objectType = node.Constructor.DeclaringType.Resolve();
                    if (objectType == null || objectType.BaseType == null || objectType.BaseType.FullName != "System.MulticastDelegate")
                    {
                        return base.VisitObjectCreationExpression(node);
                    }

                    MethodReference methodReference = (node.Arguments[1] as MethodReferenceExpression).Method;
                    MethodDefinition methodDefinition = (node.Arguments[1] as MethodReferenceExpression).MethodDefinition;

                    MethodSpecificContext delegateMethodContext = new MethodSpecificContext(methodDefinition.Body);
                    DecompilationContext innerContext = new DecompilationContext(delegateMethodContext, context.TypeContext, context.ModuleContext, context.AssemblyContext, context.Language);
                    delegateMethodContext.FieldToExpression = fieldDefToAssignedValueMap;

                    BlockStatement methodStatements = methodDefinition.Body.DecompileLambda(context.Language, innerContext);

					if ((methodStatements.Statements.Count == 1) && (methodStatements.Statements[0].CodeNodeType == CodeNodeType.ExpressionStatement) &&
						((methodStatements.Statements[0] as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.ReturnExpression))
					{
						ReturnExpression returnExpression = (methodStatements.Statements[0] as ExpressionStatement).Expression as ReturnExpression;
						ShortFormReturnExpression shortFormReturnExpression = new ShortFormReturnExpression(returnExpression.Value, returnExpression.MappedInstructions);
						methodStatements = new BlockStatement();
						methodStatements.Statements.Add(new ExpressionStatement(shortFormReturnExpression));
					}

                    this.context.MethodContext.VariableDefinitionToNameMap.AddRange(innerContext.MethodContext.VariableDefinitionToNameMap);
					this.context.MethodContext.VariableNamesCollection.UnionWith(innerContext.MethodContext.VariableNamesCollection);
                    this.context.MethodContext.AddInnerMethodParametersToContext(innerContext.MethodContext);
					this.context.MethodContext.GotoStatements.AddRange(innerContext.MethodContext.GotoStatements);
					this.context.MethodContext.GotoLabels.AddRange(innerContext.MethodContext.GotoLabels);

                    ExpressionCollection expressionCollection = new ExpressionCollection();
                    bool hasAnonymousParamterer = LambdaExpressionsHelper.HasAnonymousParameter(methodDefinition.Parameters);
                    foreach (ParameterDefinition parameter in methodDefinition.Parameters)
                    {
                        expressionCollection.Add(new LambdaParameterExpression(parameter, !hasAnonymousParamterer, null));
                    }

                    delegatesFound.Add(methodStatements);

					LambdaExpression lambdaExpression = 
						new LambdaExpression(expressionCollection, methodStatements, methodDefinition.IsAsync(), methodDefinition.IsFunction(), methodReference.Parameters, false,
                            node.Arguments[1].MappedInstructions) { ExpressionType = objectType };

					DelegateCreationExpression result = new DelegateCreationExpression(node.Constructor.DeclaringType, lambdaExpression, node.Arguments[0], node.MappedInstructions);
					return result;
				}

                return base.VisitObjectCreationExpression(node);
            }

            public override ICodeNode VisitFieldReferenceExpression(FieldReferenceExpression node)
            {
                if (state == State.ReplaceFields)
                {
                    Expression value;
                    FieldDefinition resolvedField = node.Field.Resolve();
                    if (resolvedField == null)
                    {
                        return base.VisitFieldReferenceExpression(node);
                    }

                    if (fieldDefToAssignedValueMap.TryGetValue(resolvedField, out value))
                    {
                        return value.CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions);
                    }

                    base.VisitFieldReferenceExpression(node);
                    if (node.Target == null || node.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
                    {
                        return node;
                    }

                    VariableReference variable = (node.Target as VariableReferenceExpression).Variable;
                    Dictionary<FieldDefinition, Expression> fieldToValueMap;
                    return closuresArchive.TryGetValue(variable, out fieldToValueMap) && fieldToValueMap.TryGetValue(resolvedField, out value) ?
                        value.CloneExpressionOnlyAndAttachInstructions(node.UnderlyingSameMethodInstructions) : node;
                }

                return base.VisitFieldReferenceExpression(node);
            }

            private enum State
            {
                DelegateCreate,
                FieldAssign,
                ReplaceDelegate,
                ReplaceFields
            }
        }

        private class FieldReferenceVisitor : BaseCodeVisitor
        {
            private readonly BlockStatement theBlockStatement;

            private FieldDefinition fieldDef;
            private FieldReferenceExpression assignedReference;
            private bool foundUsage;
            private HashSet<VariableReference> delegateVariableCopies;

            public FieldReferenceVisitor(BlockStatement theBlockStatement)
            {
                this.theBlockStatement = theBlockStatement;
            }

            public bool CheckForAnotherAssignment(FieldDefinition fieldDef, FieldReferenceExpression assignedReference,
                HashSet<VariableReference> delegateVariableCopies)
            {
                this.fieldDef = fieldDef;
                this.assignedReference = assignedReference;
                this.delegateVariableCopies = delegateVariableCopies;
                this.foundUsage = false;
                base.Visit(theBlockStatement);
                return foundUsage;
            }

            public override void Visit(ICodeNode node)
            {
                if (!this.foundUsage)
                {
                    base.Visit(node);
                }
            }

            public override void VisitBinaryExpression(BinaryExpression node)
            {
                if (this.foundUsage)
                {
                    return;
                }

                if (node.IsAssignmentExpression && node.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression)
                {
                    CheckFieldReference(node.Left as FieldReferenceExpression);
                    if (this.foundUsage)
                    {
                        return;
                    }
                }
                base.VisitBinaryExpression(node);
            }

            private void CheckFieldReference(FieldReferenceExpression node)
            {
                if(node != assignedReference && node.Target != null && node.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    delegateVariableCopies.Contains((node.Target as VariableReferenceExpression).Variable) && node.Field.Resolve() == this.fieldDef)
                {
                    this.foundUsage = true;
                }
            }
        }

        private class VariableCopyFinder : BaseCodeVisitor
        {
            private readonly BlockStatement theBlock;

            private HashSet<VariableReference> delegateVariablesSet;
            private HashSet<Statement> statementsToRemove;

            public VariableCopyFinder (BlockStatement theBlock)
	        {
                this.theBlock = theBlock;
	        }

            public void FindCopiesOfDelegate(HashSet<VariableReference> delegateVariablesSet, HashSet<Statement> statementsToRemove)
            {
                this.delegateVariablesSet = delegateVariablesSet;
                this.statementsToRemove = statementsToRemove;
                Visit(theBlock);
            }

            public override void VisitExpressionStatement(ExpressionStatement node)
            {
                if (node.IsAssignmentStatement() && CheckBinaryExpression(node.Expression as BinaryExpression))
                {
                    statementsToRemove.Add(node);
                }
                else
                {
                    base.VisitExpressionStatement(node);
                }
            }

            private bool CheckBinaryExpression(BinaryExpression node)
            {
                if (node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    node.Right.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                    delegateVariablesSet.Contains((node.Right as VariableReferenceExpression).Variable))
                {
                    delegateVariablesSet.Add((node.Left as VariableReferenceExpression).Variable);
                    return true;
                }
                return false;
            }
        }
    }
}
