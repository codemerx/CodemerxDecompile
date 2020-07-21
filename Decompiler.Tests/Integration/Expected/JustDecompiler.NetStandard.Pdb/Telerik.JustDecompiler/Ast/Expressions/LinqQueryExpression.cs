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
				return new LinqQueryExpression.u003cget_Childrenu003ed__11(-2);
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
				return 81;
			}
		}

		public LinqQueryExpression(List<QueryClause> clauses, TypeReference linqQueryType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Clauses(clauses);
			this.set_ExpressionType(linqQueryType);
			return;
		}

		public override Expression Clone()
		{
			stackVariable1 = this.get_Clauses();
			stackVariable2 = LinqQueryExpression.u003cu003ec.u003cu003e9__6_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<QueryClause, QueryClause>(LinqQueryExpression.u003cu003ec.u003cu003e9.u003cCloneu003eb__6_0);
				LinqQueryExpression.u003cu003ec.u003cu003e9__6_0 = stackVariable2;
			}
			return new LinqQueryExpression(new List<QueryClause>(stackVariable1.Select<QueryClause, QueryClause>(stackVariable2)), this.get_ExpressionType(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable1 = this.get_Clauses();
			stackVariable2 = LinqQueryExpression.u003cu003ec.u003cu003e9__7_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<QueryClause, QueryClause>(LinqQueryExpression.u003cu003ec.u003cu003e9.u003cCloneExpressionOnlyu003eb__7_0);
				LinqQueryExpression.u003cu003ec.u003cu003e9__7_0 = stackVariable2;
			}
			return new LinqQueryExpression(new List<QueryClause>(stackVariable1.Select<QueryClause, QueryClause>(stackVariable2)), this.get_ExpressionType(), null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as LinqQueryExpression;
			if (V_0 == null || this.get_Clauses().get_Count() != V_0.get_Clauses().get_Count())
			{
				return false;
			}
			V_1 = 0;
			while (V_1 < this.get_Clauses().get_Count())
			{
				if (!this.get_Clauses().get_Item(V_1).Equals(V_0.get_Clauses().get_Item(V_1)))
				{
					return false;
				}
				V_1 = V_1 + 1;
			}
			return true;
		}
	}
}