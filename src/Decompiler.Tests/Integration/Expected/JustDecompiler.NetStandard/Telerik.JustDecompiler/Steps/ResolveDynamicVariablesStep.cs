using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

		private readonly Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap = new Dictionary<FieldDefinition, CallSiteInfo>();

		private readonly Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap = new Dictionary<VariableReference, CallSiteInfo>();

		private readonly HashSet<Statement> statementsToRemove = new HashSet<Statement>();

		private MethodSpecificContext methodContext;

		public ResolveDynamicVariablesStep()
		{
		}

		private bool CheckArrayIndexerExpression(ArrayIndexerExpression expression, VariableReference arrayVariable, int index)
		{
			if (expression.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(expression.Target as VariableReferenceExpression).Variable != (object)arrayVariable || expression.Indices.Count != 1 || expression.Indices[0].CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return false;
			}
			return Convert.ToInt32((expression.Indices[0] as LiteralExpression).Value) == index;
		}

		private bool CheckFieldDefinition(FieldDefinition callSiteFieldDefinition)
		{
			if (callSiteFieldDefinition == null || !callSiteFieldDefinition.get_IsStatic() || callSiteFieldDefinition.get_DeclaringType() == null || !callSiteFieldDefinition.get_DeclaringType().HasCompilerGeneratedAttribute())
			{
				return false;
			}
			return (object)callSiteFieldDefinition.get_DeclaringType().get_DeclaringType() == (object)this.methodContext.Method.get_DeclaringType();
		}

		private int CheckNewArrayInitializationAndSize(Statement statement, VariableReference arrayVariable)
		{
			if (!statement.IsAssignmentStatement())
			{
				throw new Exception("Invalid statement.");
			}
			BinaryExpression expression = (statement as ExpressionStatement).Expression as BinaryExpression;
			if (expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || (object)(expression.Left as VariableReferenceExpression).Variable != (object)arrayVariable)
			{
				throw new Exception("Invalid statement.");
			}
			if (expression.Right.CodeNodeType != CodeNodeType.ArrayCreationExpression || (expression.Right as ArrayCreationExpression).Dimensions.Count != 1 || (expression.Right as ArrayCreationExpression).Dimensions[0].CodeNodeType != CodeNodeType.LiteralExpression)
			{
				throw new Exception("Invalid statement.");
			}
			return Convert.ToInt32(((expression.Right as ArrayCreationExpression).Dimensions[0] as LiteralExpression).Value);
		}

		private VariableReference GetArgumentArrayVariable(MethodInvocationExpression binderMethodInvocation)
		{
			int count = binderMethodInvocation.Arguments.Count - 1;
			Expression item = binderMethodInvocation.Arguments[count];
			VariableReferenceExpression expression = null;
			if (item.CodeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				expression = item as VariableReferenceExpression;
			}
			else if (item.CodeNodeType == CodeNodeType.ExplicitCastExpression)
			{
				ExplicitCastExpression explicitCastExpression = item as ExplicitCastExpression;
				if (explicitCastExpression.ExpressionType.GetFriendlyFullName(null) == "System.Collections.Generic.IEnumerable<Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo>" && explicitCastExpression.Expression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					expression = explicitCastExpression.Expression as VariableReferenceExpression;
				}
			}
			if (expression == null)
			{
				throw new Exception("Invalid argument: argumentInfo.");
			}
			return expression.Variable;
		}

		private MethodInvocationExpression GetBinderMethodInvocation(ExpressionStatement callSiteCreationStatement, FieldDefinition callSiteField)
		{
			if (callSiteCreationStatement == null || callSiteCreationStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression || (callSiteCreationStatement.Expression as BinaryExpression).Operator != BinaryOperator.Assign || (callSiteCreationStatement.Expression as BinaryExpression).Left.CodeNodeType != CodeNodeType.FieldReferenceExpression || (object)((callSiteCreationStatement.Expression as BinaryExpression).Left as FieldReferenceExpression).Field.Resolve() != (object)callSiteField)
			{
				throw new Exception("Last statement is not CallSite field assignment.");
			}
			MethodInvocationExpression right = (callSiteCreationStatement.Expression as BinaryExpression).Right as MethodInvocationExpression;
			if (right.MethodExpression.Method.get_DeclaringType().GetElementType().GetFriendlyFullName(null) != "System.Runtime.CompilerServices.CallSite<!0>" || right.MethodExpression.Target != null || right.MethodExpression.Method.get_Name() != "Create" || right.Arguments[0].CodeNodeType != CodeNodeType.MethodInvocationExpression)
			{
				throw new Exception("Invalid CallSite field assignment.");
			}
			MethodInvocationExpression item = right.Arguments[0] as MethodInvocationExpression;
			if (item.MethodExpression.Target != null || item.MethodExpression.Method.get_DeclaringType().GetFriendlyFullName(null) != "Microsoft.CSharp.RuntimeBinder.Binder")
			{
				throw new Exception("Invalid CallSite creation argument.");
			}
			return item;
		}

		private void GetConvertTypeArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
		{
			TypeReference typeReference;
			int num = 1;
			if (binderInvocation.Arguments[num].CodeNodeType != CodeNodeType.MethodInvocationExpression || !(binderInvocation.Arguments[num] as MethodInvocationExpression).IsTypeOfExpression(out typeReference))
			{
				throw new Exception("Invalid argument: convert type.");
			}
			callSiteInfo.ConvertType = typeReference;
		}

		private void GetDynamicArgument(MethodInvocationExpression expression, int index, CallSiteInfo callSiteInfo)
		{
			if (expression.MethodExpression.Method.get_Name() != "Create" || expression.MethodExpression.Target != null || expression.MethodExpression.Method.get_DeclaringType().GetFriendlyFullName(null) != "Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo" || expression.Arguments.Count != 2 || expression.Arguments[0].CodeNodeType != CodeNodeType.LiteralExpression)
			{
				throw new Exception("Invalid statement.");
			}
			if ((Convert.ToInt32((expression.Arguments[0] as LiteralExpression).Value) & 1) == 0)
			{
				callSiteInfo.DynamicArgumentIndices.Add(index);
			}
		}

		private void GetGenericTypeArgument(MethodInvocationExpression expression, int index, CallSiteInfo callSiteInfo)
		{
			TypeReference typeReference;
			if (!expression.IsTypeOfExpression(out typeReference))
			{
				throw new Exception("Invalid statement.");
			}
			callSiteInfo.GenericTypeArguments.Add(typeReference);
		}

		private void GetMemberNameArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
		{
			int num = 1;
			if (binderInvocation.Arguments[num].CodeNodeType == CodeNodeType.LiteralExpression)
			{
				callSiteInfo.MemberName = (binderInvocation.Arguments[num] as LiteralExpression).Value as String;
				if (callSiteInfo.MemberName != null)
				{
					return;
				}
			}
			throw new Exception("Invalid argument: member name.");
		}

		private void GetOperatorArgument(MethodInvocationExpression binderInvocation, CallSiteInfo callSiteInfo)
		{
			int num = 1;
			if (binderInvocation.Arguments[num].CodeNodeType != CodeNodeType.LiteralExpression)
			{
				throw new Exception("Invalid argument: operator.");
			}
			callSiteInfo.Operator = Convert.ToInt32((binderInvocation.Arguments[num] as LiteralExpression).Value);
		}

		private VariableReference GetTypeArrayVariable(MethodInvocationExpression binderMethodInvocation)
		{
			Expression item = binderMethodInvocation.Arguments[2];
			if (item.CodeNodeType == CodeNodeType.LiteralExpression && (item as LiteralExpression).Value == null)
			{
				return null;
			}
			VariableReferenceExpression expression = null;
			if (item.CodeNodeType == CodeNodeType.VariableReferenceExpression)
			{
				expression = item as VariableReferenceExpression;
			}
			else if (item.CodeNodeType == CodeNodeType.ExplicitCastExpression)
			{
				ExplicitCastExpression explicitCastExpression = item as ExplicitCastExpression;
				if (explicitCastExpression.ExpressionType.GetFriendlyFullName(null) == "System.Collections.Generic.IEnumerable<System.Type>" && explicitCastExpression.Expression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					expression = explicitCastExpression.Expression as VariableReferenceExpression;
				}
			}
			if (expression == null)
			{
				throw new Exception("Invalid argument: typeArguments.");
			}
			return expression.Variable;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.Visit(body);
			body = CallSiteInvocationReplacer.ReplaceInvocations(body, this.fieldToCallSiteInfoMap, this.variableToCallSiteInfoMap, this.statementsToRemove, this.methodContext.Method.get_Module().get_TypeSystem());
			this.RemoveStatements();
			return body;
		}

		private void ProcessArgumentArray(StatementCollection statements, ref int index, VariableReference typesArray, CallSiteInfo callSiteInfo, Action<MethodInvocationExpression, int, CallSiteInfo> action)
		{
			if (statements[index] is ExpressionStatement && (statements[index] as ExpressionStatement).Expression is BinaryExpression)
			{
				BinaryExpression expression = (statements[index] as ExpressionStatement).Expression as BinaryExpression;
				if (expression.IsAssignmentExpression && expression.Left is VariableReferenceExpression && expression.Right is MethodInvocationExpression)
				{
					index++;
				}
			}
			int num = index;
			index = num + 1;
			int num1 = this.CheckNewArrayInitializationAndSize(statements[num], typesArray);
			for (int i = 0; i < num1; i++)
			{
				if (!statements[index].IsAssignmentStatement())
				{
					throw new Exception("Invalid statement.");
				}
				num = index;
				index = num + 1;
				BinaryExpression binaryExpression = (statements[num] as ExpressionStatement).Expression as BinaryExpression;
				if (binaryExpression.Left.CodeNodeType != CodeNodeType.ArrayIndexerExpression || !this.CheckArrayIndexerExpression(binaryExpression.Left as ArrayIndexerExpression, typesArray, i) || binaryExpression.Right.CodeNodeType != CodeNodeType.MethodInvocationExpression)
				{
					throw new Exception("Invalid statement.");
				}
				action(binaryExpression.Right as MethodInvocationExpression, i, callSiteInfo);
			}
		}

		private void ProcessCallSiteCaching(IfStatement theIf, FieldDefinition callSiteField)
		{
			MethodInvocationExpression binderMethodInvocation = this.GetBinderMethodInvocation(theIf.Then.Statements.Last<Statement>() as ExpressionStatement, callSiteField);
			CallSiteInfo callSiteInfo = new CallSiteInfo(callSiteField, binderMethodInvocation.MethodExpression.Method.get_Name());
			int num = 0;
			if (callSiteInfo.BinderType == CallSiteBinderType.GetMember || callSiteInfo.BinderType == CallSiteBinderType.InvokeMember || callSiteInfo.BinderType == CallSiteBinderType.SetMember || callSiteInfo.BinderType == CallSiteBinderType.IsEvent)
			{
				this.GetMemberNameArgument(binderMethodInvocation, callSiteInfo);
			}
			if (callSiteInfo.BinderType == CallSiteBinderType.BinaryOperation || callSiteInfo.BinderType == CallSiteBinderType.UnaryOperation)
			{
				this.GetOperatorArgument(binderMethodInvocation, callSiteInfo);
			}
			if (callSiteInfo.BinderType == CallSiteBinderType.Convert)
			{
				this.GetConvertTypeArgument(binderMethodInvocation, callSiteInfo);
			}
			if (callSiteInfo.BinderType == CallSiteBinderType.InvokeMember)
			{
				VariableReference typeArrayVariable = this.GetTypeArrayVariable(binderMethodInvocation);
				if (typeArrayVariable != null)
				{
					callSiteInfo.GenericTypeArguments = new List<TypeReference>();
					this.ProcessArgumentArray(theIf.Then.Statements, ref num, typeArrayVariable, callSiteInfo, new Action<MethodInvocationExpression, int, CallSiteInfo>(this.GetGenericTypeArgument));
				}
			}
			if (callSiteInfo.BinderType == CallSiteBinderType.Convert || callSiteInfo.BinderType == CallSiteBinderType.IsEvent)
			{
				callSiteInfo.DynamicArgumentIndices.Add(0);
			}
			else
			{
				this.ProcessArgumentArray(theIf.Then.Statements, ref num, this.GetArgumentArrayVariable(binderMethodInvocation), callSiteInfo, new Action<MethodInvocationExpression, int, CallSiteInfo>(this.GetDynamicArgument));
			}
			this.fieldToCallSiteInfoMap.Add(callSiteField, callSiteInfo);
		}

		private void RemoveStatements()
		{
			foreach (Statement statement in this.statementsToRemove)
			{
				(statement.Parent as BlockStatement).Statements.Remove(statement);
			}
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			CallSiteInfo callSiteInfo;
			if (!node.IsAssignmentStatement())
			{
				return;
			}
			BinaryExpression expression = node.Expression as BinaryExpression;
			if (expression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression || expression.Right.CodeNodeType != CodeNodeType.FieldReferenceExpression)
			{
				return;
			}
			FieldReferenceExpression right = expression.Right as FieldReferenceExpression;
			if (right.Target == null || right.Target.CodeNodeType != CodeNodeType.FieldReferenceExpression || right.Field.get_Name() != "Target")
			{
				return;
			}
			FieldDefinition fieldDefinition = (right.Target as FieldReferenceExpression).Field.Resolve();
			if (fieldDefinition == null || !this.fieldToCallSiteInfoMap.TryGetValue(fieldDefinition, out callSiteInfo))
			{
				return;
			}
			this.variableToCallSiteInfoMap.Add((expression.Left as VariableReferenceExpression).Variable, callSiteInfo);
			this.statementsToRemove.Add(node);
		}

		public override void VisitIfStatement(IfStatement node)
		{
			if (node.Else == null && node.Condition.CodeNodeType == CodeNodeType.BinaryExpression)
			{
				BinaryExpression condition = node.Condition as BinaryExpression;
				if (condition.Operator == BinaryOperator.ValueEquality && condition.Left.CodeNodeType == CodeNodeType.FieldReferenceExpression && condition.Right.CodeNodeType == CodeNodeType.LiteralExpression && (condition.Right as LiteralExpression).Value == null)
				{
					FieldReferenceExpression left = condition.Left as FieldReferenceExpression;
					FieldDefinition fieldDefinition = left.Field.Resolve();
					if (left.Field.get_FieldType().GetElementType().GetFriendlyFullName(null) == "System.Runtime.CompilerServices.CallSite<!0>" && this.CheckFieldDefinition(fieldDefinition))
					{
						this.ProcessCallSiteCaching(node, fieldDefinition);
						this.statementsToRemove.Add(node);
						return;
					}
				}
			}
			base.VisitIfStatement(node);
		}
	}
}