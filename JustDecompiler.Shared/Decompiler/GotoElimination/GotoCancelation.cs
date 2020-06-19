using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
    /// <summary>
    /// This step removes simple Goto-patterns. Note that it must pass before RemoveLastReturn step, as it relies on returns inside the code to match patterns.
    /// </summary>
    class GotoCancelation : IDecompilationStep
    {
		private const int MaximumStatementsToCopy = 15;

        private MethodSpecificContext methodContext;
        private BlockStatement body;///This is left to make debugging easier

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.methodContext = context.MethodContext;
            this.body = body;
            RemoveGotoStatements();
            return body;
        }

        /// <summary>
        /// The entry point of the class.
        /// </summary>
        private void RemoveGotoStatements()
        {
            bool changed = false;
            ///Do eliminations in loop, because eliminating a goto pair might cause
            ///a pattern for already traversed pair to match now.
            do
            {
                changed = false;

                //update the gotoPairs
                IEnumerable<KeyValuePair<GotoStatement, Statement>> gotoPairs = Preprocess();

                foreach (KeyValuePair<GotoStatement, Statement> pair in gotoPairs)
                {
                    changed = changed || EliminateGotoPair(pair.Key, pair.Value);
                }
            } while (changed);
        }

        /// <summary>
        /// Collects a list of all gotoStatement - gotoLabel pairs.
        /// </summary>
        /// <returns>An enumeration of the found pairs.</returns>
        private IEnumerable<KeyValuePair<GotoStatement, Statement>> Preprocess()
        {
            List<KeyValuePair<GotoStatement, Statement>> gotoPairs = new List<KeyValuePair<GotoStatement, Statement>>();
            foreach (GotoStatement @goto in methodContext.GotoStatements)
            {
                Statement target = methodContext.GotoLabels[@goto.TargetLabel];
                gotoPairs.Add(new KeyValuePair<GotoStatement, Statement>(@goto, target));
            }
            return gotoPairs;
        }

        private bool EliminateGotoPair(GotoStatement gotoStatement, Statement labeledStatement)
        {
            if (TryRemoveChainedGoto(labeledStatement, gotoStatement))
            {
                return true;
            }
            if (TryRemoveGoto(labeledStatement, gotoStatement))
            {
                return true;
            }
            if (TryCopyTargetedBlock(labeledStatement, gotoStatement))
            {
                return true;
            }
            //TODO: Add more steps
            return false;
        }

        /// <summary>
        /// Replaces the goto statement with its target, if the target is goto statement.
        /// </summary>
        /// <param name="labeledStatement"></param>
        /// <param name="gotoStatement"></param>
        /// <returns></returns>
        private bool TryRemoveChainedGoto(Statement labeledStatement, GotoStatement gotoStatement)
        {
            if (!(labeledStatement is GotoStatement))
            {
                return false;
            }
            BlockStatement parent = gotoStatement.Parent as BlockStatement;
            int index = parent.Statements.IndexOf(gotoStatement);
            /// Clone of labeled statement needed here

            Statement clone = labeledStatement.CloneStatementOnly();
            clone.Label = string.Empty;

            parent.Statements.RemoveAt(index);
            parent.AddStatementAt(index, clone);


            methodContext.GotoStatements.Remove(gotoStatement);
            methodContext.GotoStatements.Add(clone as GotoStatement);

            if (!Targeted(labeledStatement.Label))
            {
                UpdateUntargetedStatement(labeledStatement, new Statement[] { labeledStatement });
            }
            return true;
        }

        /// <summary>
        /// Removes the goto statement if it targets directly the statement afterwards
        /// </summary>
        /// <param name="labeledStatement">The targeted statement.</param>
        /// <param name="gotoStatement">The goto statement.</param>
        /// <returns></returns>
        private bool TryRemoveGoto(Statement labeledStatement, GotoStatement gotoStatement)
        {
            BlockStatement gotoParent = gotoStatement.Parent as BlockStatement;
            if (gotoParent == null)
            {
                throw new DecompilationException("Goto statement not inside a block.");
            }
            int gotoIndex = gotoParent.Statements.IndexOf(gotoStatement);
            if (labeledStatement.Parent == gotoParent && gotoParent.Statements.IndexOf(labeledStatement) == gotoIndex + 1)
            {
                ///then goto and label are consecutive
                gotoParent.Statements.RemoveAt(gotoIndex);
                methodContext.GotoStatements.Remove(gotoStatement);

                ///Clean up the label from the targeted statement if possible, and remove the targeted statement if it cannot be reached anymore
                if (!Targeted(labeledStatement.Label))
                {
                    UpdateUntargetedStatement(labeledStatement, new List<Statement>());
                }

                return true;
            }
            if (gotoIndex == gotoParent.Statements.Count - 1)
            {
                ///then the goto is the last statement in the block
                ///it can potentially point to the next statement from the outer block

                Statement parent = gotoParent.Parent;
                if (parent == null)
                {
                    ///the goto is in the outermost block
                    return false;
                }

                if (parent.CodeNodeType == CodeNodeType.ConditionCase || parent.CodeNodeType == CodeNodeType.DefaultCase)
                {
                    parent = parent.Parent;
                }

                if (parent.CodeNodeType == CodeNodeType.SwitchStatement || parent.CodeNodeType == CodeNodeType.ForEachStatement
                    || parent.CodeNodeType == CodeNodeType.WhileStatement || parent.CodeNodeType == CodeNodeType.ForStatement
                    || parent.CodeNodeType == CodeNodeType.DoWhileStatement || parent.CodeNodeType == CodeNodeType.IfStatement
                    || parent.CodeNodeType == CodeNodeType.IfElseIfStatement)
                {
                    BlockStatement outerBlock = parent.Parent as BlockStatement;
                    if (labeledStatement.Parent != outerBlock)
                    {
                        return false;
                    }
                    if (outerBlock.Statements.IndexOf(parent) == outerBlock.Statements.IndexOf(labeledStatement) - 1)
                    {
                        ///Then the goto is to the next statement in the natural flow of the program
                        gotoParent.Statements.RemoveAt(gotoIndex);
                        methodContext.GotoStatements.Remove(gotoStatement);
                        if (parent.CodeNodeType != CodeNodeType.IfStatement && parent.CodeNodeType != CodeNodeType.IfElseIfStatement)
                        {
                            ///then the goto must be replaced by break;
                            gotoParent.AddStatementAt(gotoIndex, new BreakStatement(gotoStatement.UnderlyingSameMethodInstructions));
                        }

                        ///Clean up the label from the targeted statement if possible, and remove the targeted statement if it cannot be reached anymore
                        if (!Targeted(labeledStatement.Label))
                        {
                            UpdateUntargetedStatement(labeledStatement, new List<Statement>());
                        }
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Checks if given label is stil targeted by any goto statement.
        /// </summary>
        /// <param name="label">The label in question.</param>
        /// <returns>Returns True if the label is still targeted.</returns>
        private bool Targeted(string label)
        {
            foreach (GotoStatement @goto in methodContext.GotoStatements)
            {
                if (@goto.TargetLabel == label)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Removes the label of the statement. Removes the block of statements supplied by <paramref name="originalStatements"/>, if it can no longer be reached. 
        /// </summary>
        /// <param name="labeledStatement">The labeled statement.</param>
        /// <param name="originalStatements">The statements in the block, following the labeled statement. The labeled statement should be the first one in this enumeration.</param>
        private void UpdateUntargetedStatement(Statement labeledStatement, IEnumerable<Statement> originalStatements)
        {
            ///In this case block means sequence of statements, such that the control flow must pass from one to the other.
            methodContext.GotoLabels.Remove(labeledStatement.Label);
            labeledStatement.Label = string.Empty;

            if (!methodContext.StatementToLogicalConstruct.ContainsKey(labeledStatement))
            {
                /// This happens when cloned statements are targeted.
                /// Labels will be put on cloned statements only when the goto that caused the clonning was labeled, i.e.
                /// goto label1;
                /// ...
                /// label1: goto label2;
                /// ...
                /// label2: return x;
                /// 
                /// This effectively means, that in this case the check for dead code is skipped.
                return;
            }

            ILogicalConstruct construct = methodContext.StatementToLogicalConstruct[labeledStatement];
            if (construct.Parent is CaseLogicalConstruct)
            {
                //Corner case for goto from case to case in switch construct.
                //The case is always reachable from the switch.
                return;
            }

            foreach (ILogicalConstruct predecessor in construct.AllPredecessors)
            {
                if (predecessor.FollowNode == construct)
                {
                    return;
                }
            }

            ///At this point we are sure that the statements are unreachable.
            ///They should be deleted.
            foreach (Statement s in originalStatements)
            {
                BlockStatement parent = s.Parent as BlockStatement;
                parent.Statements.Remove(s);

                //TODO: Chain remove all dead goto statements
                RemoveInnerGotoStatementsFromContext(s);
            }
        }

        /// <summary>
        /// Traverses the statements subtree rooted at s and removes all GotoStatements it finds from the decompilation context.
        /// </summary>
        /// <param name="s">The root of the tree.</param>
        private void RemoveInnerGotoStatementsFromContext(Statement s)
        {
            if (s == null)
            {
                return;
            }
            switch (s.CodeNodeType)
            {
                case CodeNodeType.BlockStatement:
                case CodeNodeType.UnsafeBlock:
                    foreach (Statement st in (s as BlockStatement).Statements)
                    {
                        RemoveInnerGotoStatementsFromContext(st);
                    }
                    break;
                case CodeNodeType.GotoStatement:
                    methodContext.GotoStatements.Remove(s as GotoStatement);
                    return;
                case CodeNodeType.IfStatement:
                    {
                        IfStatement ifs = s as IfStatement;
                        RemoveInnerGotoStatementsFromContext(ifs.Then);
                        RemoveInnerGotoStatementsFromContext(ifs.Else);
                    }
                    break;
                case CodeNodeType.IfElseIfStatement:
                    {
                        foreach (KeyValuePair<Expression, BlockStatement> pairs in (s as IfElseIfStatement).ConditionBlocks)
                        {
                            RemoveInnerGotoStatementsFromContext(pairs.Value);
                        }
                        RemoveInnerGotoStatementsFromContext((s as IfElseIfStatement).Else);
                    }
                    break;
                case CodeNodeType.WhileStatement:
                    {
                        RemoveInnerGotoStatementsFromContext((s as WhileStatement).Body);
                    }
                    break;
                case CodeNodeType.DoWhileStatement:
                    {
                        RemoveInnerGotoStatementsFromContext((s as DoWhileStatement).Body);
                    }
                    break;
                case CodeNodeType.ForStatement:
                    {
                        RemoveInnerGotoStatementsFromContext((s as ForStatement).Body);
                    }
                    break;
                case CodeNodeType.ForEachStatement:
                    {
                        RemoveInnerGotoStatementsFromContext((s as ForEachStatement).Body);
                    }
                    break;
                case CodeNodeType.ConditionCase:
                case CodeNodeType.DefaultCase:
                    {
                        RemoveInnerGotoStatementsFromContext((s as SwitchCase).Body);
                    }
                    break;
                case CodeNodeType.SwitchStatement:
                    {
                        foreach (SwitchCase @case in (s as SwitchStatement).Cases)
                        {
                            RemoveInnerGotoStatementsFromContext(@case);
                        }
                    }
                    break;
                case CodeNodeType.TryStatement:
                    {
                        RemoveInnerGotoStatementsFromContext((s as TryStatement).Try);
                    }
                    break;
                case CodeNodeType.FixedStatement:
                    {
                        RemoveInnerGotoStatementsFromContext((s as FixedStatement).Body);
                    }
                    break;
                case CodeNodeType.UsingStatement:
                    {
                        RemoveInnerGotoStatementsFromContext((s as UsingStatement).Body);
                    }
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Coppies the block that is targeted from the goto if it matches the case
        /// 
        /// ... some code
        /// goto: label0;
        /// 
        /// ...
        /// ...
        /// label0: statement1
        /// *statements*
        /// return;
        /// ...
        /// </summary>
        /// <param name="labeledStatement">The targeted statement.</param>
        /// <param name="gotoStatement">The goto statement.</param>
        /// <returns>Returns True if the targeted block was coppied.</returns>
        private bool TryCopyTargetedBlock(Statement labeledStatement, GotoStatement gotoStatement)
        {
            StatementCollection toCopy = new StatementCollection();
            StatementCollection originalStatements = new StatementCollection();
            BlockStatement targetParent = labeledStatement.Parent as BlockStatement;
            if (targetParent == null)
            {
                return false;
            }
            int targetIndex = targetParent.Statements.IndexOf(labeledStatement);
            originalStatements.Add(labeledStatement);
            Statement labeledClone = labeledStatement.CloneStatementOnly();
            labeledClone.Label = string.Empty;
            toCopy.Add(labeledClone);

			int maxStatementsToCopy = MaximumStatementsToCopy;

            if (ContainsLabel(labeledClone))
            {
                return false;
            }

			maxStatementsToCopy -= GetStatementsCount(labeledClone);
			if (maxStatementsToCopy < 0)
			{
				return false;
			}

            if (!IsReturnStatement(labeledClone) && !IsThrowStatement(labeledClone))
            {
                ///Collect all statements, until a return is reached.
                int index;
                for (index = targetIndex + 1; index < targetParent.Statements.Count; index++)
                {
                    Statement nextStatement = targetParent.Statements[index];
                    if (ContainsLabel(nextStatement))
                    {
                        return false;
                    }

					maxStatementsToCopy -= GetStatementsCount(nextStatement);
					if (maxStatementsToCopy < 0)
					{
						return false;
					}

                    originalStatements.Add(nextStatement);
                    Statement clone = nextStatement.CloneStatementOnly();
                    toCopy.Add(clone);
                    if (IsReturnStatement(nextStatement) || IsThrowStatement(nextStatement))
                    {
                        break;
                    }
                }
                if (index == targetParent.Statements.Count)
                {
                    ///all the statements were traversed and no 'return' statement was found
                    return false;
                }
            }
            ///Move the coppied statements on the place of the goto statement.
            MoveStatements(gotoStatement, toCopy);

            ///Clean up the label from the targeted statement if possible, and remove the targeted statement if it cannot be reached anymore
            if (!Targeted(labeledStatement.Label))
            {
                UpdateUntargetedStatement(labeledStatement, originalStatements);
            }

            return true;
        }

        private bool IsThrowStatement(Statement labeledClone)
        {
            if (!(labeledClone is ExpressionStatement))
            {
                return false;
            }
            ExpressionStatement exprStatement = labeledClone as ExpressionStatement;
            return exprStatement.Expression.CodeNodeType == CodeNodeType.ThrowExpression;
        }

        /// <summary>
        /// Moves the statements inside <paramref name="toMove"/> right after the goto statement adn removes the goto statement.
        /// </summary>
        /// <param name="gotoStatement">The goto statement, jumping to the block to be moved.</param>
        /// <param name="toMove">The collection of statements to be moved. This should already contain cloned statements.</param>
        private void MoveStatements(GotoStatement gotoStatement, IEnumerable<Statement> toMove)
        {
            BlockStatement gotoParent = gotoStatement.Parent as BlockStatement;
            if (gotoParent == null)
            {
                throw new DecompilationException("Goto statement not inside a block.");
            }
            int gotoIndex = gotoParent.Statements.IndexOf(gotoStatement);

            gotoParent.Statements.RemoveAt(gotoIndex);
            methodContext.GotoStatements.Remove(gotoStatement);
            int startIndex = gotoIndex;

            foreach (Statement st in toMove)
            {
                gotoParent.AddStatementAt(startIndex, st);
                startIndex++;
            }
            if (!string.IsNullOrEmpty(gotoStatement.Label))
            {
                /// If the goto was target of another goto, update the label to point to the first statement of the coppied block.
                string theLabel = gotoStatement.Label;
                Statement newLabelCarrier = gotoParent.Statements[gotoIndex];
                methodContext.GotoLabels[theLabel] = newLabelCarrier;
                newLabelCarrier.Label = gotoStatement.Label;
            }
        }

		/// <summary>
		/// Checks if the Statements tree rooted in the suplied statement contains labels or goto statements.
		/// </summary>
		/// <param name="statement">The root of the statements tree.</param>
		/// <returns>Returns true if label or goto statement was found.</returns>
		private bool ContainsLabel(Statement statement)
		{
			LableAndGotoFinder finder = new LableAndGotoFinder();
			return finder.CheckForLableOrGoto(statement);
		}

		/// <summary>
		/// Return the number of statements in the tree rooted in the suplied statement.
		/// </summary>
		/// <param name="statement">The root of the statements tree.</param>
		/// <returns>he number of statements in the tree.</returns>
		private int GetStatementsCount(Statement statement)
		{
			StatementsCounter statementsCounter = new StatementsCounter();
			return statementsCounter.GetStatementsCount(statement);
		}

        /// <summary>
        /// Checks if the statement is ExpressionStatement, containing ReturnExpression.
        /// </summary>
        /// <param name="statement">The statement in question.</param>
        /// <returns>Returns true, if the statement is return statement.</returns>
        private bool IsReturnStatement(Statement statement)
        {
            if (statement is ExpressionStatement)
            {
                return (statement as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.ReturnExpression;
            }
            return false;
        }

        private class LableAndGotoFinder : BaseCodeVisitor
        {
            bool hasLableOrGoto;

            public bool CheckForLableOrGoto(Statement statement)
            {
                hasLableOrGoto = false;
                Visit(statement);
                return hasLableOrGoto;
            }

            public override void Visit(ICodeNode node)
            {
                hasLableOrGoto = hasLableOrGoto || node is GotoStatement || node is Statement && !string.IsNullOrEmpty((node as Statement).Label);
                if (!hasLableOrGoto)
                {
                    base.Visit(node);
                }
            }
        }

		private class StatementsCounter : BaseCodeVisitor
		{
			private int statementsVisited;

			public int GetStatementsCount(Statement statement)
			{
				statementsVisited = 1;
				Visit(statement);
				return statementsVisited;
			}

			public override void VisitBlockStatement(BlockStatement node)
			{
				statementsVisited = statementsVisited + node.Statements.Count;
				base.VisitBlockStatement(node);
			}
		}
    }
}
