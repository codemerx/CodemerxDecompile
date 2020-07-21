using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class MultiAssignPattern : CommonPatterns, ICodePattern
	{
		private readonly HashSet<VariableDefinition> variablesToRemove;

		private readonly MethodSpecificContext methodContext;

		public MultiAssignPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext)
		{
			this.variablesToRemove = new HashSet<VariableDefinition>();
			base(patternsContext, methodContext.get_Method().get_Module().get_TypeSystem());
			this.methodContext = methodContext;
			return;
		}

		private void RemoveFromContext()
		{
			V_0 = this.variablesToRemove.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!this.patternsContext.get_VariableToDefineUseCountContext().Remove(V_1))
					{
						continue;
					}
					dummyVar0 = this.patternsContext.get_VariableToSingleAssignmentMap().Remove(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			this.variablesToRemove.Clear();
			return;
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 0;
			startIndex = 0;
			startIndex = 0;
			while (startIndex < statements.get_Count())
			{
				if (this.TryMatchInternal(statements, startIndex, out result, out replacedStatementsCount, out V_0))
				{
					this.FixContext(V_0, 0, replacedStatementsCount - 1, (ExpressionStatement)result);
					this.RemoveFromContext();
					return true;
				}
				startIndex = startIndex + 1;
			}
			this.variablesToRemove.Clear();
			return false;
		}

		private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount, out VariableDefinition xVariableDef)
		{
			result = null;
			replacedStatementsCount = 0;
			xVariableDef = null;
			if (statements.get_Count() < 1 || statements.get_Item(startIndex).get_CodeNodeType() != 5)
			{
				return false;
			}
			V_2 = (statements.get_Item(startIndex) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (!this.IsAssignToVariableExpression(V_2, out V_0) || !this.methodContext.get_StackData().get_VariableToDefineUseInfo().ContainsKey(V_0.Resolve()))
			{
				return false;
			}
			V_1 = V_2.get_Right();
			V_3 = startIndex + 1;
			while (V_3 < statements.get_Count())
			{
				V_5 = statements.get_Item(V_3);
				if (V_5.get_CodeNodeType() != 5 || !String.IsNullOrEmpty(V_5.get_Label()))
				{
					break;
				}
				V_6 = (V_5 as ExpressionStatement).get_Expression() as BinaryExpression;
				if (V_6 == null || !V_6.get_IsAssignmentExpression() || V_6.get_Right().get_CodeNodeType() != 26 || (object)(V_6.get_Right() as VariableReferenceExpression).get_Variable() != (object)V_0)
				{
					break;
				}
				if (V_6.get_Left().get_CodeNodeType() == 30)
				{
					return false;
				}
				if (V_6.get_Left().get_CodeNodeType() == 26)
				{
					V_7 = (V_6.get_Left() as VariableReferenceExpression).get_Variable().Resolve();
					if (V_7 == V_0)
					{
						return false;
					}
					dummyVar0 = this.variablesToRemove.Add(V_7);
				}
				V_1 = new BinaryExpression(26, V_6.get_Left(), V_1, this.typeSystem, null, false);
				V_3 = V_3 + 1;
			}
			replacedStatementsCount = V_3 - startIndex;
			if (replacedStatementsCount == 1)
			{
				return false;
			}
			V_4 = new BinaryExpression(26, new VariableReferenceExpression(V_0, null), V_1, this.typeSystem, null, false);
			stackVariable66 = new ExpressionStatement(V_4);
			stackVariable66.set_Parent(statements.get_Item(startIndex).get_Parent());
			result = stackVariable66;
			xVariableDef = V_0.Resolve();
			return true;
		}
	}
}