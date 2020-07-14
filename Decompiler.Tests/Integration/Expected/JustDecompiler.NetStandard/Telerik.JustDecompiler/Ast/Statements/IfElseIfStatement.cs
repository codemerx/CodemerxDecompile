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
				// File path: C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\Decompiler.Tests\bin\Release\netcoreapp2.1\Integration\Actual\JustDecompiler.NetStandard.dll
				// 
				// Product version: 0.0.0.0
				// Exception in: System.Collections.Generic.IEnumerable<Telerik.JustDecompiler.Ast.ICodeNode> get_Children()
				// 
				// Invalid state value
				//    at Telerik.JustDecompiler.Decompiler.LogicFlow.YieldGuardedBlocksBuilder.ProcessCurrentNode(YieldExceptionHandlerInfo handlerInfo, Queue`1 bfsQueue, ILogicalConstruct currentNode) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 203
				//    at Telerik.JustDecompiler.Decompiler.LogicFlow.YieldGuardedBlocksBuilder.BuildTryBody(YieldExceptionHandlerInfo handlerInfo) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 187
				//    at Telerik.JustDecompiler.Decompiler.LogicFlow.YieldGuardedBlocksBuilder.GenerateTryFinallyHandler(YieldExceptionHandlerInfo handlerInfo) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 129
				//    at Telerik.JustDecompiler.Decompiler.LogicFlow.YieldGuardedBlocksBuilder.BuildGuardedBlocks(BlockLogicalConstruct theBlock) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\YieldGuardedBlocksBuilder.cs:line 76
				//    at Telerik.JustDecompiler.Decompiler.LogicFlow.LogicalFlowBuilderStep.BuildLogicalConstructTree() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 127
				//    at Telerik.JustDecompiler.Decompiler.LogicFlow.LogicalFlowBuilderStep.Process(DecompilationContext context, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 51
				//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.RunInternal(MethodBody body, BlockStatement block, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 81
				//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.Run(MethodBody body, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 70
				//    at Telerik.JustDecompiler.Decompiler.Extensions.DecompileStateMachine(MethodBody body, DecompilationContext enclosingContext, BaseStateMachineRemoverStep removeStateMachineStep, Func`2 stateMachineDataSelector, DecompilationContext& decompilationContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\Extensions.cs:line 103
				//    at Telerik.JustDecompiler.Decompiler.Extensions.DecompileYieldStateMachine(MethodBody body, DecompilationContext enclosingContext, YieldData& yieldData) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\Extensions.cs:line 139
				//    at Telerik.JustDecompiler.Steps.RebuildYieldStatementsStep.GetStatements() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\RebuildYieldStatementsStep.cs:line 151
				//    at Telerik.JustDecompiler.Steps.RebuildYieldStatementsStep.Match(StatementCollection statements) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\RebuildYieldStatementsStep.cs:line 49
				//    at Telerik.JustDecompiler.Steps.RebuildYieldStatementsStep.Process(DecompilationContext context, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\RebuildYieldStatementsStep.cs:line 20
				//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.RunInternal(MethodBody body, BlockStatement block, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 81
				//    at Telerik.JustDecompiler.Decompiler.PropertyDecompiler.FinishDecompilationOfMethod(BlockStatement block, DecompilationContext context, MethodSpecificContext& methodContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\PropertyDecompiler.cs:line 420
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