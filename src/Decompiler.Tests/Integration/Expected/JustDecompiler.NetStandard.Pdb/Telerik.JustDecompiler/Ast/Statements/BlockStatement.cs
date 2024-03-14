using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class BlockStatement : Statement
	{
		private StatementCollection statements = new StatementCollection();

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				BlockStatement blockStatement = null;
				foreach (ICodeNode statement in blockStatement.statements)
				{
					yield return statement;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.BlockStatement;
			}
		}

		public StatementCollection Statements
		{
			get
			{
				return this.statements;
			}
			set
			{
				this.statements = value;
				foreach (Statement statement in this.statements)
				{
					statement.Parent = this;
				}
			}
		}

		public BlockStatement()
		{
		}

		public void AddStatement(Statement statement)
		{
			this.AddStatementAt(this.statements.Count, statement);
		}

		public void AddStatementAt(int index, Statement statement)
		{
			this.statements.Insert(index, statement);
			statement.Parent = this;
		}

		public override Statement Clone()
		{
			BlockStatement blockStatement = new BlockStatement();
			base.CopyParentAndLabel(blockStatement);
			foreach (Statement statement in this.statements)
			{
				blockStatement.AddStatement(statement.Clone());
			}
			return blockStatement;
		}

		public override Statement CloneStatementOnly()
		{
			BlockStatement blockStatement = new BlockStatement();
			base.CopyParentAndLabel(blockStatement);
			foreach (Statement statement in this.statements)
			{
				blockStatement.AddStatement(statement.CloneStatementOnly());
			}
			return blockStatement;
		}
	}
}