#region license
//
//	(C) 2005 - 2007 db4objects Inc. http://www.db4o.com
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil;
using System.Linq;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class LambdaExpression : Expression
    {
        public ParameterReference[] Parameters { get; private set; }
        public bool IsExpressionTreeLambda { get; private set; }

        public LambdaExpression(ExpressionCollection arguments, BlockStatement body, bool isAsync, bool isFunction, IEnumerable<ParameterReference> parameters,
            bool isExpressionTreeLambda, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            this.Arguments = arguments;
            this.Body = body;
            this.IsAsync = isAsync;
			this.IsFunction = isFunction;
			this.Parameters = parameters.ToArray();
            this.IsExpressionTreeLambda = isExpressionTreeLambda;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                foreach (ICodeNode argument in Arguments)
                {
                    yield return argument;
                }
            }
        }

		public override bool Equals(Expression other)
		{
            /// This is not implemented, as it will require all statements to be IEqualable.
            /// At the moment of implementation, the only need for expression equality check arises in ExpressionPropagation step,
            /// which ignores lambda expressions.
            return this.Equals((object)other);
        }

        public override Expression Clone()
        {
            LambdaExpression result = new LambdaExpression(Arguments.Clone(), Body.Clone() as BlockStatement, IsAsync, IsFunction,
                Parameters, this.IsExpressionTreeLambda, instructions) { ExpressionType = this.ExpressionType };
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            LambdaExpression result = new LambdaExpression(Arguments.CloneExpressionsOnly(), Body.CloneStatementOnly() as BlockStatement, IsAsync, IsFunction,
                Parameters, this.IsExpressionTreeLambda, null) { ExpressionType = this.ExpressionType };
            return result;
        }

        public BlockStatement Body { get; set; }

        public ExpressionCollection Arguments { get; set; }

        public bool IsAsync { get; private set; }

		public bool IsFunction { get; private set; }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.LambdaExpression; }
        }
    }
}