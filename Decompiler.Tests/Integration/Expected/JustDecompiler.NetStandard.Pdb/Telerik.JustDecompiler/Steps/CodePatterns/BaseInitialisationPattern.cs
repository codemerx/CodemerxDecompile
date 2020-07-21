using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal abstract class BaseInitialisationPattern : CommonPatterns, ICodePattern
	{
		public BaseInitialisationPattern(CodePatternsContext patternsContext, TypeSystem ts)
		{
			base(patternsContext, ts);
			return;
		}

		protected bool CompareTargets(Expression assignee, Expression target)
		{
			if (target == null || assignee == null || target.get_CodeNodeType() != assignee.get_CodeNodeType())
			{
				return false;
			}
			if (target.get_CodeNodeType() != 42)
			{
				return target.Equals(assignee);
			}
			V_0 = target as PropertyReferenceExpression;
			V_1 = assignee as PropertyReferenceExpression;
			if (!V_0.get_Property().Equals(V_1.get_Property()))
			{
				return false;
			}
			return this.CompareTargets(V_1.get_Target(), V_0.get_Target());
		}

		protected bool ImplementsInterface(TypeReference type, string interfaceName)
		{
			V_0 = new BaseInitialisationPattern.u003cu003ec__DisplayClass8_0();
			V_0.interfaceName = interfaceName;
			while (type != null)
			{
				V_1 = type.Resolve();
				if (V_1 == null)
				{
					return false;
				}
				stackVariable8 = V_1.get_Interfaces();
				stackVariable10 = V_0.u003cu003e9__0;
				if (stackVariable10 == null)
				{
					dummyVar0 = stackVariable10;
					stackVariable18 = new Func<TypeReference, bool>(V_0.u003cImplementsInterfaceu003eb__0);
					V_2 = stackVariable18;
					V_0.u003cu003e9__0 = stackVariable18;
					stackVariable10 = V_2;
				}
				if (stackVariable8.Any<TypeReference>(stackVariable10))
				{
					return true;
				}
				type = V_1.get_BaseType();
			}
			return false;
		}

		protected bool TryGetArrayCreation(StatementCollection statements, int startIndex, out ArrayCreationExpression creation, out Expression assignee)
		{
			assignee = null;
			creation = null;
			if (!this.TryGetAssignment(statements, startIndex, out V_0))
			{
				return false;
			}
			if (V_0.get_Right().get_CodeNodeType() != 38)
			{
				return false;
			}
			V_1 = V_0.get_Right() as ArrayCreationExpression;
			if (!V_0.get_Right().get_HasType() || !V_0.get_Right().get_ExpressionType().get_IsArray())
			{
				return false;
			}
			if (V_1.get_Dimensions().get_Count() != 1)
			{
				return false;
			}
			V_2 = V_1.get_Dimensions().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					if (V_2.get_Current().get_CodeNodeType() == 22)
					{
						continue;
					}
					V_3 = false;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				if (V_2 != null)
				{
					V_2.Dispose();
				}
			}
		Label1:
			return V_3;
		Label0:
			creation = V_0.get_Right() as ArrayCreationExpression;
			assignee = V_0.get_Left();
			return true;
		}

		private bool TryGetAssignment(StatementCollection statements, int startIndex, out BinaryExpression binaryExpression)
		{
			binaryExpression = null;
			V_0 = statements.get_Item(startIndex);
			if (V_0.get_CodeNodeType() != 5)
			{
				return false;
			}
			V_1 = (V_0 as ExpressionStatement).get_Expression();
			if (V_1.get_CodeNodeType() != 24)
			{
				return false;
			}
			V_2 = V_1 as BinaryExpression;
			if (!V_2.get_IsAssignmentExpression())
			{
				return false;
			}
			if (V_2.get_Left().get_CodeNodeType() != 26 && V_2.get_Left().get_CodeNodeType() != 27 && V_2.get_Left().get_CodeNodeType() != 30 && V_2.get_Left().get_CodeNodeType() != 28 && V_2.get_Left().get_CodeNodeType() != 42 && V_2.get_Left().get_CodeNodeType() != 25 && V_2.get_Left().get_CodeNodeType() != 23 || (V_2.get_Left() as UnaryExpression).get_Operand().get_CodeNodeType() != 25)
			{
				return false;
			}
			binaryExpression = V_2;
			return true;
		}

		protected bool TryGetNextExpression(Statement statement, out Expression expression)
		{
			expression = null;
			if (statement.get_CodeNodeType() != 5 || !String.IsNullOrEmpty(statement.get_Label()))
			{
				return false;
			}
			expression = (statement as ExpressionStatement).get_Expression();
			return true;
		}

		protected bool TryGetObjectCreation(StatementCollection statements, int startIndex, out ObjectCreationExpression creation, out Expression assignee)
		{
			assignee = null;
			creation = null;
			if (!this.TryGetAssignment(statements, startIndex, out V_0))
			{
				return false;
			}
			if (V_0.get_Right().get_CodeNodeType() != 40)
			{
				return false;
			}
			assignee = V_0.get_Left();
			creation = V_0.get_Right() as ObjectCreationExpression;
			return true;
		}

		public virtual bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			startIndex = -1;
			result = null;
			replacedStatementsCount = -1;
			if (statements == null || statements.get_Count() - startIndex < 2)
			{
				return false;
			}
			V_0 = statements.get_Count() - 1;
			while (V_0 >= 0)
			{
				if (this.TryMatchInternal(statements, V_0, out result, out replacedStatementsCount))
				{
					V_1 = ((result as ExpressionStatement).get_Expression() as BinaryExpression).get_Left();
					if (V_1.get_CodeNodeType() == 26)
					{
						this.FixContext((V_1 as VariableReferenceExpression).get_Variable().Resolve(), 0, replacedStatementsCount - 1, null);
					}
					startIndex = V_0;
					return true;
				}
				V_0 = V_0 - 1;
			}
			return false;
		}

		protected abstract bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount);
	}
}