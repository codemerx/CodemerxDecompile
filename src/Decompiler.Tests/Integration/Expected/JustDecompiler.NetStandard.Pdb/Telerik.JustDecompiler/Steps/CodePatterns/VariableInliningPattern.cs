using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Telerik.JustDecompiler.Decompiler.Inlining;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class VariableInliningPattern : CommonPatterns, ICodePattern
	{
		private readonly MethodSpecificContext methodContext;

		private readonly RestrictedVariableInliner inliner;

		private IVariablesToNotInlineFinder finder;

		private SimpleDereferencer dereferencer;

		public VariableInliningPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext, IVariablesToNotInlineFinder finder) : base(patternsContext, methodContext.Method.get_Module().get_TypeSystem())
		{
			this.methodContext = methodContext;
			this.inliner = new RestrictedVariableInliner(this.typeSystem);
			this.finder = finder;
			this.dereferencer = new SimpleDereferencer();
		}

		private List<int> GetStatementsToInline(StatementCollection statements)
		{
			List<int> nums = new List<int>();
			HashSet<VariableDefinition> variableDefinitions = this.finder.Find(statements);
			BlockStatement parent = (BlockStatement)statements[0].Parent;
			if (parent == null)
			{
				throw new NullReferenceException("parent");
			}
			foreach (KeyValuePair<VariableDefinition, DefineUseCount> variableToDefineUseCountContext in this.patternsContext.VariableToDefineUseCountContext)
			{
				if (variableToDefineUseCountContext.Value.DefineCount != 1 || variableToDefineUseCountContext.Value.UseCount != 1 || variableDefinitions.Contains(variableToDefineUseCountContext.Key))
				{
					continue;
				}
				ExpressionStatement item = this.patternsContext.VariableToSingleAssignmentMap[variableToDefineUseCountContext.Key];
				if (item.Parent != parent)
				{
					continue;
				}
				int num = statements.IndexOf(item);
				if (num == -1)
				{
					throw new IndexOutOfRangeException("index");
				}
				nums.Add(num);
			}
			nums.Sort();
			return nums;
		}

		protected virtual bool ShouldInlineAggressively(VariableDefinition variable)
		{
			return this.methodContext.StackData.VariableToDefineUseInfo.ContainsKey(variable);
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			ICodeNode codeNode;
			replacedStatementsCount = 0;
			startIndex = -1;
			result = null;
			bool flag = false;
			if (statements.Count == 0)
			{
				return false;
			}
			HashSet<VariableDefinition> variableDefinitions = new HashSet<VariableDefinition>();
			List<int> statementsToInline = this.GetStatementsToInline(statements);
			for (int i = statementsToInline.Count - 1; i >= 0; i--)
			{
				int item = statementsToInline[i];
				ExpressionStatement expressionStatement = statements[item] as ExpressionStatement;
				VariableDefinition variableDefinition = ((expressionStatement.Expression as BinaryExpression).Left as VariableReferenceExpression).Variable.Resolve();
				if (item == statements.Count - 1 || !String.IsNullOrEmpty(expressionStatement.Label))
				{
					variableDefinitions.Add(variableDefinition);
				}
				else
				{
					List<Instruction> instructions = new List<Instruction>(expressionStatement.Expression.MappedInstructions);
					instructions.AddRange((expressionStatement.Expression as BinaryExpression).Left.UnderlyingSameMethodInstructions);
					Expression expression = (expressionStatement.Expression as BinaryExpression).Right.CloneAndAttachInstructions(instructions);
					if (this.inliner.TryInlineVariable(variableDefinition, expression, statements[item + 1], this.ShouldInlineAggressively(variableDefinition), out codeNode))
					{
						statements.RemoveAt(item);
						flag = true;
						variableDefinitions.Add(variableDefinition);
						this.methodContext.RemoveVariable(variableDefinition);
						statements[item] = (Statement)this.dereferencer.Visit(statements[item]);
					}
				}
			}
			foreach (VariableDefinition variableDefinition1 in variableDefinitions)
			{
				this.patternsContext.VariableToSingleAssignmentMap.Remove(variableDefinition1);
				this.patternsContext.VariableToDefineUseCountContext.Remove(variableDefinition1);
			}
			return flag;
		}
	}
}