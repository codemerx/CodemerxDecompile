using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps.CodePatterns;

namespace Telerik.JustDecompiler.Steps
{
	internal class CodePatternsStep : BaseCodeTransformer, IDecompilationStep
	{
		private readonly bool isAggressive;

		private bool isInFilter;

		private DecompilationContext context;

		private TypeSystem typeSystem;

		private CodePatternsContext patternsContext;

		public CodePatternsStep(bool isAggressive)
		{
			this.isAggressive = isAggressive;
			this.isInFilter = false;
		}

		protected virtual IEnumerable<ICodePattern> GetCodePatterns()
		{
			return new ICodePattern[] { new NullCoalescingPattern(this.patternsContext, this.context.MethodContext), this.GetTernaryPattern(this.patternsContext), new ArrayInitialisationPattern(this.patternsContext, this.typeSystem), new ObjectInitialisationPattern(this.patternsContext, this.typeSystem), new CollectionInitializationPattern(this.patternsContext, this.typeSystem), this.GetVariableInliningPattern(this.patternsContext) };
		}

		private TernaryConditionPattern GetTernaryPattern(CodePatternsContext patternsContext)
		{
			if (!this.isAggressive && !this.isInFilter)
			{
				return new TernaryConditionPattern(patternsContext, this.typeSystem);
			}
			return new TernaryConditionPatternAgressive(patternsContext, this.typeSystem);
		}

		private VariableInliningPattern GetVariableInliningPattern(CodePatternsContext patternsContext)
		{
			if (!this.isAggressive)
			{
				return new VariableInliningPattern(patternsContext, this.context.MethodContext, this.context.Language.VariablesToNotInlineFinder);
			}
			return new VariableInliningPatternAggressive(patternsContext, this.context.MethodContext, this.context.Language.VariablesToNotInlineFinder);
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			this.patternsContext = new CodePatternsContext(body);
			body = (BlockStatement)this.Visit(body);
			return body;
		}

		private void RemoveRange(StatementCollection statements, int startIndex, int length)
		{
			if (length == 0)
			{
				return;
			}
			int count = statements.Count;
			for (int i = startIndex; i + length < count; i++)
			{
				statements[i] = statements[i + length];
			}
			while (length > 0)
			{
				int num = count - 1;
				count = num;
				statements.RemoveAt(num);
				length--;
			}
		}

		private void RemoveRangeAndInsert(BlockStatement block, int startIndex, int length, Statement newStatement)
		{
			string label = block.Statements[startIndex].Label;
			this.RemoveRange(block.Statements, startIndex, length);
			newStatement.Label = label;
			if (!String.IsNullOrEmpty(label))
			{
				this.context.MethodContext.GotoLabels[label] = newStatement;
			}
			block.AddStatementAt(startIndex, newStatement);
		}

		public override ICodeNode VisitBlockStatement(BlockStatement node)
		{
			Statement statement;
			int num;
			int num1;
			node = (BlockStatement)base.VisitBlockStatement(node);
			if (node == null)
			{
				return node;
			}
			IEnumerable<ICodePattern> codePatterns = this.GetCodePatterns();
			bool flag = false;
			do
			{
				flag = false;
				foreach (ICodePattern codePattern in codePatterns)
				{
					if (!codePattern.TryMatch(node.Statements, out num1, out statement, out num))
					{
						continue;
					}
					flag = true;
					if (statement == null)
					{
						this.RemoveRange(node.Statements, num1, num);
						goto Label0;
					}
					else
					{
						this.RemoveRangeAndInsert(node, num1, num, statement);
						goto Label0;
					}
				}
			Label0:
			}
			while (flag);
			return node;
		}

		public override ICodeNode VisitCatchClause(CatchClause node)
		{
			if (node.Filter == null || !this.context.Language.SupportsExceptionFilters)
			{
				return base.VisitCatchClause(node);
			}
			this.isInFilter = true;
			node.Filter = (Statement)this.Visit(node.Filter);
			this.isInFilter = false;
			node.Body = (BlockStatement)this.Visit(node.Body);
			return node;
		}
	}
}