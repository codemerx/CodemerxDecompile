using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
    public class FixedStatement : BasePdbStatement
	{
		private BlockStatement body;

        public FixedStatement(Expression expression, BlockStatement body)
        {
			this.Expression = expression;
			this.Body = body;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (this.body != null)
                {
                    yield return body;
                }
                if (this.Expression != null)
                {
                    yield return Expression;
                }
            }
        }

		public override Statement Clone()
		{
            BlockStatement clonedBody = body != null ? body.Clone() as BlockStatement : null;
			FixedStatement result = new FixedStatement(Expression.Clone(), clonedBody);
			CopyParentAndLabel(result);
            return result;
        }

        public override Statement CloneStatementOnly()
        {
            BlockStatement clonedBody = body != null ? body.CloneStatementOnly() as BlockStatement : null;
            FixedStatement result = new FixedStatement(Expression.CloneExpressionOnly(), clonedBody);
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
                if (this.body != null)
                {
                    this.body.Parent = this;
                }
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.FixedStatement; }
        }
    }
}
