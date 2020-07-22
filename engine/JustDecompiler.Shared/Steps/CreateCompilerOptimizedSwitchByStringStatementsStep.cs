using Mono.Cecil.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using System;
using System.Linq;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Steps
{
    class CreateCompilerOptimizedSwitchByStringStatementsStep : BaseCodeTransformer, IDecompilationStep
    {
        private CompilerOptimizedSwitchByStringData switchByStringData;

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.switchByStringData = context.MethodContext.SwitchByStringData;
            if (this.switchByStringData.SwitchBlocksStartInstructions.Count == 0)
            {
                return body;
            }

            return (BlockStatement)Visit(body);
        }

        public override ICodeNode VisitIfElseIfStatement(IfElseIfStatement node)
        {
            SwitchData data;
            if (IsSwitchByString(node) && TryGetSwitchData(node, out data))
            {
                return Visit(ComposeSwitch(data));
            }

            return base.VisitIfElseIfStatement(node);
        }

        private bool IsSwitchByString(IfElseIfStatement node)
        {
            foreach (KeyValuePair<int, List<int>> pair in this.switchByStringData.SwitchBlocksToCasesMap)
            {
                foreach (int caseOffset in pair.Value)
                {
                    if (node.SearchableUnderlyingSameMethodInstructionOffsets.Contains(caseOffset))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool TryGetSwitchData(IfElseIfStatement node, out SwitchData data)
        {
            data = new SwitchData();
            
            foreach (KeyValuePair<Expression, BlockStatement> pair in node.ConditionBlocks)
            {
                if (!TryMatchCondition(pair.Key, pair.Value, data))
                {
                    return false;
                }
            }

            if (node.Else != null)
            {
                data.DefaultCase = node.Else;
            }

            return true;
        }

        /// <summary>
        /// There are 2 types is conditions:
        /// - Simple condition - that's when the condition is composed from unary expression with "None" operator
        ///   containing a binary expression with the actual string comparison.
        /// - Complex condition - that's when there are 2 or more simple conditions OR'd in binary expressions.
        ///   If we have 3 or more conditions, the complex ones will be always at the left hand side, e.g.
        ///                         Binary
        ///             Binary---------|---------Unary
        ///     Binary-----|-----Unary
        /// Unary--|--Unary
        /// </summary>
        private bool TryMatchCondition(Expression condition, BlockStatement block, SwitchData data)
        {
            if (condition.CodeNodeType != CodeNodeType.UnaryExpression &&
                condition.CodeNodeType != CodeNodeType.BinaryExpression)
            {
                return false;
            }

            BinaryExpression binary;
            if (condition.CodeNodeType == CodeNodeType.BinaryExpression)
            {
                binary = condition as BinaryExpression;
                if (binary.Operator == BinaryOperator.LogicalOr)
                {
                    // Basically a DFS, which will result in traversing the unary expressions in left to right order.
                    if (TryMatchCondition(binary.Left, null, data) &&
                        TryMatchCondition(binary.Right, null, data))
                    {
                        // If this is the top BinaryExpression of the complex condition
                        if (block != null)
                        {
                            // We replace the last condition with one with the block for the complex condition.
                            int lastIndex = data.CaseConditionToBlockMap.Count - 1;
                            Expression lastCondition = data.CaseConditionToBlockMap[lastIndex].Key;
                            data.CaseConditionToBlockMap[lastIndex] = new KeyValuePair<Expression, BlockStatement>(lastCondition, block);
                        }

                        return true;
                    }

                    return false;
                }
            }
            else
            {
                UnaryExpression unary = condition as UnaryExpression;
                if (unary.Operator != UnaryOperator.None ||
                    unary.Operand.CodeNodeType != CodeNodeType.BinaryExpression)
                {
                    return false;
                }

                binary = unary.Operand as BinaryExpression;
            }

            if (binary.Right.CodeNodeType != CodeNodeType.LiteralExpression ||
                binary.Operator != BinaryOperator.ValueEquality)
            {
                return false;
            }

            LiteralExpression literal = binary.Right as LiteralExpression;
            if (condition.CodeNodeType == CodeNodeType.UnaryExpression &&
                literal.ExpressionType.FullName != "System.String")
            {
                return false;
            }
            else if (condition.CodeNodeType == CodeNodeType.BinaryExpression &&
                     (literal.ExpressionType.FullName != "System.Object" ||
                      literal.Value != null))
            {
                return false;
            }

            if (data.SwitchExpression == null)
            {
                data.SwitchExpression = binary.Left;
            }
            else if (!data.SwitchExpression.Equals(binary.Left))
            {
                return false;
            }
            else
            {
                data.SwitchExpressionLoadInstructions.Add(binary.Left.UnderlyingSameMethodInstructions.First().Offset);
            }

            data.CaseConditionToBlockMap.Add(new KeyValuePair<Expression, BlockStatement>(literal, block));

            return true;
        }
        
        private CompilerOptimizedSwitchByStringStatement ComposeSwitch(SwitchData data)
        {
            CompilerOptimizedSwitchByStringStatement @switch = new CompilerOptimizedSwitchByStringStatement(data.SwitchExpression, data.SwitchExpressionLoadInstructions);
            foreach (KeyValuePair<Expression, BlockStatement> pair in data.CaseConditionToBlockMap)
            {
                if (pair.Value != null && SwitchHelpers.BlockHasFallThroughSemantics(pair.Value))
                {
                    pair.Value.AddStatement(new BreakSwitchCaseStatement());
                }

                @switch.AddCase(new ConditionCase(pair.Key, pair.Value));
            }

            if (data.HaveDefaultCase)
            {
                if (SwitchHelpers.BlockHasFallThroughSemantics(data.DefaultCase))
                {
                    data.DefaultCase.AddStatement(new BreakSwitchCaseStatement());
                }

                @switch.AddCase(new DefaultCase(data.DefaultCase));
            }
            
            return @switch;
        }

        private class SwitchData
        {
            public SwitchData()
            {
                this.SwitchExpression = null;
                this.SwitchExpressionLoadInstructions = new List<int>();
                this.CaseConditionToBlockMap = new List<KeyValuePair<Expression, BlockStatement>>();
                this.DefaultCase = null;
            }

            public Expression SwitchExpression { get; set; }

            public List<int> SwitchExpressionLoadInstructions { get; set; }

            public List<KeyValuePair<Expression, BlockStatement>> CaseConditionToBlockMap { get; set; }

            public BlockStatement DefaultCase { get; set; }

            public bool HaveDefaultCase
            {
                get
                {
                    return this.DefaultCase != null;
                }
            }
        }
    }
}
