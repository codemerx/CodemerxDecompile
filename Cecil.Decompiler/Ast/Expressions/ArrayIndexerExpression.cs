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
    public class ArrayIndexerExpression : Expression, IIndexerExpression
    {
        public ArrayIndexerExpression(Expression target, IEnumerable<Instruction> instructions)
            : base(instructions)
        {
            this.Target = target;
            this.Indices = new ExpressionCollection();
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

        internal bool IsSimpleStore { get; set; }

        public override bool Equals(Expression other)
        {
            if (!(other is ArrayIndexerExpression))
            {
                return false;
            }
            ArrayIndexerExpression indexer = other as ArrayIndexerExpression;

            return this.Target.Equals(indexer.Target) && this.Indices.Equals(indexer.Indices);
        }

        public override Expression Clone()
        {
            return new ArrayIndexerExpression(Target.Clone(), instructions) { Indices = Indices.Clone(), IsSimpleStore = this.IsSimpleStore };
        }

        public override Expression CloneExpressionOnly()
        {
            return new ArrayIndexerExpression(Target.CloneExpressionOnly(), null) { Indices = Indices.CloneExpressionsOnly(), IsSimpleStore = this.IsSimpleStore };
        }

        public override bool HasType
        {
            get
            {
                return Target.HasType;
            }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                TypeReference arrayType = Target.ExpressionType;
                if (arrayType == null)
                {
                    /// The target might be a phi variable. In the case, where the target's type is not yet resolved, the whole expression
                    /// must be left typeless.
                    return null;
                }
                if (Target is ArgumentReferenceExpression)
                {
                    TypeReference targetType = (Target as ArgumentReferenceExpression).ExpressionType;
                    if (targetType.IsByReference)
                    {
                        //the argument will be treated as ref argument, so the pointer will be omited
                        arrayType = (targetType as ByReferenceType).ElementType;
                    }
                }
                if (arrayType.IsOptionalModifier)
                {
                    arrayType = (arrayType as OptionalModifierType).ElementType;
                }
                if (arrayType.IsRequiredModifier)
                {
                    arrayType = (arrayType as RequiredModifierType).ElementType;
                }
                //TODO: see if we need to handle pointers here as well
                if (arrayType.IsArray)
                {
                    ArrayType at = arrayType as ArrayType;
                    return at.ElementType;
                }
                if (arrayType.FullName == "System.Array")
                {
                    return arrayType.Module.TypeSystem.Object;
                }
                if (arrayType.FullName == "System.String")
                {
                    return arrayType.Module.TypeSystem.Char;
                }
                throw new ArgumentOutOfRangeException("Target of array indexer expression is not an array.");
            }
            set
            {
                throw new NotSupportedException("Array indexer expression cannot change its type.");
            }
        }

        public Expression Target { get; set; }

        public ExpressionCollection Indices { get; set; }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.ArrayIndexerExpression; }
        }
    }
}