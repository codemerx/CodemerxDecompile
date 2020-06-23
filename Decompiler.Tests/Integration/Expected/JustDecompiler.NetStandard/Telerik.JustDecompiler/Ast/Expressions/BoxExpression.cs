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
	public class BoxExpression : Expression
	{
		private readonly TypeSystem theTypeSystem;

		public TypeReference BoxedAs
		{
			get;
			private set;
		}

		public Expression BoxedExpression
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				BoxExpression boxExpression = null;
				yield return boxExpression.BoxedExpression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.BoxExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.theTypeSystem.Object;
			}
			set
			{
				throw new NotSupportedException("Expression type of box expression can not be changed.");
			}
		}

		public bool IsAutoBox
		{
			get;
			set;
		}

		public BoxExpression(Expression boxedExpression, TypeSystem theTypeSystem, TypeReference boxedAsType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.theTypeSystem = theTypeSystem;
			this.BoxedExpression = boxedExpression;
			this.BoxedAs = boxedAsType;
			this.IsAutoBox = false;
		}

		public override Expression Clone()
		{
			return new BoxExpression(this.BoxedExpression.Clone(), this.theTypeSystem, this.BoxedAs, this.instructions)
			{
				IsAutoBox = this.IsAutoBox
			};
		}

		public override Expression CloneExpressionOnly()
		{
			return new BoxExpression(this.BoxedExpression.CloneExpressionOnly(), this.theTypeSystem, this.BoxedAs, null)
			{
				IsAutoBox = this.IsAutoBox
			};
		}

		public override bool Equals(Expression other)
		{
			BoxExpression boxExpression = other as BoxExpression;
			if (boxExpression == null)
			{
				return false;
			}
			if (!this.BoxedExpression.Equals(boxExpression.BoxedExpression))
			{
				return false;
			}
			return this.BoxedAs.FullName == boxExpression.BoxedAs.FullName;
		}
	}
}