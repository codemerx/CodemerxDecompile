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
				return Telerik.JustDecompiler.Ast.CodeNodeType.UnsafeBlock;
			}
		}

		public UnsafeBlockStatement(StatementCollection statements)
		{
			base.Statements = statements;
			foreach (Statement statement in base.Statements)
			{
				statement.Parent = this;
			}
		}

		public override Statement Clone()
		{
			UnsafeBlockStatement unsafeBlockStatement = new UnsafeBlockStatement(new StatementCollection());
			foreach (Statement statement in base.Statements)
			{
				unsafeBlockStatement.AddStatement(statement.Clone());
			}
			base.CopyParentAndLabel(unsafeBlockStatement);
			return unsafeBlockStatement;
		}

		public override Statement CloneStatementOnly()
		{
			UnsafeBlockStatement unsafeBlockStatement = new UnsafeBlockStatement(new StatementCollection());
			foreach (Statement statement in base.Statements)
			{
				unsafeBlockStatement.AddStatement(statement.CloneStatementOnly());
			}
			base.CopyParentAndLabel(unsafeBlockStatement);
			return unsafeBlockStatement;
		}
	}
}