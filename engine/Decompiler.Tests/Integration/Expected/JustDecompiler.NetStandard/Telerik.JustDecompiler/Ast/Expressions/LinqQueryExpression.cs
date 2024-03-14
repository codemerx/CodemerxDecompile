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
	public class LinqQueryExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
			}
		}

		public List<QueryClause> Clauses
		{
			get;
			set;
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.LinqQueryExpression;
			}
		}

		public LinqQueryExpression(List<QueryClause> clauses, TypeReference linqQueryType, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Clauses = clauses;
			this.ExpressionType = linqQueryType;
		}

		public override Expression Clone()
		{
			return new LinqQueryExpression(new List<QueryClause>(
				from clause in this.Clauses
				select (QueryClause)clause.Clone()), this.ExpressionType, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new LinqQueryExpression(new List<QueryClause>(
				from clause in this.Clauses
				select (QueryClause)clause.CloneExpressionOnly()), this.ExpressionType, null);
		}

		public override bool Equals(Expression other)
		{
			LinqQueryExpression linqQueryExpression = other as LinqQueryExpression;
			if (linqQueryExpression == null || this.Clauses.Count != linqQueryExpression.Clauses.Count)
			{
				return false;
			}
			for (int i = 0; i < this.Clauses.Count; i++)
			{
				if (!this.Clauses[i].Equals(linqQueryExpression.Clauses[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}