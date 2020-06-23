using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class CreateIfElseIfStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		public CreateIfElseIfStatementsStep()
		{
		}

		private IfElseIfStatement BuildIfElseIfStatement(IfStatement theIf)
		{
			Statement item = theIf.Else.Statements[0];
			if (item.CodeNodeType != CodeNodeType.IfStatement)
			{
				IfElseIfStatement ifElseIfStatement = (IfElseIfStatement)item;
				List<KeyValuePair<Expression, BlockStatement>> conditionBlocks = ifElseIfStatement.ConditionBlocks;
				KeyValuePair<Expression, BlockStatement> keyValuePair = new KeyValuePair<Expression, BlockStatement>(theIf.Condition, theIf.Then);
				conditionBlocks.Insert(0, keyValuePair);
				theIf.Then.Parent = ifElseIfStatement;
				return ifElseIfStatement;
			}
			IfStatement ifStatement = (IfStatement)item;
			List<KeyValuePair<Expression, BlockStatement>> keyValuePairs = new List<KeyValuePair<Expression, BlockStatement>>();
			KeyValuePair<Expression, BlockStatement> keyValuePair1 = new KeyValuePair<Expression, BlockStatement>(theIf.Condition, theIf.Then);
			KeyValuePair<Expression, BlockStatement> keyValuePair2 = new KeyValuePair<Expression, BlockStatement>(ifStatement.Condition, ifStatement.Then);
			keyValuePairs.Add(keyValuePair1);
			keyValuePairs.Add(keyValuePair2);
			return new IfElseIfStatement(keyValuePairs, ifStatement.Else);
		}

		private IfElseIfStatement HandleDirectIfStatement(IfStatement theIf)
		{
			if (theIf.Then.Statements.Count != 1)
			{
				return this.BuildIfElseIfStatement(theIf);
			}
			Statement item = theIf.Then.Statements[0];
			if (item.CodeNodeType != CodeNodeType.IfElseIfStatement && item.CodeNodeType != CodeNodeType.IfStatement)
			{
				return this.BuildIfElseIfStatement(theIf);
			}
			Statement statement = theIf.Else.Statements[0];
			if (statement.CodeNodeType == CodeNodeType.IfStatement)
			{
				this.InvertIfStatement(theIf);
				return this.BuildIfElseIfStatement(theIf);
			}
			if (item.CodeNodeType == CodeNodeType.IfStatement)
			{
				return this.BuildIfElseIfStatement(theIf);
			}
			IfElseIfStatement ifElseIfStatement = (IfElseIfStatement)statement;
			if (((IfElseIfStatement)item).ConditionBlocks.Count >= ifElseIfStatement.ConditionBlocks.Count)
			{
				this.InvertIfStatement(theIf);
			}
			return this.BuildIfElseIfStatement(theIf);
		}

		private void InvertIfStatement(IfStatement theIf)
		{
			theIf.Condition = Negator.Negate(theIf.Condition, this.context.MethodContext.Method.Module.TypeSystem);
			BlockStatement @else = theIf.Else;
			theIf.Else = theIf.Then;
			theIf.Then = @else;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			return (BlockStatement)this.Visit(body);
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			ICodeNode codeNode = base.VisitIfStatement(node);
			if (codeNode.CodeNodeType != CodeNodeType.IfStatement)
			{
				return codeNode;
			}
			IfStatement ifStatement = (IfStatement)codeNode;
			if (ifStatement.Else == null)
			{
				return ifStatement;
			}
			if (ifStatement.Else.Statements.Count == 1)
			{
				Statement item = ifStatement.Else.Statements[0];
				if (item.CodeNodeType == CodeNodeType.IfStatement || item.CodeNodeType == CodeNodeType.IfElseIfStatement)
				{
					return this.HandleDirectIfStatement(ifStatement);
				}
			}
			if (ifStatement.Then.Statements.Count != 1)
			{
				return ifStatement;
			}
			Statement statement = ifStatement.Then.Statements[0];
			if (statement.CodeNodeType != CodeNodeType.IfStatement && statement.CodeNodeType != CodeNodeType.IfElseIfStatement)
			{
				return ifStatement;
			}
			this.InvertIfStatement(ifStatement);
			return this.BuildIfElseIfStatement(ifStatement);
		}
	}
}