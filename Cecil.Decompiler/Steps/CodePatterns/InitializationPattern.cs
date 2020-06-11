using System;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	class InitializationPattern : CommonPatterns, ICodePattern
	{
		private readonly TypeSpecificContext typeContext;
		private readonly MethodDefinition method;

		public InitializationPattern(CodePatternsContext patternsContext, DecompilationContext context)
			: base(patternsContext, context.MethodContext.Method.Module.TypeSystem)
		{
            this.typeContext = context.TypeContext;
			this.method = context.MethodContext.Method;
		}

		public bool TryMatch(StatementCollection statements,out int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			startIndex = 0;
			bool matched = TryMatchArrayAssignmentInternal(statements);
			if (matched)
			{
				replacedStatementsCount = 2;
				return true;
			}
			replacedStatementsCount = 1;
			return TryMatchDirectAssignmentInternal(statements);
		}

		/// Pattern:
		/// variable = Expression;
		/// field = variable;
		/// where
		/// Expression is either array creation expression, literal constant, consturctor call from another class
		/// or combination of all of the above.
		private bool TryMatchArrayAssignmentInternal(StatementCollection statements)
		{
			if (statements.Count < 2)
			{
				return false;
			}
			ExpressionStatement theStatement = statements[0] as ExpressionStatement;
			if (theStatement == null)
			{
				return false;
			}
			BinaryExpression theAssignment = theStatement.Expression as BinaryExpression;
			VariableReference variable;
			if (theAssignment == null || !IsAssignToVariableExpression(theAssignment, out variable))
			{
				return false;
			}

			/// A check of wheather the assigned value can be used in field declaration context could be performed.
			/// At the moment, no IL samples that violate this rule were found.
			Expression assignedValue = theAssignment.Right;

			ExpressionStatement assignmentStatement = statements[1] as ExpressionStatement;
			if (assignmentStatement == null)
			{
				return false;
			}

			Expression variableReference;
            string memberFullName;
            if (!IsFieldAssignment(assignmentStatement, out variableReference, out memberFullName) &&
                !IsAutoPropertyAssignment(assignmentStatement, out variableReference, out memberFullName))
			{
                return false;
			}

            if (variableReference.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
				(variableReference as VariableReferenceExpression).Variable != variable)
			{
                return false;
			}

            /// The simple name of the field can be used here as well.
            /// Using the full name for consistency with other maps.
            
            return MapAssignmentIntoContext(memberFullName, assignedValue);
		}

		/// Pattern:
		/// field = Expression;
		/// where
		/// Expression is either array creation expression, literal constant, consturctor call from another class
		/// or combination of all of the above.
		private bool TryMatchDirectAssignmentInternal(StatementCollection statements)
		{
			ExpressionStatement theStatement = statements[0] as ExpressionStatement;
			if (theStatement == null || !string.IsNullOrEmpty(statements[0].Label))
			{
				return false;
			}

			string memberFullName;
			Expression assignedValue;
            if (!IsFieldAssignment(theStatement, out assignedValue, out memberFullName) &&
                !IsAutoPropertyAssignment(theStatement, out assignedValue, out memberFullName))
            {
                return false;
			}
            /// A check of wheather the assigned value can be used in field declaration context could be performed.
            /// At the moment, no IL samples that violate this rule were found.
            /// The simple name of the field can be used here as well.
            /// Using the full name for consistency with other maps.
            
            return MapAssignmentIntoContext(memberFullName, assignedValue);
		}
  
		private bool MapAssignmentIntoContext(string memberFullName, Expression assignedValue)
		{
			/// With the current workflow, each method is decompiled using new TypeContext.
			/// Thus, each constructor will be decompiled with new TypeContext, making
			/// the check for collisions pointless.
			/// All type contexts are merged in the ContextService, and all checks about the validity
			/// of the FieldAssignmentData dictionary are made there.

			typeContext.AssignmentData.Add(memberFullName, new InitializationAssignment(this.method, assignedValue));
			return true;
		}

		private bool IsFieldAssignment(ExpressionStatement fieldAssignmentStatement, out Expression assignedValue, out string fieldFullName)
		{
            fieldFullName = null;
			assignedValue = null;
			BinaryExpression theFieldAssignment = fieldAssignmentStatement.Expression as BinaryExpression;
			if (theFieldAssignment == null || !theFieldAssignment.IsAssignmentExpression || theFieldAssignment.Left.CodeNodeType != CodeNodeType.FieldReferenceExpression)
			{
				return false;
			}

			FieldReference theField = (theFieldAssignment.Left as FieldReferenceExpression).Field;
			if (theField == null)
			{
				return false;
			}

			assignedValue = theFieldAssignment.Right;
            fieldFullName = theField.FullName;

			return true;
		}

        private bool IsAutoPropertyAssignment(ExpressionStatement propertyAssignmentStatement, out Expression assignedValue, out string propertyFullName)
        {
            propertyFullName = null;
            assignedValue = null;
            BinaryExpression thePropertyAssignment = propertyAssignmentStatement.Expression as BinaryExpression;
            if (thePropertyAssignment == null || !thePropertyAssignment.IsAssignmentExpression || thePropertyAssignment.Left.CodeNodeType != CodeNodeType.AutoPropertyConstructorInitializerExpression)
            {
                return false;
            }

            PropertyDefinition theProperty = (thePropertyAssignment.Left as AutoPropertyConstructorInitializerExpression).Property;
            if (theProperty == null)
            {
                return false;
            }

            assignedValue = thePropertyAssignment.Right;
            propertyFullName = theProperty.FullName;

            return true;
        }
	}
}
