using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
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

		public VariableInliningPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext, IVariablesToNotInlineFinder finder)
		{
			base(patternsContext, methodContext.get_Method().get_Module().get_TypeSystem());
			this.methodContext = methodContext;
			this.inliner = new RestrictedVariableInliner(this.typeSystem);
			this.finder = finder;
			this.dereferencer = new SimpleDereferencer();
			return;
		}

		private List<int> GetStatementsToInline(StatementCollection statements)
		{
			V_0 = new List<int>();
			V_1 = this.finder.Find(statements);
			V_2 = (BlockStatement)statements.get_Item(0).get_Parent();
			if (V_2 == null)
			{
				throw new NullReferenceException("parent");
			}
			V_3 = this.patternsContext.get_VariableToDefineUseCountContext().GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = V_3.get_Current();
					if (V_4.get_Value().DefineCount != 1 || V_4.get_Value().UseCount != 1 || V_1.Contains(V_4.get_Key()))
					{
						continue;
					}
					V_5 = this.patternsContext.get_VariableToSingleAssignmentMap().get_Item(V_4.get_Key());
					if (V_5.get_Parent() != V_2)
					{
						continue;
					}
					V_6 = statements.IndexOf(V_5);
					if (V_6 == -1)
					{
						throw new IndexOutOfRangeException("index");
					}
					V_0.Add(V_6);
				}
			}
			finally
			{
				((IDisposable)V_3).Dispose();
			}
			V_0.Sort();
			return V_0;
		}

		protected virtual bool ShouldInlineAggressively(VariableDefinition variable)
		{
			return this.methodContext.get_StackData().get_VariableToDefineUseInfo().ContainsKey(variable);
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			replacedStatementsCount = 0;
			startIndex = -1;
			result = null;
			V_0 = false;
			if (statements.get_Count() == 0)
			{
				return false;
			}
			V_1 = new HashSet<VariableDefinition>();
			V_2 = this.GetStatementsToInline(statements);
			V_3 = V_2.get_Count() - 1;
			while (V_3 >= 0)
			{
				V_4 = V_2.get_Item(V_3);
				V_5 = statements.get_Item(V_4) as ExpressionStatement;
				V_6 = ((V_5.get_Expression() as BinaryExpression).get_Left() as VariableReferenceExpression).get_Variable().Resolve();
				if (V_4 == statements.get_Count() - 1 || !String.IsNullOrEmpty(V_5.get_Label()))
				{
					dummyVar0 = V_1.Add(V_6);
				}
				else
				{
					V_7 = new List<Instruction>(V_5.get_Expression().get_MappedInstructions());
					V_7.AddRange((V_5.get_Expression() as BinaryExpression).get_Left().get_UnderlyingSameMethodInstructions());
					V_8 = (V_5.get_Expression() as BinaryExpression).get_Right().CloneAndAttachInstructions(V_7);
					if (this.inliner.TryInlineVariable(V_6, V_8, statements.get_Item(V_4 + 1), this.ShouldInlineAggressively(V_6), out V_9))
					{
						statements.RemoveAt(V_4);
						V_0 = true;
						dummyVar1 = V_1.Add(V_6);
						this.methodContext.RemoveVariable(V_6);
						statements.set_Item(V_4, (Statement)this.dereferencer.Visit(statements.get_Item(V_4)));
					}
				}
				V_3 = V_3 - 1;
			}
			V_10 = V_1.GetEnumerator();
			try
			{
				while (V_10.MoveNext())
				{
					V_11 = V_10.get_Current();
					dummyVar2 = this.patternsContext.get_VariableToSingleAssignmentMap().Remove(V_11);
					dummyVar3 = this.patternsContext.get_VariableToDefineUseCountContext().Remove(V_11);
				}
			}
			finally
			{
				((IDisposable)V_10).Dispose();
			}
			return V_0;
		}
	}
}