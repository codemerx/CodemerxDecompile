using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
namespace Telerik.JustDecompiler.Steps.SwitchByString
{
	class SwitchByStringMatcher
	{
		private const string DictionaryType = "System.Collections.Generic.Dictionary`2<System.String,System.Int32>";
		public FieldReferenceExpression DictionaryField { get; private set; }
		public VariableReferenceExpression StringVariable { get; private set; }
		public VariableReferenceExpression IntVariable { get; private set; }

		public bool TryMatch(IfStatement node)
		{
			this.DictionaryField = null;
			this.StringVariable = null;
			this.IntVariable = null;
			if (IsNullCheck(node.Condition) || node.Else == null)
			{
				if (CheckOuterIfBody(node.Then))
				{
					return true;
				}
			}
			return false;
		}
  
		private bool CheckOuterIfBody(BlockStatement then)
		{
			//2 statements -> the first is the check if the dictionary is initialized
			// the second is taking the int representation of the string and the actual switch
			if (then.Statements.Count != 2)
			{
				return false;
			}

			IfStatement firstIfStatement = then.Statements[0] as IfStatement;
			if (firstIfStatement == null || !CheckDictionaryIf(firstIfStatement))
			{
				return false;
			}

			IfStatement secondIfStatement = then.Statements[1] as IfStatement;
			if(secondIfStatement == null || !CheckSwitchingIf(secondIfStatement))
			{
				return false;
			}

			return true;
		}
  
		#region Dictionary Creation

		private bool CheckDictionaryIf(IfStatement dictionaryIf)
		{
			if (dictionaryIf.Else != null)
			{
				return false;
			}
			if(!CheckDictionaryIfCondition(dictionaryIf.Condition))
			{
				return false;
			}
			if (DictionaryField == null)
			{
				// This is sanity check.
				// If the check for the condition succeesd, then the condition field must be set.
				return false;
			}
			return CheckDictionaryIfBody(dictionaryIf.Then);
		}
  
		private bool CheckDictionaryIfBody(BlockStatement then)
		{
			/// Check for the pattern
			/// someVariable = new Dictionary<string,int>();
			/// someVariable.Add("SomeString",0);
			/// <moreAdds>
			/// conditionField = someField;
			if (then.Statements.Count < 1)
			{ 
				return false;
			}
			VariableReferenceExpression localDictionaryVariable;
			if (!CheckDictionaryCreation(then.Statements[0], out localDictionaryVariable ))
			{
				return false;
			}
			if (localDictionaryVariable == null)
			{
				// sanity check.
				return false;
			}

			for (int i = 1; i < then.Statements.Count - 1; i++)
			{
				// check push expressions
				if (!CheckDictionaryAdd(then.Statements[i], localDictionaryVariable))
				{
					return false;
				}
			}

			return CheckDictionaryFieldAssignExpression(then.Statements[then.Statements.Count - 1], localDictionaryVariable);
		}
  
		private bool CheckDictionaryCreation(Statement firstStatement, out VariableReferenceExpression localDictionaryVariable)
		{
			localDictionaryVariable = null;
			if (!(firstStatement is ExpressionStatement))
			{
				return false;
			}

			BinaryExpression creation = (firstStatement as ExpressionStatement).Expression as BinaryExpression;
			if (creation == null)
			{
				return false;
			}
			if (creation.Operator != BinaryOperator.Assign)
			{
				return false;
			}

			if (! (creation.Right is ObjectCreationExpression))
			{
				return false;
			}

			ObjectCreationExpression dictCreation = creation.Right as ObjectCreationExpression;
			if (dictCreation.ExpressionType.FullName != DictionaryType)
			{
				return false;
			}

			if (!( creation.Left is VariableReferenceExpression))
			{
				return false;
			}
			localDictionaryVariable = creation.Left as VariableReferenceExpression;
			return true;
		}
		
		private bool CheckDictionaryAdd(Statement statement, VariableReferenceExpression localDictionaryVariable)
		{
			if (!(statement is ExpressionStatement))
			{
				return false;
			}

			MethodInvocationExpression addInvocation = (statement as ExpressionStatement).Expression as MethodInvocationExpression;
			if (addInvocation == null)
			{
				return false;
			}

			if (addInvocation.MethodExpression.Target == null || !addInvocation.MethodExpression.Target.Equals(localDictionaryVariable))
			{
				return false;
			}

			if(!IsAddMethod(addInvocation.MethodExpression.Method))
			{
				return false;
			}
			return true;
		}

		private bool CheckDictionaryFieldAssignExpression(Statement statement, VariableReferenceExpression localDictionaryVariable)
		{
			if (!(statement is ExpressionStatement))
			{
				return false;
			}

			BinaryExpression assignment = (statement as ExpressionStatement).Expression as BinaryExpression;
			if (assignment == null || assignment.Operator != BinaryOperator.Assign)
			{
				return false;
			}

			return assignment.Left.Equals(DictionaryField) && assignment.Right.Equals(localDictionaryVariable);
		}

		/// <summary>
		/// Checks the condition of the if, that creates the switch dictionary.
		/// </summary>
		/// <param name="condition"></param>
		/// <returns></returns>
		private bool CheckDictionaryIfCondition(Expression condition)
		{
			/// Check for the pattern
			/// <PrivateImplementationDetails>*Something*.$$method*Something* == null
			if (!(condition is BinaryExpression))
			{
				return false;
			}

			BinaryExpression binaryCondition = condition as BinaryExpression;

			if (!(binaryCondition.Right is LiteralExpression) || (binaryCondition.Right as LiteralExpression).Value != null)
			{
				return false;
			}
			if (binaryCondition.Operator != BinaryOperator.ValueEquality)
			{
				return false;
			}

			if (!(binaryCondition.Left is FieldReferenceExpression))
			{
				return false;
			}

			FieldReferenceExpression conditionField = binaryCondition.Left as FieldReferenceExpression;
			if (conditionField.ExpressionType.FullName != DictionaryType)
			{
				return false;
			}

			if (conditionField.Field.DeclaringType.FullName.IndexOf("<PrivateImplementationDetails>") != 0 ||
				conditionField.Field.Name.IndexOf("$$method") != 0)
			{
				return false;
			}
			this.DictionaryField = conditionField;
			return true;
		}

		private bool IsAddMethod(MethodReference method)
		{
			if (method.DeclaringType.FullName != DictionaryType)
			{
				return false;
			}

			if (method.Name != "Add")
			{
				return false;
			}

			if (method.Parameters.Count != 2)
			{
				return false;
			}

			TypeReference firstParameterType = method.Parameters[0].ResolveParameterType(method);
			if (firstParameterType.FullName != "System.String")
			{
				return false;
			}

			TypeReference secondParameterType = method.Parameters[1].ResolveParameterType(method);
			if (secondParameterType.FullName != "System.Int32")
			{
				return false;
			}

			return true;
		}


		#endregion

		#region Switching if

		private bool CheckSwitchingIf(IfStatement secondIfStatement)
		{
			if (!CheckSwitchingIfCondition(secondIfStatement.Condition))
			{
				return false;
			}
			return CheckSwitcingIfBody(secondIfStatement.Then);
		}
  
		private bool CheckSwitcingIfBody(BlockStatement body)
		{
			if (body.Statements.Count < 1)
			{
				return false;
			}

			Statement theStatement = body.Statements[0];
			if (theStatement is SwitchStatement)
			{
				SwitchStatement theSwitch = theStatement as SwitchStatement;
				if (theSwitch.Condition.Equals(IntVariable))
				{
					return true;
				}
				return false;
			}
			if (theStatement is IfElseIfStatement)
			{
				IfElseIfStatement theIrregularSwitch = theStatement as IfElseIfStatement;
				foreach (KeyValuePair<Expression, BlockStatement> condBlock in theIrregularSwitch.ConditionBlocks)
				{
					BinaryExpression theCondition = condBlock.Key as BinaryExpression;
					if (theCondition == null || !CheckIrregularSwitchCaseCondition(theCondition))
					{ 
						return false;
					}
				}
				return true;
			}
			return false;
		}
  
		private bool CheckIrregularSwitchCaseCondition(BinaryExpression theCondition)
		{
			if (theCondition.Operator == BinaryOperator.ValueEquality)
			{
				return theCondition.Left.Equals(IntVariable) && theCondition.Right is LiteralExpression 
					&& theCondition.Right.ExpressionType.FullName == "System.Int32";
			}
			if (theCondition.Operator == BinaryOperator.LogicalOr)
			{
				BinaryExpression leftSide = theCondition.Left as BinaryExpression;
				BinaryExpression rightSide = theCondition.Right as BinaryExpression;
				if (leftSide == null || rightSide == null)
				{
					return false;
				}
				return CheckIrregularSwitchCaseCondition(leftSide) && CheckIrregularSwitchCaseCondition(rightSide);
			}
			return false;
		}
  
		private bool CheckSwitchingIfCondition(Expression condition)
		{
			UnaryExpression unaryCondition = condition as UnaryExpression;
			if (unaryCondition == null || unaryCondition.Operator != UnaryOperator.None)
			{
				return false;
			}

			MethodInvocationExpression unaryOperand = unaryCondition.Operand as MethodInvocationExpression;
			if (unaryOperand == null)
			{
				return false;
			}

			if (unaryOperand.MethodExpression.Target == null || !unaryOperand.MethodExpression.Target.Equals(DictionaryField))
			{
				return false;
			}

			if(!IsTryGetMethod(unaryOperand.MethodExpression.Method))
			{
				return false;
			}

			VariableReferenceExpression firstArgument = unaryOperand.Arguments[0] as VariableReferenceExpression;
			if (firstArgument == null || firstArgument.ExpressionType.FullName != "System.String")
			{
				return false;
			}
			UnaryExpression secondArgument = unaryOperand.Arguments[1] as UnaryExpression;
			if (secondArgument == null || secondArgument.Operator != UnaryOperator.AddressReference)
			{
				return false;
			}
			VariableReferenceExpression secondArgumentOperand = secondArgument.Operand as VariableReferenceExpression;
			if (secondArgumentOperand == null || secondArgumentOperand.ExpressionType.FullName != "System.Int32")
			{
				return false;
			}

			StringVariable = firstArgument;
			IntVariable = secondArgumentOperand;
			return true;
		}
  
		private bool IsTryGetMethod(MethodReference method)
		{
			if (method.DeclaringType.FullName != DictionaryType)
			{
				return false;
			}

			if (method.Name != "TryGetValue")
			{
				return false;
			}

			if (method.Parameters.Count != 2)
			{
				return false;
			}

			TypeReference firstParameterType = method.Parameters[0].ResolveParameterType(method);
			if (firstParameterType.FullName != "System.String")
			{
				return false;
			}

			TypeReference secondParameterType = method.Parameters[1].ResolveParameterType(method);
			if (secondParameterType.FullName != "System.Int32&")
			{
				return false;
			}

			return true;
		}

		#endregion

		private bool IsNullCheck(Expression condition)
		{
			if (!(condition is BinaryExpression))
			{
				return false;
			}
			BinaryExpression binaryCond = condition as BinaryExpression;
			if (binaryCond.Operator == BinaryOperator.ValueInequality)
			{
				if (binaryCond.Right is LiteralExpression && (binaryCond.Right as LiteralExpression).Value == null)
				{
					return true;
				}
			}
			return false;
		}
	}
}
