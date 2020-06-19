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
// Warning: generated do not edit
using System.Collections.Generic;
using Mono.Cecil;
using System;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class UnaryExpression : Expression
    {
        private TypeReference expressionType;

        public UnaryExpression(UnaryOperator @operator, Expression operand, IEnumerable<Instruction> instructions)
            :base(instructions)
        {
            this.Operator = @operator;
            this.Operand = operand;
			
			if (operand is UnaryExpression && @operator == UnaryOperator.None)
			{ 
				// flatten it
				this.Operand = (operand as UnaryExpression).Operand;
				this.Operator = (operand as UnaryExpression).Operator;
				this.instructions.AddRange((operand as UnaryExpression).instructions);
			}
        }

        public UnaryOperator Operator { get; set; }

        public Expression Operand { get; set; }

        public override IEnumerable<ICodeNode> Children
        {
            get { yield return Operand; }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is UnaryExpression))
            {
                return false;
            }
            UnaryExpression unary = other as UnaryExpression;
            if(this.Operator!= unary.Operator)
            {
                return false;
            }
            return this.Operand.Equals(unary.Operand);
        }

        public override Expression Clone()
        {
            UnaryExpression result = new UnaryExpression(Operator, Operand.Clone(), instructions);
            result.expressionType = this.expressionType;
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            UnaryExpression result = new UnaryExpression(Operator, Operand.CloneExpressionOnly(), new Instruction[0]);
            result.expressionType = this.expressionType;
            return result;
        }

        public override TypeReference ExpressionType
        {
            get
            {
                if (expressionType == null)
                {
                    DecideExpressionType();
                }
                return expressionType;
            }
            set
            {
                throw new NotSupportedException("Cannot change the type of Unary expression.");
            }
        }

        public void DecideExpressionType()
        {
            if (Operand.ExpressionType == null)
            {
                this.expressionType = null;
				return;
            }
            if (this.Operator == UnaryOperator.AddressDereference)
            {
                TypeReference tr = Operand.ExpressionType;
                if (tr == null)
                {
                    this.expressionType = null;
                    return;
                }
				while (tr.IsOptionalModifier || tr.IsRequiredModifier)
				{
					tr = (tr as TypeSpecification).ElementType;
				}
                if (tr.IsPointer)
                {
					//the same type reference, excluding the * at the end of the name
					this.expressionType = (tr as PointerType).ElementType;
                    return;
                }
				if (tr.IsByReference)
				{
					//the same type reference, excluding the & at the end of the name
					this.expressionType = (tr as ByReferenceType).ElementType;
					return;
				}
                if (tr.IsPinned)
                {
                    this.expressionType = ((tr as PinnedType).ElementType as ByReferenceType).ElementType;
                    return;
                }
                this.expressionType = tr;
            }
            else if (this.Operator == UnaryOperator.AddressOf)
            {
                TypeReference tr = Operand.ExpressionType;
                //the same type reference, adding the * at the end of the name
                this.expressionType = new PointerType(tr);
            }
			else if (this.Operator == UnaryOperator.AddressReference)
			{
				TypeReference tr = Operand.ExpressionType;
				this.expressionType = new ByReferenceType(tr);
			}
			else
			{
				this.expressionType = Operand.ExpressionType;
			}

        }

        public override bool HasType
        {
            get
            {
                return Operand.HasType;
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get
            {
                return CodeNodeType.UnaryExpression;
            }
        }
    }
}