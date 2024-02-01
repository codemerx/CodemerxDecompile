using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class DynamicIndexerExpression : Expression, IIndexerExpression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				DynamicIndexerExpression dynamicIndexerExpression = null;
				yield return dynamicIndexerExpression.Target;
				foreach (ICodeNode index in dynamicIndexerExpression.Indices)
				{
					yield return index;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.DynamicIndexerExpression;
			}
		}

		public ExpressionCollection Indices
		{
			get;
			set;
		}

		public Expression Target
		{
			get;
			set;
		}

		public DynamicIndexerExpression(Expression target, TypeReference expressionType, IEnumerable<Instruction> instructions) : this(target, new ExpressionCollection(), expressionType, instructions)
		{
		}

		private DynamicIndexerExpression(Expression target, ExpressionCollection indices, TypeReference expressionType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Target = target;
			this.Indices = indices;
			this.ExpressionType = expressionType;
		}

		public override Expression Clone()
		{
			return new DynamicIndexerExpression(this.Target.Clone(), this.Indices.Clone(), this.ExpressionType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new DynamicIndexerExpression(this.Target.CloneExpressionOnly(), this.Indices.CloneExpressionsOnly(), this.ExpressionType, null);
		}

		public override bool Equals(Expression other)
		{
			if (other.CodeNodeType != Telerik.JustDecompiler.Ast.CodeNodeType.DynamicIndexerExpression)
			{
				return false;
			}
			DynamicIndexerExpression dynamicIndexerExpression = other as DynamicIndexerExpression;
			if (!this.Target.Equals(dynamicIndexerExpression.Target))
			{
				return false;
			}
			return this.Indices.Equals(dynamicIndexerExpression.Indices);
		}
	}
}