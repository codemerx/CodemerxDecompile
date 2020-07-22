using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.SwitchByString
{
	internal class SwitchByStringMatcher
	{
		private const string DictionaryType = "System.Collections.Generic.Dictionary`2<System.String,System.Int32>";

		public FieldReferenceExpression DictionaryField
		{
			get;
			private set;
		}

		public VariableReferenceExpression IntVariable
		{
			get;
			private set;
		}

		public VariableReferenceExpression StringVariable
		{
			get;
			private set;
		}

		public SwitchByStringMatcher()
		{
			base();
			return;
		}

		private bool CheckDictionaryAdd(Statement statement, VariableReferenceExpression localDictionaryVariable)
		{
			if (statement as ExpressionStatement == null)
			{
				return false;
			}
			V_0 = (statement as ExpressionStatement).get_Expression() as MethodInvocationExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_MethodExpression().get_Target() == null || !V_0.get_MethodExpression().get_Target().Equals(localDictionaryVariable))
			{
				return false;
			}
			if (!this.IsAddMethod(V_0.get_MethodExpression().get_Method()))
			{
				return false;
			}
			return true;
		}

		private bool CheckDictionaryCreation(Statement firstStatement, out VariableReferenceExpression localDictionaryVariable)
		{
			localDictionaryVariable = null;
			if (firstStatement as ExpressionStatement == null)
			{
				return false;
			}
			V_0 = (firstStatement as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_Operator() != 26)
			{
				return false;
			}
			if (V_0.get_Right() as ObjectCreationExpression == null)
			{
				return false;
			}
			if (String.op_Inequality((V_0.get_Right() as ObjectCreationExpression).get_ExpressionType().get_FullName(), "System.Collections.Generic.Dictionary`2<System.String,System.Int32>"))
			{
				return false;
			}
			if (V_0.get_Left() as VariableReferenceExpression == null)
			{
				return false;
			}
			localDictionaryVariable = V_0.get_Left() as VariableReferenceExpression;
			return true;
		}

		private bool CheckDictionaryFieldAssignExpression(Statement statement, VariableReferenceExpression localDictionaryVariable)
		{
			if (statement as ExpressionStatement == null)
			{
				return false;
			}
			V_0 = (statement as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_0 == null || V_0.get_Operator() != 26)
			{
				return false;
			}
			if (!V_0.get_Left().Equals(this.get_DictionaryField()))
			{
				return false;
			}
			return V_0.get_Right().Equals(localDictionaryVariable);
		}

		private bool CheckDictionaryIf(IfStatement dictionaryIf)
		{
			if (dictionaryIf.get_Else() != null)
			{
				return false;
			}
			if (!this.CheckDictionaryIfCondition(dictionaryIf.get_Condition()))
			{
				return false;
			}
			if (this.get_DictionaryField() == null)
			{
				return false;
			}
			return this.CheckDictionaryIfBody(dictionaryIf.get_Then());
		}

		private bool CheckDictionaryIfBody(BlockStatement then)
		{
			if (then.get_Statements().get_Count() < 1)
			{
				return false;
			}
			if (!this.CheckDictionaryCreation(then.get_Statements().get_Item(0), out V_0))
			{
				return false;
			}
			if (V_0 == null)
			{
				return false;
			}
			V_1 = 1;
			while (V_1 < then.get_Statements().get_Count() - 1)
			{
				if (!this.CheckDictionaryAdd(then.get_Statements().get_Item(V_1), V_0))
				{
					return false;
				}
				V_1 = V_1 + 1;
			}
			return this.CheckDictionaryFieldAssignExpression(then.get_Statements().get_Item(then.get_Statements().get_Count() - 1), V_0);
		}

		private bool CheckDictionaryIfCondition(Expression condition)
		{
			if (condition as BinaryExpression == null)
			{
				return false;
			}
			V_0 = condition as BinaryExpression;
			if (V_0.get_Right() as LiteralExpression == null || (V_0.get_Right() as LiteralExpression).get_Value() != null)
			{
				return false;
			}
			if (V_0.get_Operator() != 9)
			{
				return false;
			}
			if (V_0.get_Left() as FieldReferenceExpression == null)
			{
				return false;
			}
			V_1 = V_0.get_Left() as FieldReferenceExpression;
			if (String.op_Inequality(V_1.get_ExpressionType().get_FullName(), "System.Collections.Generic.Dictionary`2<System.String,System.Int32>"))
			{
				return false;
			}
			if (V_1.get_Field().get_DeclaringType().get_FullName().IndexOf("<PrivateImplementationDetails>") != 0 || V_1.get_Field().get_Name().IndexOf("$$method") != 0)
			{
				return false;
			}
			this.set_DictionaryField(V_1);
			return true;
		}

		private bool CheckIrregularSwitchCaseCondition(BinaryExpression theCondition)
		{
			if (theCondition.get_Operator() == 9)
			{
				if (!theCondition.get_Left().Equals(this.get_IntVariable()) || theCondition.get_Right() as LiteralExpression == null)
				{
					return false;
				}
				return String.op_Equality(theCondition.get_Right().get_ExpressionType().get_FullName(), "System.Int32");
			}
			if (theCondition.get_Operator() != 11)
			{
				return false;
			}
			V_0 = theCondition.get_Left() as BinaryExpression;
			V_1 = theCondition.get_Right() as BinaryExpression;
			if (V_0 == null || V_1 == null)
			{
				return false;
			}
			if (!this.CheckIrregularSwitchCaseCondition(V_0))
			{
				return false;
			}
			return this.CheckIrregularSwitchCaseCondition(V_1);
		}

		private bool CheckOuterIfBody(BlockStatement then)
		{
			if (then.get_Statements().get_Count() != 2)
			{
				return false;
			}
			V_0 = then.get_Statements().get_Item(0) as IfStatement;
			if (V_0 == null || !this.CheckDictionaryIf(V_0))
			{
				return false;
			}
			V_1 = then.get_Statements().get_Item(1) as IfStatement;
			if (V_1 != null && this.CheckSwitchingIf(V_1))
			{
				return true;
			}
			return false;
		}

		private bool CheckSwitchingIf(IfStatement secondIfStatement)
		{
			if (!this.CheckSwitchingIfCondition(secondIfStatement.get_Condition()))
			{
				return false;
			}
			return this.CheckSwitcingIfBody(secondIfStatement.get_Then());
		}

		private bool CheckSwitchingIfCondition(Expression condition)
		{
			V_0 = condition as UnaryExpression;
			if (V_0 == null || V_0.get_Operator() != 11)
			{
				return false;
			}
			V_1 = V_0.get_Operand() as MethodInvocationExpression;
			if (V_1 == null)
			{
				return false;
			}
			if (V_1.get_MethodExpression().get_Target() == null || !V_1.get_MethodExpression().get_Target().Equals(this.get_DictionaryField()))
			{
				return false;
			}
			if (!this.IsTryGetMethod(V_1.get_MethodExpression().get_Method()))
			{
				return false;
			}
			V_2 = V_1.get_Arguments().get_Item(0) as VariableReferenceExpression;
			if (V_2 == null || String.op_Inequality(V_2.get_ExpressionType().get_FullName(), "System.String"))
			{
				return false;
			}
			V_3 = V_1.get_Arguments().get_Item(1) as UnaryExpression;
			if (V_3 == null || V_3.get_Operator() != 7)
			{
				return false;
			}
			V_4 = V_3.get_Operand() as VariableReferenceExpression;
			if (V_4 == null || String.op_Inequality(V_4.get_ExpressionType().get_FullName(), "System.Int32"))
			{
				return false;
			}
			this.set_StringVariable(V_2);
			this.set_IntVariable(V_4);
			return true;
		}

		private bool CheckSwitcingIfBody(BlockStatement body)
		{
			if (body.get_Statements().get_Count() < 1)
			{
				return false;
			}
			V_0 = body.get_Statements().get_Item(0);
			if (V_0 as SwitchStatement != null)
			{
				if ((V_0 as SwitchStatement).get_Condition().Equals(this.get_IntVariable()))
				{
					return true;
				}
				return false;
			}
			if (V_0 as IfElseIfStatement == null)
			{
				return false;
			}
			V_1 = (V_0 as IfElseIfStatement).get_ConditionBlocks().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_Key() as BinaryExpression;
					if (V_3 != null && this.CheckIrregularSwitchCaseCondition(V_3))
					{
						continue;
					}
					V_4 = false;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
		Label1:
			return V_4;
		Label0:
			return true;
		}

		private bool IsAddMethod(MethodReference method)
		{
			if (String.op_Inequality(method.get_DeclaringType().get_FullName(), "System.Collections.Generic.Dictionary`2<System.String,System.Int32>"))
			{
				return false;
			}
			if (String.op_Inequality(method.get_Name(), "Add"))
			{
				return false;
			}
			if (method.get_Parameters().get_Count() != 2)
			{
				return false;
			}
			if (String.op_Inequality(method.get_Parameters().get_Item(0).ResolveParameterType(method).get_FullName(), "System.String"))
			{
				return false;
			}
			if (String.op_Inequality(method.get_Parameters().get_Item(1).ResolveParameterType(method).get_FullName(), "System.Int32"))
			{
				return false;
			}
			return true;
		}

		private bool IsNullCheck(Expression condition)
		{
			if (condition as BinaryExpression == null)
			{
				return false;
			}
			V_0 = condition as BinaryExpression;
			if (V_0.get_Operator() == 10 && V_0.get_Right() as LiteralExpression != null && (V_0.get_Right() as LiteralExpression).get_Value() == null)
			{
				return true;
			}
			return false;
		}

		private bool IsTryGetMethod(MethodReference method)
		{
			if (String.op_Inequality(method.get_DeclaringType().get_FullName(), "System.Collections.Generic.Dictionary`2<System.String,System.Int32>"))
			{
				return false;
			}
			if (String.op_Inequality(method.get_Name(), "TryGetValue"))
			{
				return false;
			}
			if (method.get_Parameters().get_Count() != 2)
			{
				return false;
			}
			if (String.op_Inequality(method.get_Parameters().get_Item(0).ResolveParameterType(method).get_FullName(), "System.String"))
			{
				return false;
			}
			if (String.op_Inequality(method.get_Parameters().get_Item(1).ResolveParameterType(method).get_FullName(), "System.Int32&"))
			{
				return false;
			}
			return true;
		}

		public bool TryMatch(IfStatement node)
		{
			this.set_DictionaryField(null);
			this.set_StringVariable(null);
			this.set_IntVariable(null);
			if (!this.IsNullCheck(node.get_Condition()) && node.get_Else() != null || !this.CheckOuterIfBody(node.get_Then()))
			{
				return false;
			}
			return true;
		}
	}
}