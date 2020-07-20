using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Steps.DynamicVariables;

namespace Telerik.JustDecompiler.Steps
{
	internal class ResolveDynamicVariablesStep : BaseCodeVisitor, IDecompilationStep
	{
		private const int UseCompileTimeType = 1;

		private const string CallSiteInstanceTypeName = "System.Runtime.CompilerServices.CallSite<!0>";

		private const string InvalidStatementExceptionString = "Invalid statement.";

		private const string IEnumerableOfCSharpArgumentInfo = "System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>";

		private const string IEnumerableOfSystemType = "System.Collections.Generic.IEnumerable<System.Type>";

		private readonly Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap;

		private readonly Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap;

		private readonly HashSet<Statement> statementsToRemove;

		private MethodSpecificContext methodContext;

		public ResolveDynamicVariablesStep()
		{
			this.fieldToCallSiteInfoMap = new Dictionary<FieldDefinition, CallSiteInfo>();
			this.variableToCallSiteInfoMap = new Dictionary<VariableReference, CallSiteInfo>();
			this.statementsToRemove = new HashSet<Statement>();
			base();
			return;
		}

		private bool CheckArrayIndexerExpression(ArrayIndexerExpression expression, VariableReference arrayVariable, int index)
		{
			if (expression.get_Target().get_CodeNodeType() != 26 || (object)(expression.get_Target() as VariableReferenceExpression).get_Variable() != (object)arrayVariable || expression.get_Indices().get_Count() != 1 || expression.get_Indices().get_Item(0).get_CodeNodeType() != 22)
			{
				return false;
			}
			return Convert.ToInt32((expression.get_Indices().get_Item(0) as LiteralExpression).get_Value()) == index;
		}

		private bool CheckFieldDefinition(FieldDefinition callSiteFieldDefinition)
		{
			if (callSiteFieldDefinition == null || !callSiteFieldDefinition.get_IsStatic() || callSiteFieldDefinition.get_DeclaringType() == null || !callSiteFieldDefinition.get_DeclaringType().HasCompilerGeneratedAttribute())
			{
				return false;
			}
			return (object)callSiteFieldDefinition.get_DeclaringType().get_DeclaringType() == (object)this.methodContext.get_Method().get_DeclaringType();
		}

		private int CheckNewArrayInitializationAndSize(Statement statement, VariableReference arrayVariable)
		{
			if (!statement.IsAssignmentStatement())
			{
				throw new Exception("Invalid statement.");
			}
			V_0 = (statement as ExpressionStatement).get_Expression() as BinaryExpression;
			if (V_0.get_Left().get_CodeNodeType() != 26 || (object)(V_0.get_Left() as VariableReferenceExpression).get_Variable() != (object)arrayVariable)
			{
				throw new Exception("Invalid statement.");
			}
			if (V_0.get_Right().get_CodeNodeType() != 38 || (V_0.get_Right() as ArrayCreationExpression).get_Dimensions().get_Count() != 1 || (V_0.get_Right() as ArrayCreationExpression).get_Dimensions().get_Item(0).get_CodeNodeType() != 22)
			{
				throw new Exception("Invalid statement.");
			}
			return Convert.ToInt32(((V_0.get_Right() as ArrayCreationExpression).get_Dimensions().get_Item(0) as LiteralExpression).get_Value());
		}

		private VariableReference GetArgumentArrayVariable(MethodInvocationExpression binderMethodInvocation)
		{
			V_0 = binderMethodInvocation.get_Arguments().get_Count() - 1;
			V_1 = binderMethodInvocation.get_Arguments().get_Item(V_0);
			V_2 = null;
			if (V_1.get_CodeNodeType() != 26)
			{
				if (V_1.get_CodeNodeType() == 31)
				{
					V_3 = V_1 as ExplicitCastExpression;
					if (String.op_Equality(V_3.get_ExpressionType().GetFriendlyFullName(null), "System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>") && V_3.get_Expression().get_CodeNodeType() == 26)
					{
						V_2 = V_3.get_Expression() as VariableReferenceExpression;
					}
				}
			}
			else
			{
				V_2 = V_1 as VariableReferenceExpression;
			}
			if (V_2 == null)
			{
				throw new Exception("Invalid argument: argumentInfo.");
			}
			return V_2.get_Variable();
		}

		private MethodInvocationExpression GetBinderMethodInvocation(ExpressionStatement callSiteCreationStatement, FieldDefinition callSiteField)
		{
			if (callSiteCreationStatement == null || callSiteCreationStatement.get_Expression().get_CodeNodeType() != 24 || (callSiteCreationStatement.get_Expression() as BinaryExpression).get_Operator() != 26 || (callSiteCreationStatement.get_Expression() as BinaryExpression).get_Left().get_CodeNodeType() != 30 || (object)((callSiteCreationStatement.get_Expression() as BinaryExpression).get_Left() as FieldReferenceExpression).get_Field().Resolve() != (object)callSiteField)
			{
				throw new Exception("Last statement is not CallSite field assignment.");
			}
			V_0 = (callSiteCreationStatement.get_Expression() as BinaryExpression).get_Right() as MethodInvocationExpression;
			if (String.op_Inequality(V_0.get_MethodExpression().get_Method().get_DeclaringType().GetElementType().GetFriendlyFullName(null), "System.Runtime.CompilerServices.CallSite<!0>") || V_0.get_MethodExpression().get_Target() != null || String.op_Inequality(V_0.get_MethodExpression().get_Method().get_Name(), "Create") || V_0.get_Arguments().get_Item(0).get_CodeNodeType() != 19)
			{
				throw new Exception("Invalid CallSite field assignment.");
			}
			V_1 = V_0.get_Arguments().get_Item(0) as MethodInvocationExpression;
			if (V_1.get_MethodExpression().get_Target() != null || String.op_Inequality(V_1.get_MethodExpression().get_Method().get_DeclaringType().GetFriendlyFullName(null), "Microsoft.CSharp.RuntimeBinder.Binder"))
			{
				throw new Exception("Invalid CallSite creation argument.");
			}
			return V_1;
		}

		private void GetConvertTypeArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
		{
			V_0 = 1;
			if (binderInvocation.get_Arguments().get_Item(V_0).get_CodeNodeType() != 19 || !(binderInvocation.get_Arguments().get_Item(V_0) as MethodInvocationExpression).IsTypeOfExpression(out V_1))
			{
				throw new Exception("Invalid argument: convert type.");
			}
			callSiteInfo.set_ConvertType(V_1);
			return;
		}

		private void GetDynamicArgument(MethodInvocationExpression expression, int index, CallSiteInfo callSiteInfo)
		{
			if (String.op_Inequality(expression.get_MethodExpression().get_Method().get_Name(), "Create") || expression.get_MethodExpression().get_Target() != null || String.op_Inequality(expression.get_MethodExpression().get_Method().get_DeclaringType().GetFriendlyFullName(null), "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo") || expression.get_Arguments().get_Count() != 2 || expression.get_Arguments().get_Item(0).get_CodeNodeType() != 22)
			{
				throw new Exception("Invalid statement.");
			}
			if (Convert.ToInt32((expression.get_Arguments().get_Item(0) as LiteralExpression).get_Value()) & 1 == 0)
			{
				callSiteInfo.get_DynamicArgumentIndices().Add(index);
			}
			return;
		}

		private void GetGenericTypeArgument(MethodInvocationExpression expression, int index, CallSiteInfo callSiteInfo)
		{
			if (!expression.IsTypeOfExpression(out V_0))
			{
				throw new Exception("Invalid statement.");
			}
			callSiteInfo.get_GenericTypeArguments().Add(V_0);
			return;
		}

		private void GetMemberNameArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
		{
			V_0 = 1;
			if (binderInvocation.get_Arguments().get_Item(V_0).get_CodeNodeType() == 22)
			{
				callSiteInfo.set_MemberName((binderInvocation.get_Arguments().get_Item(V_0) as LiteralExpression).get_Value() as String);
				if (callSiteInfo.get_MemberName() != null)
				{
					return;
				}
			}
			throw new Exception("Invalid argument: member name.");
		}

		private void GetOperatorArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
		{
			V_0 = 1;
			if (binderInvocation.get_Arguments().get_Item(V_0).get_CodeNodeType() != 22)
			{
				throw new Exception("Invalid argument: operator.");
			}
			callSiteInfo.set_Operator(Convert.ToInt32((binderInvocation.get_Arguments().get_Item(V_0) as LiteralExpression).get_Value()));
			return;
		}

		private VariableReference GetTypeArrayVariable(MethodInvocationExpression binderMethodInvocation)
		{
			V_1 = binderMethodInvocation.get_Arguments().get_Item(2);
			if (V_1.get_CodeNodeType() == 22 && (V_1 as LiteralExpression).get_Value() == null)
			{
				return null;
			}
			V_2 = null;
			if (V_1.get_CodeNodeType() != 26)
			{
				if (V_1.get_CodeNodeType() == 31)
				{
					V_3 = V_1 as ExplicitCastExpression;
					if (String.op_Equality(V_3.get_ExpressionType().GetFriendlyFullName(null), "System.Collections.Generic.IEnumerable<System.Type>") && V_3.get_Expression().get_CodeNodeType() == 26)
					{
						V_2 = V_3.get_Expression() as VariableReferenceExpression;
					}
				}
			}
			else
			{
				V_2 = V_1 as VariableReferenceExpression;
			}
			if (V_2 == null)
			{
				throw new Exception("Invalid argument: typeArguments.");
			}
			return V_2.get_Variable();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.Visit(body);
			body = CallSiteInvocationReplacer.ReplaceInvocations(body, this.fieldToCallSiteInfoMap, this.variableToCallSiteInfoMap, this.statementsToRemove, this.methodContext.get_Method().get_Module().get_TypeSystem());
			this.RemoveStatements();
			return body;
		}

		private void ProcessArgumentArray(StatementCollection statements, ref int index, VariableReference typesArray, CallSiteInfo callSiteInfo, Action<MethodInvocationExpression, int, CallSiteInfo> action)
		{
			if (statements.get_Item(index) as ExpressionStatement != null && (statements.get_Item(index) as ExpressionStatement).get_Expression() as BinaryExpression != null)
			{
				V_1 = (statements.get_Item(index) as ExpressionStatement).get_Expression() as BinaryExpression;
				if (V_1.get_IsAssignmentExpression() && V_1.get_Left() as VariableReferenceExpression != null && V_1.get_Right() as MethodInvocationExpression != null)
				{
					index = index + 1;
				}
			}
			V_2 = index;
			index = V_2 + 1;
			V_0 = this.CheckNewArrayInitializationAndSize(statements.get_Item(V_2), typesArray);
			V_3 = 0;
			while (V_3 < V_0)
			{
				if (!statements.get_Item(index).IsAssignmentStatement())
				{
					throw new Exception("Invalid statement.");
				}
				V_2 = index;
				index = V_2 + 1;
				V_4 = (statements.get_Item(V_2) as ExpressionStatement).get_Expression() as BinaryExpression;
				if (V_4.get_Left().get_CodeNodeType() != 39 || !this.CheckArrayIndexerExpression(V_4.get_Left() as ArrayIndexerExpression, typesArray, V_3) || V_4.get_Right().get_CodeNodeType() != 19)
				{
					throw new Exception("Invalid statement.");
				}
				action.Invoke(V_4.get_Right() as MethodInvocationExpression, V_3, callSiteInfo);
				V_3 = V_3 + 1;
			}
			return;
		}

		private void ProcessCallSiteCaching(IfStatement theIf, FieldDefinition callSiteField)
		{
			V_0 = this.GetBinderMethodInvocation(theIf.get_Then().get_Statements().Last<Statement>() as ExpressionStatement, callSiteField);
			V_1 = new CallSiteInfo(callSiteField, V_0.get_MethodExpression().get_Method().get_Name());
			V_2 = 0;
			if (V_1.get_BinderType() == 3 || V_1.get_BinderType() == 6 || V_1.get_BinderType() == 9 || V_1.get_BinderType() == 7)
			{
				this.GetMemberNameArgument(V_0, V_1);
			}
			if (V_1.get_BinderType() == CallSiteBinderType.BinaryOperation || V_1.get_BinderType() == 10)
			{
				this.GetOperatorArgument(V_0, V_1);
			}
			if (V_1.get_BinderType() == 1)
			{
				this.GetConvertTypeArgument(V_0, V_1);
			}
			if (V_1.get_BinderType() == 6)
			{
				V_3 = this.GetTypeArrayVariable(V_0);
				if (V_3 != null)
				{
					V_1.set_GenericTypeArguments(new List<TypeReference>());
					this.ProcessArgumentArray(theIf.get_Then().get_Statements(), ref V_2, V_3, V_1, new Action<MethodInvocationExpression, int, CallSiteInfo>(this.GetGenericTypeArgument));
				}
			}
			if (V_1.get_BinderType() == 1 || V_1.get_BinderType() == 7)
			{
				V_1.get_DynamicArgumentIndices().Add(0);
			}
			else
			{
				this.ProcessArgumentArray(theIf.get_Then().get_Statements(), ref V_2, this.GetArgumentArrayVariable(V_0), V_1, new Action<MethodInvocationExpression, int, CallSiteInfo>(this.GetDynamicArgument));
			}
			this.fieldToCallSiteInfoMap.Add(callSiteField, V_1);
			return;
		}

		private void RemoveStatements()
		{
			V_0 = this.statementsToRemove.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					dummyVar0 = (V_1.get_Parent() as BlockStatement).get_Statements().Remove(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			if (!node.IsAssignmentStatement())
			{
				return;
			}
			V_0 = node.get_Expression() as BinaryExpression;
			if (V_0.get_Left().get_CodeNodeType() != 26 || V_0.get_Right().get_CodeNodeType() != 30)
			{
				return;
			}
			V_1 = V_0.get_Right() as FieldReferenceExpression;
			if (V_1.get_Target() == null || V_1.get_Target().get_CodeNodeType() != 30 || String.op_Inequality(V_1.get_Field().get_Name(), "Target"))
			{
				return;
			}
			V_2 = (V_1.get_Target() as FieldReferenceExpression).get_Field().Resolve();
			if (V_2 == null || !this.fieldToCallSiteInfoMap.TryGetValue(V_2, out V_3))
			{
				return;
			}
			this.variableToCallSiteInfoMap.Add((V_0.get_Left() as VariableReferenceExpression).get_Variable(), V_3);
			dummyVar0 = this.statementsToRemove.Add(node);
			return;
		}

		public override void VisitIfStatement(IfStatement node)
		{
			if (node.get_Else() == null && node.get_Condition().get_CodeNodeType() == 24)
			{
				V_0 = node.get_Condition() as BinaryExpression;
				if (V_0.get_Operator() == 9 && V_0.get_Left().get_CodeNodeType() == 30 && V_0.get_Right().get_CodeNodeType() == 22 && (V_0.get_Right() as LiteralExpression).get_Value() == null)
				{
					stackVariable28 = V_0.get_Left() as FieldReferenceExpression;
					V_1 = stackVariable28.get_Field().Resolve();
					if (String.op_Equality(stackVariable28.get_Field().get_FieldType().GetElementType().GetFriendlyFullName(null), "System.Runtime.CompilerServices.CallSite<!0>") && this.CheckFieldDefinition(V_1))
					{
						this.ProcessCallSiteCaching(node, V_1);
						dummyVar0 = this.statementsToRemove.Add(node);
						return;
					}
				}
			}
			this.VisitIfStatement(node);
			return;
		}
	}
}