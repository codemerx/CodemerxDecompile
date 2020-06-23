using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class OperatorStep
	{
		private readonly BaseCodeTransformer codeTransformer;

		private readonly TypeSystem typeSystem;

		private readonly static Dictionary<string, BinaryOperator> binaryOperators;

		private readonly static Dictionary<string, UnaryOperator> unaryOperators;

		static OperatorStep()
		{
			OperatorStep.binaryOperators = new Dictionary<string, BinaryOperator>()
			{
				{ "op_Equality", BinaryOperator.ValueEquality },
				{ "op_Inequality", BinaryOperator.ValueInequality },
				{ "op_GreaterThan", BinaryOperator.GreaterThan },
				{ "op_GreaterThanOrEqual", BinaryOperator.GreaterThanOrEqual },
				{ "op_LessThan", BinaryOperator.LessThan },
				{ "op_LessThanOrEqual", BinaryOperator.LessThanOrEqual },
				{ "op_Addition", BinaryOperator.Add },
				{ "op_Subtraction", BinaryOperator.Subtract },
				{ "op_Division", BinaryOperator.Divide },
				{ "op_Multiply", BinaryOperator.Multiply },
				{ "op_Modulus", BinaryOperator.Modulo },
				{ "op_BitwiseAnd", BinaryOperator.BitwiseAnd },
				{ "op_BitwiseOr", BinaryOperator.BitwiseOr },
				{ "op_ExclusiveOr", BinaryOperator.BitwiseXor },
				{ "op_RightShift", BinaryOperator.RightShift },
				{ "op_LeftShift", BinaryOperator.LeftShift }
			};
			OperatorStep.unaryOperators = new Dictionary<string, UnaryOperator>()
			{
				{ "op_UnaryNegation", UnaryOperator.Negate },
				{ "op_LogicalNot", UnaryOperator.LogicalNot },
				{ "op_OnesComplement", UnaryOperator.BitwiseNot },
				{ "op_Decrement", UnaryOperator.PostDecrement },
				{ "op_Increment", UnaryOperator.PostIncrement },
				{ "op_UnaryPlus", UnaryOperator.UnaryPlus }
			};
		}

		public OperatorStep(BaseCodeTransformer codeTransformer, TypeSystem typeSystem)
		{
			this.codeTransformer = codeTransformer;
			this.typeSystem = typeSystem;
		}

		private bool AreTheSame(Expression first, Expression second)
		{
			if (first.CodeNodeType != second.CodeNodeType)
			{
				return false;
			}
			return first.Equals(second);
		}

		private ICodeNode BuildBinaryExpression(BinaryOperator @operator, Expression left, Expression right, TypeReference expressionType, IEnumerable<Instruction> instructions)
		{
			BinaryExpression binaryExpression = new BinaryExpression(@operator, (Expression)this.codeTransformer.Visit(left), (Expression)this.codeTransformer.Visit(right), expressionType, this.typeSystem, instructions, true);
			if (binaryExpression.IsComparisonExpression || binaryExpression.IsLogicalExpression)
			{
				binaryExpression.ExpressionType = left.ExpressionType.Module.TypeSystem.Boolean;
			}
			return binaryExpression;
		}

		private ICodeNode BuildUnaryExpression(UnaryOperator @operator, Expression expression, IEnumerable<Instruction> instructions)
		{
			return new UnaryExpression(@operator, (Expression)this.codeTransformer.Visit(expression), instructions);
		}

		internal BinaryExpression VisitAssignExpression(BinaryExpression node)
		{
			BinaryOperator binaryOperator;
			Expression left = node.Left;
			TypeDefinition typeDefinition = left.ExpressionType.Resolve();
			if (typeDefinition == null)
			{
				return null;
			}
			if (typeDefinition.BaseType != null && typeDefinition.BaseType.Name == "MulticastDelegate")
			{
				Expression right = node.Right;
				MethodInvocationExpression expression = right as MethodInvocationExpression;
				if (right is ExplicitCastExpression)
				{
					expression = (right as ExplicitCastExpression).Expression as MethodInvocationExpression;
				}
				if (expression == null)
				{
					return null;
				}
				if (expression.Arguments.Count == 2)
				{
					Expression item = expression.Arguments[0];
					Expression item1 = expression.Arguments[1];
					if (!this.AreTheSame(item, left))
					{
						return null;
					}
					if (expression.MethodExpression.Method.Name != "Combine")
					{
						if (expression.MethodExpression.Method.Name != "Remove")
						{
							return null;
						}
						binaryOperator = BinaryOperator.SubtractAssign;
					}
					else
					{
						binaryOperator = BinaryOperator.AddAssign;
					}
					List<Instruction> instructions = new List<Instruction>(node.MappedInstructions);
					instructions.AddRange(expression.InvocationInstructions);
					return new BinaryExpression(binaryOperator, left, item1, this.typeSystem, instructions, false);
				}
			}
			return null;
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			BinaryOperator binaryOperator;
			UnaryOperator unaryOperator;
			MethodReferenceExpression methodExpression = node.MethodExpression;
			if (methodExpression == null || methodExpression.Method.CallingConvention == MethodCallingConvention.StdCall)
			{
				return null;
			}
			MethodReference method = methodExpression.Method;
			if (OperatorStep.binaryOperators.TryGetValue(method.Name, out binaryOperator))
			{
				return this.BuildBinaryExpression(binaryOperator, node.Arguments[0], node.Arguments[1], method.FixedReturnType, node.InvocationInstructions);
			}
			if (OperatorStep.unaryOperators.TryGetValue(method.Name, out unaryOperator))
			{
				return this.BuildUnaryExpression(unaryOperator, node.Arguments[0], node.InvocationInstructions);
			}
			if (method.Name == "op_True")
			{
				return (Expression)this.codeTransformer.Visit(node.Arguments[0]);
			}
			if (method.Name == "op_False")
			{
				return new ConditionExpression((Expression)this.codeTransformer.Visit(node.Arguments[0]), new LiteralExpression(false, this.typeSystem, null), new LiteralExpression(true, this.typeSystem, null), node.InvocationInstructions);
			}
			if (method.Name == "op_Explicit")
			{
				return new ExplicitCastExpression((Expression)this.codeTransformer.Visit(node.Arguments[0]), node.ExpressionType, node.InvocationInstructions);
			}
			if (method.Name == "op_Implicit")
			{
				return new ImplicitCastExpression((Expression)this.codeTransformer.Visit(node.Arguments[0]), node.ExpressionType, node.InvocationInstructions);
			}
			if (!(method.Name == "get_Chars") || !(node.MethodExpression.Target.ExpressionType.FullName == "System.String"))
			{
				return null;
			}
			ArrayIndexerExpression arrayIndexerExpression = new ArrayIndexerExpression(node.MethodExpression.Target, node.InvocationInstructions);
			foreach (Expression argument in node.Arguments)
			{
				arrayIndexerExpression.Indices.Add(argument);
			}
			return arrayIndexerExpression;
		}
	}
}