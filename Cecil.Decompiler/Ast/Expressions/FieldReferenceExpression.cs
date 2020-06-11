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
    public class FieldReferenceExpression : Expression
    {
        public FieldReferenceExpression(Expression target, FieldReference field, IEnumerable<Instruction> instructions)
            : base(instructions)
        {
            this.Target = target;
            this.Field = field;
        }

        internal bool IsSimpleStore { get; set; }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (this.Target != null)
                {
                    yield return this.Target;
                }
            }
        }

        public override bool Equals(Expression other)
        {
            if (!(other is FieldReferenceExpression))
            {
                return false;
            }
            FieldReferenceExpression fieldRef = other as FieldReferenceExpression;
            if (this.Target == null)
            {
                if (fieldRef.Target != null)
                {
                    return false;
                }
            }
            else if (!this.Target.Equals(fieldRef.Target))
            {
                return false;
            }
            return this.Field.FullName == fieldRef.Field.FullName;
        }

        public override Expression Clone()
        {
            Expression clonedTarget = Target != null ? Target.Clone() : null;
            FieldReferenceExpression result = new FieldReferenceExpression(clonedTarget, this.Field, instructions) { IsSimpleStore = this.IsSimpleStore };
            return result;
        }

        public override Expression CloneExpressionOnly()
        {
            Expression clonedTarget = Target != null ? Target.CloneExpressionOnly() : null;
            FieldReferenceExpression result = new FieldReferenceExpression(clonedTarget, this.Field, null) { IsSimpleStore = this.IsSimpleStore };
            return result;
        }

        public Expression Target { get; set; }

        public FieldReference Field { get; private set; }

        public bool IsStatic
        {
            get
            {
                return Target == null;
            }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                return Field.FieldType;
            }
            set
            {
                throw new NotSupportedException("Field expression cannot change its type.");
            }
        }

        public override bool HasType
        {
            get
            {
                return true;
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.FieldReferenceExpression; }
        }
    }
}