using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	internal class AutoImplementedEventMatcher
	{
		private readonly EventDefinition eventDef;

		private FieldDefinition eventField;

		private ILanguage language;

		public AutoImplementedEventMatcher(EventDefinition eventDef, ILanguage language)
		{
			this.eventDef = eventDef;
			this.language = language;
		}

		private bool CheckLoopBody(BlockStatement loopBody, VariableReference v0Variable, VariableReference v1Variable, string operationName)
		{
			Expression expression;
			Expression expression1;
			if (loopBody.Statements.Count != 3)
			{
				return false;
			}
			if (!loopBody.Statements[0].IsAssignmentStatement())
			{
				return false;
			}
			BinaryExpression binaryExpression = (loopBody.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			if (binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || binaryExpression.Right.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(binaryExpression.Right as VariableReferenceExpression).Variable != (object)v0Variable)
			{
				return false;
			}
			if ((object)(binaryExpression.Left as VariableReferenceExpression).Variable != (object)v1Variable)
			{
				return false;
			}
			if (!this.IsDelegateOperationStatement(loopBody.Statements[1], operationName, out expression, out expression1) || expression.CodeNodeType != CodeNodeType.VariableReferenceExpression || expression1.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(expression1 as VariableReferenceExpression).Variable != (object)v1Variable)
			{
				return false;
			}
			VariableReference variable = (expression as VariableReferenceExpression).Variable;
			if (!loopBody.Statements[2].IsAssignmentStatement())
			{
				return false;
			}
			BinaryExpression binaryExpression1 = (loopBody.Statements[2] as ExpressionStatement).Expression as BinaryExpression;
			if (binaryExpression1.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(binaryExpression1.Left as VariableReferenceExpression).Variable != (object)v0Variable || binaryExpression1.Right.CodeNodeType != CodeNodeType.MethodInvocationExpression)
			{
				return false;
			}
			MethodInvocationExpression right = binaryExpression1.Right as MethodInvocationExpression;
			if (right.MethodExpression.Method.get_DeclaringType().get_FullName() != "System.Threading.Interlocked" || right.MethodExpression.Method.get_HasThis() || right.MethodExpression.Method.get_Name() != "CompareExchange" || right.Arguments.Count != 3 || right.Arguments[0].CodeNodeType != CodeNodeType.UnaryExpression)
			{
				return false;
			}
			UnaryExpression item = right.Arguments[0] as UnaryExpression;
			if (item.Operator != UnaryOperator.AddressReference || item.Operand.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)(item.Operand as FieldReferenceExpression).Field.Resolve() != (object)this.eventField)
			{
				return false;
			}
			if (right.Arguments[1].CodeNodeType == CodeNodeType.VariableReferenceExpression && (object)(right.Arguments[1] as VariableReferenceExpression).Variable == (object)variable && right.Arguments[2].CodeNodeType == CodeNodeType.VariableReferenceExpression && (object)(right.Arguments[2] as VariableReferenceExpression).Variable == (object)v1Variable)
			{
				return true;
			}
			return false;
		}

		private bool CheckMethodAndDecompile(MethodDefinition methodDef, out BlockStatement methodBody)
		{
			if (!methodDef.get_HasParameters() || methodDef.get_Parameters().get_Count() != 1)
			{
				methodBody = null;
				return false;
			}
			DecompilationPipeline intermediateRepresenationPipeline = BaseLanguage.IntermediateRepresenationPipeline;
			intermediateRepresenationPipeline.Run(methodDef.get_Body(), this.language);
			methodBody = intermediateRepresenationPipeline.Body;
			return true;
		}

		private FieldDefinition GetField(EventDefinition eventDef)
		{
			if (eventDef.get_InvokeMethod() != null || eventDef.get_AddMethod() == null || eventDef.get_RemoveMethod() == null)
			{
				return null;
			}
			FieldDefinition fieldWithName = AutoImplementedEventMatcher.GetFieldWithName(eventDef.get_DeclaringType(), eventDef.get_EventType().get_FullName(), String.Concat(eventDef.get_Name(), "Event")) ?? AutoImplementedEventMatcher.GetFieldWithName(eventDef.get_DeclaringType(), eventDef.get_EventType().get_FullName(), eventDef.get_Name());
			if (fieldWithName == null)
			{
				return null;
			}
			if (!this.IsThreadUnsafeEvent(fieldWithName) && !this.IsThreadSafeEvent(fieldWithName))
			{
				return null;
			}
			return fieldWithName;
		}

		private static FieldDefinition GetFieldWithName(TypeDefinition typeDef, string eventTypeFullName, string name)
		{
			FieldDefinition fieldDefinition;
			Mono.Collections.Generic.Collection<FieldDefinition>.Enumerator enumerator = typeDef.get_Fields().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					FieldDefinition current = enumerator.get_Current();
					if (!(current.get_Name() == name) || !(current.get_FieldType().get_FullName() == eventTypeFullName))
					{
						continue;
					}
					fieldDefinition = current;
					return fieldDefinition;
				}
				return null;
			}
			finally
			{
				enumerator.Dispose();
			}
			return fieldDefinition;
		}

		public bool IsAutoImplemented(out FieldDefinition eventField)
		{
			eventField = this.GetField(this.eventDef);
			return (object)eventField != (object)null;
		}

		public bool IsAutoImplemented()
		{
			FieldDefinition fieldDefinition;
			return this.IsAutoImplemented(out fieldDefinition);
		}

		private bool IsDelegateOperationStatement(Statement statement, string operationName, out Expression newValueHolder, out Expression oldValueHolder)
		{
			newValueHolder = null;
			oldValueHolder = null;
			if (!statement.IsAssignmentStatement())
			{
				return false;
			}
			BinaryExpression expression = (statement as ExpressionStatement).Expression as BinaryExpression;
			if (expression.Right.CodeNodeType != CodeNodeType.ExplicitCastExpression || (expression.Right as ExplicitCastExpression).Expression.CodeNodeType != CodeNodeType.MethodInvocationExpression)
			{
				return false;
			}
			MethodInvocationExpression methodInvocationExpression = (expression.Right as ExplicitCastExpression).Expression as MethodInvocationExpression;
			if (methodInvocationExpression.Arguments.Count != 2 || methodInvocationExpression.MethodExpression.Method.get_HasThis() || methodInvocationExpression.MethodExpression.Method.get_DeclaringType().get_FullName() != "System.Delegate" || methodInvocationExpression.MethodExpression.Method.get_Name() != operationName)
			{
				return false;
			}
			if (methodInvocationExpression.Arguments[1].CodeNodeType != CodeNodeType.ArgumentReferenceExpression)
			{
				return false;
			}
			newValueHolder = expression.Left;
			oldValueHolder = methodInvocationExpression.Arguments[0];
			return true;
		}

		private bool IsThreadSafeAutoImplOperation(MethodDefinition methodDef, string operationName)
		{
			BlockStatement blockStatement;
			if (!this.CheckMethodAndDecompile(methodDef, out blockStatement) || blockStatement.Statements.Count != 3)
			{
				return false;
			}
			VariableReference variable = null;
			VariableReference variableReference = null;
			if (!blockStatement.Statements[0].IsAssignmentStatement())
			{
				return false;
			}
			BinaryExpression expression = (blockStatement.Statements[0] as ExpressionStatement).Expression as BinaryExpression;
			if (expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || expression.Right.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)(expression.Right as FieldReferenceExpression).Field.Resolve() != (object)this.eventField)
			{
				return false;
			}
			variable = (expression.Left as VariableReferenceExpression).Variable;
			if (blockStatement.Statements[1].CodeNodeType != CodeNodeType.DoWhileStatement || blockStatement.Statements[2].CodeNodeType != CodeNodeType.ExpressionStatement || (blockStatement.Statements[2] as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.ReturnExpression)
			{
				return false;
			}
			DoWhileStatement item = blockStatement.Statements[1] as DoWhileStatement;
			Expression condition = item.Condition;
			if (condition.CodeNodeType == CodeNodeType.UnaryExpression && (condition as UnaryExpression).Operator == UnaryOperator.None)
			{
				condition = (condition as UnaryExpression).Operand;
			}
			if (condition.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression binaryExpression = condition as BinaryExpression;
			ExplicitCastExpression left = binaryExpression.Left as ExplicitCastExpression;
			ExplicitCastExpression right = binaryExpression.Right as ExplicitCastExpression;
			if (binaryExpression.Operator != BinaryOperator.ValueInequality || left == null || left.TargetType.get_Name() != "Object" || left.Expression.CodeNodeType != CodeNodeType.VariableReferenceExpression || right == null || right.Expression.CodeNodeType != CodeNodeType.VariableReferenceExpression || right.TargetType.get_Name() != "Object")
			{
				return false;
			}
			if ((object)(left.Expression as VariableReferenceExpression).Variable != (object)variable)
			{
				return false;
			}
			variableReference = (right.Expression as VariableReferenceExpression).Variable;
			return this.CheckLoopBody(item.Body, variable, variableReference, operationName);
		}

		private bool IsThreadSafeEvent(FieldDefinition eventField)
		{
			this.eventField = eventField;
			if (!this.IsThreadSafeAutoImplOperation(this.eventDef.get_AddMethod(), "Combine"))
			{
				return false;
			}
			return this.IsThreadSafeAutoImplOperation(this.eventDef.get_RemoveMethod(), "Remove");
		}

		private bool IsThreadUnsafeEvent(FieldDefinition eventField)
		{
			this.eventField = eventField;
			if (!this.IsThreadUnsafeOperation(this.eventDef.get_AddMethod(), "Combine"))
			{
				return false;
			}
			return this.IsThreadUnsafeOperation(this.eventDef.get_RemoveMethod(), "Remove");
		}

		private bool IsThreadUnsafeOperation(MethodDefinition methodDef, string operationName)
		{
			BlockStatement blockStatement;
			Expression expression;
			Expression expression1;
			if (!this.CheckMethodAndDecompile(methodDef, out blockStatement))
			{
				return false;
			}
			if (blockStatement.Statements.Count != 2 || blockStatement.Statements[1].CodeNodeType != CodeNodeType.ExpressionStatement || (blockStatement.Statements[1] as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.ReturnExpression)
			{
				return false;
			}
			if (!this.IsDelegateOperationStatement(blockStatement.Statements[0], operationName, out expression, out expression1))
			{
				return false;
			}
			if (expression.CodeNodeType == CodeNodeType.FieldReferenceExpression && (object)(expression as FieldReferenceExpression).Field.Resolve() == (object)this.eventField && expression1.CodeNodeType == CodeNodeType.FieldReferenceExpression && (object)(expression1 as FieldReferenceExpression).Field.Resolve() == (object)this.eventField)
			{
				return true;
			}
			return false;
		}
	}
}