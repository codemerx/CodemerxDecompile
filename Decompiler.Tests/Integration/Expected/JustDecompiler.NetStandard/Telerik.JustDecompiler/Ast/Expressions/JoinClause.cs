using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class JoinClause : QueryClause
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				JoinClause joinClause = null;
				yield return joinClause.InnerIdentifier;
				yield return joinClause.InnerCollection;
				yield return joinClause.OuterKey;
				yield return joinClause.InnerKey;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.JoinClause;
			}
		}

		public Expression InnerCollection
		{
			get;
			set;
		}

		public Expression InnerIdentifier
		{
			get;
			set;
		}

		public Expression InnerKey
		{
			get;
			set;
		}

		public Expression OuterKey
		{
			get;
			set;
		}

		public JoinClause(Expression innerIdentifier, Expression innerCollection, Expression outerKey, Expression innerKey, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.InnerIdentifier = innerIdentifier;
			this.InnerCollection = innerCollection;
			this.OuterKey = outerKey;
			this.InnerKey = innerKey;
		}

		public override Expression Clone()
		{
			return new JoinClause(this.InnerIdentifier.Clone(), this.InnerCollection.Clone(), this.OuterKey.Clone(), this.InnerKey.Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new JoinClause(this.InnerIdentifier.CloneExpressionOnly(), this.InnerCollection.CloneExpressionOnly(), this.OuterKey.CloneExpressionOnly(), this.InnerKey.CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			JoinClause joinClause = other as JoinClause;
			if (joinClause == null || !this.InnerIdentifier.Equals(joinClause.InnerIdentifier) || !this.InnerCollection.Equals(joinClause.InnerCollection) || !this.OuterKey.Equals(joinClause.OuterKey))
			{
				return false;
			}
			return this.InnerKey.Equals(joinClause.InnerKey);
		}
	}
}