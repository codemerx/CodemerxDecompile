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
	public class BlockExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new BlockExpression.u003cget_Childrenu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 18;
			}
		}

		public ExpressionCollection Expressions
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Block expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Block expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public BlockExpression(IEnumerable<Instruction> instructions)
		{
			this(new ExpressionCollection(), instructions);
			return;
		}

		public BlockExpression(ExpressionCollection expressions, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expressions(expressions);
			return;
		}

		public override Expression Clone()
		{
			return new BlockExpression(this.get_Expressions().Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new BlockExpression(this.get_Expressions().CloneExpressionsOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as BlockExpression == null)
			{
				return false;
			}
			return this.get_Expressions().Equals((other as BlockExpression).get_Expressions());
		}
	}
}