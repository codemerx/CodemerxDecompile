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
	public class ObjectCreationExpression : Expression
	{
		public ExpressionCollection Arguments
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ObjectCreationExpression.u003cget_Childrenu003ed__2(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 40;
			}
		}

		public MethodReference Constructor
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				if (this.get_Type() != null)
				{
					return this.get_Type();
				}
				return this.get_Constructor().get_DeclaringType();
			}
			set
			{
				throw new NotSupportedException("Object creation expression cannot change it's type");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public InitializerExpression Initializer
		{
			get;
			set;
		}

		public TypeReference Type
		{
			get;
			set;
		}

		public ObjectCreationExpression(MethodReference constructor, TypeReference type, InitializerExpression initializer, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Constructor(constructor);
			this.set_Type(type);
			this.set_Initializer(initializer);
			this.set_Arguments(new ExpressionCollection());
			return;
		}

		public override Expression Clone()
		{
			if (this.get_Initializer() != null)
			{
				stackVariable5 = this.get_Initializer().Clone() as InitializerExpression;
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = stackVariable5;
			stackVariable13 = new ObjectCreationExpression(this.get_Constructor(), this.get_Type(), V_0, this.instructions);
			stackVariable13.set_Arguments(this.get_Arguments().Clone());
			return stackVariable13;
		}

		public override Expression CloneExpressionOnly()
		{
			if (this.get_Initializer() != null)
			{
				stackVariable5 = this.get_Initializer().CloneExpressionOnly() as InitializerExpression;
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = stackVariable5;
			stackVariable12 = new ObjectCreationExpression(this.get_Constructor(), this.get_Type(), V_0, null);
			stackVariable12.set_Arguments(this.get_Arguments().CloneExpressionsOnly());
			return stackVariable12;
		}

		public override bool Equals(Expression other)
		{
			if (other as ObjectCreationExpression == null)
			{
				return false;
			}
			V_0 = other as ObjectCreationExpression;
			if (this.get_Constructor() != null)
			{
				if (V_0.get_Constructor() == null || String.op_Inequality(this.get_Constructor().get_FullName(), V_0.get_Constructor().get_FullName()))
				{
					return false;
				}
			}
			else
			{
				if (V_0.get_Constructor() != null)
				{
					return false;
				}
			}
			if (this.get_Arguments() != null)
			{
				if (V_0.get_Arguments() == null || !this.get_Arguments().Equals(V_0.get_Arguments()))
				{
					return false;
				}
			}
			else
			{
				if (V_0.get_Arguments() != null)
				{
					return false;
				}
			}
			if (this.get_Type() != null)
			{
				if (V_0.get_Type() == null || String.op_Inequality(this.get_Type().get_FullName(), V_0.get_Type().get_FullName()))
				{
					return false;
				}
			}
			else
			{
				if (V_0.get_Type() != null)
				{
					return false;
				}
			}
			if (this.get_Initializer() != null)
			{
				if (V_0.get_Initializer() == null || !this.get_Initializer().Equals(V_0.get_Initializer()))
				{
					return false;
				}
			}
			else
			{
				if (V_0.get_Initializer() != null)
				{
					return false;
				}
			}
			return true;
		}
	}
}