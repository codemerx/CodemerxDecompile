using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Steps.DynamicVariables
{
	internal class CallSiteInvocationReplacer : BaseCodeTransformer
	{
		private const string InvalidIsEventString = "Invalid IsEvent construction";

		private readonly Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap;

		private readonly Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap;

		private readonly Dictionary<IfStatement, MethodInvocationExpression> isEventIfStatements = new Dictionary<IfStatement, MethodInvocationExpression>();

		private readonly TypeReference objectTypeRef;

		private readonly TypeSystem typeSystem;

		private IfStatement closestIf;

		private CallSiteInvocationReplacer(Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap, Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap, TypeSystem typeSystem)
		{
			this.fieldToCallSiteInfoMap = fieldToCallSiteInfoMap;
			this.variableToCallSiteInfoMap = variableToCallSiteInfoMap;
			this.typeSystem = typeSystem;
			this.objectTypeRef = typeSystem.get_Object();
		}

		private bool CanReplaceIf(BlockStatement statementBlock)
		{
			if ((statementBlock.Statements.Count == 2 || statementBlock.Statements.Count == 3) && statementBlock.Statements[1].CodeNodeType == CodeNodeType.ExpressionStatement && (statementBlock.Statements[1] as ExpressionStatement).Expression.CodeNodeType == CodeNodeType.DynamicMemberReferenceExpression)
			{
				if (statementBlock.Statements.Count == 3 && (statementBlock.Statements[2].CodeNodeType != CodeNodeType.ExpressionStatement || (statementBlock.Statements[2] as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.ReturnExpression))
				{
					return false;
				}
				return true;
			}
			if ((statementBlock.Statements[1] as ExpressionStatement).Expression.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			BinaryExpression expression = (statementBlock.Statements[1] as ExpressionStatement).Expression as BinaryExpression;
			if (expression.IsAssignmentExpression && expression.Right.CodeNodeType == CodeNodeType.DynamicMemberReferenceExpression)
			{
				return true;
			}
			return false;
		}

		private Expression GenerateBinaryExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.Count != 2)
			{
				throw new Exception("Invalid number of arguments for binary expression.");
			}
			return new BinaryExpression(DynamicHelper.GetBinaryOperator(callSiteInfo.Operator), arguments[0], arguments[1], this.objectTypeRef, this.typeSystem, instructions, false);
		}

		private Expression GenerateConvertExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.Count != 1)
			{
				throw new Exception("Invalid number of arguments for convert expression.");
			}
			return new ExplicitCastExpression(arguments[0], callSiteInfo.ConvertType, instructions);
		}

		private Expression GenerateExpression(CallSiteInfo callSiteInfo, IEnumerable<Expression> originalArguments, IEnumerable<Instruction> instructions)
		{
			IList<Expression> allButFirst = this.GetAllButFirst(originalArguments);
			this.MarkDynamicArguments(callSiteInfo, allButFirst);
			switch (callSiteInfo.BinderType)
			{
				case CallSiteBinderType.BinaryOperation:
				{
					return this.GenerateBinaryExpression(callSiteInfo, allButFirst, instructions);
				}
				case CallSiteBinderType.Convert:
				{
					return this.GenerateConvertExpression(callSiteInfo, allButFirst, instructions);
				}
				case CallSiteBinderType.GetIndex:
				{
					return this.GenerateGetIndexExpression(allButFirst, instructions);
				}
				case CallSiteBinderType.GetMember:
				{
					return this.GenerateGetMemberExpression(callSiteInfo, allButFirst, instructions);
				}
				case CallSiteBinderType.Invoke:
				{
					return this.GenerateInvokeExpression(callSiteInfo, allButFirst, instructions);
				}
				case CallSiteBinderType.InvokeConstructor:
				{
					return this.GenerateInvokeConstructorExpression(allButFirst, instructions);
				}
				case CallSiteBinderType.InvokeMember:
				{
					return this.GenerateInvokeMemeberExpression(callSiteInfo, allButFirst, instructions);
				}
				case CallSiteBinderType.IsEvent:
				{
					throw new Exception("Invalid binder type.");
				}
				case CallSiteBinderType.SetIndex:
				{
					return this.GenerateSetIndexExpression(allButFirst, instructions);
				}
				case CallSiteBinderType.SetMember:
				{
					return this.GenerateSetMemberExpression(callSiteInfo, allButFirst, instructions);
				}
				case CallSiteBinderType.UnaryOperation:
				{
					return this.GenerateUnaryExpression(callSiteInfo, allButFirst, instructions);
				}
				default:
				{
					throw new Exception("Invalid binder type.");
				}
			}
		}

		private Expression GenerateGetIndexExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.Count < 2)
			{
				throw new Exception("Invalid number of arguments for get index expression.");
			}
			DynamicIndexerExpression dynamicIndexerExpression = new DynamicIndexerExpression(arguments[0], this.objectTypeRef, instructions);
			for (int i = 1; i < arguments.Count; i++)
			{
				dynamicIndexerExpression.Indices.Add(arguments[i]);
			}
			return dynamicIndexerExpression;
		}

		private Expression GenerateGetMemberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.Count != 1)
			{
				throw new Exception("Invalid number of arguments for get member expression.");
			}
			return new DynamicMemberReferenceExpression(arguments[0], callSiteInfo.MemberName, this.objectTypeRef, instructions);
		}

		private Expression GenerateInvokeConstructorExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			TypeReference typeReference;
			if (arguments.Count < 1)
			{
				throw new Exception("Invalid number of arguments for invoke constructor expression.");
			}
			if (arguments[0].CodeNodeType != CodeNodeType.MethodInvocationExpression || !(arguments[0] as MethodInvocationExpression).IsTypeOfExpression(out typeReference))
			{
				throw new Exception("Invalid type argument for invoke constructor expression.");
			}
			return new DynamicConstructorInvocationExpression(typeReference, this.GetAllButFirst(arguments), instructions);
		}

		private Expression GenerateInvokeExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			return this.GenerateInvokeMemeberExpression(callSiteInfo, arguments, instructions);
		}

		private Expression GenerateInvokeMemeberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			Expression item;
			TypeReference typeReference;
			if (arguments.Count < 1)
			{
				throw new Exception("Invalid number of arguments for invoke expression.");
			}
			if (arguments[0].CodeNodeType != CodeNodeType.MethodInvocationExpression || !(arguments[0] as MethodInvocationExpression).IsTypeOfExpression(out typeReference))
			{
				item = arguments[0];
			}
			else
			{
				item = new TypeReferenceExpression(typeReference, arguments[0].UnderlyingSameMethodInstructions);
			}
			return new DynamicMemberReferenceExpression(item, callSiteInfo.MemberName, this.objectTypeRef, instructions, this.GetAllButFirst(arguments), callSiteInfo.GenericTypeArguments);
		}

		private Expression GenerateSetIndexExpression(IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.Count < 3)
			{
				throw new Exception("Invalid number of arguments for set index expression.");
			}
			DynamicIndexerExpression dynamicIndexerExpression = new DynamicIndexerExpression(arguments[0], this.objectTypeRef, instructions);
			for (int i = 1; i < arguments.Count - 1; i++)
			{
				dynamicIndexerExpression.Indices.Add(arguments[i]);
			}
			return new BinaryExpression(BinaryOperator.Assign, dynamicIndexerExpression, arguments[arguments.Count - 1], this.typeSystem, null, false);
		}

		private Expression GenerateSetMemberExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.Count != 2)
			{
				throw new Exception("Invalid number of arguments for set index expression.");
			}
			return new BinaryExpression(BinaryOperator.Assign, new DynamicMemberReferenceExpression(arguments[0], callSiteInfo.MemberName, this.objectTypeRef, instructions), arguments[1], this.typeSystem, null, false);
		}

		private Expression GenerateUnaryExpression(CallSiteInfo callSiteInfo, IList<Expression> arguments, IEnumerable<Instruction> instructions)
		{
			if (arguments.Count != 1)
			{
				throw new Exception("Invalid number of arguments for unary expression.");
			}
			if (callSiteInfo.Operator == ExpressionType.IsTrue)
			{
				return arguments[0];
			}
			return new UnaryExpression(DynamicHelper.GetUnaryOperator(callSiteInfo.Operator), arguments[0], instructions);
		}

		private IList<Expression> GetAllButFirst(IEnumerable<Expression> expressionEnumeration)
		{
			List<Expression> expressions = new List<Expression>();
			using (IEnumerator<Expression> enumerator = expressionEnumeration.GetEnumerator())
			{
				enumerator.MoveNext();
				while (enumerator.MoveNext())
				{
					expressions.Add(this.RemoveUnneededCast(enumerator.Current));
				}
			}
			return expressions;
		}

		private void ManageIsEventOperations(HashSet<Statement> statementsToRemove)
		{
			foreach (KeyValuePair<IfStatement, MethodInvocationExpression> isEventIfStatement in this.isEventIfStatements)
			{
				IfStatement key = isEventIfStatement.Key;
				MethodInvocationExpression value = isEventIfStatement.Value;
				if (key.Condition.CodeNodeType != CodeNodeType.UnaryExpression || (key.Condition as UnaryExpression).Operator != UnaryOperator.None || (key.Condition as UnaryExpression).Operand != value)
				{
					if (key.Condition.CodeNodeType != CodeNodeType.UnaryExpression || (key.Condition as UnaryExpression).Operator != UnaryOperator.Negate || (key.Condition as UnaryExpression).Operand != value)
					{
						throw new Exception("Invalid invocation of IsEvent operation.");
					}
					this.ReplaceIfWith(key, key.Else);
				}
				else
				{
					Statement item = key.Then.Statements[key.Then.Statements.Count - 1];
					if (item.CodeNodeType == CodeNodeType.ExpressionStatement && ((ExpressionStatement)item).Expression.CodeNodeType == CodeNodeType.ReturnExpression)
					{
						BlockStatement parent = (BlockStatement)key.Parent;
						for (int i = parent.Statements.IndexOf(key) + 1; i < parent.Statements.Count; i++)
						{
							statementsToRemove.Add(parent.Statements[i]);
						}
					}
					this.ReplaceIfWith(key, key.Then);
				}
				foreach (Statement statement in key.Then.Statements)
				{
					statementsToRemove.Remove(statement);
				}
				if (key.Else == null)
				{
					continue;
				}
				foreach (Statement statement1 in key.Else.Statements)
				{
					statementsToRemove.Remove(statement1);
				}
			}
		}

		private void MarkDynamicArguments(CallSiteInfo callSiteInfo, IList<Expression> arguments)
		{
			foreach (int dynamicArgumentIndex in callSiteInfo.DynamicArgumentIndices)
			{
				if (arguments[dynamicArgumentIndex].CodeNodeType == CodeNodeType.BinaryExpression || arguments[dynamicArgumentIndex].CodeNodeType == CodeNodeType.UnaryExpression && (arguments[dynamicArgumentIndex] as UnaryExpression).Operator == UnaryOperator.None || arguments[dynamicArgumentIndex].CodeNodeType == CodeNodeType.DynamicIndexerExpression || arguments[dynamicArgumentIndex].CodeNodeType == CodeNodeType.DynamicMemberReferenceExpression || DynamicElementAnalyzer.Analyze(arguments[dynamicArgumentIndex]))
				{
					continue;
				}
				ExplicitCastExpression explicitCastExpression = new ExplicitCastExpression(arguments[dynamicArgumentIndex], this.objectTypeRef, null)
				{
					DynamicPositioningFlags = new Boolean[] { true }
				};
				arguments[dynamicArgumentIndex] = explicitCastExpression;
			}
		}

		private Expression RemoveUnneededCast(Expression expression)
		{
			while (expression.CodeNodeType == CodeNodeType.ExplicitCastExpression && (expression as ExplicitCastExpression).TargetType.get_Name()[0] == '!')
			{
				expression = (expression as ExplicitCastExpression).Expression;
			}
			return expression;
		}

		private void ReplaceIfWith(IfStatement theIf, BlockStatement statementBlock)
		{
			if (!this.CanReplaceIf(statementBlock))
			{
				throw new Exception("Invalid IsEvent construction");
			}
			DynamicMemberReferenceExpression expression = (statementBlock.Statements[1] as ExpressionStatement).Expression as DynamicMemberReferenceExpression ?? ((statementBlock.Statements[1] as ExpressionStatement).Expression as BinaryExpression).Right as DynamicMemberReferenceExpression;
			if (expression.MemberName == null || !expression.IsMethodInvocation || expression.IsGenericMethod || expression.InvocationArguments.Count != 1)
			{
				throw new Exception("Invalid IsEvent construction");
			}
			int num = expression.MemberName.IndexOf('\u005F');
			if (num != 3 && num != 6)
			{
				throw new Exception("Invalid IsEvent construction");
			}
			DynamicMemberReferenceExpression dynamicMemberReferenceExpression = new DynamicMemberReferenceExpression(expression.Target, expression.MemberName.Substring(num + 1), expression.ExpressionType, expression.MappedInstructions);
			BinaryExpression binaryExpression = new BinaryExpression((num == 3 ? BinaryOperator.AddAssign : BinaryOperator.SubtractAssign), dynamicMemberReferenceExpression, expression.InvocationArguments[0], dynamicMemberReferenceExpression.ExpressionType, this.typeSystem, null, false);
			BlockStatement parent = (BlockStatement)theIf.Parent;
			int num1 = parent.Statements.IndexOf(theIf);
			ExpressionStatement expressionStatement = new ExpressionStatement(binaryExpression)
			{
				Parent = parent
			};
			parent.Statements[num1] = expressionStatement;
			if (statementBlock.Statements.Count == 3)
			{
				parent.AddStatementAt(num1 + 1, statementBlock.Statements[2].Clone());
			}
		}

		public static BlockStatement ReplaceInvocations(BlockStatement block, Dictionary<FieldDefinition, CallSiteInfo> fieldToCallSiteInfoMap, Dictionary<VariableReference, CallSiteInfo> variableToCallSiteInfoMap, HashSet<Statement> statementsToRemove, TypeSystem typeSystem)
		{
			CallSiteInvocationReplacer callSiteInvocationReplacer = new CallSiteInvocationReplacer(fieldToCallSiteInfoMap, variableToCallSiteInfoMap, typeSystem);
			BlockStatement blockStatement = (BlockStatement)callSiteInvocationReplacer.Visit(block);
			callSiteInvocationReplacer.ManageIsEventOperations(statementsToRemove);
			return blockStatement;
		}

		public override ICodeNode VisitExpressionStatement(ExpressionStatement node)
		{
			if (node.IsAssignmentStatement())
			{
				BinaryExpression expression = node.Expression as BinaryExpression;
				if (expression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression && expression.Right.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					FieldDefinition fieldDefinition = (expression.Right as FieldReferenceExpression).Field.Resolve();
					if (fieldDefinition != null && this.fieldToCallSiteInfoMap.ContainsKey(fieldDefinition))
					{
						return null;
					}
				}
			}
			return base.VisitExpressionStatement(node);
		}

		public override ICodeNode VisitIfStatement(IfStatement node)
		{
			this.closestIf = node;
			return base.VisitIfStatement(node);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			CallSiteInfo callSiteInfo;
			CallSiteInfo callSiteInfo1;
			Expression expression = (Expression)base.VisitMethodInvocationExpression(node);
			MethodInvocationExpression methodInvocationExpression = expression as MethodInvocationExpression;
			if (methodInvocationExpression != null && methodInvocationExpression.MethodExpression.Target != null && methodInvocationExpression.MethodExpression.Method.get_Name() == "Invoke")
			{
				if (methodInvocationExpression.MethodExpression.Target.CodeNodeType == CodeNodeType.FieldReferenceExpression && (methodInvocationExpression.MethodExpression.Target as FieldReferenceExpression).Field.get_Name() == "Target" && (methodInvocationExpression.MethodExpression.Target as FieldReferenceExpression).Target != null && (methodInvocationExpression.MethodExpression.Target as FieldReferenceExpression).Target.CodeNodeType == CodeNodeType.FieldReferenceExpression)
				{
					FieldDefinition fieldDefinition = ((methodInvocationExpression.MethodExpression.Target as FieldReferenceExpression).Target as FieldReferenceExpression).Field.Resolve();
					if (fieldDefinition != null && this.fieldToCallSiteInfoMap.TryGetValue(fieldDefinition, out callSiteInfo))
					{
						if (callSiteInfo.BinderType != CallSiteBinderType.IsEvent)
						{
							return this.GenerateExpression(callSiteInfo, methodInvocationExpression.Arguments, methodInvocationExpression.InvocationInstructions);
						}
						this.isEventIfStatements.Add(this.closestIf, methodInvocationExpression);
					}
				}
				else if (methodInvocationExpression.MethodExpression.Target.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					VariableReference variable = (methodInvocationExpression.MethodExpression.Target as VariableReferenceExpression).Variable;
					if (this.variableToCallSiteInfoMap.TryGetValue(variable, out callSiteInfo1))
					{
						if (callSiteInfo1.BinderType != CallSiteBinderType.IsEvent)
						{
							return this.GenerateExpression(callSiteInfo1, methodInvocationExpression.Arguments, methodInvocationExpression.InvocationInstructions);
						}
						this.isEventIfStatements.Add(this.closestIf, methodInvocationExpression);
					}
				}
			}
			return expression;
		}
	}
}