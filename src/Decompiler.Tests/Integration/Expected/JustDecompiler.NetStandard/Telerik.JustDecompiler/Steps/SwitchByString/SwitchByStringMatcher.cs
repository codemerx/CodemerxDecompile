using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
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
		}

		private bool CheckDictionaryAdd(Statement statement, VariableReferenceExpression localDictionaryVariable)
		{
			if (!(statement is ExpressionStatement))
			{
				return false;
			}
			MethodInvocationExpression expression = (statement as ExpressionStatement).Expression as MethodInvocationExpression;
			if (expression == null)
			{
				return false;
			}
			if (expression.MethodExpression.Target == null || !expression.MethodExpression.Target.Equals(localDictionaryVariable))
			{
				return false;
			}
			if (!this.IsAddMethod(expression.MethodExpression.Method))
			{
				return false;
			}
			return true;
		}

		private bool CheckDictionaryCreation(Statement firstStatement, out VariableReferenceExpression localDictionaryVariable)
		{
			localDictionaryVariable = null;
			if (!(firstStatement is ExpressionStatement))
			{
				return false;
			}
			BinaryExpression expression = (firstStatement as ExpressionStatement).Expression as BinaryExpression;
			if (expression == null)
			{
				return false;
			}
			if (expression.Operator != BinaryOperator.Assign)
			{
				return false;
			}
			if (!(expression.Right is ObjectCreationExpression))
			{
				return false;
			}
			if ((expression.Right as ObjectCreationExpression).ExpressionType.get_FullName() != "System.Collections.Generic.Dictionary`2<System.String,System.Int32>")
			{
				return false;
			}
			if (!(expression.Left is VariableReferenceExpression))
			{
				return false;
			}
			localDictionaryVariable = expression.Left as VariableReferenceExpression;
			return true;
		}

		private bool CheckDictionaryFieldAssignExpression(Statement statement, VariableReferenceExpression localDictionaryVariable)
		{
			if (!(statement is ExpressionStatement))
			{
				return false;
			}
			BinaryExpression expression = (statement as ExpressionStatement).Expression as BinaryExpression;
			if (expression == null || expression.Operator != BinaryOperator.Assign)
			{
				return false;
			}
			if (!expression.Left.Equals(this.DictionaryField))
			{
				return false;
			}
			return expression.Right.Equals(localDictionaryVariable);
		}

		private bool CheckDictionaryIf(IfStatement dictionaryIf)
		{
			if (dictionaryIf.Else != null)
			{
				return false;
			}
			if (!this.CheckDictionaryIfCondition(dictionaryIf.Condition))
			{
				return false;
			}
			if (this.DictionaryField == null)
			{
				return false;
			}
			return this.CheckDictionaryIfBody(dictionaryIf.Then);
		}

		private bool CheckDictionaryIfBody(BlockStatement then)
		{
			VariableReferenceExpression variableReferenceExpression;
			if (then.Statements.Count < 1)
			{
				return false;
			}
			if (!this.CheckDictionaryCreation(then.Statements[0], out variableReferenceExpression))
			{
				return false;
			}
			if (variableReferenceExpression == null)
			{
				return false;
			}
			for (int i = 1; i < then.Statements.Count - 1; i++)
			{
				if (!this.CheckDictionaryAdd(then.Statements[i], variableReferenceExpression))
				{
					return false;
				}
			}
			return this.CheckDictionaryFieldAssignExpression(then.Statements[then.Statements.Count - 1], variableReferenceExpression);
		}

		private bool CheckDictionaryIfCondition(Expression condition)
		{
			if (!(condition is BinaryExpression))
			{
				return false;
			}
			BinaryExpression binaryExpression = condition as BinaryExpression;
			if (!(binaryExpression.Right is LiteralExpression) || (binaryExpression.Right as LiteralExpression).Value != null)
			{
				return false;
			}
			if (binaryExpression.Operator != BinaryOperator.ValueEquality)
			{
				return false;
			}
			if (!(binaryExpression.Left is FieldReferenceExpression))
			{
				return false;
			}
			FieldReferenceExpression left = binaryExpression.Left as FieldReferenceExpression;
			if (left.ExpressionType.get_FullName() != "System.Collections.Generic.Dictionary`2<System.String,System.Int32>")
			{
				return false;
			}
			if (left.Field.get_DeclaringType().get_FullName().IndexOf("<PrivateImplementationDetails>") != 0 || left.Field.get_Name().IndexOf("$$method") != 0)
			{
				return false;
			}
			this.DictionaryField = left;
			return true;
		}

		private bool CheckIrregularSwitchCaseCondition(BinaryExpression theCondition)
		{
			if (theCondition.Operator == BinaryOperator.ValueEquality)
			{
				if (!theCondition.Left.Equals(this.IntVariable) || !(theCondition.Right is LiteralExpression))
				{
					return false;
				}
				return theCondition.Right.ExpressionType.get_FullName() == "System.Int32";
			}
			if (theCondition.Operator != BinaryOperator.LogicalOr)
			{
				return false;
			}
			BinaryExpression left = theCondition.Left as BinaryExpression;
			BinaryExpression right = theCondition.Right as BinaryExpression;
			if (left == null || right == null)
			{
				return false;
			}
			if (!this.CheckIrregularSwitchCaseCondition(left))
			{
				return false;
			}
			return this.CheckIrregularSwitchCaseCondition(right);
		}

		private bool CheckOuterIfBody(BlockStatement then)
		{
			if (then.Statements.Count != 2)
			{
				return false;
			}
			IfStatement item = then.Statements[0] as IfStatement;
			if (item == null || !this.CheckDictionaryIf(item))
			{
				return false;
			}
			IfStatement ifStatement = then.Statements[1] as IfStatement;
			if (ifStatement != null && this.CheckSwitchingIf(ifStatement))
			{
				return true;
			}
			return false;
		}

		private bool CheckSwitchingIf(IfStatement secondIfStatement)
		{
			if (!this.CheckSwitchingIfCondition(secondIfStatement.Condition))
			{
				return false;
			}
			return this.CheckSwitcingIfBody(secondIfStatement.Then);
		}

		private bool CheckSwitchingIfCondition(Expression condition)
		{
			UnaryExpression unaryExpression = condition as UnaryExpression;
			if (unaryExpression == null || unaryExpression.Operator != UnaryOperator.None)
			{
				return false;
			}
			MethodInvocationExpression operand = unaryExpression.Operand as MethodInvocationExpression;
			if (operand == null)
			{
				return false;
			}
			if (operand.MethodExpression.Target == null || !operand.MethodExpression.Target.Equals(this.DictionaryField))
			{
				return false;
			}
			if (!this.IsTryGetMethod(operand.MethodExpression.Method))
			{
				return false;
			}
			VariableReferenceExpression item = operand.Arguments[0] as VariableReferenceExpression;
			if (item == null || item.ExpressionType.get_FullName() != "System.String")
			{
				return false;
			}
			UnaryExpression item1 = operand.Arguments[1] as UnaryExpression;
			if (item1 == null || item1.Operator != UnaryOperator.AddressReference)
			{
				return false;
			}
			VariableReferenceExpression variableReferenceExpression = item1.Operand as VariableReferenceExpression;
			if (variableReferenceExpression == null || variableReferenceExpression.ExpressionType.get_FullName() != "System.Int32")
			{
				return false;
			}
			this.StringVariable = item;
			this.IntVariable = variableReferenceExpression;
			return true;
		}

		private bool CheckSwitcingIfBody(BlockStatement body)
		{
			bool flag;
			if (body.Statements.Count < 1)
			{
				return false;
			}
			Statement item = body.Statements[0];
			if (item is SwitchStatement)
			{
				if ((item as SwitchStatement).Condition.Equals(this.IntVariable))
				{
					return true;
				}
				return false;
			}
			if (!(item is IfElseIfStatement))
			{
				return false;
			}
			List<KeyValuePair<Expression, BlockStatement>>.Enumerator enumerator = (item as IfElseIfStatement).ConditionBlocks.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					BinaryExpression key = enumerator.Current.Key as BinaryExpression;
					if (key != null && this.CheckIrregularSwitchCaseCondition(key))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool IsAddMethod(MethodReference method)
		{
			if (method.get_DeclaringType().get_FullName() != "System.Collections.Generic.Dictionary`2<System.String,System.Int32>")
			{
				return false;
			}
			if (method.get_Name() != "Add")
			{
				return false;
			}
			if (method.get_Parameters().get_Count() != 2)
			{
				return false;
			}
			if (method.get_Parameters().get_Item(0).ResolveParameterType(method).get_FullName() != "System.String")
			{
				return false;
			}
			if (method.get_Parameters().get_Item(1).ResolveParameterType(method).get_FullName() != "System.Int32")
			{
				return false;
			}
			return true;
		}

		private bool IsNullCheck(Expression condition)
		{
			if (!(condition is BinaryExpression))
			{
				return false;
			}
			BinaryExpression binaryExpression = condition as BinaryExpression;
			if (binaryExpression.Operator == BinaryOperator.ValueInequality && binaryExpression.Right is LiteralExpression && (binaryExpression.Right as LiteralExpression).Value == null)
			{
				return true;
			}
			return false;
		}

		private bool IsTryGetMethod(MethodReference method)
		{
			if (method.get_DeclaringType().get_FullName() != "System.Collections.Generic.Dictionary`2<System.String,System.Int32>")
			{
				return false;
			}
			if (method.get_Name() != "TryGetValue")
			{
				return false;
			}
			if (method.get_Parameters().get_Count() != 2)
			{
				return false;
			}
			if (method.get_Parameters().get_Item(0).ResolveParameterType(method).get_FullName() != "System.String")
			{
				return false;
			}
			if (method.get_Parameters().get_Item(1).ResolveParameterType(method).get_FullName() != "System.Int32&")
			{
				return false;
			}
			return true;
		}

		public bool TryMatch(IfStatement node)
		{
			this.DictionaryField = null;
			this.StringVariable = null;
			this.IntVariable = null;
			if (!this.IsNullCheck(node.Condition) && node.Else != null || !this.CheckOuterIfBody(node.Then))
			{
				return false;
			}
			return true;
		}
	}
}