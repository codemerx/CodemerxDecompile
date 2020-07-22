using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class InitializationPattern : CommonPatterns, ICodePattern
	{
		private readonly TypeSpecificContext typeContext;

		private readonly MethodDefinition method;

		public InitializationPattern(CodePatternsContext patternsContext, DecompilationContext context)
		{
			base(patternsContext, context.get_MethodContext().get_Method().get_Module().get_TypeSystem());
			this.typeContext = context.get_TypeContext();
			this.method = context.get_MethodContext().get_Method();
			return;
		}

		private bool IsAutoPropertyAssignment(ExpressionStatement propertyAssignmentStatement, out Expression assignedValue, out string propertyFullName)
		{
			propertyFullName = null;
			assignedValue = null;
			V_0 = propertyAssignmentStatement.get_Expression() as BinaryExpression;
			if (V_0 == null || !V_0.get_IsAssignmentExpression() || V_0.get_Left().get_CodeNodeType() != 91)
			{
				return false;
			}
			V_1 = (V_0.get_Left() as AutoPropertyConstructorInitializerExpression).get_Property();
			if (V_1 == null)
			{
				return false;
			}
			assignedValue = V_0.get_Right();
			propertyFullName = V_1.get_FullName();
			return true;
		}

		private bool IsFieldAssignment(ExpressionStatement fieldAssignmentStatement, out Expression assignedValue, out string fieldFullName)
		{
			fieldFullName = null;
			assignedValue = null;
			V_0 = fieldAssignmentStatement.get_Expression() as BinaryExpression;
			if (V_0 == null || !V_0.get_IsAssignmentExpression() || V_0.get_Left().get_CodeNodeType() != 30)
			{
				return false;
			}
			V_1 = (V_0.get_Left() as FieldReferenceExpression).get_Field();
			if (V_1 == null)
			{
				return false;
			}
			assignedValue = V_0.get_Right();
			fieldFullName = V_1.get_FullName();
			return true;
		}

		private bool MapAssignmentIntoContext(string memberFullName, Expression assignedValue)
		{
			this.typeContext.get_AssignmentData().Add(memberFullName, new InitializationAssignment(this.method, assignedValue));
			return true;
		}

		public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			startIndex = 0;
			if (this.TryMatchArrayAssignmentInternal(statements))
			{
				replacedStatementsCount = 2;
				return true;
			}
			replacedStatementsCount = 1;
			return this.TryMatchDirectAssignmentInternal(statements);
		}

		private bool TryMatchArrayAssignmentInternal(StatementCollection statements)
		{
			if (statements.get_Count() < 2)
			{
				return false;
			}
			V_0 = statements.get_Item(0) as ExpressionStatement;
			if (V_0 == null)
			{
				return false;
			}
			V_1 = V_0.get_Expression() as BinaryExpression;
			if (V_1 == null || !this.IsAssignToVariableExpression(V_1, out V_2))
			{
				return false;
			}
			V_3 = V_1.get_Right();
			V_4 = statements.get_Item(1) as ExpressionStatement;
			if (V_4 == null)
			{
				return false;
			}
			if (!this.IsFieldAssignment(V_4, out V_5, out V_6) && !this.IsAutoPropertyAssignment(V_4, out V_5, out V_6))
			{
				return false;
			}
			if (V_5.get_CodeNodeType() != 26 || (object)(V_5 as VariableReferenceExpression).get_Variable() != (object)V_2)
			{
				return false;
			}
			return this.MapAssignmentIntoContext(V_6, V_3);
		}

		private bool TryMatchDirectAssignmentInternal(StatementCollection statements)
		{
			V_0 = statements.get_Item(0) as ExpressionStatement;
			if (V_0 == null || !String.IsNullOrEmpty(statements.get_Item(0).get_Label()))
			{
				return false;
			}
			if (!this.IsFieldAssignment(V_0, out V_2, out V_1) && !this.IsAutoPropertyAssignment(V_0, out V_2, out V_1))
			{
				return false;
			}
			return this.MapAssignmentIntoContext(V_1, V_2);
		}
	}
}