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
	public class ArrayCreationExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ArrayCreationExpression.u003cget_Childrenu003ed__14(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 38;
			}
		}

		public ExpressionCollection Dimensions
		{
			get;
			set;
		}

		public TypeReference ElementType
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return new ArrayType(this.get_ElementType(), this.get_Dimensions().get_Count());
			}
			set
			{
				throw new NotSupportedException("Array creation expression cannot change its type.");
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

		public ArrayCreationExpression(TypeReference type, InitializerExpression initializer, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_ElementType(type);
			this.set_Initializer(initializer);
			this.set_Dimensions(new ExpressionCollection());
			return;
		}

		public override Expression Clone()
		{
			if (this.get_Initializer() != null)
			{
				stackVariable5 = (InitializerExpression)this.get_Initializer().Clone();
			}
			else
			{
				stackVariable5 = null;
			}
			stackVariable11 = new ArrayCreationExpression(this.get_ElementType(), stackVariable5, this.instructions);
			stackVariable11.set_Dimensions(this.get_Dimensions().Clone());
			return stackVariable11;
		}

		public override Expression CloneExpressionOnly()
		{
			if (this.get_Initializer() != null)
			{
				stackVariable5 = (InitializerExpression)this.get_Initializer().CloneExpressionOnly();
			}
			else
			{
				stackVariable5 = null;
			}
			stackVariable10 = new ArrayCreationExpression(this.get_ElementType(), stackVariable5, null);
			stackVariable10.set_Dimensions(this.get_Dimensions().CloneExpressionsOnly());
			return stackVariable10;
		}

		public override bool Equals(Expression other)
		{
			if (other as ArrayCreationExpression == null)
			{
				return false;
			}
			V_0 = other as ArrayCreationExpression;
			if (this.get_Initializer() != null)
			{
				if (!this.get_Initializer().Equals(V_0.get_Initializer()))
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
			if (!String.op_Equality(this.get_ElementType().get_FullName(), V_0.get_ElementType().get_FullName()))
			{
				return false;
			}
			return this.get_Dimensions().Equals(V_0.get_Dimensions());
		}
	}
}