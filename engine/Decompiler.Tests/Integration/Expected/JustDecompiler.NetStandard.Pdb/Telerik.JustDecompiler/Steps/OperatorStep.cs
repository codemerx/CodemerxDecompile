using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
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
			stackVariable0 = new Dictionary<string, BinaryOperator>();
			stackVariable0.Add("op_Equality", 9);
			stackVariable0.Add("op_Inequality", 10);
			stackVariable0.Add("op_GreaterThan", 15);
			stackVariable0.Add("op_GreaterThanOrEqual", 16);
			stackVariable0.Add("op_LessThan", 13);
			stackVariable0.Add("op_LessThanOrEqual", 14);
			stackVariable0.Add("op_Addition", 1);
			stackVariable0.Add("op_Subtraction", 3);
			stackVariable0.Add("op_Division", 7);
			stackVariable0.Add("op_Multiply", 5);
			stackVariable0.Add("op_Modulus", 24);
			stackVariable0.Add("op_BitwiseAnd", 22);
			stackVariable0.Add("op_BitwiseOr", 21);
			stackVariable0.Add("op_ExclusiveOr", 23);
			stackVariable0.Add("op_RightShift", 19);
			stackVariable0.Add("op_LeftShift", 17);
			OperatorStep.binaryOperators = stackVariable0;
			stackVariable33 = new Dictionary<string, UnaryOperator>();
			stackVariable33.Add("op_UnaryNegation", 0);
			stackVariable33.Add("op_LogicalNot", 1);
			stackVariable33.Add("op_OnesComplement", 2);
			stackVariable33.Add("op_Decrement", 3);
			stackVariable33.Add("op_Increment", 4);
			stackVariable33.Add("op_UnaryPlus", 10);
			OperatorStep.unaryOperators = stackVariable33;
			return;
		}

		public OperatorStep(BaseCodeTransformer codeTransformer, TypeSystem typeSystem)
		{
			base();
			this.codeTransformer = codeTransformer;
			this.typeSystem = typeSystem;
			return;
		}

		private bool AreTheSame(Expression first, Expression second)
		{
			if (first.get_CodeNodeType() != second.get_CodeNodeType())
			{
				return false;
			}
			return first.Equals(second);
		}

		private ICodeNode BuildBinaryExpression(BinaryOperator operator, Expression left, Expression right, TypeReference expressionType, IEnumerable<Instruction> instructions)
		{
			V_0 = new BinaryExpression(operator, (Expression)this.codeTransformer.Visit(left), (Expression)this.codeTransformer.Visit(right), expressionType, this.typeSystem, instructions, true);
			if (V_0.get_IsComparisonExpression() || V_0.get_IsLogicalExpression())
			{
				V_0.set_ExpressionType(left.get_ExpressionType().get_Module().get_TypeSystem().get_Boolean());
			}
			return V_0;
		}

		private ICodeNode BuildUnaryExpression(UnaryOperator operator, Expression expression, IEnumerable<Instruction> instructions)
		{
			return new UnaryExpression(operator, (Expression)this.codeTransformer.Visit(expression), instructions);
		}

		internal BinaryExpression VisitAssignExpression(BinaryExpression node)
		{
			V_0 = node.get_Left();
			V_1 = V_0.get_ExpressionType().Resolve();
			if (V_1 == null)
			{
				return null;
			}
			if (V_1.get_BaseType() != null && String.op_Equality(V_1.get_BaseType().get_Name(), "MulticastDelegate"))
			{
				V_2 = node.get_Right();
				V_3 = V_2 as MethodInvocationExpression;
				if (V_2 as ExplicitCastExpression != null)
				{
					V_3 = (V_2 as ExplicitCastExpression).get_Expression() as MethodInvocationExpression;
				}
				if (V_3 == null)
				{
					return null;
				}
				if (V_3.get_Arguments().get_Count() == 2)
				{
					V_4 = V_3.get_Arguments().get_Item(0);
					V_5 = V_3.get_Arguments().get_Item(1);
					if (!this.AreTheSame(V_4, V_0))
					{
						return null;
					}
					if (!String.op_Equality(V_3.get_MethodExpression().get_Method().get_Name(), "Combine"))
					{
						if (!String.op_Equality(V_3.get_MethodExpression().get_Method().get_Name(), "Remove"))
						{
							return null;
						}
						V_6 = 4;
					}
					else
					{
						V_6 = 2;
					}
					V_7 = new List<Instruction>(node.get_MappedInstructions());
					V_7.AddRange(V_3.get_InvocationInstructions());
					return new BinaryExpression(V_6, V_0, V_5, this.typeSystem, V_7, false);
				}
			}
			return null;
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			V_0 = node.get_MethodExpression();
			if (V_0 == null || V_0.get_Method().get_CallingConvention() == 2)
			{
				return null;
			}
			V_1 = V_0.get_Method();
			if (OperatorStep.binaryOperators.TryGetValue(V_1.get_Name(), out V_2))
			{
				return this.BuildBinaryExpression(V_2, node.get_Arguments().get_Item(0), node.get_Arguments().get_Item(1), V_1.get_FixedReturnType(), node.get_InvocationInstructions());
			}
			if (OperatorStep.unaryOperators.TryGetValue(V_1.get_Name(), out V_3))
			{
				return this.BuildUnaryExpression(V_3, node.get_Arguments().get_Item(0), node.get_InvocationInstructions());
			}
			if (String.op_Equality(V_1.get_Name(), "op_True"))
			{
				return (Expression)this.codeTransformer.Visit(node.get_Arguments().get_Item(0));
			}
			if (String.op_Equality(V_1.get_Name(), "op_False"))
			{
				return new ConditionExpression((Expression)this.codeTransformer.Visit(node.get_Arguments().get_Item(0)), new LiteralExpression(false, this.typeSystem, null), new LiteralExpression(true, this.typeSystem, null), node.get_InvocationInstructions());
			}
			if (String.op_Equality(V_1.get_Name(), "op_Explicit"))
			{
				return new ExplicitCastExpression((Expression)this.codeTransformer.Visit(node.get_Arguments().get_Item(0)), node.get_ExpressionType(), node.get_InvocationInstructions());
			}
			if (String.op_Equality(V_1.get_Name(), "op_Implicit"))
			{
				return new ImplicitCastExpression((Expression)this.codeTransformer.Visit(node.get_Arguments().get_Item(0)), node.get_ExpressionType(), node.get_InvocationInstructions());
			}
			if (!String.op_Equality(V_1.get_Name(), "get_Chars") || !String.op_Equality(node.get_MethodExpression().get_Target().get_ExpressionType().get_FullName(), "System.String"))
			{
				return null;
			}
			V_4 = new ArrayIndexerExpression(node.get_MethodExpression().get_Target(), node.get_InvocationInstructions());
			V_5 = node.get_Arguments().GetEnumerator();
			try
			{
				while (V_5.MoveNext())
				{
					V_6 = V_5.get_Current();
					V_4.get_Indices().Add(V_6);
				}
			}
			finally
			{
				if (V_5 != null)
				{
					V_5.Dispose();
				}
			}
			return V_4;
		}
	}
}