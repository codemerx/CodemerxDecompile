using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public abstract class QueryClause : Expression
	{
		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Query clauses have no type.");
			}
			set
			{
				throw new NotSupportedException("Query clauses cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public QueryClause(IEnumerable<Instruction> instructions)
		{
			base(instructions);
			return;
		}
	}
}