using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class DynamicIndexerExpression : Expression, IIndexerExpression
    {
		public ExpressionCollection Indices { get; set; }
		public Expression Target { get; set; }

        public DynamicIndexerExpression(Expression target, TypeReference expressionType, IEnumerable<Instruction> instructions)
            : this(target, new ExpressionCollection(), expressionType, instructions)
        {
        }

        private DynamicIndexerExpression(Expression target, ExpressionCollection indices, TypeReference expressionType, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            this.Target = target;
            this.Indices = indices;
            this.ExpressionType = expressionType;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Target;
                foreach (ICodeNode index in Indices)
                {
                    yield return index;
                }
            }
        }

		public override bool Equals(Expression other)
		{
            if(other.CodeNodeType != CodeNodeType.DynamicIndexerExpression)
            {
                return false;
            }

            DynamicIndexerExpression otherIndexer = other as DynamicIndexerExpression;
            return this.Target.Equals(otherIndexer.Target) && this.Indices.Equals(otherIndexer.Indices);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.DynamicIndexerExpression; }
        }

        public override Expression Clone()
        {
			DynamicIndexerExpression result = new DynamicIndexerExpression(Target.Clone(), Indices.Clone(), ExpressionType, this.instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            DynamicIndexerExpression result = new DynamicIndexerExpression(Target.CloneExpressionOnly(), Indices.CloneExpressionsOnly(), ExpressionType, null);
            return result;
        }
    }
}
