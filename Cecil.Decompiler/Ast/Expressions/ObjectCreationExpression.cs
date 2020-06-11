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
    public class ObjectCreationExpression : Expression
	{
		public ObjectCreationExpression(MethodReference constructor, TypeReference type, InitializerExpression initializer, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
            this.Constructor = constructor;
            this.Type = type;
            this.Initializer = initializer;
			this.Arguments = new ExpressionCollection();
		}

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if (Initializer != null)
                {
                    yield return Initializer;
                }
                foreach (ICodeNode argument in Arguments)
                {
                    yield return argument;
                }
            }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is ObjectCreationExpression))
            {
                return false;
            }

            ObjectCreationExpression objectCreation = other as ObjectCreationExpression;

            #region CheckConstructor

            if (this.Constructor == null)
            {
                if (objectCreation.Constructor != null)
                {
                    return false;
                }
            }
            else
            {
                if (objectCreation.Constructor == null || this.Constructor.FullName != objectCreation.Constructor.FullName)
                {
                    return false;
                }
            }
            #endregion

            #region CheckArguments

            if (this.Arguments == null)
            {
                if (objectCreation.Arguments != null)
                {
                    return false;
                }
            }
            else if (objectCreation.Arguments == null || ! this.Arguments.Equals(objectCreation.Arguments))
            {
                return false;
            }
            #endregion

            #region CheckType

            if (this.Type == null)
            {
                if (objectCreation.Type != null)
                { 
                    return false;
                }
            }
            else if (objectCreation.Type == null || this.Type.FullName != objectCreation.Type.FullName)
            {
                return false;
            }
            #endregion

            #region CheckInitializer
            if (this.Initializer == null)
            {
                if (objectCreation.Initializer != null)
                {
                    return false;
                }
            }
            else if (objectCreation.Initializer == null || !this.Initializer.Equals(objectCreation.Initializer))
            {
                return false;
            }
            #endregion

            return true;
        }

        public override Expression Clone()
        {
			InitializerExpression initializerClone = Initializer != null ? Initializer.Clone() as InitializerExpression : null;
			ObjectCreationExpression result = new ObjectCreationExpression(Constructor, Type, initializerClone, this.instructions);
			result.Arguments = this.Arguments.Clone();
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
			InitializerExpression initializerClone = Initializer != null ? Initializer.CloneExpressionOnly() as InitializerExpression : null;
            ObjectCreationExpression result = new ObjectCreationExpression(Constructor, Type, initializerClone, null)
                { Arguments = this.Arguments.CloneExpressionsOnly() };
            return result;
        }

        public MethodReference Constructor { get; set; }

        public TypeReference Type { get; set; }

        public ExpressionCollection Arguments { get; set; }

        public InitializerExpression Initializer { get; set; }

        public override bool HasType
        {
            get
            {
                return true;
            }
        }

        public override TypeReference ExpressionType
        {
            get
            {
                if (Type != null)
                {
                    return Type;
                }
                return Constructor.DeclaringType;
            }
            set
            {
                throw new NotSupportedException("Object creation expression cannot change it's type");
            }
        }

        public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.ObjectCreationExpression; }
        }
    }
}