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
				stackVariable1 = new JoinClause.u003cget_Childrenu003ed__23(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 78;
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

		public JoinClause(Expression innerIdentifier, Expression innerCollection, Expression outerKey, Expression innerKey, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_InnerIdentifier(innerIdentifier);
			this.set_InnerCollection(innerCollection);
			this.set_OuterKey(outerKey);
			this.set_InnerKey(innerKey);
			return;
		}

		public override Expression Clone()
		{
			return new JoinClause(this.get_InnerIdentifier().Clone(), this.get_InnerCollection().Clone(), this.get_OuterKey().Clone(), this.get_InnerKey().Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new JoinClause(this.get_InnerIdentifier().CloneExpressionOnly(), this.get_InnerCollection().CloneExpressionOnly(), this.get_OuterKey().CloneExpressionOnly(), this.get_InnerKey().CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as JoinClause;
			if (V_0 == null || !this.get_InnerIdentifier().Equals(V_0.get_InnerIdentifier()) || !this.get_InnerCollection().Equals(V_0.get_InnerCollection()) || !this.get_OuterKey().Equals(V_0.get_OuterKey()))
			{
				return false;
			}
			return this.get_InnerKey().Equals(V_0.get_InnerKey());
		}
	}
}