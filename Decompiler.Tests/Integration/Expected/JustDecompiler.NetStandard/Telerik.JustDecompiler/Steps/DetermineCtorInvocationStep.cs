using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
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
			base();
			return;
		}

		private StatementCollection GetStatementsForInvocation(out int endIndex, out int startIndex, out bool isBaseCtor)
		{
			startIndex = 0;
			V_0 = this.methodBodyBlock.get_Statements();
			V_1 = new StatementCollection();
			isBaseCtor = false;
			V_2 = 0;
			while (V_2 < V_0.get_Count())
			{
				if (!this.IsVariableDeclaration(V_0.get_Item(V_2)) || V_1.get_Count() != 0)
				{
					V_1.Add(V_0.get_Item(V_2).Clone());
					if (this.IsCtorInvocation(V_0.get_Item(V_2), out isBaseCtor))
					{
						if (!isBaseCtor)
						{
							break;
						}
						this.methodContext.set_IsBaseConstructorInvokingConstructor(true);
						break;
					}
				}
				else
				{
					startIndex = startIndex + 1;
				}
				V_2 = V_2 + 1;
			}
			endIndex = V_2;
			if (V_2 == V_0.get_Count())
			{
				endIndex = -1;
				V_1 = null;
			}
			return V_1;
		}

		private bool IsCtorInvocation(Statement statement, out bool isBaseCtor)
		{
			isBaseCtor = false;
			if (statement.get_CodeNodeType() != 5)
			{
				return false;
			}
			V_0 = (statement as ExpressionStatement).get_Expression() as MethodInvocationExpression;
			if (V_0 == null || V_0.get_CodeNodeType() != 52 && V_0.get_CodeNodeType() != 53)
			{
				return false;
			}
			isBaseCtor = V_0.get_CodeNodeType() == 52;
			return true;
		}

		private bool IsVariableDeclaration(Statement statements)
		{
			if (statements.get_CodeNodeType() != 5)
			{
				return false;
			}
			if (((ExpressionStatement)statements).get_Expression().get_CodeNodeType() != 27)
			{
				return false;
			}
			return true;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			if (!context.get_MethodContext().get_Method().get_IsConstructor() || body.get_Statements().get_Count() == 0)
			{
				return body;
			}
			this.methodContext = context.get_MethodContext();
			this.typeSystem = this.methodContext.get_Method().get_Module().get_TypeSystem();
			this.methodBodyBlock = body;
			this.context = context;
			this.ProcessCtorInvocation();
			return body;
		}

		private void ProcessCtorInvocation()
		{
			V_3 = this.GetStatementsForInvocation(out V_1, out V_0, out V_2);
			if (V_3 == null)
			{
				return;
			}
			this.patternsContext = new CodePatternsContext(V_3);
			stackVariable10 = new ICodePattern[5];
			stackVariable10[0] = new NullCoalescingPattern(this.patternsContext, this.methodContext);
			stackVariable10[1] = new TernaryConditionPatternAgressive(this.patternsContext, this.typeSystem);
			stackVariable10[2] = new ArrayInitialisationPattern(this.patternsContext, this.typeSystem);
			stackVariable10[3] = new VariableInliningPatternAggressive(this.patternsContext, this.methodContext, this.context.get_Language().get_VariablesToNotInlineFinder());
			stackVariable10[4] = new MultiAssignPattern(this.patternsContext, this.methodContext);
			V_4 = new List<ICodePattern>(stackVariable10);
			if (V_2)
			{
				V_4.Add(new InitializationPattern(this.patternsContext, this.context));
			}
			if (!this.ProcessStatementCollection(V_3, V_4))
			{
				return;
			}
			if (V_3.get_Item(0).get_CodeNodeType() != 5)
			{
				return;
			}
			V_5 = (V_3.get_Item(0) as ExpressionStatement).get_Expression() as MethodInvocationExpression;
			if (V_5.get_CodeNodeType() != 52 && V_5.get_CodeNodeType() != 53)
			{
				return;
			}
			this.methodContext.set_CtorInvokeExpression(V_5);
			return;
		}

		private bool ProcessStatementCollection(StatementCollection statements, IEnumerable<ICodePattern> patternInvokeArray)
		{
			V_1 = 0;
			while (V_1 < statements.get_Count())
			{
				if (statements.get_Item(V_1).get_CodeNodeType() == 3)
				{
					V_2 = statements.get_Item(V_1) as IfStatement;
					dummyVar0 = this.ProcessStatementCollection(V_2.get_Then().get_Statements(), patternInvokeArray);
					if (V_2.get_Else() != null)
					{
						dummyVar1 = this.ProcessStatementCollection(V_2.get_Else().get_Statements(), patternInvokeArray);
					}
				}
				V_1 = V_1 + 1;
			}
			do
			{
				V_0 = false;
				V_3 = patternInvokeArray.GetEnumerator();
				try
				{
					while (V_3.MoveNext())
					{
						V_4 = -1;
						V_7 = V_3.get_Current().TryMatch(statements, out V_6, out V_5, out V_4);
						V_0 = V_0 | V_7;
						if (!V_7)
						{
							continue;
						}
						if (V_5 == null)
						{
							this.RemoveRange(statements, V_6, V_4);
							goto Label0;
						}
						else
						{
							this.RemoveRangeAndInsert(statements, V_6, V_4, V_5);
							goto Label0;
						}
					}
				}
				finally
				{
					if (V_3 != null)
					{
						V_3.Dispose();
					}
				}
			Label0:
			}
			while (V_0);
			return statements.get_Count() == 1;
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

		private void RemoveRangeAndInsert(StatementCollection statements, int startIndex, int length, Statement newStatement)
		{
			statements.set_Item(startIndex, newStatement);
			this.RemoveRange(statements, startIndex + 1, length - 1);
			return;
		}
	}
}