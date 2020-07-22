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
	public class LiteralExpression : Expression
	{
		private object value;

		private readonly TypeSystem typeSystem;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new LiteralExpression.u003cget_Childrenu003ed__4(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 22;
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
			set
			{
				this.value = value;
				this.ResolveType();
				return;
			}
		}

		public LiteralExpression(object value, TypeSystem typeSystem, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.typeSystem = typeSystem;
			this.set_Value(value);
			return;
		}

		public override Expression Clone()
		{
			return new LiteralExpression(this.value, this.typeSystem, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new LiteralExpression(this.value, this.typeSystem, null);
		}

		public override bool Equals(Expression other)
		{
			if (other as LiteralExpression == null)
			{
				return false;
			}
			if (this.get_Value() == null)
			{
				return (object)(other as LiteralExpression).get_Value() == (object)null;
			}
			return this.get_Value().Equals((other as LiteralExpression).get_Value());
		}

		private void ResolveType()
		{
			if (this.get_Value() == null)
			{
				this.set_ExpressionType(this.typeSystem.get_Object());
				return;
			}
			V_0 = this.get_Value().GetType().get_Name();
			if (V_0 != null)
			{
				if (String.op_Equality(V_0, "String"))
				{
					this.set_ExpressionType(this.typeSystem.get_String());
					return;
				}
				if (String.op_Equality(V_0, "Boolean"))
				{
					this.set_ExpressionType(this.typeSystem.get_Boolean());
					return;
				}
				if (String.op_Equality(V_0, "Byte"))
				{
					this.set_ExpressionType(this.typeSystem.get_Byte());
					return;
				}
				if (String.op_Equality(V_0, "SByte"))
				{
					this.set_ExpressionType(this.typeSystem.get_SByte());
					return;
				}
				if (String.op_Equality(V_0, "Char"))
				{
					this.set_ExpressionType(this.typeSystem.get_Char());
					return;
				}
				if (String.op_Equality(V_0, "UInt16"))
				{
					this.set_ExpressionType(this.typeSystem.get_UInt16());
					return;
				}
				if (String.op_Equality(V_0, "Int16"))
				{
					this.set_ExpressionType(this.typeSystem.get_Int16());
					return;
				}
				if (String.op_Equality(V_0, "Int32"))
				{
					this.set_ExpressionType(this.typeSystem.get_Int32());
					return;
				}
				if (String.op_Equality(V_0, "UInt32"))
				{
					this.set_ExpressionType(this.typeSystem.get_UInt32());
					return;
				}
				if (String.op_Equality(V_0, "Int64"))
				{
					this.set_ExpressionType(this.typeSystem.get_Int64());
					return;
				}
				if (String.op_Equality(V_0, "UInt64"))
				{
					this.set_ExpressionType(this.typeSystem.get_UInt64());
					return;
				}
				if (String.op_Equality(V_0, "Single"))
				{
					this.set_ExpressionType(this.typeSystem.get_Single());
					return;
				}
				if (String.op_Equality(V_0, "Double"))
				{
					this.set_ExpressionType(this.typeSystem.get_Double());
					return;
				}
				if (String.op_Equality(V_0, "IntPtr"))
				{
					this.set_ExpressionType(this.typeSystem.get_IntPtr());
					return;
				}
			}
			throw new ArgumentOutOfRangeException("Unknown type for literal expression.");
		}
	}
}