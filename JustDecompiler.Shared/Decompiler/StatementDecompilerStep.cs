using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Loops;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Decompiler
{
    public class StatementDecompilerStep : BaseInstructionVisitor, IDecompilationStep
    {
        private ExpressionDecompilerData expressions;
        private BlockLogicalConstruct logicalTree;
        private ControlFlowGraph theCFG;
        private DecompilationContext context;
        private TypeSystem typeSystem;

        private readonly Dictionary<CFGBlockLogicalConstruct, HashSet<CFGBlockLogicalConstruct>> gotoEndpointToOrigins;
        private readonly Dictionary<CFGBlockLogicalConstruct, HashSet<CFGBlockLogicalConstruct>> gotoOriginsToEndpoints;
        private readonly Dictionary<ILogicalConstruct, string> gotoEndpointConstructToLabel;
        private readonly Dictionary<CFGBlockLogicalConstruct, Dictionary<CFGBlockLogicalConstruct, GotoStatement>> gotoOriginBlockToGoToStatement;
        private readonly Dictionary<ILogicalConstruct, List<Statement>> logicalConstructToStatements;
        private readonly Dictionary<Statement, ILogicalConstruct> statementToLogicalConstruct;
        private readonly Dictionary<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> breaksOriginToEndPoint;
        private readonly Dictionary<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> continuesOriginToEndPoint;
        private readonly Dictionary<IBreaksContainer, CFGBlockLogicalConstruct> breaksContainerToBreakEndPoint;

        //output to the decompilation context
        private List<GotoStatement> contextGotoStatements;
        private Dictionary<string, Statement> contextGotoLabels;

        private uint gotoLabelsCounter = 0;
        private readonly Stack<ILogicalConstruct> parents;

        public StatementDecompilerStep()
        {
            gotoEndpointToOrigins = new Dictionary<CFGBlockLogicalConstruct, HashSet<CFGBlockLogicalConstruct>>();
            gotoEndpointConstructToLabel = new Dictionary<ILogicalConstruct, string>();
            gotoOriginBlockToGoToStatement = new Dictionary<CFGBlockLogicalConstruct, Dictionary<CFGBlockLogicalConstruct, GotoStatement>>();
            gotoOriginsToEndpoints = new Dictionary<CFGBlockLogicalConstruct, HashSet<CFGBlockLogicalConstruct>>();
            logicalConstructToStatements = new Dictionary<ILogicalConstruct, List<Statement>>();
            statementToLogicalConstruct = new Dictionary<Statement, ILogicalConstruct>();
            parents = new Stack<ILogicalConstruct>();
            breaksOriginToEndPoint = new Dictionary<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct>();
            continuesOriginToEndPoint = new Dictionary<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct>();
            breaksContainerToBreakEndPoint = new Dictionary<IBreaksContainer, CFGBlockLogicalConstruct>();
        }

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.context = context;
            contextGotoLabels = context.MethodContext.GotoLabels;
            contextGotoStatements = context.MethodContext.GotoStatements;
            theCFG = context.MethodContext.ControlFlowGraph;
            this.typeSystem = context.MethodContext.Method.Module.TypeSystem;

            logicalTree = context.MethodContext.LogicalConstructsTree;
            expressions = context.MethodContext.Expressions;
            body = (BlockStatement)ProcessLogicalConstruct(logicalTree, false)[0];

            body = InsertGotoEndpoints(body);
            context.MethodContext.StatementToLogicalConstruct = statementToLogicalConstruct;
            context.MethodContext.LogicalConstructToStatements = logicalConstructToStatements;
            return body;
        }

        private BlockStatement InsertGotoEndpoints(BlockStatement body)
        {
            bool goToOnlyReeachableConstructsFound = false;
            do
            {
                HashSet<ILogicalConstruct> reachableOnlyByGoTo = new HashSet<ILogicalConstruct>();
                foreach (CFGBlockLogicalConstruct gotoEndPointBlock in gotoEndpointToOrigins.Keys)
                {
                    ILogicalConstruct gotoEndPointConstruct = FindTopMostParentOfBlock(gotoEndPointBlock);
                    //If we end up here there are parts of the graph that are reachable only by goto, i.e. the normal followNode traversal never reached them
                    //and there were no statements generated for them. We should traverse them first before adding goto labels
                    if (!ExistsStatementForConstruct(gotoEndPointConstruct))
                    {
                        reachableOnlyByGoTo.Add(gotoEndPointConstruct);
                    }
                }

                goToOnlyReeachableConstructsFound = reachableOnlyByGoTo.Count > 0;

                foreach (ILogicalConstruct traverseCandidate in reachableOnlyByGoTo)
                {
                    if (!ExistsStatementForConstruct(traverseCandidate))
                    {
                        foreach (Statement statement in TraverseGoToOnlyReachableStatements(traverseCandidate))
                        {
                            body.AddStatement(statement);
                        }
                    }
                }
            } while (goToOnlyReeachableConstructsFound);

            foreach (CFGBlockLogicalConstruct gotoEndPointBlock in gotoEndpointToOrigins.Keys)
            {
                CFGBlockLogicalConstruct effectiveGotoEndPoint = GetEffectiveGotoEndPoint(gotoEndPointBlock);
                foreach (CFGBlockLogicalConstruct gotoOriginBlock in gotoEndpointToOrigins[gotoEndPointBlock])
                {
                    //finding the topmost goto endpoint in the parents hierarchy that ends at the same CFG block
                    ILogicalConstruct gotoEndPointConstruct = FindGoToEndpointConstruct(effectiveGotoEndPoint, gotoOriginBlock);

                    if (!gotoEndpointConstructToLabel.ContainsKey(gotoEndPointConstruct))
                    {
                        string label = GenerateGoToLabel();
                        AddAndRegisterGoToLabel(gotoEndPointConstruct, label);
                        gotoEndpointConstructToLabel[gotoEndPointConstruct] = label;
                    }

                    gotoOriginBlockToGoToStatement[gotoOriginBlock][gotoEndPointBlock].TargetLabel = gotoEndpointConstructToLabel[gotoEndPointConstruct];
                }
            }

            return body;
        }

        private CFGBlockLogicalConstruct GetEffectiveGotoEndPoint(CFGBlockLogicalConstruct gotoEndPointBlock)
        {
            CFGBlockLogicalConstruct effectiveGotoEndPoint = gotoEndPointBlock;
            List<Statement> constructStatements;
            while (logicalConstructToStatements.TryGetValue(effectiveGotoEndPoint, out constructStatements) && constructStatements.Count == 0)
            {
                IEnumerator<CFGBlockLogicalConstruct> enumerator = effectiveGotoEndPoint.CFGSuccessors.GetEnumerator();
                using (enumerator)
                {
                    if (!enumerator.MoveNext())
                    {
                        throw new Exception("End block with no statements reached.");
                    }

                    effectiveGotoEndPoint = enumerator.Current;

                    if (enumerator.MoveNext())
                    {
                        throw new Exception("No statements generated for multi exit block");
                    }
                }
            }

            return effectiveGotoEndPoint;
        }

        private bool ExistsStatementForConstruct(ILogicalConstruct theConstruct)
        {
            if (theConstruct is ConditionLogicalConstruct &&
                (theConstruct as ConditionLogicalConstruct).LogicalContainer != null)
            {
                return logicalConstructToStatements.ContainsKey((theConstruct as ConditionLogicalConstruct).LogicalContainer);
            }
            else
            {
                return logicalConstructToStatements.ContainsKey(theConstruct);
            }
        }

        private ILogicalConstruct FindGoToEndpointConstruct(CFGBlockLogicalConstruct gotoEndPoint, CFGBlockLogicalConstruct gotoOrigin)
        {
            //we might have a block that jumps to itself
            if (gotoEndPoint == gotoOrigin)
            {
                //the condition might be a start of an if or a loop. in that case the goto should be directed to its parent.
                if (gotoEndPoint.Parent is ConditionLogicalConstruct)
                {
                    if (gotoEndPoint.Parent.Parent is LoopLogicalConstruct ||
                        gotoEndPoint.Parent.Parent is IfLogicalConstruct)
                    {
                        return (ILogicalConstruct)gotoEndPoint.Parent.Parent;
                    }
                }
                return (ILogicalConstruct)gotoEndPoint.Parent;
            }

            ILogicalConstruct commonParent = (ILogicalConstruct)LogicalFlowUtilities.FindFirstCommonParent(new ISingleEntrySubGraph[] { gotoEndPoint, gotoOrigin });
            ILogicalConstruct result = gotoEndPoint;
            while (result.Parent != commonParent)
            {
                result = (ILogicalConstruct)result.Parent;
            }

            //sanity check
            if (result.FirstBlock != gotoEndPoint)
            {
                throw new Exception("GoTo misplaced.");
            }

            if (result is CaseLogicalConstruct)
            {
                //Corner case for a goto from case to case in a switch construct.
                result = result.Entry as ILogicalConstruct;
            }
            else if (result is CFGBlockLogicalConstruct && result.Parent is SwitchLogicalConstruct)
            {
                //Corner case for a goto from case to the entry of the switch.
                result = result.Parent as ILogicalConstruct;
            }

            return result;
        }

        private ILogicalConstruct FindTopMostParentOfBlock(CFGBlockLogicalConstruct gotoEndPoint)
        {
            //finding the topmost goto endpoint in the parents hierarchy that ends at the same CFG block
            ILogicalConstruct topMostParent = gotoEndPoint;
            while (topMostParent.Parent != null &&
                   ((ILogicalConstruct)topMostParent.Parent).FirstBlock == gotoEndPoint)
            {
                topMostParent = (ILogicalConstruct)topMostParent.Parent;
            }

            if (topMostParent is BlockLogicalConstruct)
            {
                topMostParent = (ILogicalConstruct)topMostParent.Entry;
            }
            return topMostParent;
        }

        private List<Statement> TraverseGoToOnlyReachableStatements(ILogicalConstruct start)
        {
            List<Statement> result = new List<Statement>();
            ILogicalConstruct current = start;
            do
            {
                result.AddRange(ProcessLogicalConstruct(current, true));
                if (current.FollowNode != null && ExistsStatementForConstruct(current.FollowNode)) //going back to already traversed part of the code. GoTo needed
                {
                    break;
                }
                current = current.FollowNode;
            } while (current != null);

            return result;
        }

        private List<Statement> ProcessLogicalConstruct(ILogicalConstruct theConstruct, bool gotoReachableConstruct)
        {
            if (logicalConstructToStatements.ContainsKey(theConstruct))
            {
                return logicalConstructToStatements[theConstruct];
            }
            List<Statement> results = new List<Statement>();

            // finding the goto successors
            FindAndMarkGoToExits(theConstruct, gotoReachableConstruct);

            //needs to be here. a lot of stuff relies on that
            parents.Push(theConstruct);

            if (theConstruct is BlockLogicalConstruct)
            {
                results.Add(ProcessBlockLogicalConstruct(theConstruct, gotoReachableConstruct));
            }
            else
            {
                LoopLogicalConstruct closestLoopParent = GetInnerMostParentOfType<LoopLogicalConstruct>(theConstruct);
                bool closestLoopParentExists = closestLoopParent != null && ExistsStatementForConstruct(closestLoopParent);
                IBreaksContainer closestBreakContainerParent = GetInnerMostParentOfType<IBreaksContainer>(theConstruct);
                bool closestBreakContainerExists = closestBreakContainerParent != null && ExistsStatementForConstruct(closestBreakContainerParent);

                if (theConstruct is ExceptionHandlingLogicalConstruct)
                {
                    results.Add(ProcessExceptionHandlingLogicalConstruct(theConstruct as ExceptionHandlingLogicalConstruct, gotoReachableConstruct));
                }
                else if (theConstruct is IfLogicalConstruct)
                {
                    results.Add(ProcessIfLogicalConstruct(theConstruct as IfLogicalConstruct, gotoReachableConstruct));
                }
                else if (theConstruct is LoopLogicalConstruct)
                {
                    ProcessLoopLogicalConstruct(theConstruct as LoopLogicalConstruct, gotoReachableConstruct, results);
                }
                else if (theConstruct is SwitchLogicalConstruct)
                {
                    results.Add(ProcessSwitchLogicalConstruct(theConstruct as SwitchLogicalConstruct, gotoReachableConstruct));
                }
                else if (theConstruct is CFGBlockLogicalConstruct) // handles PartialCFGBlockLogicalConstruct too
                {
                    ProcessCfgBlockLogicalConstruct(theConstruct as CFGBlockLogicalConstruct, results, closestBreakContainerExists, closestLoopParentExists);
                }
                else if (theConstruct is ConditionLogicalConstruct)
                {
                    results.Add(ProcessConditionLogicalConstruct(theConstruct as ConditionLogicalConstruct, closestBreakContainerExists,
                                        closestBreakContainerParent, closestLoopParentExists));
                }
            }
            parents.Pop();
            logicalConstructToStatements.Add(theConstruct, results);
            foreach (Statement statement in results)
            {
                statementToLogicalConstruct[statement] = theConstruct;
            }
            return results;
        }

        #region Process logical constructs

        private IfStatement ProcessConditionLogicalConstruct(ConditionLogicalConstruct clc, bool closestBreakContainerExists, IBreaksContainer closestBreakContainerParent, bool closestLoopParentExists)
        {
            BlockStatement theThenBlock = new BlockStatement();
            BlockStatement theElseBlock = new BlockStatement();
            if (!closestBreakContainerExists && CheckForBreak(clc))
            {
                CFGBlockLogicalConstruct breakEndPoint = breaksContainerToBreakEndPoint[closestBreakContainerParent];
                List<Instruction> jumps = GetJumpingInstructions(clc, breakEndPoint);
                if (clc.TrueCFGSuccessor == breakEndPoint)
                {
                    // There are possibly many jumps, that target the true successor. Think of a good way to represent them all.
                    theThenBlock.AddStatement(new BreakStatement(jumps));
                }
                else if (clc.FalseCFGSuccessor == breakEndPoint)
                {
                    theElseBlock.AddStatement(new BreakStatement(jumps));
                }
                else //sanity check
                {
                    throw new Exception("Incorrect mark as break child!");
                }
            }

            if (!closestLoopParentExists && CheckForContinue(clc))
            {
                CFGBlockLogicalConstruct loopCFGEntry = GetInnerMostParentOfType<LoopLogicalConstruct>().LoopContinueEndPoint;

                if (clc.TrueCFGSuccessor == loopCFGEntry && theThenBlock.Statements.Count == 0)
                {
                    theThenBlock.AddStatement(new ContinueStatement(GetJumpingInstructions(clc, loopCFGEntry)));
                }
                else if (clc.FalseCFGSuccessor == loopCFGEntry && theElseBlock.Statements.Count == 0)
                {
                    theElseBlock.AddStatement(new ContinueStatement(GetJumpingInstructions(clc, loopCFGEntry)));
                }
                else //sanity check
                {
                    throw new Exception("Incorrect mark as continue child!");
                }
            }

            bool gotoFromTrue = gotoEndpointToOrigins.ContainsKey(clc.TrueCFGSuccessor) &&
                                TryCreateGoTosForConditinalConstructSuccessor(theThenBlock, clc, clc.TrueCFGSuccessor);

            bool gotoFromFalse = gotoEndpointToOrigins.ContainsKey(clc.FalseCFGSuccessor) &&
                                 TryCreateGoTosForConditinalConstructSuccessor(theElseBlock, clc, clc.FalseCFGSuccessor);

            if (!gotoFromTrue && !gotoFromFalse && theThenBlock.Statements.Count == 0 && theElseBlock.Statements.Count == 0)
            {
                throw new Exception("Orphaned condition not properly marked as goto!");
            }

            if (theThenBlock.Statements.Count == 0)
            {
                BlockStatement swapHolder = theThenBlock;
                theThenBlock = theElseBlock;
                theElseBlock = swapHolder;

                clc.Negate(typeSystem);
            }

            if (theElseBlock.Statements.Count == 0)
            {
                theElseBlock = null;
            }

            IfStatement theIf = new IfStatement(clc.ConditionExpression, theThenBlock, theElseBlock);
            return theIf;
        }

        private List<Instruction> GetJumpingInstructions(ILogicalConstruct from, CFGBlockLogicalConstruct to)
        {
            //TODO: Handle corner cases better
            if (!(from is CFGBlockLogicalConstruct) || from.Parent is ConditionLogicalConstruct ||
                !IsUnconditionalJump((from as CFGBlockLogicalConstruct).TheBlock.Last))
            {
                return null;
            }

            List<Instruction> result = new List<Instruction>();
            foreach (CFGBlockLogicalConstruct block in from.CFGBlocks)
            {
                if (block.CFGSuccessors.Contains(to))
                {
                    Instruction i = block.TheBlock.Last;
                    result.Add(i);
                }
            }
            return result;
        }

        private void ProcessCfgBlockLogicalConstruct(CFGBlockLogicalConstruct theBlock, List<Statement> results, bool closestBreakContainerExists, bool closestLoopParentExists)
        {
            foreach (Expression expr in theBlock.LogicalConstructExpressions)
            {
                results.Add(new ExpressionStatement(expr));
            }

            if (!closestBreakContainerExists && CheckForBreak(theBlock))
            {
                Instruction[] jumps = IsUnconditionalJump(theBlock.TheBlock.Last) ? new Instruction[] { theBlock.TheBlock.Last } : null;
                results.Add(new BreakStatement(jumps));
            }
            else if (!closestLoopParentExists && CheckForContinue(theBlock))
            {
                Instruction[] jumps = IsUnconditionalJump(theBlock.TheBlock.Last) ? new Instruction[] { theBlock.TheBlock.Last } : null;
                results.Add(new ContinueStatement(jumps));
            }
            else if (gotoOriginsToEndpoints.ContainsKey(theBlock))
            {
                AppendGoToStartingAt(theBlock, results);
            }

            if (results.Count == 0 && gotoEndpointToOrigins.ContainsKey(theBlock))
            {
                results.Add(new EmptyStatement());
            }
        }

        private bool IsUnconditionalJump(Instruction instruction)
        {
            Code code = instruction.OpCode.Code;
            return code == Code.Br || code == Code.Br_S;
        }

        private SwitchStatement ProcessSwitchLogicalConstruct(SwitchLogicalConstruct theLogicalSwitch, bool gotoReachableConstruct)
        {
            SwitchStatement theSwitchStatement = new SwitchStatement(theLogicalSwitch.SwitchConditionExpression,
                        (theLogicalSwitch.Entry as ILogicalConstruct).FirstBlock.TheBlock.Last);

            Dictionary<CFGBlockLogicalConstruct, GotoStatement> endpointToStatement = new Dictionary<CFGBlockLogicalConstruct, GotoStatement>();
            gotoOriginBlockToGoToStatement.Add(theLogicalSwitch.FirstBlock, endpointToStatement);

            for (int i = 0, j = 0; i != theLogicalSwitch.ConditionCases.Length || j != theLogicalSwitch.NonDominatedCFGSuccessors.Count; )
            {
                List<int> caseNumbers;
                CaseLogicalConstruct theCaseConstruct = null;
                CFGBlockLogicalConstruct cfgSuccessor = null;
                if (i < theLogicalSwitch.ConditionCases.Length && j < theLogicalSwitch.NonDominatedCFGSuccessors.Count)
                {
                    if (theLogicalSwitch.ConditionCases[i].CaseNumbers[0] < theLogicalSwitch.NonDominatedCFGSuccessors[j].Key[0])
                    {
                        theCaseConstruct = theLogicalSwitch.ConditionCases[i];
                        caseNumbers = theCaseConstruct.CaseNumbers;
                        i++;
                    }
                    else
                    {
                        caseNumbers = theLogicalSwitch.NonDominatedCFGSuccessors[j].Key;
                        cfgSuccessor = theLogicalSwitch.NonDominatedCFGSuccessors[j].Value;
                        j++;
                    }
                }
                else if (i == theLogicalSwitch.ConditionCases.Length)
                {
                    caseNumbers = theLogicalSwitch.NonDominatedCFGSuccessors[j].Key;
                    cfgSuccessor = theLogicalSwitch.NonDominatedCFGSuccessors[j].Value;
                    j++;
                }
                else
                {
                    theCaseConstruct = theLogicalSwitch.ConditionCases[i];
                    caseNumbers = theCaseConstruct.CaseNumbers;
                    i++;
                }

                for (int k = 0; k < caseNumbers.Count - 1; k++)
                {
                    int caseIndex = caseNumbers[k];
                    theSwitchStatement.AddCase(new ConditionCase() { Condition = new LiteralExpression(caseIndex, typeSystem, null) });
                }

                int lastIndex = caseNumbers[caseNumbers.Count - 1];
                LiteralExpression condition = new LiteralExpression(lastIndex, typeSystem, null);
                condition.ExpressionType = this.context.MethodContext.Method.Module.TypeSystem.Int32;
                BlockStatement caseBody;

                if (theCaseConstruct != null)
                {
                    caseBody = (BlockStatement)ProcessLogicalConstruct(theCaseConstruct, gotoReachableConstruct)[0];
                }
                else
                {
                    caseBody = new BlockStatement();
                    HashSet<CFGBlockLogicalConstruct> gotoOrigins;
                    CFGBlockLogicalConstruct continueEndpoint;
                    if (continuesOriginToEndPoint.TryGetValue(theLogicalSwitch.FirstBlock, out continueEndpoint) && continueEndpoint == cfgSuccessor)
                    {
                        caseBody.AddStatement(new ContinueStatement(null));
                    }
                    else if (gotoEndpointToOrigins.TryGetValue(cfgSuccessor, out gotoOrigins) && gotoOrigins.Contains(theLogicalSwitch.FirstBlock))
                    {
                        GotoStatement theGoto = ConstructAndRecordGoToStatementToContext("", GetJumpingInstructions(theLogicalSwitch.FirstBlock, cfgSuccessor));
                        endpointToStatement.Add(cfgSuccessor, theGoto);

                        caseBody.AddStatement(theGoto);
                    }
                    else
                    {
                        caseBody.AddStatement(new BreakStatement(null));
                    }
                }

                if (SwitchHelpers.BlockHasFallThroughSemantics(caseBody))
                {
                    caseBody.AddStatement(new BreakSwitchCaseStatement());
                }
                theSwitchStatement.AddCase(new ConditionCase(condition, caseBody));
            }

            if (theLogicalSwitch.DefaultCase != null)
            {
                BlockStatement defaultCaseBody = (BlockStatement)ProcessLogicalConstruct(theLogicalSwitch.DefaultCase, gotoReachableConstruct)[0];

                if (gotoEndpointToOrigins.ContainsKey(theLogicalSwitch.DefaultCase.FirstBlock) || defaultCaseBody.Statements.Count > 1 || defaultCaseBody.Statements.Count == 1 &&
                    defaultCaseBody.Statements[0].CodeNodeType != CodeNodeType.BreakStatement)
                {
                    if (SwitchHelpers.BlockHasFallThroughSemantics(defaultCaseBody))
                    {
                        defaultCaseBody.AddStatement(new BreakSwitchCaseStatement());
                    }
                    theSwitchStatement.AddCase(new DefaultCase(defaultCaseBody));
                }
            }
            else
            {
                BlockStatement defaultCaseBody = new BlockStatement();
                HashSet<CFGBlockLogicalConstruct> gotoOrigins;
                CFGBlockLogicalConstruct continueEndpoint;
                if (continuesOriginToEndPoint.TryGetValue(theLogicalSwitch.FirstBlock, out continueEndpoint) && continueEndpoint == theLogicalSwitch.DefaultCFGSuccessor)
                {
                    defaultCaseBody.AddStatement(new ContinueStatement(null));
                }
                else if (gotoEndpointToOrigins.TryGetValue(theLogicalSwitch.DefaultCFGSuccessor, out gotoOrigins) && gotoOrigins.Contains(theLogicalSwitch.FirstBlock))
                {
                    GotoStatement theGoto = ConstructAndRecordGoToStatementToContext("", GetJumpingInstructions(theLogicalSwitch.FirstBlock, theLogicalSwitch.DefaultCFGSuccessor));
                    endpointToStatement.Add(theLogicalSwitch.DefaultCFGSuccessor, theGoto);

                    defaultCaseBody.AddStatement(theGoto);
                }

                if (defaultCaseBody.Statements.Count > 0)
                {
                    theSwitchStatement.AddCase(new DefaultCase(defaultCaseBody));
                }
            }
            return theSwitchStatement;
        }
        
        private void ProcessLoopLogicalConstruct(LoopLogicalConstruct theLogicalLoop, bool gotoReachableConstruct, List<Statement> results)
        {
            //LoopLogicalConstruct theLogicalLoop = theConstruct as LoopLogicalConstruct;
            BlockStatement loopBody;
            if (theLogicalLoop.LoopBodyBlock != null) // the loop might be empty, i.e. it might contain no statements
            {
                loopBody = (BlockStatement)ProcessLogicalConstruct(theLogicalLoop.LoopBodyBlock, gotoReachableConstruct)[0];
            }
            else
            {
                loopBody = new BlockStatement();
            }
            Expression theLoopCondition;
            if (theLogicalLoop.LoopCondition != null)
            {
                theLoopCondition = theLogicalLoop.LoopCondition.ConditionExpression;
            }
            else //infinite loop
            {
                theLoopCondition = new LiteralExpression(true, typeSystem, null);
            }
            if (theLogicalLoop.LoopType == LoopType.PreTestedLoop ||
                theLogicalLoop.LoopType == LoopType.InfiniteLoop)
            {
                results.Add(new WhileStatement(theLoopCondition, loopBody));
            }
            else if (theLogicalLoop.LoopType == LoopType.PostTestedLoop)
            {
                results.Add(new DoWhileStatement(theLoopCondition, loopBody));
            }

            if (theLogicalLoop.LoopCondition != null)
            {
                List<CFGBlockLogicalConstruct> goToStartPointHosts = HostsGoToStartPoint(theLogicalLoop.LoopCondition);
                if (goToStartPointHosts.Count != 0)
                {
                    AppendGoToFromConditionStartingAt(goToStartPointHosts, theLogicalLoop.LoopCondition.FalseCFGSuccessor, results);
                }
            }
        }

        private IfStatement ProcessIfLogicalConstruct(IfLogicalConstruct theIf, bool gotoReachableConstruct)
        {
            BlockStatement then = (BlockStatement)ProcessLogicalConstruct(theIf.Then, gotoReachableConstruct)[0];
            BlockStatement @else = null;
            if (theIf.Else != null)
            {
                @else = (BlockStatement)ProcessLogicalConstruct(theIf.Else, gotoReachableConstruct)[0];
            }

            if (@else != null && @else.Statements.Count == 0)
            {
                //happens in some tests in debug mode
                //where the else block contains only nop and br.s instructions
                //example -> MethodCalls.CheckingAResultFromAMethod.Debug
                @else = null;
            }

            if (@else != null && then.Statements.Count == 0)
            {
                theIf.Negate(typeSystem);
                then = @else;
                @else = null;
            }

            IfStatement theIfStatement = new IfStatement(theIf.Condition.ConditionExpression, then, @else);

            List<CFGBlockLogicalConstruct> goToStartPointHosts = HostsGoToStartPoint(theIf.Condition);
            if (goToStartPointHosts.Count != 0)
            {
                if (theIf.Else != null)
                {
                    throw new Exception("Malformed IF statement.");
                }
                else
                {
                    List<Statement> elseBody = new List<Statement>();
                    AppendGoToFromConditionStartingAt(goToStartPointHosts, theIf.Condition.FalseCFGSuccessor, elseBody);

                    theIfStatement.Else = new BlockStatement();
                    theIfStatement.Else.AddStatement(elseBody[0]);
                }
            }

            return theIfStatement;
        }

        private TryStatement ProcessExceptionHandlingLogicalConstruct(ExceptionHandlingLogicalConstruct exceptionHandlingConstruct, bool gotoReachableConstruct)
        {
            BlockStatement @try = (BlockStatement)ProcessLogicalConstruct(exceptionHandlingConstruct.Try, gotoReachableConstruct)[0];
            if (exceptionHandlingConstruct is TryFinallyLogicalConstruct)
            {
                TryFinallyLogicalConstruct tryFinallyConstruct = exceptionHandlingConstruct as TryFinallyLogicalConstruct;
                FinallyClause @finally = new FinallyClause((BlockStatement)ProcessLogicalConstruct(tryFinallyConstruct.Finally, gotoReachableConstruct)[0]);

                return new TryStatement(@try, null, @finally);
            }
            else if (exceptionHandlingConstruct is TryFaultLogicalConstruct)
            {
                TryFaultLogicalConstruct tryFaultConstruct = exceptionHandlingConstruct as TryFaultLogicalConstruct;
                BlockStatement fault = (BlockStatement)ProcessLogicalConstruct(tryFaultConstruct.Fault, gotoReachableConstruct)[0];

                return new TryStatement(@try, fault, null);
            }
            else //TryCatchFilterLogicalConstruct
            {
                TryCatchFilterLogicalConstruct tryCatchFilterConstruct = exceptionHandlingConstruct as TryCatchFilterLogicalConstruct;
                TryStatement theTry = new TryStatement() { Try = @try };

                foreach (IFilteringExceptionHandler handler in tryCatchFilterConstruct.Handlers)
                {
                    if (handler is ExceptionHandlingBlockCatch)
                    {
                        ExceptionHandlingBlockCatch catchHandler = handler as ExceptionHandlingBlockCatch;
                        BlockStatement catchBody = (BlockStatement)ProcessLogicalConstruct(catchHandler, gotoReachableConstruct)[0];

                        VariableDefinition varDef;
                        bool successfulGet = context.MethodContext.StackData.ExceptionHandlerStartToExceptionVariableMap.TryGetValue(catchHandler.FirstBlock.TheBlock.First.Offset,
                            out varDef);
                        VariableDeclarationExpression variableDeclaration = successfulGet ? new VariableDeclarationExpression(varDef, null) : null;

                        theTry.AddToCatchClauses(new CatchClause(catchBody, catchHandler.CatchType, variableDeclaration));
                    }
                    else //ExceptionHandlingBlockFilter
                    {
                        ExceptionHandlingBlockFilter filterHandler = handler as ExceptionHandlingBlockFilter;
                        BlockStatement filterBody = (BlockStatement)ProcessLogicalConstruct(filterHandler.Filter, gotoReachableConstruct)[0];
                        BlockStatement handlerBody = (BlockStatement)ProcessLogicalConstruct(filterHandler.Handler, gotoReachableConstruct)[0];

                        theTry.AddToCatchClauses(new CatchClause(handlerBody, null, null, filterBody));
                    }
                }

                return theTry;
            }
        }

        private BlockStatement ProcessBlockLogicalConstruct(ILogicalConstruct theConstruct, bool gotoReachableConstruct)
        {
            BlockStatement result = new BlockStatement();
            ILogicalConstruct current = (ILogicalConstruct)theConstruct.Entry;
            while (current != null)
            {
                foreach (Statement statement in ProcessLogicalConstruct(current, gotoReachableConstruct))
                {
                    result.AddStatement(statement);
                }

                current = current.FollowNode;
            }
            return result;
        }

        #endregion

        private bool TryCreateGoTosForConditinalConstructSuccessor(BlockStatement theBlock, ConditionLogicalConstruct clc, CFGBlockLogicalConstruct theSuccessor)
        {
            GotoStatement theGoTo = null;

            bool foundGoToStart = false;
            //Hooking up the goto statement to all the CFG blocks in the condition that jump to the goto endpoint.
            //When we generate the goto endpoints later we'll set up the label of the goto statement respectively
            foreach (CFGBlockLogicalConstruct goToEndpointPredecessor in theSuccessor.CFGPredecessors)
            {
                if (clc.CFGBlocks.Contains(goToEndpointPredecessor) && gotoEndpointToOrigins[theSuccessor].Contains(goToEndpointPredecessor))
                {
                    if (theGoTo == null)
                    {
                        theGoTo = ConstructAndRecordGoToStatementToContext("", GetJumpingInstructions(goToEndpointPredecessor, theSuccessor));
                        theBlock.AddStatement(theGoTo);
                    }

                    if (!gotoOriginBlockToGoToStatement.ContainsKey(goToEndpointPredecessor))
                    {
                        gotoOriginBlockToGoToStatement.Add(goToEndpointPredecessor, new Dictionary<CFGBlockLogicalConstruct, GotoStatement>());
                    }
                    gotoOriginBlockToGoToStatement[goToEndpointPredecessor].Add(theSuccessor, theGoTo);
                    foundGoToStart = true;
                }
            }

            return foundGoToStart;
        }

        private void AppendGoToStartingAt(CFGBlockLogicalConstruct goToStartPointHost, List<Statement> results)
        {
            CFGBlockLogicalConstruct theGoToEndpoint = gotoOriginsToEndpoints[goToStartPointHost].FirstOrDefault<CFGBlockLogicalConstruct>();

            GotoStatement theGoTo = ConstructAndRecordGoToStatementToContext("", GetJumpingInstructions(goToStartPointHost, theGoToEndpoint));

            Dictionary<CFGBlockLogicalConstruct, GotoStatement> endpointToStatement = new Dictionary<CFGBlockLogicalConstruct, GotoStatement>();
            endpointToStatement.Add(theGoToEndpoint, theGoTo);

            gotoOriginBlockToGoToStatement.Add(goToStartPointHost, endpointToStatement);
            results.Add(theGoTo);
        }

        private List<CFGBlockLogicalConstruct> HostsGoToStartPoint(ConditionLogicalConstruct condition)
        {
            List<CFGBlockLogicalConstruct> gotoStartPoints = new List<CFGBlockLogicalConstruct>();
            foreach (CFGBlockLogicalConstruct conditionCFGBlock in condition.CFGBlocks)
            {
                if (gotoOriginsToEndpoints.ContainsKey(conditionCFGBlock))
                {
                    gotoStartPoints.Add(conditionCFGBlock);
                }
            }

            return gotoStartPoints;
        }

        private void AppendGoToFromConditionStartingAt(List<CFGBlockLogicalConstruct> goToStartPointHosts, CFGBlockLogicalConstruct theSuccessor,
            List<Statement> results)
        {
            AppendGoToStartingAt(goToStartPointHosts[0], results);
            GotoStatement theGoto = gotoOriginBlockToGoToStatement[goToStartPointHosts[0]][theSuccessor];
            for (int i = 1; i < goToStartPointHosts.Count; i++)
            {
                if (!gotoOriginBlockToGoToStatement.ContainsKey(goToStartPointHosts[i]))
                {
                    gotoOriginBlockToGoToStatement[goToStartPointHosts[i]] = new Dictionary<CFGBlockLogicalConstruct, GotoStatement>();
                }
                gotoOriginBlockToGoToStatement[goToStartPointHosts[i]].Add(theSuccessor, theGoto);
            }
        }

        #region ConditionalStatement
        private bool CheckForContinue(ILogicalConstruct theConstruct)
        {
            if (theConstruct is CFGBlockLogicalConstruct)
            {
                if (continuesOriginToEndPoint.ContainsKey(theConstruct as CFGBlockLogicalConstruct))
                {
                    return true;
                }
            }
            else if (theConstruct is ConditionLogicalConstruct)
            {
                foreach (CFGBlockLogicalConstruct child in theConstruct.Children)
                {
                    if (continuesOriginToEndPoint.ContainsKey(child))
                    {
                        return true;
                    }
                }
            }
            else
            {
                throw new Exception("The given construct cannot be continue.");
            }

            return false;
        }

        private T GetInnerMostParentOfType<T>() where T : class
        {
            ILogicalConstruct[] parentsSoFar = parents.ToArray();
            foreach (ILogicalConstruct parent in parentsSoFar)
            {
                if (parent is T)
                {
                    return parent as T;
                }
            }

            return null;
        }

        private T GetInnerMostParentOfType<T>(ILogicalConstruct startNode) where T : class
        {
            ILogicalConstruct currentParent = startNode.Parent as ILogicalConstruct;
            while (currentParent != null)
            {
                if (currentParent is T)
                {
                    return currentParent as T;
                }

                currentParent = currentParent.Parent as ILogicalConstruct;
            }

            return null;
        }

        private bool CheckForBreak(ILogicalConstruct theConstruct)
        {
            if (theConstruct is CFGBlockLogicalConstruct)
            {
                if (breaksOriginToEndPoint.ContainsKey(theConstruct as CFGBlockLogicalConstruct))
                {
                    return true;
                }
            }
            else if (theConstruct is ConditionLogicalConstruct)
            {
                foreach (CFGBlockLogicalConstruct child in theConstruct.Children)
                {
                    if (breaksOriginToEndPoint.ContainsKey(child))
                    {
                        return true;
                    }
                }
            }
            else
            {
                throw new Exception("The given construct cannot be break.");
            }

            return false;
        }
        #endregion

        #region Block

        private void FindAndMarkGoToExits(ILogicalConstruct current, bool gotoReachableConstruct)
        {
            if (current is IBreaksContainer)
            {
                DetermineBreaksEndPoint((IBreaksContainer)current);
            }

            if (current is CFGBlockLogicalConstruct)
            {
                FindBreak(current as CFGBlockLogicalConstruct);
                FindContinue(current as CFGBlockLogicalConstruct);
            }

            // finding the goto successors
            foreach (ILogicalConstruct successor in current.AllSuccessors)
            {
                CFGBlockLogicalConstruct nearestFollowBlock = GetNearestFollowNode(current);
                if (successor.FirstBlock != nearestFollowBlock)
                {
                    MarkGoTosIfNotLoopEdge(current, successor);
                }
                else
                {
                    /// Handling the case where the follow node is not the false successor of the condition of a loop.
                    if (current is LoopLogicalConstruct && (current as LoopLogicalConstruct).LoopType != LoopType.InfiniteLoop &&
                        (current as LoopLogicalConstruct).LoopCondition.FalseCFGSuccessor != nearestFollowBlock.FirstBlock)
                    {
                        MarkGoTosIfNotLoopEdge(current, successor);
                    }
                    /// Handling the case where gotoReachable only constructs have as a follow node a part of the code that was traversed 
                    /// following the regular path, i.e. starting from the input block entry and following the follow nodes.
                    /// Since this is a goto reachable only part of the code it might be located anywhere 
                    /// (normally it would be located after the return of the method).
                    /// Hence any jump from it to the "regular" workflow should be implemented as a goto.
                    else if (gotoReachableConstruct)
                    {
                        ILogicalConstruct nearestTopLevelFollowingContruct = nearestFollowBlock;
                        while (nearestTopLevelFollowingContruct.Parent != null &&
                                ((ILogicalConstruct)nearestTopLevelFollowingContruct.Parent).FirstBlock == nearestFollowBlock)
                        {
                            nearestTopLevelFollowingContruct = (ILogicalConstruct)nearestTopLevelFollowingContruct.Parent;
                        }

                        if (ExistsStatementForConstruct(nearestTopLevelFollowingContruct))
                        {
                            MarkGoTosIfNotLoopEdge(current, successor);
                        }
                    }

                    /// Else do nothing. Couldn't think of a way to have a goto from a NON_CYCLICAL construct to its follow node
                    /// i.e. any path from a construct to its follow node should be normal exit from teh construct.
                    /// E.g. a jump from inside a loop body to it's follow node will be a break 
                    /// (if that jump is nested inside some otehr construct that resides in the body, say if for example, 
                    /// then a goto will be generated since that if has a follow node different than the follow node of the loop)
                    /// If there is a jump to if's follow node inside its then or else blocks, then no instructions that come after that jump will be executed 
                    /// so it's gonna be the last instruction to be executed from the then/else block. So, it makes no sense puting a goto there 
                    /// since the control flow will be transferred to the follow node anyways. 
                    /// If that jump is nested inside some other construct in the then/else block same logic as for the loops applies.
                    /// The same reasoning is valid for switch cases, too.
                }
            }

            return;
        }

        private void MarkGoTosIfNotLoopEdge(ILogicalConstruct start, ILogicalConstruct end)
        {
            CFGBlockLogicalConstruct gotoEndpoint = end.FirstBlock;

            LoopLogicalConstruct closestLoopParent = GetInnerMostParentOfType<LoopLogicalConstruct>();
            HashSet<CFGBlockLogicalConstruct> startCFGConstructs = start.CFGBlocks;

            foreach (CFGBlockLogicalConstruct predecessor in gotoEndpoint.CFGPredecessors)
            {
                if (startCFGConstructs.Contains(predecessor))
                {
                    ILogicalConstruct predecessorLoopParent;
                    if (IsLoopCondition(predecessor))
                    {
                        predecessorLoopParent = GetInnerMostParentOfType<LoopLogicalConstruct>(predecessor.Parent.Parent as ILogicalConstruct);
                    }
                    else
                    {
                        predecessorLoopParent = GetInnerMostParentOfType<LoopLogicalConstruct>(predecessor);
                    }
                    ILogicalConstruct predecessorBreakContainer = GetInnerMostParentOfType<IBreaksContainer>(predecessor);

                    CFGBlockLogicalConstruct breakEdgeEnd = FindBreak(predecessor);
                    CFGBlockLogicalConstruct continueEdgeEnd = FindContinue(predecessor);
                    if (breakEdgeEnd == gotoEndpoint && !ExistsStatementForConstruct(predecessorBreakContainer) ||
                        continueEdgeEnd == gotoEndpoint && !ExistsStatementForConstruct(predecessorLoopParent))
                    {
                        continue;
                    }

                    if (closestLoopParent != null && predecessorLoopParent == closestLoopParent && !ExistsStatementForConstruct(closestLoopParent) &&
                        closestLoopParent.LoopContinueEndPoint == gotoEndpoint &&
                        CheckForNormalFlowReachability(predecessor, closestLoopParent))
                    {
                        continue;
                    }

                    HashSet<CFGBlockLogicalConstruct> endpointsFromGivenStart;
                    bool gotoEdgeAlreadyRecorded = (gotoOriginsToEndpoints.TryGetValue(predecessor, out endpointsFromGivenStart) && endpointsFromGivenStart.Contains(gotoEndpoint));
                    /// Might be already added by a parent construct it exits, i.e. if there is if(1){if(2){goto OUTSIDEIF1}} 
                    /// then if(1) will add the goto OUTSIDEIF1 before if(2) tries to.
                    /// We still have to check for goto exits in if2, however cause there might be a goto there to a point outside if2 
                    /// but inside if1 that if1 check won't catch.
                    if (!gotoEdgeAlreadyRecorded)
                    {
                        if (!gotoEndpointToOrigins.ContainsKey(gotoEndpoint))
                        {
                            gotoEndpointToOrigins.Add(gotoEndpoint, new HashSet<CFGBlockLogicalConstruct>());
                        }

                        if (!gotoEndpointToOrigins[gotoEndpoint].Contains(predecessor))
                        {
                            gotoEndpointToOrigins[gotoEndpoint].Add(predecessor);
                        }

                        if (!gotoOriginsToEndpoints.ContainsKey(predecessor))
                        {
                            gotoOriginsToEndpoints.Add(predecessor, new HashSet<CFGBlockLogicalConstruct>(new CFGBlockLogicalConstruct[] { gotoEndpoint }));
                        }
                        else
                        {
                            /// We have either n-way branch (IL switch), i.e. multiple branches from the same origin or 
                            /// we have a two-way branch with null follow node, i.e both of its successors will be generated by gotos.
                            gotoOriginsToEndpoints[predecessor].Add(gotoEndpoint);
                        }
                    }
                }
            }

            return;
        }

        private bool IsLoopCondition(CFGBlockLogicalConstruct cfgBlock)
        {
            return cfgBlock.Parent is ConditionLogicalConstruct &&
                (cfgBlock.Parent as ConditionLogicalConstruct).LogicalContainer as LoopLogicalConstruct != null;
        }

        private string GenerateGoToLabel()
        {
            return "Label" + gotoLabelsCounter++.ToString();
        }

        /// <summary>
        /// Gets the NEAREST NON-NULL follow node in the arguments parent chain.
        /// I.E. If one has:
        /// ......
        /// while
        /// {
        ///		while
        ///		{
        ///			if()
        ///			{
        ///				statment1
        ///			}
        ///		}
        /// }
        /// statment2
        /// ......
        ///  this method will return statement2 if statement1 gets passed as an argument. 
        ///  Used to decide whether goto is necessary (for the jump at the end of statement1 block)
        /// </summary>
        /// <returns></returns>
        private CFGBlockLogicalConstruct GetNearestFollowNode(ILogicalConstruct current)
        {
            if (current.CFGFollowNode != null)
            {
                return current.CFGFollowNode;
            }
            else
            {
                ILogicalConstruct[] parentsSoFar = parents.ToArray();

                foreach (ILogicalConstruct parent in parentsSoFar)
                {
                    if (parent.CFGFollowNode != null)
                    {
                        return parent.CFGFollowNode;
                    }
                }
            }

            return null;
        }
        #endregion

        private void AddAndRegisterGoToLabel(ILogicalConstruct gotoConstruct, string theLabel)
        {
            Statement labeledStatement = null;
            //loop conditions are processed as part of processing the looplogicalconstruct that contains them.
            //Hence, they won't have separate entries in logicalConstructToStatements collection but they still might serve as goto endpoint (see below)
            //if (!logicalConstructToStatements.ContainsKey(gotoConstruct))
            //{

            //we might have a jump to the loop condition from a construct that's a child of that loop but is
            //reachable only by goto, i.e. a construct that is a child of the loop body block but which won't be traversed
            //as part of the normal follownode order. if that's the case that jump won't be marked as continue (which it in fact is).
            if (gotoConstruct is ConditionLogicalConstruct &&
                (gotoConstruct as ConditionLogicalConstruct).LogicalContainer != null)
            {
                ConditionLogicalConstruct theCondition = gotoConstruct as ConditionLogicalConstruct;
                if (theCondition.LogicalContainer is LoopLogicalConstruct)
                {
                    LoopLogicalConstruct loopTarget = (LoopLogicalConstruct)gotoConstruct.Parent;

                    if (loopTarget.LoopType == LoopType.PostTestedLoop)
                    {
                        labeledStatement = new EmptyStatement();
                        DoWhileStatement theLoopStatement = (DoWhileStatement)logicalConstructToStatements[loopTarget][0];
                        theLoopStatement.Body.AddStatement(labeledStatement);
                    }
                    else if (loopTarget.LoopType == LoopType.PreTestedLoop)
                    {
                        labeledStatement = logicalConstructToStatements[loopTarget][0];
                    }
                    else
                    {
                        throw new Exception("Infinite loop with condition encountered.");
                    }
                }
                else if (theCondition.LogicalContainer is IfLogicalConstruct)
                {
                    labeledStatement = logicalConstructToStatements[theCondition.LogicalContainer][0];
                }
                else
                {
                    throw new Exception("Condition containing construct unaccounted for.");
                }
            }
            else if (gotoConstruct is CFGBlockLogicalConstruct && gotoConstruct.Parent is SwitchLogicalConstruct)
            {
                labeledStatement = logicalConstructToStatements[gotoConstruct.Parent as ILogicalConstruct][0];
            }
            else
            {
                labeledStatement = logicalConstructToStatements[gotoConstruct][0];
            }

            labeledStatement.Label = theLabel;
            contextGotoLabels.Add(labeledStatement.Label, labeledStatement);
        }

        private GotoStatement ConstructAndRecordGoToStatementToContext(string label, IEnumerable<Instruction> jumps)
        {
            GotoStatement theGoTo = new GotoStatement(label, jumps);
            contextGotoStatements.Add(theGoTo);
            return theGoTo;
        }


        private void DetermineBreaksEndPoint(IBreaksContainer breaksContainer)
        {
            CFGBlockLogicalConstruct breakEndPoint = GetBreakContainerCFGFollowNode(breaksContainer);
            if (breakEndPoint != null)
            {
                breaksContainerToBreakEndPoint[breaksContainer] = breakEndPoint;
            }
        }

        private CFGBlockLogicalConstruct FindBreak(CFGBlockLogicalConstruct startPoint)
        {
            if (breaksOriginToEndPoint.ContainsKey(startPoint))
            {
                return breaksOriginToEndPoint[startPoint];
            }

            IBreaksContainer breaksContainer = GetInnerMostParentOfType<IBreaksContainer>(startPoint);
            if (breaksContainer == null || ExistsStatementForConstruct(breaksContainer))
            {
                //This means that the breaksContainer is goto reachable from the start point, so there should not be a break from the start point.
                return null;
            }

            CFGBlockLogicalConstruct breakEdgeEndPoint;
            if (breaksContainerToBreakEndPoint.TryGetValue(breaksContainer, out breakEdgeEndPoint))
            {
                if (startPoint.CFGSuccessors.Contains(breakEdgeEndPoint))
                {
                    breaksOriginToEndPoint[startPoint] = breakEdgeEndPoint;
                    return breakEdgeEndPoint;
                }
            }

            return null;
        }

        private CFGBlockLogicalConstruct GetBreakContainerCFGFollowNode(IBreaksContainer breaksContainer)
        {
            ILogicalConstruct currentConstruct = breaksContainer;
            while (currentConstruct != null && currentConstruct.CFGFollowNode == null)
            {
                currentConstruct = currentConstruct.Parent as ILogicalConstruct;
                if (currentConstruct is IBreaksContainer)
                {
                    return null;
                }
            }

            if (currentConstruct == null || ExistsStatementForConstruct(currentConstruct.FollowNode))
            {
                return null;
            }

            if (breaksContainer is SwitchLogicalConstruct)
            {
                return currentConstruct.CFGFollowNode;
            }
            else
            {
                LoopLogicalConstruct loopConstruct = (LoopLogicalConstruct)breaksContainer;
                ILogicalConstruct childToCheckForExitEdge;
                if (loopConstruct.LoopType == LoopType.InfiniteLoop)
                {
                    childToCheckForExitEdge = loopConstruct.LoopBodyBlock;
                }
                else
                {
                    childToCheckForExitEdge = loopConstruct.LoopCondition;
                }

                if (childToCheckForExitEdge.CFGSuccessors.Contains(currentConstruct.CFGFollowNode))
                {
                    return currentConstruct.CFGFollowNode;
                }
                else
                {
                    return null;
                }
            }
        }

        private CFGBlockLogicalConstruct FindContinue(CFGBlockLogicalConstruct startPoint)
        {
            if (continuesOriginToEndPoint.ContainsKey(startPoint))
            {
                return continuesOriginToEndPoint[startPoint];
            }

            LoopLogicalConstruct loopConstruct = GetInnerMostParentOfType<LoopLogicalConstruct>(startPoint);
            if (loopConstruct == null || ExistsStatementForConstruct(loopConstruct))
            {
                return null;
            }

            if (startPoint.CFGSuccessors.Contains(loopConstruct.LoopContinueEndPoint))
            {
                if (!CheckForNormalFlowReachability(startPoint, loopConstruct))
                {
                    continuesOriginToEndPoint[startPoint] = loopConstruct.LoopContinueEndPoint;
                    return loopConstruct.LoopContinueEndPoint;
                }

                /// Check if the jump is from switch case. If it is, then in C# a break/contunue must be added, because the normal
                /// control flow of switch case is fall-through, whereas in VB it's leaving the switch. Consider creating separate dictionary 
                /// only for SwitchCaseBreaks to cover for this case.
                IBreaksContainer closestSwitch = GetInnerMostParentOfType<IBreaksContainer>(startPoint);
                if (closestSwitch != null)
                {
                    LoopLogicalConstruct closestSwitchLoop = GetInnerMostParentOfType<LoopLogicalConstruct>(closestSwitch);
                    if (closestSwitchLoop != null && closestSwitchLoop == loopConstruct)
                    {
                        continuesOriginToEndPoint[startPoint] = loopConstruct.LoopContinueEndPoint;
                        return loopConstruct.LoopContinueEndPoint;
                    }
                }
            }

            return null;
        }

        private bool CheckForNormalFlowReachability(ILogicalConstruct startPoint, ILogicalConstruct parentConstruct)
        {
            ILogicalConstruct currentConstruct = startPoint;
            while (currentConstruct != parentConstruct)
            {
                if (currentConstruct.CFGFollowNode != null)
                {
                    return false;
                }

                currentConstruct = currentConstruct.Parent as ILogicalConstruct;
            }

            return true;
        }
    }
}