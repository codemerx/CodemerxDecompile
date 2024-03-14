using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class IfElseIfStatement : BasePdbStatement
	{
		private List<KeyValuePair<Expression, BlockStatement>> conditionBlocks;

		private BlockStatement @else;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				// 
				// Current member / type: System.Collections.Generic.IEnumerable`1<Telerik.JustDecompiler.Ast.ICodeNode> Telerik.JustDecompiler.Ast.Statements.IfElseIfStatement::get_Children()
				// Exception in: System.Collections.Generic.IEnumerable<Telerik.JustDecompiler.Ast.ICodeNode> get_Children()
				// Invalid state value
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.IfElseIfStatement;
			}
		}

		public List<KeyValuePair<Expression, BlockStatement>> ConditionBlocks
		{
			get
			{
				return this.conditionBlocks;
			}
			set
			{
				this.conditionBlocks = value;
				foreach (KeyValuePair<Expression, BlockStatement> conditionBlock in this.conditionBlocks)
				{
					conditionBlock.Value.Parent = this;
				}
			}
		}

		public BlockStatement Else
		{
			get
			{
				return this.@else;
			}
			set
			{
				this.@else = value;
				if (this.@else != null)
				{
					this.@else.Parent = this;
				}
			}
		}

		public IfElseIfStatement(List<KeyValuePair<Expression, BlockStatement>> conditionBlocks, BlockStatement @else)
		{
			this.ConditionBlocks = conditionBlocks;
			this.Else = @else;
		}

		public override Statement Clone()
		{
			return this.CloneStatement(true);
		}

		private Statement CloneStatement(bool copyInstructions)
		{
			List<KeyValuePair<Expression, BlockStatement>> keyValuePairs = new List<KeyValuePair<Expression, BlockStatement>>();
			foreach (KeyValuePair<Expression, BlockStatement> conditionBlock in this.conditionBlocks)
			{
				Expression expression = (copyInstructions ? conditionBlock.Key.Clone() : conditionBlock.Key.CloneExpressionOnly());
				keyValuePairs.Add(new KeyValuePair<Expression, BlockStatement>(expression, (copyInstructions ? (BlockStatement)conditionBlock.Value.Clone() : (BlockStatement)conditionBlock.Value.CloneStatementOnly())));
			}
			BlockStatement blockStatement = null;
			if (this.@else != null)
			{
				blockStatement = (copyInstructions ? (BlockStatement)this.@else.Clone() : (BlockStatement)this.@else.CloneStatementOnly());
			}
			IfElseIfStatement ifElseIfStatement = new IfElseIfStatement(keyValuePairs, blockStatement);
			base.CopyParentAndLabel(ifElseIfStatement);
			return ifElseIfStatement;
		}

		public override Statement CloneStatementOnly()
		{
			return this.CloneStatement(false);
		}
	}
}