using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class CatchClause : BasePdbStatement
	{
		public BlockStatement Body
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				CatchClause catchClause = null;
				if (catchClause.Variable != null)
				{
					yield return catchClause.Variable;
				}
				if (catchClause.Filter != null)
				{
					yield return catchClause.Filter;
				}
				if (catchClause.Body != null)
				{
					yield return catchClause.Body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.CatchClause;
			}
		}

		public Statement Filter
		{
			get;
			set;
		}

		public TypeReference Type
		{
			get;
			set;
		}

		public VariableDeclarationExpression Variable
		{
			get;
			set;
		}

		public CatchClause()
		{
		}

		public CatchClause(BlockStatement body, TypeReference type, VariableDeclarationExpression variable, Statement filter = null)
		{
			this.Body = body;
			this.Type = type;
			this.Variable = variable;
			this.Filter = filter;
		}

		public override Statement Clone()
		{
			VariableDeclarationExpression variableDeclarationExpression;
			Statement statement;
			BlockStatement blockStatement;
			if (this.Body != null)
			{
				blockStatement = (BlockStatement)this.Body.Clone();
			}
			else
			{
				blockStatement = null;
			}
			if (this.Variable != null)
			{
				variableDeclarationExpression = (VariableDeclarationExpression)this.Variable.Clone();
			}
			else
			{
				variableDeclarationExpression = null;
			}
			VariableDeclarationExpression variableDeclarationExpression1 = variableDeclarationExpression;
			if (this.Filter != null)
			{
				statement = this.Filter.Clone();
			}
			else
			{
				statement = null;
			}
			return new CatchClause(blockStatement, this.Type, variableDeclarationExpression1, statement);
		}

		public override Statement CloneStatementOnly()
		{
			VariableDeclarationExpression variableDeclarationExpression;
			Statement statement;
			BlockStatement blockStatement;
			if (this.Body != null)
			{
				blockStatement = (BlockStatement)this.Body.CloneStatementOnly();
			}
			else
			{
				blockStatement = null;
			}
			if (this.Variable != null)
			{
				variableDeclarationExpression = (VariableDeclarationExpression)this.Variable.CloneExpressionOnly();
			}
			else
			{
				variableDeclarationExpression = null;
			}
			VariableDeclarationExpression variableDeclarationExpression1 = variableDeclarationExpression;
			if (this.Filter != null)
			{
				statement = this.Filter.CloneStatementOnly();
			}
			else
			{
				statement = null;
			}
			return new CatchClause(blockStatement, this.Type, variableDeclarationExpression1, statement);
		}
	}
}