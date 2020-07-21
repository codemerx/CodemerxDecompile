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
	public class InitializerExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new InitializerExpression.u003cget_Childrenu003ed__24(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 88;
			}
		}

		public BlockExpression Expression
		{
			get;
			set;
		}

		public ExpressionCollection Expressions
		{
			get
			{
				return this.get_Expression().get_Expressions();
			}
			set
			{
				this.get_Expression().set_Expressions(value);
				return;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Initializer expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Initializer expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.InitializerType InitializerType
		{
			get;
			set;
		}

		public bool IsMultiLine
		{
			get;
			set;
		}

		public InitializerExpression(ExpressionCollection expressions, Telerik.JustDecompiler.Ast.Expressions.InitializerType initializerType)
		{
			this(new BlockExpression(expressions, null), initializerType, Enumerable.Empty<Instruction>());
			return;
		}

		public InitializerExpression(BlockExpression expression, Telerik.JustDecompiler.Ast.Expressions.InitializerType initializerType)
		{
			this(expression, initializerType, Enumerable.Empty<Instruction>());
			return;
		}

		public InitializerExpression(BlockExpression expression, Telerik.JustDecompiler.Ast.Expressions.InitializerType initializerType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			this.set_InitializerType(initializerType);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			stackVariable8 = new InitializerExpression(this.get_Expression().Clone() as BlockExpression, this.get_InitializerType(), this.instructions);
			stackVariable8.set_IsMultiLine(this.get_IsMultiLine());
			return stackVariable8;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			stackVariable7 = new InitializerExpression(this.get_Expression().Clone() as BlockExpression, this.get_InitializerType(), null);
			stackVariable7.set_IsMultiLine(this.get_IsMultiLine());
			return stackVariable7;
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			V_0 = other as InitializerExpression;
			if (V_0 == null || !this.get_Expression().Equals(V_0.get_Expression()))
			{
				return false;
			}
			return this.get_InitializerType() == V_0.get_InitializerType();
		}
	}
}