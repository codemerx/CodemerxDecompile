using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
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
			base();
			this.eventDef = eventDef;
			this.language = language;
			return;
		}

		private bool CheckLoopBody(BlockStatement loopBody, VariableReference v0Variable, VariableReference v1Variable, string operationName)
		{
			if (loopBody.get_Statements().get_Count() != 3)
			{
				return false;
			}
			if (!loopBody.get_Statements().get_Item(0).IsAssignmentStatement())
			{
				return false;
			}
			V_0 = (loopBody.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_0.get_Left().get_CodeNodeType() != 26 || V_0.get_Right().get_CodeNodeType() != 26 || (object)(V_0.get_Right() as VariableReferenceExpression).get_Variable() != (object)v0Variable)
			{
				return false;
			}
			if ((object)(V_0.get_Left() as VariableReferenceExpression).get_Variable() != (object)v1Variable)
			{
				return false;
			}
			if (!this.IsDelegateOperationStatement(loopBody.get_Statements().get_Item(1), operationName, out V_1, out V_2) || V_1.get_CodeNodeType() != 26 || V_2.get_CodeNodeType() != 26 || (object)(V_2 as VariableReferenceExpression).get_Variable() != (object)v1Variable)
			{
				return false;
			}
			V_3 = (V_1 as VariableReferenceExpression).get_Variable();
			if (!loopBody.get_Statements().get_Item(2).IsAssignmentStatement())
			{
				return false;
			}
			V_4 = (loopBody.get_Statements().get_Item(2) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_4.get_Left().get_CodeNodeType() != 26 || (object)(V_4.get_Left() as VariableReferenceExpression).get_Variable() != (object)v0Variable || V_4.get_Right().get_CodeNodeType() != 19)
			{
				return false;
			}
			V_5 = V_4.get_Right() as MethodInvocationExpression;
			if (String.op_Inequality(V_5.get_MethodExpression().get_Method().get_DeclaringType().get_FullName(), "System.Threading.Interlocked") || V_5.get_MethodExpression().get_Method().get_HasThis() || String.op_Inequality(V_5.get_MethodExpression().get_Method().get_Name(), "CompareExchange") || V_5.get_Arguments().get_Count() != 3 || V_5.get_Arguments().get_Item(0).get_CodeNodeType() != 23)
			{
				return false;
			}
			V_6 = V_5.get_Arguments().get_Item(0) as UnaryExpression;
			if (V_6.get_Operator() != 7 || V_6.get_Operand().get_CodeNodeType() != 30 || (object)(V_6.get_Operand() as FieldReferenceExpression).get_Field().Resolve() != (object)this.eventField)
			{
				return false;
			}
			if (V_5.get_Arguments().get_Item(1).get_CodeNodeType() == 26 && (object)(V_5.get_Arguments().get_Item(1) as VariableReferenceExpression).get_Variable() == (object)V_3 && V_5.get_Arguments().get_Item(2).get_CodeNodeType() == 26 && (object)(V_5.get_Arguments().get_Item(2) as VariableReferenceExpression).get_Variable() == (object)v1Variable)
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
			V_0 = BaseLanguage.get_IntermediateRepresenationPipeline();
			dummyVar0 = V_0.Run(methodDef.get_Body(), this.language);
			methodBody = V_0.get_Body();
			return true;
		}

		private FieldDefinition GetField(EventDefinition eventDef)
		{
			if (eventDef.get_InvokeMethod() != null || eventDef.get_AddMethod() == null || eventDef.get_RemoveMethod() == null)
			{
				return null;
			}
			V_0 = AutoImplementedEventMatcher.GetFieldWithName(eventDef.get_DeclaringType(), eventDef.get_EventType().get_FullName(), String.Concat(eventDef.get_Name(), "Event"));
			if (V_0 == null)
			{
				V_0 = AutoImplementedEventMatcher.GetFieldWithName(eventDef.get_DeclaringType(), eventDef.get_EventType().get_FullName(), eventDef.get_Name());
			}
			if (V_0 == null)
			{
				return null;
			}
			if (!this.IsThreadUnsafeEvent(V_0) && !this.IsThreadSafeEvent(V_0))
			{
				return null;
			}
			return V_0;
		}

		private static FieldDefinition GetFieldWithName(TypeDefinition typeDef, string eventTypeFullName, string name)
		{
			V_0 = typeDef.get_Fields().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!String.op_Equality(V_1.get_Name(), name) || !String.op_Equality(V_1.get_FieldType().get_FullName(), eventTypeFullName))
					{
						continue;
					}
					V_2 = V_1;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return null;
		}

		public bool IsAutoImplemented(out FieldDefinition eventField)
		{
			eventField = this.GetField(this.eventDef);
			return (object)eventField != (object)null;
		}

		public bool IsAutoImplemented()
		{
			return this.IsAutoImplemented(out V_0);
		}

		private bool IsDelegateOperationStatement(Statement statement, string operationName, out Expression newValueHolder, out Expression oldValueHolder)
		{
			newValueHolder = null;
			oldValueHolder = null;
			if (!statement.IsAssignmentStatement())
			{
				return false;
			}
			V_0 = (statement as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_0.get_Right().get_CodeNodeType() != 31 || (V_0.get_Right() as ExplicitCastExpression).get_Expression().get_CodeNodeType() != 19)
			{
				return false;
			}
			V_1 = (V_0.get_Right() as ExplicitCastExpression).get_Expression() as MethodInvocationExpression;
			if (V_1.get_Arguments().get_Count() != 2 || V_1.get_MethodExpression().get_Method().get_HasThis() || String.op_Inequality(V_1.get_MethodExpression().get_Method().get_DeclaringType().get_FullName(), "System.Delegate") || String.op_Inequality(V_1.get_MethodExpression().get_Method().get_Name(), operationName))
			{
				return false;
			}
			if (V_1.get_Arguments().get_Item(1).get_CodeNodeType() != 25)
			{
				return false;
			}
			newValueHolder = V_0.get_Left();
			oldValueHolder = V_1.get_Arguments().get_Item(0);
			return true;
		}

		private bool IsThreadSafeAutoImplOperation(MethodDefinition methodDef, string operationName)
		{
			if (!this.CheckMethodAndDecompile(methodDef, out V_0) || V_0.get_Statements().get_Count() != 3)
			{
				return false;
			}
			V_1 = null;
			V_2 = null;
			if (!V_0.get_Statements().get_Item(0).IsAssignmentStatement())
			{
				return false;
			}
			V_3 = (V_0.get_Statements().get_Item(0) as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_3.get_Left().get_CodeNodeType() != 26 || V_3.get_Right().get_CodeNodeType() != 30 || (object)(V_3.get_Right() as FieldReferenceExpression).get_Field().Resolve() != (object)this.eventField)
			{
				return false;
			}
			V_1 = (V_3.get_Left() as VariableReferenceExpression).get_Variable();
			if (V_0.get_Statements().get_Item(1).get_CodeNodeType() != 8 || V_0.get_Statements().get_Item(2).get_CodeNodeType() != 5 || (V_0.get_Statements().get_Item(2) as ExpressionStatement).get_Expression().get_CodeNodeType() != 57)
			{
				return false;
			}
			V_4 = V_0.get_Statements().get_Item(1) as DoWhileStatement;
			V_5 = V_4.get_Condition();
			if (V_5.get_CodeNodeType() == 23 && (V_5 as UnaryExpression).get_Operator() == 11)
			{
				V_5 = (V_5 as UnaryExpression).get_Operand();
			}
			if (V_5.get_CodeNodeType() != 24)
			{
				return false;
			}
			stackVariable78 = V_5 as BinaryExpression;
			V_6 = stackVariable78.get_Left() as ExplicitCastExpression;
			V_7 = stackVariable78.get_Right() as ExplicitCastExpression;
			if (stackVariable78.get_Operator() != 10 || V_6 == null || String.op_Inequality(V_6.get_TargetType().get_Name(), "Object") || V_6.get_Expression().get_CodeNodeType() != 26 || V_7 == null || V_7.get_Expression().get_CodeNodeType() != 26 || String.op_Inequality(V_7.get_TargetType().get_Name(), "Object"))
			{
				return false;
			}
			if ((object)(V_6.get_Expression() as VariableReferenceExpression).get_Variable() != (object)V_1)
			{
				return false;
			}
			V_2 = (V_7.get_Expression() as VariableReferenceExpression).get_Variable();
			return this.CheckLoopBody(V_4.get_Body(), V_1, V_2, operationName);
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
			if (!this.CheckMethodAndDecompile(methodDef, out V_0))
			{
				return false;
			}
			if (V_0.get_Statements().get_Count() != 2 || V_0.get_Statements().get_Item(1).get_CodeNodeType() != 5 || (V_0.get_Statements().get_Item(1) as ExpressionStatement).get_Expression().get_CodeNodeType() != 57)
			{
				return false;
			}
			if (!this.IsDelegateOperationStatement(V_0.get_Statements().get_Item(0), operationName, out V_1, out V_2))
			{
				return false;
			}
			if (V_1.get_CodeNodeType() == 30 && (object)(V_1 as FieldReferenceExpression).get_Field().Resolve() == (object)this.eventField && V_2.get_CodeNodeType() == 30 && (object)(V_2 as FieldReferenceExpression).get_Field().Resolve() == (object)this.eventField)
			{
				return true;
			}
			return false;
		}
	}
}