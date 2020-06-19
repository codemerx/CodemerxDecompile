using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class DynamicConstructorInvocationExpression : Expression
    {
		public DynamicConstructorInvocationExpression(TypeReference constructorType, IEnumerable<Expression> arguments, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
            this.ConstructorType = constructorType;
            this.Arguments = new ExpressionCollection(arguments);
        }

        public TypeReference ConstructorType { get; private set; }

        public ExpressionCollection Arguments { get; internal set; }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                foreach (ICodeNode argument in this.Arguments)
                {
                    yield return argument;
                }
            }
        }

		public override bool Equals(Expression other)
		{
            if(other.CodeNodeType != CodeNodeType.DynamicConstructorInvocationExpression)
            {
                return false;
            }

            DynamicConstructorInvocationExpression otherExpression = other as DynamicConstructorInvocationExpression;
            return this.ConstructorType.FullName == otherExpression.ConstructorType.FullName && this.Arguments.Equals(otherExpression.Arguments);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.DynamicConstructorInvocationExpression; }
        }

        public override Expression Clone()
        {
            DynamicConstructorInvocationExpression result = new DynamicConstructorInvocationExpression(ConstructorType, Arguments.Clone(), instructions);
            return result;
        }

        public override Expression CloneExpressionOnly()
        {
            DynamicConstructorInvocationExpression result = new DynamicConstructorInvocationExpression(ConstructorType, Arguments.CloneExpressionsOnly(), null);
            return result;
        }

        public override TypeReference ExpressionType
        {
            get
            {
                return this.ConstructorType;
            }
        }
    }
}
