using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal static class CatchClausesFilterPattern
	{
		private static bool TryGetVariableDeclaration(ExpressionStatement statement, VariableReferenceExpression variableReference, ref VariableDefinition variableDefinition, ref IEnumerable<Instruction> instructions)
		{
			if (statement != null)
			{
				V_0 = statement.get_Expression() as BinaryExpression;
				if (V_0 != null && V_0.get_IsAssignmentExpression())
				{
					V_1 = V_0.get_Left() as VariableReferenceExpression;
					V_2 = V_0.get_Right() as VariableReferenceExpression;
					if (V_1 != null && V_2 != null && V_2.Equals(variableReference))
					{
						variableDefinition = V_1.get_Variable().Resolve();
						instructions = V_1.get_MappedInstructions();
						return true;
					}
				}
			}
			return false;
		}

		public static bool TryMatch(BlockStatement filter, out VariableDeclarationExpression variableDeclaration, out Expression filterExpression)
		{
			variableDeclaration = null;
			filterExpression = null;
			if (!CatchClausesFilterPattern.TryMatchVariableDeclaration(filter, out variableDeclaration))
			{
				return false;
			}
			if (filter.get_Statements().get_Count() != 3)
			{
				return false;
			}
			V_0 = filter.get_Statements().get_Item(1) as IfStatement;
			if (V_0 == null)
			{
				return false;
			}
			V_1 = null;
			V_2 = null;
			if ((V_0.get_Condition() as BinaryExpression).get_Operator() != 10)
			{
				V_1 = V_0.get_Else();
				V_2 = V_0.get_Then();
			}
			else
			{
				V_1 = V_0.get_Then();
				V_2 = V_0.get_Else();
			}
			V_3 = null;
			V_4 = null;
			if (V_1.get_Statements().get_Count() != 1 && V_1.get_Statements().get_Count() != 2 && V_1.get_Statements().get_Count() != 3 || V_2.get_Statements().get_Count() != 1)
			{
				return false;
			}
			if (V_1.get_Statements().get_Count() != 2)
			{
				if (V_1.get_Statements().get_Count() == 3)
				{
					V_8 = V_1.get_Statements().get_Item(0) as ExpressionStatement;
					V_9 = V_1.get_Statements().get_Item(1) as ExpressionStatement;
					if (V_8 == null || V_9 == null)
					{
						return false;
					}
					if (V_8.get_Expression().get_CodeNodeType() != 24 || V_9.get_Expression().get_CodeNodeType() != 19)
					{
						if (V_8.get_Expression().get_CodeNodeType() != 19 || V_9.get_Expression().get_CodeNodeType() != 24)
						{
							return false;
						}
						V_4 = V_8;
						V_3 = V_9;
					}
					else
					{
						V_3 = V_8;
						V_4 = V_9;
					}
				}
			}
			else
			{
				V_7 = V_1.get_Statements().get_Item(0) as ExpressionStatement;
				if (V_7 == null)
				{
					return false;
				}
				if (V_7.get_Expression().get_CodeNodeType() != 24)
				{
					if (V_7.get_Expression().get_CodeNodeType() != 19)
					{
						return false;
					}
					V_4 = V_7;
				}
				else
				{
					V_3 = V_7;
				}
			}
			if (V_3 != null)
			{
				V_10 = V_3.get_Expression() as BinaryExpression;
				if (V_10 == null || !V_10.get_IsAssignmentExpression() || String.op_Inequality(V_10.get_ExpressionType().get_FullName(), variableDeclaration.get_ExpressionType().get_FullName()))
				{
					return false;
				}
				stackVariable98 = V_10.get_Left() as VariableReferenceExpression;
				V_11 = V_10.get_Right() as VariableReferenceExpression;
				if (stackVariable98 == null || V_11 == null)
				{
					return false;
				}
			}
			if (V_4 != null)
			{
				V_12 = V_4.get_Expression() as MethodInvocationExpression;
				if (V_12 == null || String.op_Inequality(V_12.get_MethodExpression().get_Method().get_FullName(), "System.Void Microsoft.VisualBasic.CompilerServices.ProjectData::SetProjectError(System.Exception)"))
				{
					return false;
				}
			}
			V_5 = filter.get_Statements().get_Item(2) as ExpressionStatement;
			if (V_5 == null)
			{
				return false;
			}
			V_6 = V_5.get_Expression() as VariableReferenceExpression;
			if (V_6 == null)
			{
				return false;
			}
			if (!CatchClausesFilterPattern.TryMatchFilterExpression(V_0, variableDeclaration.get_Variable().get_VariableType(), V_6, out filterExpression))
			{
				return false;
			}
			return true;
		}

		private static bool TryMatchFilterExpression(IfStatement ifStatement, TypeReference variableType, VariableReferenceExpression lastExpression, out Expression filterExpression)
		{
			filterExpression = null;
			if ((ifStatement.get_Condition() as BinaryExpression).get_Operator() != 10)
			{
				V_0 = ifStatement.get_Else().get_Statements().get_Item(ifStatement.get_Then().get_Statements().get_Count() - 1) as ExpressionStatement;
			}
			else
			{
				V_0 = ifStatement.get_Then().get_Statements().get_Item(ifStatement.get_Then().get_Statements().get_Count() - 1) as ExpressionStatement;
			}
			if (V_0 == null)
			{
				return false;
			}
			V_1 = V_0.get_Expression() as BinaryExpression;
			if (!V_1.get_IsAssignmentExpression())
			{
				return false;
			}
			if (String.op_Inequality(V_1.get_ExpressionType().get_FullName(), "System.Boolean"))
			{
				return false;
			}
			V_2 = V_1.get_Left() as VariableReferenceExpression;
			if (V_2 == null)
			{
				return false;
			}
			if (!V_2.Equals(lastExpression))
			{
				return false;
			}
			filterExpression = V_1.get_Right();
			return true;
		}

		public static bool TryMatchMethodStructure(BlockStatement blockStatement)
		{
			V_0 = blockStatement.get_Statements().Last<Statement>() as ExpressionStatement;
			if (V_0 == null)
			{
				return false;
			}
			V_1 = V_0.get_Expression() as VariableReferenceExpression;
			if (V_1 == null)
			{
				return false;
			}
			if (String.op_Inequality(V_1.get_ExpressionType().get_FullName(), "System.Boolean"))
			{
				return false;
			}
			return true;
		}

		private static bool TryMatchVariableDeclaration(BlockStatement filter, out VariableDeclarationExpression variableDeclaration)
		{
			variableDeclaration = null;
			V_0 = filter.get_Statements().get_Item(0) as ExpressionStatement;
			if (V_0 == null)
			{
				return false;
			}
			V_1 = V_0.get_Expression() as BinaryExpression;
			if (V_1 == null || !V_1.get_IsAssignmentExpression())
			{
				return false;
			}
			V_2 = V_1.get_Left() as VariableReferenceExpression;
			if (V_2 == null)
			{
				return false;
			}
			if (V_1.get_Right() as SafeCastExpression == null)
			{
				return false;
			}
			V_3 = filter.get_Statements().get_Item(1) as IfStatement;
			if (V_3 == null)
			{
				return false;
			}
			V_4 = V_3.get_Condition() as BinaryExpression;
			if (V_4 == null)
			{
				return false;
			}
			V_5 = V_4.get_Left() as VariableReferenceExpression;
			if (V_5 == null)
			{
				return false;
			}
			V_6 = V_4.get_Right() as LiteralExpression;
			if (V_6 == null)
			{
				return false;
			}
			if (!V_5.Equals(V_2) || V_6.get_Value() != null || V_4.get_Operator() != 9 && V_4.get_Operator() != 10)
			{
				return false;
			}
			V_7 = V_2.get_Variable().Resolve();
			V_8 = V_2.get_MappedInstructions();
			if (V_4.get_Operator() != 10)
			{
				V_9 = V_3.get_Else();
			}
			else
			{
				V_9 = V_3.get_Then();
			}
			if (!CatchClausesFilterPattern.TryGetVariableDeclaration(V_9.get_Statements().get_Item(0) as ExpressionStatement, V_2, ref V_7, ref V_8) && V_9.get_Statements().get_Count() >= 2)
			{
				dummyVar0 = CatchClausesFilterPattern.TryGetVariableDeclaration(V_9.get_Statements().get_Item(1) as ExpressionStatement, V_2, ref V_7, ref V_8);
			}
			variableDeclaration = new VariableDeclarationExpression(V_7, V_8);
			return true;
		}
	}
}