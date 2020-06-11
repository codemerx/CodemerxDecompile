using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
    public class LockStatement : BasePdbStatement
	{
		private BlockStatement body;
        private readonly List<Instruction> mappedFinallyInstructions;
        
        public LockStatement(Expression expression, BlockStatement body, IEnumerable<Instruction> finallyInstructions)
        {
			this.Expression = expression;
			this.body = body;
            this.mappedFinallyInstructions = new List<Instruction>();
            if (finallyInstructions != null)
            {
                this.mappedFinallyInstructions.AddRange(finallyInstructions);
                this.mappedFinallyInstructions.Sort((x, y) => x.Offset.CompareTo(y.Offset));
            }
        }

        protected override IEnumerable<Instruction> GetOwnInstructions()
        {
            foreach (Instruction instruction in this.mappedFinallyInstructions)
            {
                yield return instruction;
            }
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (this.Expression != null)
                {
                    yield return this.Expression;
                }
                if (this.body != null)
                {
                    yield return this.body;
                }
            }
        }

		public override Statement Clone()
		{
            BlockStatement clonedBody = null;
            if (body != null)
            {
                clonedBody = body.Clone() as BlockStatement;
            }
			LockStatement result = new LockStatement(Expression.Clone(), clonedBody, this.mappedFinallyInstructions);
			CopyParentAndLabel(result);
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            BlockStatement clonedBody = body != null ? body.CloneStatementOnly() as BlockStatement : null;
            LockStatement result = new LockStatement(Expression.CloneExpressionOnly(), clonedBody, null);
            CopyParentAndLabel(result);
            return result;
        }

		public Expression Expression { get; set; }

        public BlockStatement Body
        {
            get { return body; }
            set 
            { 
                this.body = value;
                if(this.body != null)
                {
                    this.body.Parent = this;
                }
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.LockStatement; }
        }
    }
}
