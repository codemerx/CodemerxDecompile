using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class UnsafeBlockStatement : BlockStatement
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 1;
			}
		}

		public UnsafeBlockStatement(StatementCollection statements)
		{
			base();
			this.set_Statements(statements);
			V_0 = this.get_Statements().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_0.get_Current().set_Parent(this);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		public override Statement Clone()
		{
			V_0 = new UnsafeBlockStatement(new StatementCollection());
			V_1 = this.get_Statements().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.AddStatement(V_2.Clone());
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new UnsafeBlockStatement(new StatementCollection());
			V_1 = this.get_Statements().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.AddStatement(V_2.CloneStatementOnly());
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			this.CopyParentAndLabel(V_0);
			return V_0;
		}
	}
}