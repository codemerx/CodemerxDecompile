using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps.CodePatterns;

namespace Telerik.JustDecompiler.Steps
{
	internal class DetermineCtorInvocationStep : IDecompilationStep
	{
		private MethodSpecificContext methodContext;

		private BlockStatement methodBodyBlock;

		private TypeSystem typeSystem;

		private DecompilationContext context;

		private CodePatternsContext patternsContext;

		public DetermineCtorInvocationStep()
		{
		}

		private StatementCollection GetStatementsForInvocation(out int endIndex, out int startIndex, out bool isBaseCtor)
		{
			int i;
			startIndex = 0;
			StatementCollection statements = this.methodBodyBlock.Statements;
			StatementCollection statementCollection = new StatementCollection();
			isBaseCtor = false;
			for (i = 0; i < statements.Count; i++)
			{
				if (!this.IsVariableDeclaration(statements[i]) || statementCollection.Count != 0)
				{
					statementCollection.Add(statements[i].Clone());
					if (this.IsCtorInvocation(statements[i], out isBaseCtor))
					{
						if (!isBaseCtor)
						{
							break;
						}
						this.methodContext.IsBaseConstructorInvokingConstructor = true;
						break;
					}
				}
				else
				{
					startIndex++;
				}
			}
			endIndex = i;
			if (i == statements.Count)
			{
				endIndex = -1;
				statementCollection = null;
			}
			return statementCollection;
		}

		private bool IsCtorInvocation(Statement statement, out bool isBaseCtor)
		{
			isBaseCtor = false;
			if (statement.CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			MethodInvocationExpression expression = (statement as ExpressionStatement).Expression as MethodInvocationExpression;
			if (expression == null || expression.CodeNodeType != CodeNodeType.BaseCtorExpression && expression.CodeNodeType != CodeNodeType.ThisCtorExpression)
			{
				return false;
			}
			isBaseCtor = expression.CodeNodeType == CodeNodeType.BaseCtorExpression;
			return true;
		}

		private bool IsVariableDeclaration(Statement statements)
		{
			if (statements.CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return false;
			}
			if (((ExpressionStatement)statements).Expression.CodeNodeType != CodeNodeType.VariableDeclarationExpression)
			{
				return false;
			}
			return true;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			if (!context.MethodContext.Method.IsConstructor || body.Statements.Count == 0)
			{
				return body;
			}
			this.methodContext = context.MethodContext;
			this.typeSystem = this.methodContext.Method.Module.TypeSystem;
			this.methodBodyBlock = body;
			this.context = context;
			this.ProcessCtorInvocation();
			return body;
		}

		private void ProcessCtorInvocation()
		{
			int num;
			int num1;
			bool flag;
			StatementCollection statementsForInvocation = this.GetStatementsForInvocation(out num1, out num, out flag);
			if (statementsForInvocation == null)
			{
				return;
			}
			this.patternsContext = new CodePatternsContext(statementsForInvocation);
			List<ICodePattern> codePatterns = new List<ICodePattern>(new ICodePattern[] { new NullCoalescingPattern(this.patternsContext, this.methodContext), new TernaryConditionPatternAgressive(this.patternsContext, this.typeSystem), new ArrayInitialisationPattern(this.patternsContext, this.typeSystem), new VariableInliningPatternAggressive(this.patternsContext, this.methodContext, this.context.Language.VariablesToNotInlineFinder), new MultiAssignPattern(this.patternsContext, this.methodContext) });
			if (flag)
			{
				codePatterns.Add(new InitializationPattern(this.patternsContext, this.context));
			}
			if (!this.ProcessStatementCollection(statementsForInvocation, codePatterns))
			{
				return;
			}
			if (statementsForInvocation[0].CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				return;
			}
			MethodInvocationExpression expression = (statementsForInvocation[0] as ExpressionStatement).Expression as MethodInvocationExpression;
			if (expression.CodeNodeType != CodeNodeType.BaseCtorExpression && expression.CodeNodeType != CodeNodeType.ThisCtorExpression)
			{
				return;
			}
			this.methodContext.CtorInvokeExpression = expression;
		}

		private bool ProcessStatementCollection(StatementCollection statements, IEnumerable<ICodePattern> patternInvokeArray)
		{
			bool flag;
			Statement statement;
			int num;
			for (int i = 0; i < statements.Count; i++)
			{
				if (statements[i].CodeNodeType == CodeNodeType.IfStatement)
				{
					IfStatement item = statements[i] as IfStatement;
					this.ProcessStatementCollection(item.Then.Statements, patternInvokeArray);
					if (item.Else != null)
					{
						this.ProcessStatementCollection(item.Else.Statements, patternInvokeArray);
					}
				}
			}
			do
			{
				flag = false;
				foreach (ICodePattern codePattern in patternInvokeArray)
				{
					int num1 = -1;
					bool flag1 = codePattern.TryMatch(statements, out num, out statement, out num1);
					flag |= flag1;
					if (!flag1)
					{
						continue;
					}
					if (statement == null)
					{
						this.RemoveRange(statements, num, num1);
						goto Label0;
					}
					else
					{
						this.RemoveRangeAndInsert(statements, num, num1, statement);
						goto Label0;
					}
				}
			Label0:
			}
			while (flag);
			return statements.Count == 1;
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

		private void RemoveRangeAndInsert(StatementCollection statements, int startIndex, int length, Statement newStatement)
		{
			statements[startIndex] = newStatement;
			this.RemoveRange(statements, startIndex + 1, length - 1);
		}
	}
}