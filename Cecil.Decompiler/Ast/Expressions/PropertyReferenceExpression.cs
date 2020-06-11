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
using Mono.Cecil;
using System;
using Mono.Cecil.Extensions;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class PropertyReferenceExpression : MethodInvocationExpression
	{
		public override TypeReference ExpressionType
		{
			get
			{
				if (!IsSetter)
				{
					return MethodExpression.ExpressionType;
				}
				else
				{
					int last = MethodExpression.Method.Parameters.Count - 1;
					var result = MethodExpression.Method.Parameters[last].ResolveParameterType(MethodExpression.Method);
					return result;
				}
			}
			set
			{
				throw new NotSupportedException("Property reference type cannot be changed.");
			}
		}

		public bool IsSetter
		{
			get
			{
				if (this.MethodExpression.MethodDefinition == null)
				{
					/// this case shouldn't be happening - no PropertyReferenceExpression should be created in this case.
					return false;
				}
				return this.MethodExpression.MethodDefinition.IsSetter;
			}
		}

		public override bool Equals(Expression other)
		{
			if (other == null || !(other is PropertyReferenceExpression))
			{
				return false;
			}
			return base.Equals((other as MethodInvocationExpression));
		}

		public PropertyDefinition Property { get; private set; }

		public TypeReference DeclaringType 
		{
			get 
			{
				return MethodExpression.Method.DeclaringType; 
			} 
		}

		// All property usages are transformed method calls.
		public PropertyReferenceExpression(MethodInvocationExpression invocation, IEnumerable<Instruction> instructions)
			: base(invocation.MethodExpression, instructions)
		{
            this.instructions.AddRange(invocation.MappedInstructions);
			this.Arguments = invocation.Arguments.Clone();
			this.MethodExpression = (MethodReferenceExpression)invocation.MethodExpression.Clone();
			this.VirtualCall = invocation.VirtualCall;
			this.Property = ResolveProperty();
			if (IsSetter)
			{
				this.Arguments.RemoveAt(invocation.Arguments.Count - 1);
			}
		}

        private PropertyReferenceExpression(MethodReferenceExpression methodExpression, PropertyDefinition property, bool virtualCall, IEnumerable<Instruction> instructions)
            : base(methodExpression, instructions)
        {
            this.MethodExpression = methodExpression;
            this.Property = property;
            this.VirtualCall = virtualCall;
        }

		private PropertyDefinition ResolveProperty()
		{
			TypeDefinition type = MethodExpression.Method.DeclaringType.Resolve();
			MethodDefinition accessor = MethodExpression.Method.Resolve();
			PropertyDefinition result = null;
			if (type != null)
			{
				foreach (PropertyDefinition property in type.Properties)
				{
					if (property.GetMethod == accessor)
					{
						result = property;
						break;
					}
					if (property.SetMethod == accessor)
					{
						result = property;
						break;
					}
				}
			}
            return result;
        }

		public override CodeNodeType CodeNodeType
		{
			get
			{
				return CodeNodeType.PropertyReferenceExpression;
			}
		}

		public override Expression Clone()
		{
            MethodReferenceExpression methodExpressionClone = (MethodReferenceExpression)this.MethodExpression.Clone();
            PropertyReferenceExpression result = new PropertyReferenceExpression(methodExpressionClone, this.Property, this.VirtualCall, this.instructions)
                { Arguments = this.Arguments.Clone() };
			return result;
		}

        public override Expression CloneExpressionOnly()
        {
            MethodReferenceExpression methodExpressionClone = (MethodReferenceExpression)this.MethodExpression.CloneExpressionOnly();
            PropertyReferenceExpression result = new PropertyReferenceExpression(methodExpressionClone, this.Property, this.VirtualCall, null)
                { Arguments = this.Arguments.CloneExpressionsOnly() };
            return result;
        }

		public Expression Target
		{
			get 
			{
				return MethodExpression.Target;
			}
			set
			{
				MethodExpression.Target = value;
			}
		}

		public bool IsIndexer { get { return this.Arguments.Count > 0; } }
	}
}
