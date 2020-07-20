using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class CollectionInitializationPattern : BaseInitialisationPattern
	{
		public CollectionInitializationPattern(CodePatternsContext patternsContext, TypeSystem typeSystem)
		{
			base(patternsContext, typeSystem);
			return;
		}

		protected override bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 0;
			if (!this.TryGetObjectCreation(statements, startIndex, out V_0, out V_1))
			{
				return false;
			}
			if (V_0.get_Initializer() != null && V_0.get_Initializer().get_InitializerType() != InitializerType.CollectionInitializer)
			{
				return false;
			}
			if (!this.ImplementsInterface(V_0.get_Type(), "System.Collections.IEnumerable"))
			{
				return false;
			}
			V_2 = new ExpressionCollection();
			V_3 = startIndex + 1;
			while (V_3 < statements.get_Count() && this.TryGetNextExpression(statements.get_Item(V_3), out V_4) && V_4.get_CodeNodeType() == 19)
			{
				V_5 = V_4 as MethodInvocationExpression;
				V_6 = V_5.get_MethodExpression().get_MethodDefinition();
				if (!this.CompareTargets(V_1, V_5.get_MethodExpression().get_Target()) || String.op_Inequality(V_6.get_Name(), "Add") || V_5.get_Arguments().get_Count() == 0)
				{
					break;
				}
				if (V_5.get_Arguments().get_Count() != 1)
				{
					stackVariable88 = V_5.get_Arguments();
					stackVariable89 = CollectionInitializationPattern.u003cu003ec.u003cu003e9__1_0;
					if (stackVariable89 == null)
					{
						dummyVar0 = stackVariable89;
						stackVariable89 = new Func<Expression, Expression>(CollectionInitializationPattern.u003cu003ec.u003cu003e9.u003cTryMatchInternalu003eb__1_0);
						CollectionInitializationPattern.u003cu003ec.u003cu003e9__1_0 = stackVariable89;
					}
					V_7 = new BlockExpression(new ExpressionCollection(stackVariable88.Select<Expression, Expression>(stackVariable89)), null);
					V_2.Add(V_7);
				}
				else
				{
					V_2.Add(V_5.get_Arguments().get_Item(0).Clone());
				}
				V_3 = V_3 + 1;
			}
			if (V_2.get_Count() == 0)
			{
				return false;
			}
			if (V_0.get_Initializer() != null)
			{
				V_9 = V_2.GetEnumerator();
				try
				{
					while (V_9.MoveNext())
					{
						V_10 = V_9.get_Current();
						V_0.get_Initializer().get_Expressions().Add(V_10);
					}
				}
				finally
				{
					if (V_9 != null)
					{
						V_9.Dispose();
					}
				}
			}
			else
			{
				V_8 = new InitializerExpression(V_2, 0);
				V_8.set_IsMultiLine(true);
				V_0.set_Initializer(V_8);
			}
			result = statements.get_Item(startIndex);
			replacedStatementsCount = V_2.get_Count() + 1;
			return true;
		}
	}
}