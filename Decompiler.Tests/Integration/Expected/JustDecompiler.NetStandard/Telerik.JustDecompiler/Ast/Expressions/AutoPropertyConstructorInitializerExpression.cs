using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class AutoPropertyConstructorInitializerExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new AutoPropertyConstructorInitializerExpression.u003cget_Childrenu003ed__15(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 91;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Property().get_PropertyType();
			}
			set
			{
				throw new NotSupportedException("Auto-property constructor initializer cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return (object)this.get_Property().get_PropertyType() != (object)null;
			}
		}

		public PropertyDefinition Property
		{
			get;
			private set;
		}

		public Expression Target
		{
			get;
			private set;
		}

		public AutoPropertyConstructorInitializerExpression(PropertyDefinition property, Expression target, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Property(property);
			this.set_Target(target);
			return;
		}

		public override Expression Clone()
		{
			stackVariable1 = this.get_Property();
			if (this.get_Target() != null)
			{
				stackVariable6 = this.get_Target().Clone();
			}
			else
			{
				stackVariable6 = null;
			}
			return new AutoPropertyConstructorInitializerExpression(stackVariable1, stackVariable6, this.get_MappedInstructions());
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable1 = this.get_Property();
			if (this.get_Target() != null)
			{
				stackVariable6 = this.get_Target().CloneExpressionOnly();
			}
			else
			{
				stackVariable6 = null;
			}
			return new AutoPropertyConstructorInitializerExpression(stackVariable1, stackVariable6, null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as AutoPropertyConstructorInitializerExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (this.get_Target() != null)
			{
				if (!this.get_Target().Equals(V_0.get_Target()))
				{
					return false;
				}
			}
			else
			{
				if (V_0.get_Target() != null)
				{
					return false;
				}
			}
			return String.op_Equality(this.get_Property().get_FullName(), V_0.get_Property().get_FullName());
		}
	}
}