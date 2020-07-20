using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
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
			base();
			this.isAggressive = isAggressive;
			this.isInFilter = false;
			return;
		}

		protected virtual IEnumerable<ICodePattern> GetCodePatterns()
		{
			stackVariable1 = new ICodePattern[6];
			stackVariable1[0] = new NullCoalescingPattern(this.patternsContext, this.context.get_MethodContext());
			stackVariable1[1] = this.GetTernaryPattern(this.patternsContext);
			stackVariable1[2] = new ArrayInitialisationPattern(this.patternsContext, this.typeSystem);
			stackVariable1[3] = new ObjectInitialisationPattern(this.patternsContext, this.typeSystem);
			stackVariable1[4] = new CollectionInitializationPattern(this.patternsContext, this.typeSystem);
			stackVariable1[5] = this.GetVariableInliningPattern(this.patternsContext);
			return stackVariable1;
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
				return new VariableInliningPattern(patternsContext, this.context.get_MethodContext(), this.context.get_Language().get_VariablesToNotInlineFinder());
			}
			return new VariableInliningPatternAggressive(patternsContext, this.context.get_MethodContext(), this.context.get_Language().get_VariablesToNotInlineFinder());
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
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
			V_0 = statements.get_Count();
			V_1 = startIndex;
			while (V_1 + length < V_0)
			{
				statements.set_Item(V_1, statements.get_Item(V_1 + length));
				V_1 = V_1 + 1;
			}
			while (length > 0)
			{
				stackVariable23 = V_0 - 1;
				V_0 = stackVariable23;
				statements.RemoveAt(stackVariable23);
				length = length - 1;
			}
			return;
		}

		private void RemoveRangeAndInsert(BlockStatement block, int startIndex, int length, Statement newStatement)
		{
			V_0 = block.get_Statements().get_Item(startIndex).get_Label();
			this.RemoveRange(block.get_Statements(), startIndex, length);
			newStatement.set_Label(V_0);
			if (!String.IsNullOrEmpty(V_0))
			{
				this.context.get_MethodContext().get_GotoLabels().set_Item(V_0, newStatement);
			}
			block.AddStatementAt(startIndex, newStatement);
			return;
		}

		public override ICodeNode VisitBlockStatement(BlockStatement node)
		{
			node = (BlockStatement)this.VisitBlockStatement(node);
			if (node == null)
			{
				return node;
			}
			V_0 = this.GetCodePatterns();
			V_1 = false;
			do
			{
				V_1 = false;
				V_5 = V_0.GetEnumerator();
				try
				{
					while (V_5.MoveNext())
					{
						if (!V_5.get_Current().TryMatch(node.get_Statements(), out V_4, out V_2, out V_3))
						{
							continue;
						}
						V_1 = true;
						if (V_2 == null)
						{
							this.RemoveRange(node.get_Statements(), V_4, V_3);
							goto Label0;
						}
						else
						{
							this.RemoveRangeAndInsert(node, V_4, V_3, V_2);
							goto Label0;
						}
					}
				}
				finally
				{
					if (V_5 != null)
					{
						V_5.Dispose();
					}
				}
			Label0:
			}
			while (V_1);
			return node;
		}

		public override ICodeNode VisitCatchClause(CatchClause node)
		{
			if (node.get_Filter() == null || !this.context.get_Language().get_SupportsExceptionFilters())
			{
				return this.VisitCatchClause(node);
			}
			this.isInFilter = true;
			node.set_Filter((Statement)this.Visit(node.get_Filter()));
			this.isInFilter = false;
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}
	}
}