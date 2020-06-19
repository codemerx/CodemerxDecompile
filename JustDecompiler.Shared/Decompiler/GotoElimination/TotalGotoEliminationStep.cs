using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Cil;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using System.Linq;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
    /// <summary>
    /// Eliminates all goto statements by introducing new statements and moving code. For complete algorithm, see
    /// <see cref="Taming Control Flow A Structured Approach to Eliminating Goto Statements.pdf"/> in DecompilationPapers folder.
    /// </summary>
    [Obsolete]
    class TotalGotoEliminationStep : IDecompilationStep
    {
        private MethodSpecificContext methodContext;
        private readonly Dictionary<string, VariableDefinition> labelToVariable;
        private readonly List<VariableDefinition> switchVariables;
        private VariableReference breakVariable;
        private bool usedBreakVariable;
        private VariableReference continueVariable;
        private bool usedContinueVariable;
        private readonly Dictionary<VariableDefinition, Statement> variableToAssignment;
        private readonly Dictionary<VariableDefinition, bool> assignedOnly;
        private TypeSystem typeSystem;
        private BlockStatement body; ///Field left to ease debugging

        public TotalGotoEliminationStep()
        {
            this.labelToVariable = new Dictionary<string, VariableDefinition>();
            this.switchVariables = new List<VariableDefinition>();
            this.variableToAssignment = new Dictionary<VariableDefinition, Statement>();
            this.assignedOnly = new Dictionary<VariableDefinition, bool>();
        }

        /// <summary>
        /// The entry point of the step.
        /// </summary>
        /// <param name="context">The decompilation context for the current method.</param>
        /// <param name="body">The body of the current method.</param>
        /// <returns>Returns the updated body of the current method.</returns>
        public virtual BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.methodContext = context.MethodContext;
            this.typeSystem = methodContext.Method.Module.TypeSystem;
            this.body = body;
            RemoveGotoStatements();

            methodContext.Variables.AddRange(switchVariables);
            methodContext.VariablesToRename.UnionWith(switchVariables);

            methodContext.Variables.AddRange(labelToVariable.Values);
            methodContext.VariablesToRename.UnionWith(labelToVariable.Values);

            CleanupUnneededVariables();

            AddDefaultAssignmentsToNewConditionalVariables(body);

            return body;
        }

        /// <summary>
        /// Adds the starting assignments to the newly introduced flag variables.
        /// </summary>
        /// <param name="body">The body of the method.</param>
        private void AddDefaultAssignmentsToNewConditionalVariables(BlockStatement body)
        {
            foreach (VariableDefinition variable in labelToVariable.Values)
            {
                ///Add assignments for every label variable.
                BinaryExpression defaultAssignment =
                    new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(variable, null), GetLiteralExpression(false), typeSystem, null);
                body.AddStatementAt(0, new ExpressionStatement(defaultAssignment));
            }
            if (usedBreakVariable)
            {
                ///Add assignment for the break variable.
                BinaryExpression defaultBreakAssignment =
                    new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(breakVariable, null), GetLiteralExpression(false), typeSystem, null);
                body.AddStatementAt(0, new ExpressionStatement(defaultBreakAssignment));
            }
            if (usedContinueVariable)
            {
                ///Add assignment for the continue variable.
                BinaryExpression defaultContinueAssignment =
                    new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(continueVariable, null), GetLiteralExpression(false), typeSystem, null);
                body.AddStatementAt(0, new ExpressionStatement(defaultContinueAssignment));
            }
        }

        /// <summary>
        /// Removes all the assignments of label variables, that are never used in conditionals.
        /// </summary>
        private void CleanupUnneededVariables()
        {
            foreach (VariableDefinition varDef in assignedOnly.Keys)
            {
                if (assignedOnly[varDef])
                {
                    Statement toRemove = variableToAssignment[varDef];
                    BlockStatement block = toRemove.Parent as BlockStatement;
                    block.Statements.Remove(toRemove);

                    ///Next line greatly relies on the way the name of the variable is created.
                    ///It takes into account that the names are in the form <label>_cond and uses this
                    ////to induce the label for the variable and to remove the variable entry from the dictionary
                    ///so that no unneeded variables are inserted into the method.
                    labelToVariable.Remove(varDef.Name.Remove(varDef.Name.Length - 5));
                }
            }
        }

        /// <summary>
        /// The entry part for the algorithm.
        /// </summary>
        private void RemoveGotoStatements()
        {
            IEnumerable<KeyValuePair<IfStatement, Statement>> gotoPairs = Preprocess();

            ///Eliminate each goto statement, one by one
            ///It is known that the order of elimination of the goto statements has direct consequences on how much code is added.
            ///However, no heuristics have been implemented at this stage.
            foreach (KeyValuePair<IfStatement, Statement> pair in gotoPairs)
            {
                EliminateGotoPair(pair.Key, pair.Value);
            }

            ///Clear all labels, as they are no longer needed.
            ClearLabelStatements();
        }

        /// <summary>
        /// Removes all labels in the code. This method should be invoked after all goto statements have been removed.
        /// </summary>
        private void ClearLabelStatements()
        {
            foreach (Statement labelStatement in methodContext.GotoLabels.Values)
            {
                labelStatement.Label = string.Empty;
            }
        }

        #region Preprocessing

        /// <summary>
        /// Does the preprocessing needed for the algorithm and collects all goto-label pairs.
        /// </summary>
        /// <returns>Returns an enumeration of all goto-label pairs in the method.</returns>
        internal IEnumerable<KeyValuePair<IfStatement, Statement>> Preprocess()
        {
            /// Makes all unconditional gotos conditional gotos.
            ReplaceUnconditionalGoto();
            /// Adds boolean variables that indicate if a jump to the label is required.
            AddLabelVariables();

            /// Generates a list of goto-label pairs and sorts it by label index.
            IEnumerable<KeyValuePair<IfStatement, Statement>> gotoPairs = GetGotoPairs();
            gotoPairs = gotoPairs.OrderBy(x => x.Value.Label);

            /// Hopefully those names arent taken.
            this.breakVariable = new VariableDefinition("breakCondition", methodContext.Method.Module.TypeSystem.Boolean, this.methodContext.Method);
            methodContext.VariablesToRename.Add(this.breakVariable.Resolve());
            this.continueVariable = new VariableDefinition("continueCondition", methodContext.Method.Module.TypeSystem.Boolean, this.methodContext.Method);
            methodContext.VariablesToRename.Add(this.continueVariable.Resolve());
            return gotoPairs;
        }

        /// <summary>
        /// Collects a list of all goto-target pairs in the code.
        /// </summary>
        /// <returns>Returns a list, containing all pairs goto-target.</returns>
        private List<KeyValuePair<IfStatement, Statement>> GetGotoPairs()
        {
            List<KeyValuePair<IfStatement, Statement>> result = new List<KeyValuePair<IfStatement, Statement>>();
            foreach (GotoStatement gotoHead in methodContext.GotoStatements)
            {
                Statement target = methodContext.GotoLabels[gotoHead.TargetLabel];
                IfStatement gotoCondition = gotoHead.Parent.Parent as IfStatement; // goto -> block -> if
                if (gotoCondition == null)
                {
                    throw new ArgumentOutOfRangeException("Goto not embeded in condition.");
                }
                KeyValuePair<IfStatement, Statement> toAdd = new KeyValuePair<IfStatement, Statement>(gotoCondition, target);
                result.Add(toAdd);
            }
            return result;
        }

        /// <summary>
        /// Declare boolean variable for each label, and include <code>variable = false;</code> statements just after the label.
        /// Moves the label from the original statement to the variable assignment.
        /// </summary>
        private void AddLabelVariables()
        {
            List<Statement> oldLabels = new List<Statement>(methodContext.GotoLabels.Values);
            foreach (Statement oldLabel in oldLabels)
            {
                string label = oldLabel.Label;
                oldLabel.Label = string.Empty;
                VariableDefinition labelVariable = GetLabelVariable(label);

                ///Add cond = false expression right after the label.
                BlockStatement containingBlock = oldLabel.Parent as BlockStatement;
                ///All labeled statements must be inside BlockStatement.
                if (containingBlock == null)
                {
                    throw new ArgumentOutOfRangeException("Label target is not within a block.");
                }

                int labelIndex = containingBlock.Statements.IndexOf(oldLabel);
                ///Generate the <code>variable=false;</code> statement;
                BinaryExpression assignment =
                    new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(labelVariable, null), GetLiteralExpression(false), typeSystem, null);
                assignment.Right.ExpressionType = methodContext.Method.Module.TypeSystem.Boolean;
                ExpressionStatement assignmentStatement = new ExpressionStatement(assignment);
                ///Add the label to the new assignment
                assignmentStatement.Label = label;
                methodContext.GotoLabels[label] = assignmentStatement;
                ///Push the assignment just before the old labeled statement
                variableToAssignment.Add(labelVariable, assignmentStatement);
                containingBlock.AddStatementAt(labelIndex, assignmentStatement);
            }
        }

        /// <summary>
        /// Embeds each unconditional goto in <code>if(true){}</code> statement.
        /// </summary>
        private void ReplaceUnconditionalGoto()
        {
            foreach (GotoStatement jump in methodContext.GotoStatements)
            {
                if (IsUnconditionalJump(jump))
                {
                    EmbedIntoDefaultIf(jump);
                }
            }
        }

        /// <summary>
        /// Checks if a GotoStatement is surrounded by an if statement.
        /// </summary>
        /// <param name="jump">The GotoStatement to be checked.</param>
        /// <returns>Returns true, if the goto is unconditional.</returns>
        private bool IsUnconditionalJump(GotoStatement jump)
        {
            /// Only gotos from the type
            /// If(condition)
            /// {
            ///     goto Label;
            /// }
            /// are considered conditional.
            BlockStatement directParent = jump.Parent as BlockStatement;
            if (directParent == null)
            {
                ///Dummy check.
                throw new ArgumentOutOfRangeException("Goto statement outside of block.");
            }
            if (directParent.Parent == null)
            {
                ///The goto is in the outermost block statement.
                return true;
            }
            if (directParent.Parent is IfStatement == false)
            {
                ///The goto is not inside if statement at all.
                return true;
            }

            IfStatement parentIfStatement = directParent.Parent as IfStatement;
            if (parentIfStatement.Then == directParent && directParent.Statements.Count == 1 && parentIfStatement.Else == null)
            {
                ///Only case where goto is conditional - inside an if without else, and the only statement in the then block.
                return false;
            }

            return true;
        }

        /// <summary>
        /// Embeds unconditional goto into <code> if(true){} </code> statement
        /// </summary>
        /// <param name="jump">The unconditional goto.</param>
        private void EmbedIntoDefaultIf(GotoStatement jump)
        {
            BlockStatement parentBlock = jump.Parent as BlockStatement;
            BlockStatement thenBlock = new BlockStatement();
            thenBlock.AddStatement(jump);
            Expression condition = GetLiteralExpression(true);
            IfStatement embeddingStatement = new IfStatement(condition, thenBlock, null);

            ///Should be initialized by the statements themselves, but just in case.
            thenBlock.Parent = embeddingStatement;
            
            ///Replace the old goto statement with the new embedding If
            int jumpIndex = parentBlock.Statements.IndexOf(jump);
            parentBlock.Statements.RemoveAt(jumpIndex);
            parentBlock.AddStatementAt(jumpIndex, embeddingStatement);

            if (parentBlock.Parent is ConditionCase)
            {
                ///If the goto was the last statement in case add break after the embedding if.
                int embeddingStatementIndex = parentBlock.Statements.IndexOf(embeddingStatement);
                if (embeddingStatementIndex == parentBlock.Statements.Count)
                {
                    parentBlock.AddStatement(new BreakStatement(null));
                }
            }
        }

        #endregion

        /// <summary>
        /// The realisation of algorithm for single goto statement.
        /// </summary>
        /// <param name="gotoStatement">The goto statement to be eliminated.</param>
        /// <param name="labeledStatement">The labeled statement corresponding to <paramref name="gotoStatement"/>.</param>
        private void EliminateGotoPair(IfStatement gotoStatement, Statement labeledStatement)
        {
            ///Get the relation between goto and label. For more info about the relations, see Chapter 2.2 of
            ///<see cref="Taming Control Flow A Structured Approach to Eliminating Goto Statements.pdf"/>.
            int gotoLevel = CalculateLevel(gotoStatement);
            int labelLevel = CalculateLevel(labeledStatement);

            bool gotoPrecedesLabel = Precedes(gotoStatement, labeledStatement);
            GotoToLabelRelation relation = ResolveRelation(gotoStatement, labeledStatement);

            ///Perform the movements described in Chapter 2.2, until the goto and the label are in the same BlockStatement.
            while (relation != GotoToLabelRelation.Siblings)
            {
                if (relation == GotoToLabelRelation.IndirectlyRelated)
                {
                    MoveOut(gotoStatement, labeledStatement.Label);
                }
                if (relation == GotoToLabelRelation.DirectlyRelated)
                {
                    if (gotoLevel > labelLevel)
                    {
                        MoveOut(gotoStatement, labeledStatement.Label);
                    }
                    else
                    {
                        if (!gotoPrecedesLabel)
                        {
                            Statement labelContainer = GetSameLevelParent(labeledStatement, gotoStatement);
                            LiftGoto(gotoStatement, labelContainer, labeledStatement.Label);
                        }

                        Statement labeledStatementSameLevelParent = GetSameLevelParent(labeledStatement, gotoStatement);
                        MoveIn(gotoStatement, labeledStatementSameLevelParent, labeledStatement.Label);
                    }
                }

                ///Update the relation info between the goto statement and the labeled target statement
                gotoLevel = CalculateLevel(gotoStatement);
                labelLevel = CalculateLevel(labeledStatement);
                relation = ResolveRelation(gotoStatement, labeledStatement);
                gotoPrecedesLabel = Precedes(gotoStatement, labeledStatement);
            }

            ///Perform the appropriate goto-elimination transformation, as described in Chapter 2.1
            if (gotoPrecedesLabel)
            {
                EliminateViaIf(gotoStatement, labeledStatement);
            }
            else
            {
                EliminateViaDoWhile(gotoStatement, labeledStatement);
            }
        }

        /// <summary>
        /// Gets the parent of the <paramref name="labeledStatement"/>, that is on the same level as the <paramref name="gotoStatement"/>.
        /// </summary>
        /// <param name="labeledStatement">The labeled statement.</param>
        /// <param name="gotoStatement">The goto statement.</param>
        /// <returns>Returns the parent of the labeled statement that is on the same level as the goto statement.</returns>
        private Statement GetSameLevelParent(Statement labeledStatement, IfStatement gotoStatement)
        {
            ///This method must be used only when the goto statement is in the same block with a construct on the parent chain of the labeled statement.
            Stack<Statement> stack = GetParentsChain(labeledStatement);
            BlockStatement gotoParent = gotoStatement.Parent as BlockStatement;
            while (!gotoParent.Statements.Contains(stack.Peek()))
            {
                stack.Pop();
            }
            return stack.Peek();
        }

        /// <summary>
        /// Eliminates the goto statement via introducing do-while. Should be used when the goto comes after the label.
        /// </summary>
        /// <param name="gotoStatement">The goto statement.</param>
        /// <param name="labeledStatement">The labeled statement.</param>
        private void EliminateViaDoWhile(IfStatement gotoStatement, Statement labeledStatement)
        {
            ICollection<BreakStatement> breaks = new List<BreakStatement>();
            ICollection<ContinueStatement> continues = new List<ContinueStatement>();
            BlockStatement containingBlock = GetOuterBlock(labeledStatement);

            int labelIndex = containingBlock.Statements.IndexOf(labeledStatement);
            int gotoIndex = containingBlock.Statements.IndexOf(gotoStatement);
            BlockStatement loopBody = CollectStatements(labelIndex, gotoIndex, containingBlock);

            /// Checks if the gotoStatement is inside a loop/switch statement.
            /// If so, then some breaks might be enclosed in the new do-while. Additional breaks need to be included in this case
            if (ShouldCheck(gotoStatement))
            {
                ContinueAndBreakFinder finder = new ContinueAndBreakFinder();
                finder.Visit(loopBody);
                breaks = finder.Breaks;
                continues = finder.Continues;
            }

            /// Add condition = true before each enclosed break statement.
            foreach (BreakStatement statement in breaks)
            {
                BlockStatement breakBlock = GetOuterBlock(statement);
                int breakIndex = breakBlock.Statements.IndexOf(statement);
                BinaryExpression assign =
					new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(breakVariable, null), GetLiteralExpression(true), typeSystem, null);
                usedBreakVariable = true;
                ExpressionStatement assignment = new ExpressionStatement(assign);
                breakBlock.AddStatementAt(breakIndex, assignment);
            }

            /// Add condition = true before each enclosed continue statement
            /// and replace the continue statement with break statement
            foreach (ContinueStatement statement in continues)
            {
                BlockStatement continueBlock = GetOuterBlock(statement);
                int continueIndex = continueBlock.Statements.IndexOf(statement);
                BinaryExpression assign =
					new BinaryExpression(BinaryOperator.Assign, new VariableReferenceExpression(continueVariable, null), GetLiteralExpression(true), typeSystem, null);
                usedContinueVariable = true;
                ExpressionStatement assignment = new ExpressionStatement(assign);
                continueBlock.Statements.RemoveAt(continueIndex);
                continueBlock.AddStatementAt(continueIndex, new BreakStatement(null));
                continueBlock.AddStatementAt(continueIndex, assignment);
            }

            /// Replace the goto with do-while loop.
            DoWhileStatement doWhileLoop = new DoWhileStatement(gotoStatement.Condition, loopBody);
            gotoIndex = containingBlock.Statements.IndexOf(gotoStatement);
            containingBlock.AddStatementAt(gotoIndex, doWhileLoop);
            containingBlock.Statements.Remove(gotoStatement);

            if (breaks.Count > 0)
            {
                /// Add condition for the outer break, accounting for the enclosed breaks.
                AddBreakContinueConditional(gotoIndex + 1, containingBlock, new BreakStatement(null), breakVariable);//gotoindex + 1 should be the place after the newly inserted do-while loop
            }
            if (continues.Count > 0)
            {
                /// Add condition for the outer break, accounting for the enclosed continues.
                AddBreakContinueConditional(gotoIndex + 1, containingBlock, new ContinueStatement(null), continueVariable);
            }
        }

        /// <summary>
        /// Adds 
        /// <code>
        /// If(<paramref name="conditionVariable"/>)
        /// {
        ///     <paramref name="statement"/>
        /// }
        /// </code>
        /// statement at <paramref name="index"/> in <paramref name="containingBlock"/>
        /// </summary>
        /// <param name="index">The index at which the generated statement must be inserted.</param>
        /// <param name="containingBlock">The block, in which the new statement is inserted.</param>
        /// <param name="statement">The only statement in the then clause.</param>
        /// <param name="conditionVariable">The condition of the if.</param>
        private void AddBreakContinueConditional(int index, BlockStatement containingBlock, Statement statement, VariableReference conditionVariable)
        {
            BlockStatement thenBlock = new BlockStatement();
            thenBlock.AddStatement(statement);
			VariableReferenceExpression ifCondition = new VariableReferenceExpression(conditionVariable, null);
            IfStatement enclosingIfStatement = new IfStatement(ifCondition, thenBlock, null);
            containingBlock.AddStatementAt(index, enclosingIfStatement);
        }

        /// <summary>
        /// Decides if a check for contained breaks/continues should be performed.
        /// </summary>
        /// <param name="gotoStatement">The goto statement that causes the check.</param>
        /// <returns>Return strue if a check needs to be performed.</returns>
        private bool ShouldCheck(IfStatement gotoStatement)
        {
            Statement parent = gotoStatement.Parent;
            while (parent != null)
            {
                if (parent is SwitchStatement || parent is ForStatement || parent is ForEachStatement || parent is WhileStatement || parent is DoWhileStatement)
                {
                    return true;
                }
                parent = parent.Parent;
            }
            return false;
        }

        /// <summary>
        /// Eliminates the goto statement by inserting if statement. For more details, see "Chapter 2.1 - Goto statement is before label statement" in
        /// <see cref="Taming Control Flow A Structured Approach to Eliminating Goto Statements.pdf"/>.
        /// </summary>
        /// <param name="gotoStatement">The goto statement to be removed.</param>
        /// <param name="labeledStatement">The corresponding labeled statement.</param>
        private void EliminateViaIf(IfStatement gotoStatement, Statement labeledStatement)
        {
            BlockStatement containingBlock = labeledStatement.Parent as BlockStatement;//should not be null at this point

            int startingIndex = containingBlock.Statements.IndexOf(gotoStatement);
            int endingIndex = containingBlock.Statements.IndexOf(labeledStatement);
            if (startingIndex == endingIndex - 1)
            {
                //case:                 changed to:
                //if(condition)         -----
                //{                     -----
                //  goto: label1;       -----
                //}                     -----
                //label1: stmt1;        label1: stmt1;
                containingBlock.Statements.RemoveAt(startingIndex);
                return;
            }

            BlockStatement newThenBlock = CollectStatements(startingIndex + 1, endingIndex, containingBlock);
            Expression condition = Negator.Negate(gotoStatement.Condition, typeSystem);
            while (newThenBlock.Statements[0] is IfStatement)
            { 
                //flattens ifs , for instance
                //case:                     to:
                //if (a)                    if(a)
                //{                         {
                //    if (a)                    statement1;
                //    {                         stuff
                //        statement1;           statement2;
                //        stuff                 ...
                //    }                     }
                //    statement2
                //    ...
                //}
                IfStatement firstStatement = newThenBlock.Statements[0] as IfStatement;
                if (AreEqual(firstStatement.Condition, condition) && firstStatement.Else == null)
                {
                    newThenBlock.Statements.RemoveAt(0); //remove the if

                    for (int i = 0; i < firstStatement.Then.Statements.Count; i++)
                    {
                        newThenBlock.AddStatement(firstStatement.Then.Statements[i]);
                    }
                }
                else
                {
                    break;
                }
            }
            gotoStatement.Then = newThenBlock;
            gotoStatement.Condition = condition;
        }

        /// <summary>
        /// Decides if two expressions have the same semantics.
        /// </summary>
        /// <param name="first"> The first expression.</param>
        /// <param name="second">The second expression.</param>
        /// <returns>Returns true if the expressions have the same semantics.</returns>
        private bool AreEqual(Expression first, Expression second)
        {
            return first.Equals(second);
        }

        /// <summary>
        /// Inserts the selected statements into new BlockStatement and returns it. In the process the collected statements are detached from their previous containing block.
        /// </summary>
        /// <param name="startingIndex">The index of the first statement to be selected.</param>
        /// <param name="endingIndex">The index of the last statement. This is the first one that doesn't get selected.</param>
        /// <param name="containingBlock">The block from which statements are retrieved.</param>
        /// <returns>The new block containining the selected statements.</returns>
        private BlockStatement CollectStatements(int startingIndex, int endingIndex, BlockStatement containingBlock)
        {
            BlockStatement newThenBlock = new BlockStatement();

            for (int i = startingIndex; i < endingIndex; endingIndex--)
            {
                newThenBlock.AddStatement(containingBlock.Statements[i]);
                containingBlock.Statements.RemoveAt(i);
            }
            return newThenBlock;
        }

        /// <summary>
        /// Performs a move-in transformation as described in Chapter 2.2.2 in <see cref="Taming Control Flow A Structured Approach to Eliminating Goto Statements.pdf"/>.
        /// </summary>
        /// <param name="gotoStatement">The goto statement to be moved in.</param>
        /// <param name="targetStatement">The statement that the goto will move in.</param>
        /// <param name="label">The target label.</param>
        private void MoveIn(IfStatement gotoStatement, Statement targetStatement, string label)
        {
            /// Preprocessing.
            BlockStatement containingBlock = gotoStatement.Parent as BlockStatement;
			VariableReferenceExpression conditionVar = new VariableReferenceExpression(GetLabelVariable(label), null);

            /// Create the statement conditionVariable = originalCondition;
            /// Add the assigning statement before gotoStatement and replace originalCondition with conditionVariable 
            ExtractConditionIntoVariable(conditionVar.CloneExpressionOnly() as VariableReferenceExpression, gotoStatement, containingBlock);

            /// Collect the statements between the goto jump and the target statement.
            int gotoIndex = containingBlock.Statements.IndexOf(gotoStatement);
            int targetIndex = containingBlock.Statements.IndexOf(targetStatement);
            BlockStatement thenBlock = CollectStatements(gotoIndex + 1, targetIndex, containingBlock);

            /// Create the new if.
			IfStatement outerIfStatement =
                new IfStatement(new UnaryExpression(UnaryOperator.LogicalNot, conditionVar.CloneExpressionOnly(), null), thenBlock, null);

            /// Replace the old goto with the new if statement.
            if (outerIfStatement.Then.Statements.Count > 0)
            {
                containingBlock.AddStatementAt(gotoIndex, outerIfStatement);
            }
            containingBlock.Statements.Remove(gotoStatement);

            /// At this point the original goto statement is completely detached from the AST. It must be reattached as the first statement in the target.
            if (targetStatement is DoWhileStatement)
            {
                /// Loops with more than one entry can be created only by this step, so the do-while loops is the only one
                /// that must be considered.
                (targetStatement as DoWhileStatement).Body.AddStatementAt(0, gotoStatement);
            }
            else if (targetStatement is IfStatement)
            {
                /// If statements with more than one entry to then/else blocks can be created only by this step, so that's why
                /// the only case that must be considered is the case when the target is in the then block.
                IfStatement targetIf = targetStatement as IfStatement;
                targetIf.Condition = UpdateCondition(targetIf.Condition, conditionVar.CloneExpressionOnly() as VariableReferenceExpression); /// This greatly depends on the short-circut evaluation.
                targetIf.Then.AddStatementAt(0, gotoStatement);
            }
            else if (targetStatement is SwitchCase)
            {
                MoveInCase(gotoStatement, targetStatement as SwitchCase, label);
            }
            else if (targetStatement is WhileStatement)
            {
                /// This case should not be reachable.
                WhileStatement @while = targetStatement as WhileStatement;
                @while.Body.AddStatementAt(0, gotoStatement);
                @while.Condition = UpdateCondition(@while.Condition, conditionVar.CloneExpressionOnly() as VariableReferenceExpression);
            }
            else
            {
                throw new NotSupportedException("Unsupported target statement for goto jump.");
            }
        }

        /// <summary>
        /// This method updates a boolean condition, so that it passes when either its old form was true, or the supplied variable is true.
        /// The method greatly depends on the short-circut evaluation.
        /// </summary>
        /// <param name="oldCondition">The condition that needs to be updated.</param>
        /// <param name="conditionVar">The new variable, that will be added to the condition.</param>
        /// <returns>Returns the updated condition.</returns>
        private BinaryExpression UpdateCondition(Expression oldCondition, VariableReferenceExpression conditionVar)
        {
            /// Covers the case a || a || a || oldCondition.
            /// Cases a || b || a || b || oldCondition should not be present, since all goto's that jump to a given label
            /// are processed before moving to the next label. Thus, if we move to variable 'b', we have no reason to go back to 'a'.
            if (oldCondition is BinaryExpression)
            { 
                BinaryExpression oldBinary = oldCondition as BinaryExpression;
                if (oldBinary.Left is VariableReferenceExpression)
                {
                    if ((oldBinary.Left as VariableReferenceExpression).Variable == conditionVar.Variable)
                    {
                        return oldBinary;
                    }
                }
            }

            ///Generate the new condition.
			BinaryExpression result = new BinaryExpression(BinaryOperator.LogicalOr, conditionVar, oldCondition, typeSystem, null);
            result.ExpressionType = methodContext.Method.Module.TypeSystem.Boolean;
            return result;
        }

        /// <summary>
        /// Resolves the variable, carrying the condition for given labeled statement. If no such variable exists,
        /// it's created.
        /// </summary>
        /// <param name="label">The target label.</param>
        /// <returns>Returns VariableDefinition of the required variable.</returns>
        private VariableDefinition GetLabelVariable(string label)
        {
            if (!labelToVariable.ContainsKey(label))
            {
                ///Generate the variable
                TypeReference boolType = methodContext.Method.Module.TypeSystem.Boolean;
                string varName = label + "_cond";
                VariableDefinition variable = new VariableDefinition(varName, boolType, this.methodContext.Method);
                labelToVariable.Add(label, variable);
                assignedOnly.Add(variable, true);
                return variable;
            }
            VariableDefinition result = labelToVariable[label];
            /// If the variable is being needed for second time, then it's used to extract some kind of expression.
            /// Thus, it will be used as a condition to an if statement.
            assignedOnly[result] = false; 
            return result;
        }

        /// <summary>
        /// Contains the logic for moving a  goto inside the case of switch statement. For more information 
        /// see Chapter "2.2.2 Inward-movement Transformations : Moving a goto into a switch statement" from
        /// <see cref="Taming Control Flow A Structured Approach to Eliminating Goto Statements.pdf"/>.
        /// </summary>
        /// <param name="gotoStatement">The goto statement to be moved in.</param>
        /// <param name="switchCase">The switch case that contains the target of the goto statement.</param>
        /// <param name="label">The label of the target.</param>
        private void MoveInCase(IfStatement gotoStatement, SwitchCase switchCase, string label)
        {
			VariableReferenceExpression conditionVariable = new VariableReferenceExpression(GetLabelVariable(label), null);
            BlockStatement containingBlock = GetOuterBlock(gotoStatement);
            SwitchStatement switchStatement = switchCase.Parent as SwitchStatement;

            int gotoIndex = containingBlock.Statements.IndexOf(gotoStatement);
            int switchIndex = containingBlock.Statements.IndexOf(switchStatement);

            /// Generate variable and extract the switch condition
            string switchVariableName = "switch" + switchStatement.ConditionBlock.First.Offset;
            TypeReference switchVariableType = GetSwitchType(switchStatement);
            VariableDefinition switchVariable = new VariableDefinition(switchVariableName, switchVariableType, this.methodContext.Method);
            switchVariables.Add(switchVariable);
			VariableReferenceExpression switchVarEx = new VariableReferenceExpression(switchVariable, null);
            ExtractConditionIntoVariable(switchVarEx, switchStatement, containingBlock);

            BlockStatement thenBlock = CollectStatements(gotoIndex + 1, switchIndex + 1, containingBlock);

            BlockStatement elseBlock = new BlockStatement();

            /// Adds swCond = caseCond; in the last part of else.
			BinaryExpression assignSwitchVariable = new BinaryExpression(BinaryOperator.Assign, switchVarEx.CloneExpressionOnly(), GetCaseConditionExpression(switchCase), typeSystem, null);
            elseBlock.AddStatement(new ExpressionStatement(assignSwitchVariable));

			IfStatement precedingIf = new IfStatement(new UnaryExpression(UnaryOperator.LogicalNot, conditionVariable, null), thenBlock, elseBlock);

            /// Attach new if and move the goto conditional.
            /// Attach it only if the then block is not empty.
            if (precedingIf.Then.Statements.Count != 0)
            {
                containingBlock.AddStatementAt(gotoIndex, precedingIf);
            }
            containingBlock.Statements.Remove(gotoStatement);
            switchCase.Body.AddStatementAt(0, gotoStatement);
        }

        /// <summary>
        /// Gets the condition that is representing the supplied case.
        /// </summary>
        /// <param name="switchCase">The switch case, whose condition is being needed.</param>
        /// <returns>Returns expression, evaluating so that the switch jumps to <paramref name="switchCase"/>.</returns>
        private Expression GetCaseConditionExpression(SwitchCase switchCase)
        {
            if (switchCase is ConditionCase)
            {
                return (switchCase as ConditionCase).Condition;
            }
            else
            {
                /// SwitchCase is the default case
                SwitchStatement switchStatement = switchCase.Parent as SwitchStatement;
                //TypeReference switchType = GetSwitchType(switchCase.Parent as SwitchStatement);

                /// this could possibly be long instead of int, but this case should be quite rare
                int result = 1;
                foreach (SwitchCase @case in switchStatement.Cases)
                {
                    if (@case is DefaultCase)
                    {
                        continue;
                    }
                    result += (int)(((@case as ConditionCase).Condition as LiteralExpression).Value);
                }
                return GetLiteralExpression(result);
            }
        }

        /// <summary>
        /// Resolves the type of the switch variable.
        /// </summary>
        /// <param name="switchStatement">The switch statement.</param>
        /// <returns>Returns TypeReference to the type of the switch variable.</returns>
        private TypeReference GetSwitchType(SwitchStatement switchStatement)
        {
            /// switchStatement.Cases.First() without LINQ.
            ConditionCase cc = null;
            IEnumerable<SwitchCase> cases = switchStatement.Cases;
            foreach (SwitchCase @case in cases)
            {
                if (@case is ConditionCase)
                {
                    cc = @case as ConditionCase;
                    break;
                }
            }

            LiteralExpression condition = cc.Condition as LiteralExpression;
            if (condition == null)
            {
                throw new NotSupportedException("Case should have literal condition.");
            }

            TypeReference result = condition.ExpressionType;
            return result;
        }

        /// <summary>
        /// Extracts the condition of the condition statement into new variable. Replaces the condition of the statement with the new variable and inserts 
        /// the assignment statement right before the condition statement.
        /// </summary>
        /// <param name="conditionVar">The variable which should be assigned the condition.</param>
        /// <param name="gotoStatement">The conditional goto.</param>
        /// <param name="containingBlock">The block that contains the conditional goto.</param>
        private void ExtractConditionIntoVariable(VariableReferenceExpression conditionVar, ConditionStatement statement, BlockStatement containingBlock)
        {
            /// Create the statement conditionVariable = originalCondition;
			BinaryExpression conditionAssignment = new BinaryExpression(BinaryOperator.Assign, conditionVar, statement.Condition, typeSystem, null);
            ExpressionStatement assignStatement = new ExpressionStatement(conditionAssignment);

            /// Insert the statement before the original goto statement.
            int gotoIndex = containingBlock.Statements.IndexOf(statement);
            containingBlock.AddStatementAt(gotoIndex, assignStatement);

            /// Update the condition of the goto statement.
            statement.Condition = conditionVar.CloneExpressionOnly();
        }

        /// <summary>
        /// Performs lifting operation on the goto statement. For more information, see
        /// Chapter "2.2.3 Goto-lifting Transformation" in <see cref="Taming Control Flow A Structured Approach to Eliminating Goto Statements.pdf"/>.
        /// </summary>
        /// <param name="gotoStatement">The goto to be lifted.</param>
        /// <param name="labelContainingStatement">The statement, containing the target of the goto.</param>
        /// <param name="label">The label of the target.</param>
        private void LiftGoto(IfStatement gotoStatement, Statement labelContainingStatement, string label)
        {
            BlockStatement containingBlock = GetOuterBlock(gotoStatement);
			VariableReferenceExpression variableEx = new VariableReferenceExpression(GetLabelVariable(label), null);

            ExtractConditionIntoVariable(variableEx, gotoStatement, containingBlock);

            /// Extract the do-while loop body from the current block
            int gotoIndex = containingBlock.Statements.IndexOf(gotoStatement);
            int labelIndex = containingBlock.Statements.IndexOf(labelContainingStatement);
            BlockStatement loopBody = CollectStatements(labelIndex, gotoIndex, containingBlock);

            /// Add the goto statement as the first one in the new do-while loop
            loopBody.AddStatementAt(0, gotoStatement);

            /// Remove the goto from its old parent block and attach the do-while loop on its place
            gotoIndex = containingBlock.Statements.IndexOf(gotoStatement);
            containingBlock.Statements.Remove(gotoStatement);
            DoWhileStatement doWhileLoop = new DoWhileStatement(gotoStatement.Condition.CloneExpressionOnly(), loopBody);
            containingBlock.AddStatementAt(gotoIndex, doWhileLoop);
        }

        /// <summary>
        /// Performs a move out operation on the goto statement. For more information see
        /// Chapter 2.2.1 "Outward-movement Transformations"
        /// </summary>
        /// <param name="gotoStatement"></param>
        /// <param name="label"></param>
        private void MoveOut(IfStatement gotoStatement, string label)
        {
            /// Preprocessing.
            BlockStatement containingBlock = gotoStatement.Parent as BlockStatement;
            BlockStatement outerBlock = GetOuterBlock(containingBlock);
			VariableReferenceExpression conditionVar = new VariableReferenceExpression(GetLabelVariable(label), null);

            ExtractConditionIntoVariable(conditionVar.CloneExpressionOnly() as VariableReferenceExpression, gotoStatement, containingBlock);

            Statement oldEnclosingStatement = containingBlock.Parent;
            if (oldEnclosingStatement is SwitchCase)
            {
                oldEnclosingStatement = oldEnclosingStatement.Parent;
            }
            if (containingBlock.Parent is SwitchCase || containingBlock.Parent is WhileStatement || containingBlock.Parent is DoWhileStatement || containingBlock.Parent is ForStatement
                || containingBlock.Parent is ForEachStatement)
            {
                /// Then we can exit using break.
                /// Create the if - break.
                BlockStatement thenBlock = new BlockStatement();
                thenBlock.AddStatement(new BreakStatement(null));  //might have to keep track of brakes
                IfStatement breakIf = new IfStatement(conditionVar.CloneExpressionOnly(), thenBlock, null);

                /// Replace the original goto
                int gotoIndex = containingBlock.Statements.IndexOf(gotoStatement);
                containingBlock.Statements.Remove(gotoStatement);
                containingBlock.AddStatementAt(gotoIndex, breakIf);
            }
            else if (containingBlock.Parent is IfStatement || containingBlock.Parent is TryStatement || containingBlock.Parent is IfElseIfStatement)
            {
                /// Then we can exit via introducing another condition.
                int gotoIndex = containingBlock.Statements.IndexOf(gotoStatement);
                int index = gotoIndex + 1;
                BlockStatement thenBlock = new BlockStatement();
                while (index < containingBlock.Statements.Count)
                {
                    thenBlock.AddStatement(containingBlock.Statements[index]);
                    containingBlock.Statements.RemoveAt(index);
                }
				UnaryExpression condition = new UnaryExpression(UnaryOperator.LogicalNot, conditionVar.CloneExpressionOnly(), null);
                IfStatement innerIf = new IfStatement(condition, thenBlock, null);

                /// At this point the goto statement should be the last one in the block.
                /// Simply replace it with the new if.
                containingBlock.Statements.Remove(gotoStatement);
                /// If the then block is empty, the if should not be added.
                if (innerIf.Then.Statements.Count != 0)
                {
                    containingBlock.AddStatement(innerIf);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("Goto statement can not leave this parent construct.");
            }

            /// Add the goto statement after the construct.
            /// The goto statement shoud be detached from the AST at this point.
            outerBlock.AddStatementAt(outerBlock.Statements.IndexOf(oldEnclosingStatement) + 1, gotoStatement);
        }

        /// <summary>
        /// Gets the first BlockStatement, that contains <paramref name="statement"/>.
        /// </summary>
        /// <param name="statement">The statement, which is being contained.</param>
        /// <returns>Returns the found BlockStatement.</returns>
        private BlockStatement GetOuterBlock(Statement statement)
        {
            Statement result = statement.Parent;
            while (result is BlockStatement == false)
            {
                result = result.Parent;
            }
            return result as BlockStatement;
        }

        /// <summary>
        /// Resolves the relation between <paramref name="gotoStatement"/> and its corresponding <paramref name="labeledStatement"/>.
        /// The relations are as defined in Chapter "2.2 Goto-movement Transformations" in
        /// <see cref="Taming Control Flow A Structured Approach to Eliminating Goto Statements.pdf"/>.
        /// </summary>
        /// <param name="gotoStatement">The goto statement.</param>
        /// <param name="labeledStatement">The targeted statement.</param>
        /// <returns>Returns the resolved relation between the goto and the target.</returns>
        private GotoToLabelRelation ResolveRelation(IfStatement gotoStatement, Statement labeledStatement)
        {
            BlockStatement commonBlock = GetLowestCommonParent(gotoStatement, labeledStatement) as BlockStatement;

            if (commonBlock == null)
            {
                /// Goto and label are part of same construct, but different branches of it,
                /// for instace then and else or different cases of a switch
                return GotoToLabelRelation.IndirectlyRelated;
            }

            if (commonBlock.Statements.Contains(gotoStatement) && commonBlock.Statements.Contains(labeledStatement))
            {
                /// The goto and its target are on the same level.
                return GotoToLabelRelation.Siblings;
            }

            if (commonBlock.Statements.Contains(gotoStatement) || commonBlock.Statements.Contains(labeledStatement))
            {
                /// Either the goto or the target is nested in an expression, deeper in the common block.
                return GotoToLabelRelation.DirectlyRelated;
            }

            return GotoToLabelRelation.IndirectlyRelated;
        }

        /// <summary>
        /// Gets the first statement, that contains <paramref name="first"/> and <paramref name="second"/>.
        /// </summary>
        /// <param name="first">The first statement.</param>
        /// <param name="second">The second statement.</param>
        /// <returns>Returns the found statement.</returns>
        private Statement GetLowestCommonParent(Statement first, Statement second)
        {
            Stack<Statement> gotoParentsStack = GetParentsChain(first);
            Stack<Statement> labelParentsStack = GetParentsChain(second);

            Statement lastCommonParent = null;
            while (gotoParentsStack.Peek() == labelParentsStack.Peek())
            {
                lastCommonParent = gotoParentsStack.Pop();
                labelParentsStack.Pop();
            }

            return lastCommonParent;
        }

        /// <summary>
        /// Determines if <paramref name="first"/> statement precedes <paramref name="second"/> statement in the flow of the program.
        /// </summary>
        /// <param name="first">The first statement.</param>
        /// <param name="second">The second statement.</param>
        /// <returns>Returns true, if <paramref name="first"/> precedes <paramref name="second"/>.</returns>
        private bool Precedes(Statement first, Statement second)
        {
            Stack<Statement> firstParentsStack = GetParentsChain(first);
            Stack<Statement> secondParentsStack = GetParentsChain(second);

            ///Find the last common parent of the two statements.
            Statement lastCommonParent = null;
            while (firstParentsStack.Peek() == secondParentsStack.Peek())
            {
                lastCommonParent = firstParentsStack.Pop();
                secondParentsStack.Pop();
            }

            ///The parent is either SwitchStatement or BlockStatement
            if (lastCommonParent as SwitchStatement != null)
            {
                /// Pass all the cases, and determine which statement will be first.
                foreach (SwitchCase @case in (lastCommonParent as SwitchStatement).Cases)
                {
                    if (@case == firstParentsStack.Peek())
                    {
                        return true;
                    }
                }
                return false;
            }
            if (lastCommonParent as BlockStatement == null)
            {
                throw new ArgumentException("No common block found.");
            }

            int firstIndex = (lastCommonParent as BlockStatement).Statements.IndexOf(firstParentsStack.Peek());
            int secondIndex = (lastCommonParent as BlockStatement).Statements.IndexOf(secondParentsStack.Peek());

            return firstIndex < secondIndex;
        }

        /// <summary>
        /// Collects the parents chain for <paramref name="statement"/>.
        /// </summary>
        /// <param name="statement">The statement whose parent chain is being computed.</param>
        /// <returns>Returns the chain, in order from topmost structure, to innermost.</returns>
        private Stack<Statement> GetParentsChain(Statement statement)
        {
            Stack<Statement> result = new Stack<Statement>();
            while (statement != null)
            {
                result.Push(statement);
                statement = statement.Parent;
            }
            return result;
        }

        /// <summary>
        /// Calculates in how much statements <paramref name="statement"/> is nested.
        /// </summary>
        /// <param name="statement">The statement.</param>
        /// <returns>The number of statement, containing <paramref name="statement"/>.</returns>
        private int CalculateLevel(Statement statement)
        {
            int result = 0;
            while (statement != null)
            {
                statement = statement.Parent;
                result++;
            }
            return result;
        }

        private LiteralExpression GetLiteralExpression(object value)
        {
			return new LiteralExpression(value, typeSystem, null);
        }

        /// <summary>
        /// An enumeration, representing the relations between goto and its target, as described in 
        /// Chapter "2.2 Goto-movement Transformations" in <see cref="Taming Control Flow A Structured Approach to Eliminating Goto Statements.pdf"/>.
        /// </summary>
        enum GotoToLabelRelation
        {
            Siblings,
            DirectlyRelated,
            IndirectlyRelated
        }
    }
}
