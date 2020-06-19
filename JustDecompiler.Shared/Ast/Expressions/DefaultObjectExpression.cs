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
using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class DefaultObjectExpression : Expression
    {
		public DefaultObjectExpression(TypeReference type, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
			this.Type = type;
		}

        public override IEnumerable<ICodeNode> Children
        {
            get { yield break; }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is DefaultObjectExpression))
            {
                return false;
            }
            return this.Type.FullName == (other as DefaultObjectExpression).Type.FullName;
        }

        public override Expression Clone()
        {
			DefaultObjectExpression result = new DefaultObjectExpression(Type, instructions);
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
            DefaultObjectExpression result = new DefaultObjectExpression(Type, null);
            return result;
        }

        public TypeReference Type { get; set; }

        public override TypeReference ExpressionType
        {
            get
            {
                return Type;
            }
            set
            {
                throw new System.NotSupportedException("Default object creation expression cannot change its type.");
            }
        }

        public override bool HasType
        {
            get
            {
                return Type != null;
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.DefaultObjectExpression; }
        }
    }
}
