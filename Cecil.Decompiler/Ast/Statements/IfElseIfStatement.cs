using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
    public class IfElseIfStatement : BasePdbStatement
    {
        private List<KeyValuePair<Expression, BlockStatement>> conditionBlocks;
        private BlockStatement @else;

        public IfElseIfStatement(List<KeyValuePair<Expression, BlockStatement>> conditionBlocks, BlockStatement @else)
        {
            this.ConditionBlocks = conditionBlocks;
            this.Else = @else;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                foreach (KeyValuePair<Expression, BlockStatement> conditionBlock in ConditionBlocks)
                {
                    yield return conditionBlock.Key;
                    yield return conditionBlock.Value;
                }

                if (this.@else != null)
                {
                    yield return this.@else;
                }
            }
        }

		public List<KeyValuePair<Expression, BlockStatement>> ConditionBlocks
		{
            get
            {
                return conditionBlocks;
            }

            set
            {
                this.conditionBlocks = value;
                foreach (KeyValuePair<Expression, BlockStatement> pair in this.conditionBlocks)
                {
                    pair.Value.Parent = this;
                }
            }
        }

        public BlockStatement Else
        {
            get
            {
                return @else;
            }

            set
            {
                @else = value;
                if(@else != null)
                {
                    @else.Parent = this;
                }
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.IfElseIfStatement; }
        }

        public override Statement Clone()
        {
            return CloneStatement(true);
        }

        public override Statement CloneStatementOnly()
        {
            return CloneStatement(false);
        }

        private Statement CloneStatement(bool copyInstructions)
        {
            List<KeyValuePair<Expression, BlockStatement>> conditionBlocksClone = new List<KeyValuePair<Expression, BlockStatement>>();
            foreach (KeyValuePair<Expression, BlockStatement> pair in conditionBlocks)
            {
                Expression conditionClone = copyInstructions ? pair.Key.Clone() : pair.Key.CloneExpressionOnly();
                BlockStatement blockClone = copyInstructions ? (BlockStatement)pair.Value.Clone() : (BlockStatement)pair.Value.CloneStatementOnly();
                conditionBlocksClone.Add(new KeyValuePair<Expression, BlockStatement>(conditionClone, blockClone));
            }

            BlockStatement elseClone = null;
            if (@else != null)
            {
                elseClone = copyInstructions ? (BlockStatement)this.@else.Clone() : (BlockStatement)this.@else.CloneStatementOnly();
            }

            IfElseIfStatement result = new IfElseIfStatement(conditionBlocksClone, elseClone);
            CopyParentAndLabel(result);
            return result;
        }
    }
}
