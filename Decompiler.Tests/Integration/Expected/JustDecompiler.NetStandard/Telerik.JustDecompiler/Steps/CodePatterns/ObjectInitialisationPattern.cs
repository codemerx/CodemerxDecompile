using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class ObjectInitialisationPattern : BaseInitialisationPattern
	{
		public ObjectInitialisationPattern(CodePatternsContext patternsContext, TypeSystem typeSystem)
		{
			base(patternsContext, typeSystem);
			return;
		}

		private string GetName(Expression initializer)
		{
			V_0 = initializer.get_CodeNodeType();
			if (V_0 == 85)
			{
				return (initializer as PropertyInitializerExpression).get_Property().get_Name();
			}
			if (V_0 != 86)
			{
				throw new ArgumentException("Expected field or property");
			}
			return (initializer as FieldInitializerExpression).get_Field().get_Name();
		}

		private bool IsObjectPropertyOrFieldAssignment(BinaryExpression assignment, Expression assignee)
		{
			if (assignment == null || !assignment.get_IsAssignmentExpression())
			{
				return false;
			}
			V_2 = assignment.get_Left().get_CodeNodeType();
			if (V_2 == 30)
			{
				V_1 = assignment.get_Left() as FieldReferenceExpression;
				return this.CompareTargets(assignee, V_1.get_Target());
			}
			if (V_2 != 42)
			{
				return false;
			}
			V_0 = assignment.get_Left() as PropertyReferenceExpression;
			if (!this.CompareTargets(assignee, V_0.get_Target()))
			{
				return false;
			}
			if (V_0.get_IsSetter() && !V_0.get_IsIndexer())
			{
				return true;
			}
			return false;
		}

		protected override bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 0;
			if (!this.TryGetObjectCreation(statements, startIndex, out V_0, out V_1))
			{
				return false;
			}
			V_2 = new ExpressionCollection();
			V_3 = new HashSet<string>();
			if (V_0.get_Initializer() != null)
			{
				if (V_0.get_Initializer().get_InitializerType() != 1)
				{
					return false;
				}
				V_4 = V_0.get_Initializer().get_Expressions().GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_5 = V_4.get_Current();
						V_6 = this.GetName((V_5 as BinaryExpression).get_Left());
						dummyVar0 = V_3.Add(V_6);
					}
				}
				finally
				{
					if (V_4 != null)
					{
						V_4.Dispose();
					}
				}
			}
			V_7 = startIndex + 1;
			while (V_7 < statements.get_Count() && this.TryGetNextExpression(statements.get_Item(V_7), out V_8))
			{
				V_9 = V_8 as BinaryExpression;
				if (!this.IsObjectPropertyOrFieldAssignment(V_9, V_1))
				{
					break;
				}
				V_10 = null;
				if (V_9.get_Left().get_CodeNodeType() != 42)
				{
					if (V_9.get_Left().get_CodeNodeType() == 30)
					{
						V_13 = (V_9.get_Left() as FieldReferenceExpression).get_Field().Resolve();
						if (!this.Visit(V_13.get_Name(), V_3))
						{
							break;
						}
						V_10 = new FieldInitializerExpression(V_13, V_13.get_FieldType(), V_9.get_Left().get_UnderlyingSameMethodInstructions());
					}
				}
				else
				{
					V_12 = (V_9.get_Left() as PropertyReferenceExpression).get_Property();
					if (!this.Visit(V_12.get_Name(), V_3))
					{
						break;
					}
					V_10 = new PropertyInitializerExpression(V_12, V_12.get_PropertyType(), V_9.get_Left().get_UnderlyingSameMethodInstructions());
				}
				V_11 = new BinaryExpression(26, V_10, V_9.get_Right().Clone(), this.typeSystem, null, false);
				V_2.Add(V_11);
				V_7 = V_7 + 1;
			}
			if (V_2.get_Count() == 0)
			{
				return false;
			}
			if (V_0.get_Initializer() != null)
			{
				V_4 = V_2.GetEnumerator();
				try
				{
					while (V_4.MoveNext())
					{
						V_15 = V_4.get_Current();
						V_0.get_Initializer().get_Expressions().Add(V_15);
					}
				}
				finally
				{
					if (V_4 != null)
					{
						V_4.Dispose();
					}
				}
			}
			else
			{
				V_14 = new InitializerExpression(V_2, 1);
				V_14.set_IsMultiLine(true);
				V_0.set_Initializer(V_14);
			}
			result = statements.get_Item(startIndex);
			replacedStatementsCount = V_2.get_Count() + 1;
			return true;
		}

		private bool Visit(string name, ICollection<string> visitedPropertyNames)
		{
			if (visitedPropertyNames.Contains(name))
			{
				return false;
			}
			visitedPropertyNames.Add(name);
			return true;
		}
	}
}