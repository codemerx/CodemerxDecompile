using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class LiteralExpression : Expression
	{
		private object @value;

		private readonly TypeSystem typeSystem;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.LiteralExpression;
			}
		}

		public object Value
		{
			get
			{
				return this.@value;
			}
			set
			{
				this.@value = value;
				this.ResolveType();
			}
		}

		public LiteralExpression(object value, TypeSystem typeSystem, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.typeSystem = typeSystem;
			this.Value = value;
		}

		public override Expression Clone()
		{
			return new LiteralExpression(this.@value, this.typeSystem, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new LiteralExpression(this.@value, this.typeSystem, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is LiteralExpression))
			{
				return false;
			}
			if (this.Value == null)
			{
				return (object)(other as LiteralExpression).Value == (object)null;
			}
			return this.Value.Equals((other as LiteralExpression).Value);
		}

		private void ResolveType()
		{
			if (this.Value == null)
			{
				this.ExpressionType = this.typeSystem.get_Object();
				return;
			}
			string name = this.Value.GetType().Name;
			if (name != null)
			{
				if (name == "String")
				{
					this.ExpressionType = this.typeSystem.get_String();
					return;
				}
				if (name == "Boolean")
				{
					this.ExpressionType = this.typeSystem.get_Boolean();
					return;
				}
				if (name == "Byte")
				{
					this.ExpressionType = this.typeSystem.get_Byte();
					return;
				}
				if (name == "SByte")
				{
					this.ExpressionType = this.typeSystem.get_SByte();
					return;
				}
				if (name == "Char")
				{
					this.ExpressionType = this.typeSystem.get_Char();
					return;
				}
				if (name == "UInt16")
				{
					this.ExpressionType = this.typeSystem.get_UInt16();
					return;
				}
				if (name == "Int16")
				{
					this.ExpressionType = this.typeSystem.get_Int16();
					return;
				}
				if (name == "Int32")
				{
					this.ExpressionType = this.typeSystem.get_Int32();
					return;
				}
				if (name == "UInt32")
				{
					this.ExpressionType = this.typeSystem.get_UInt32();
					return;
				}
				if (name == "Int64")
				{
					this.ExpressionType = this.typeSystem.get_Int64();
					return;
				}
				if (name == "UInt64")
				{
					this.ExpressionType = this.typeSystem.get_UInt64();
					return;
				}
				if (name == "Single")
				{
					this.ExpressionType = this.typeSystem.get_Single();
					return;
				}
				if (name == "Double")
				{
					this.ExpressionType = this.typeSystem.get_Double();
					return;
				}
				if (name == "IntPtr")
				{
					this.ExpressionType = this.typeSystem.get_IntPtr();
					return;
				}
			}
			throw new ArgumentOutOfRangeException("Unknown type for literal expression.");
		}
	}
}