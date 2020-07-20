using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
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
				return 42;
			}
		}

		public TypeReference DeclaringType
		{
			get
			{
				return this.get_MethodExpression().get_Method().get_DeclaringType();
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				if (!this.get_IsSetter())
				{
					return this.get_MethodExpression().get_ExpressionType();
				}
				V_0 = this.get_MethodExpression().get_Method().get_Parameters().get_Count() - 1;
				return this.get_MethodExpression().get_Method().get_Parameters().get_Item(V_0).ResolveParameterType(this.get_MethodExpression().get_Method());
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
				return this.get_Arguments().get_Count() > 0;
			}
		}

		public bool IsSetter
		{
			get
			{
				if (this.get_MethodExpression().get_MethodDefinition() == null)
				{
					return false;
				}
				return this.get_MethodExpression().get_MethodDefinition().get_IsSetter();
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
				return this.get_MethodExpression().get_Target();
			}
			set
			{
				this.get_MethodExpression().set_Target(value);
				return;
			}
		}

		public PropertyReferenceExpression(MethodInvocationExpression invocation, IEnumerable<Instruction> instructions)
		{
			base(invocation.get_MethodExpression(), instructions);
			this.instructions.AddRange(invocation.get_MappedInstructions());
			this.set_Arguments(invocation.get_Arguments().Clone());
			this.set_MethodExpression((MethodReferenceExpression)invocation.get_MethodExpression().Clone());
			this.set_VirtualCall(invocation.get_VirtualCall());
			this.set_Property(this.ResolveProperty());
			if (this.get_IsSetter())
			{
				this.get_Arguments().RemoveAt(invocation.get_Arguments().get_Count() - 1);
			}
			return;
		}

		private PropertyReferenceExpression(MethodReferenceExpression methodExpression, PropertyDefinition property, bool virtualCall, IEnumerable<Instruction> instructions)
		{
			base(methodExpression, instructions);
			this.set_MethodExpression(methodExpression);
			this.set_Property(property);
			this.set_VirtualCall(virtualCall);
			return;
		}

		public override Expression Clone()
		{
			stackVariable10 = new PropertyReferenceExpression((MethodReferenceExpression)this.get_MethodExpression().Clone(), this.get_Property(), this.get_VirtualCall(), this.instructions);
			stackVariable10.set_Arguments(this.get_Arguments().Clone());
			return stackVariable10;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable9 = new PropertyReferenceExpression((MethodReferenceExpression)this.get_MethodExpression().CloneExpressionOnly(), this.get_Property(), this.get_VirtualCall(), null);
			stackVariable9.set_Arguments(this.get_Arguments().CloneExpressionsOnly());
			return stackVariable9;
		}

		public override bool Equals(Expression other)
		{
			if (other == null || other as PropertyReferenceExpression == null)
			{
				return false;
			}
			return this.Equals(other as MethodInvocationExpression);
		}

		private PropertyDefinition ResolveProperty()
		{
			V_0 = this.get_MethodExpression().get_Method().get_DeclaringType().Resolve();
			V_1 = this.get_MethodExpression().get_Method().Resolve();
			V_2 = null;
			if (V_0 != null)
			{
				V_3 = V_0.get_Properties().GetEnumerator();
				try
				{
					while (V_3.MoveNext())
					{
						V_4 = V_3.get_Current();
						if ((object)V_4.get_GetMethod() != (object)V_1)
						{
							if ((object)V_4.get_SetMethod() != (object)V_1)
							{
								continue;
							}
							V_2 = V_4;
							goto Label0;
						}
						else
						{
							V_2 = V_4;
							goto Label0;
						}
					}
				}
				finally
				{
					V_3.Dispose();
				}
			}
		Label0:
			return V_2;
		}
	}
}