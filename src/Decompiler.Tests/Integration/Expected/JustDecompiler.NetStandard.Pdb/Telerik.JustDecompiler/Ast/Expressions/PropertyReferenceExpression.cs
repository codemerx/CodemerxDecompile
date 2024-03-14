using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class PropertyReferenceExpression : MethodInvocationExpression
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.PropertyReferenceExpression;
			}
		}

		public TypeReference DeclaringType
		{
			get
			{
				return base.MethodExpression.Method.get_DeclaringType();
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				if (!this.IsSetter)
				{
					return base.MethodExpression.ExpressionType;
				}
				int count = base.MethodExpression.Method.get_Parameters().get_Count() - 1;
				return base.MethodExpression.Method.get_Parameters().get_Item(count).ResolveParameterType(base.MethodExpression.Method);
			}
			set
			{
				throw new NotSupportedException("Property reference type cannot be changed.");
			}
		}

		public bool IsIndexer
		{
			get
			{
				return base.Arguments.Count > 0;
			}
		}

		public bool IsSetter
		{
			get
			{
				if (base.MethodExpression.MethodDefinition == null)
				{
					return false;
				}
				return base.MethodExpression.MethodDefinition.get_IsSetter();
			}
		}

		public PropertyDefinition Property
		{
			get;
			private set;
		}

		public Expression Target
		{
			get
			{
				return base.MethodExpression.Target;
			}
			set
			{
				base.MethodExpression.Target = value;
			}
		}

		public PropertyReferenceExpression(MethodInvocationExpression invocation, IEnumerable<Instruction> instructions) : base(invocation.MethodExpression, instructions)
		{
			this.instructions.AddRange(invocation.MappedInstructions);
			base.Arguments = invocation.Arguments.Clone();
			base.MethodExpression = (MethodReferenceExpression)invocation.MethodExpression.Clone();
			base.VirtualCall = invocation.VirtualCall;
			this.Property = this.ResolveProperty();
			if (this.IsSetter)
			{
				base.Arguments.RemoveAt(invocation.Arguments.Count - 1);
			}
		}

		private PropertyReferenceExpression(MethodReferenceExpression methodExpression, PropertyDefinition property, bool virtualCall, IEnumerable<Instruction> instructions) : base(methodExpression, instructions)
		{
			base.MethodExpression = methodExpression;
			this.Property = property;
			base.VirtualCall = virtualCall;
		}

		public override Expression Clone()
		{
			return new PropertyReferenceExpression((MethodReferenceExpression)base.MethodExpression.Clone(), this.Property, base.VirtualCall, this.instructions)
			{
				Arguments = base.Arguments.Clone()
			};
		}

		public override Expression CloneExpressionOnly()
		{
			return new PropertyReferenceExpression((MethodReferenceExpression)base.MethodExpression.CloneExpressionOnly(), this.Property, base.VirtualCall, null)
			{
				Arguments = base.Arguments.CloneExpressionsOnly()
			};
		}

		public override bool Equals(Expression other)
		{
			if (other == null || !(other is PropertyReferenceExpression))
			{
				return false;
			}
			return base.Equals(other as MethodInvocationExpression);
		}

		private PropertyDefinition ResolveProperty()
		{
			TypeDefinition typeDefinition = base.MethodExpression.Method.get_DeclaringType().Resolve();
			MethodDefinition methodDefinition = base.MethodExpression.Method.Resolve();
			PropertyDefinition propertyDefinition = null;
			if (typeDefinition != null)
			{
				foreach (PropertyDefinition property in typeDefinition.get_Properties())
				{
					if ((object)property.get_GetMethod() != (object)methodDefinition)
					{
						if ((object)property.get_SetMethod() != (object)methodDefinition)
						{
							continue;
						}
						propertyDefinition = property;
						return propertyDefinition;
					}
					else
					{
						propertyDefinition = property;
						return propertyDefinition;
					}
				}
			}
			return propertyDefinition;
		}
	}
}