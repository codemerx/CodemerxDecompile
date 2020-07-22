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
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public class DelegateCreationExpression : Expression
	{
		public DelegateCreationExpression(TypeReference type, Expression method, Expression target, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
            this.Type = type;
            this.MethodExpression = method;
            this.Target = target;
			this.TypeIsImplicitlyInferable = ContainsAnonymousType(type);
		}
  
		private static bool ContainsAnonymousType(TypeReference type)
		{
			TypeDefinition resolvedType = type.Resolve();

			if (resolvedType== null)
			{
				return false;
			}
			if (resolvedType.IsAnonymous())
			{
				return true;
			}
			return type.ContainsAnonymousType();
		}

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                yield return Target;
                yield return MethodExpression;
            }
        }

		public override bool Equals(Expression other)
		{
            if (!(other is DelegateCreationExpression))
            {
                return false;
            }
            DelegateCreationExpression delegateCreation = other as DelegateCreationExpression;
            #region TargetsComparison

            if (this.Target == null)
            {
                if (delegateCreation.Target != null)
                {
                    return false;
                }
            }
            else if (!this.Target.Equals(delegateCreation.Target))
            {
                return false;
            }
            #endregion

            #region MethodComparison

            if (!this.MethodExpression.Equals(delegateCreation.MethodExpression))
            {
                return false;
            }
            #endregion

            #region TypeComparison

            return this.Type.FullName == delegateCreation.Type.FullName;
            #endregion
        }

        public override Expression Clone()
        {
            DelegateCreationExpression result = new DelegateCreationExpression(Type, MethodExpression.Clone(), Target.Clone(), this.instructions);
			result.TypeIsImplicitlyInferable = this.TypeIsImplicitlyInferable;
			return result;
        }

        public override Expression CloneExpressionOnly()
        {
			DelegateCreationExpression result = new DelegateCreationExpression(Type, MethodExpression.CloneExpressionOnly(), Target.CloneExpressionOnly(), null);
			result.TypeIsImplicitlyInferable = this.TypeIsImplicitlyInferable;
			return result;
        }

        public TypeReference Type { get; private set; }

        public Expression MethodExpression { get; set; }

		public Expression Target { get; set; }

		public bool TypeIsImplicitlyInferable { get; set; }

		public override CodeNodeType CodeNodeType
        {
            get { return CodeNodeType.DelegateCreationExpression; }
        }

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
				return this.Type;
			}
			set
			{
				throw new NotSupportedException("Delegate creation expression cannot change its type.");
			}
		}
    }
}