using System;
using Mono.Cecil;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class LambdaParameterExpression : Expression
    {
        public ParameterReference Parameter { get; private set; }
        public bool DisplayType { get; private set; }

        public LambdaParameterExpression(ParameterReference parameterRef, bool displayType, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            if (parameterRef == null)
            {
                throw new ArgumentNullException("parameterRef");
            }

            this.Parameter = parameterRef;
            this.DisplayType = displayType;
        }

        public override bool Equals(Expression other)
        {
            return object.ReferenceEquals(this, other);
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.LambdaParameterExpression; }
        }

        public override Expression Clone()
        {
            return new LambdaParameterExpression(this.Parameter, this.DisplayType, instructions);
        }

        public override Expression CloneExpressionOnly()
        {
            return new LambdaParameterExpression(this.Parameter, this.DisplayType, null);
        }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                return this.Parameter.ParameterType;
            }
            set
            {
                throw new Exception("Cannot set the type of lambda parameter.");
            }
        }
    }
}
