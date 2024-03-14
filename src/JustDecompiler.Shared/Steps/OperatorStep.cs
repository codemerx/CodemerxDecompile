using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Steps
{
	public class OperatorStep
	{
        private readonly BaseCodeTransformer codeTransformer;
        private readonly TypeSystem typeSystem;

		public OperatorStep(BaseCodeTransformer codeTransformer, TypeSystem typeSystem)
		{
			this.codeTransformer = codeTransformer;
            this.typeSystem = typeSystem;
		}

		static readonly Dictionary<string, BinaryOperator> binaryOperators
			= new Dictionary<string, BinaryOperator> () {

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
			{ "op_LeftShift", BinaryOperator.LeftShift },
		};

		static readonly Dictionary<string, UnaryOperator> unaryOperators
			= new Dictionary<string, UnaryOperator> () {

			{ "op_UnaryNegation", UnaryOperator.Negate },
			{ "op_LogicalNot", UnaryOperator.LogicalNot },
			{ "op_OnesComplement", UnaryOperator.BitwiseNot },
			{ "op_Decrement", UnaryOperator.PostDecrement },
			{ "op_Increment", UnaryOperator.PostIncrement },
            { "op_UnaryPlus", UnaryOperator.UnaryPlus}
		};

        public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
        {
            MethodReferenceExpression methodReference = node.MethodExpression;
            if ((methodReference == null) ||
                (methodReference.Method.CallingConvention == MethodCallingConvention.StdCall))
            {
                return null;
            }

			MethodReference method = methodReference.Method;

			BinaryOperator binaryOperator;
            if (binaryOperators.TryGetValue (method.Name, out binaryOperator))
            {
                return BuildBinaryExpression(binaryOperator, node.Arguments[0], node.Arguments[1], method.FixedReturnType, node.InvocationInstructions);
            }

			UnaryOperator unaryOperator;
            if (unaryOperators.TryGetValue (method.Name, out unaryOperator))
            {
                return BuildUnaryExpression(unaryOperator, node.Arguments[0], node.InvocationInstructions);
            }
			
            if(method.Name == "op_True")
            {
                return (Expression)codeTransformer.Visit(node.Arguments[0]);
            }
            else if(method.Name == "op_False")
            {
                //TODO: Must consider better representation
                return new ConditionExpression((Expression)codeTransformer.Visit(node.Arguments[0]),
                        new LiteralExpression(false, typeSystem, null), new LiteralExpression(true, typeSystem, null), node.InvocationInstructions);
            }

            if (method.Name == "op_Explicit")
            {
                return new ExplicitCastExpression((Expression)codeTransformer.Visit(node.Arguments[0]), node.ExpressionType, node.InvocationInstructions);
            }

            if (method.Name == "op_Implicit")
            {
                return new ImplicitCastExpression((Expression)codeTransformer.Visit(node.Arguments[0]), node.ExpressionType, node.InvocationInstructions);
            }

            if (method.Name == "get_Chars" && node.MethodExpression.Target.ExpressionType.FullName == "System.String")
            {
                ArrayIndexerExpression stringIndexing = new ArrayIndexerExpression(node.MethodExpression.Target, node.InvocationInstructions);
                foreach (Expression arg in node.Arguments)
                {
                    stringIndexing.Indices.Add(arg);
                }
                return stringIndexing;
            }

			return null;
		}

		ICodeNode BuildUnaryExpression(UnaryOperator @operator, Expression expression, IEnumerable<Instruction> instructions)
		{
			return new UnaryExpression(@operator, (Expression)codeTransformer.Visit(expression), instructions);
		}

		ICodeNode BuildBinaryExpression(BinaryOperator @operator, Expression left, Expression right, TypeReference expressionType, IEnumerable<Instruction> instructions)
		{
			BinaryExpression result =
				new BinaryExpression(@operator, (Expression)codeTransformer.Visit(left), (Expression)codeTransformer.Visit(right), 
								expressionType, typeSystem, instructions, true);
			if (result.IsComparisonExpression || result.IsLogicalExpression)
			{
				result.ExpressionType = left.ExpressionType.Module.TypeSystem.Boolean;
			}
			return result;
		}

        internal BinaryExpression VisitAssignExpression(BinaryExpression node)
        {
            Expression left = node.Left;
            TypeDefinition tr = left.ExpressionType.Resolve();
            if (tr == null)
            {
                return null;
            }
            if (tr.BaseType!= null && tr.BaseType.Name == "MulticastDelegate")
            {
                Expression right = node.Right;
                MethodInvocationExpression methodInvocation = right as MethodInvocationExpression;
                if (right is ExplicitCastExpression)
                {
                    methodInvocation = (right as ExplicitCastExpression).Expression as MethodInvocationExpression;
                }
                if (methodInvocation == null)
                {
                    return null;
                }
                if (methodInvocation.Arguments.Count == 2)
                {
                    Expression firstArgument = methodInvocation.Arguments[0];
                    Expression secondArgument = methodInvocation.Arguments[1];
                    BinaryOperator @operator;

                    if (!AreTheSame(firstArgument, left))
                    {
                        return null;
                    }
                    //if firstArgument == leftSide

                    if (methodInvocation.MethodExpression.Method.Name == "Combine")
                    {
                        @operator = BinaryOperator.AddAssign;
                    }
                    else if (methodInvocation.MethodExpression.Method.Name == "Remove")
                    {
                        @operator = BinaryOperator.SubtractAssign;
                    }
                    else
                    {
                        return null;
                    }
                    List<Instruction> instructions = new List<Instruction>(node.MappedInstructions);
                    instructions.AddRange(methodInvocation.InvocationInstructions);
                    return new BinaryExpression(@operator, left, secondArgument, typeSystem, instructions);
                }
            }
            return null;
        }

        private bool AreTheSame(Expression first, Expression second)
        {
            //those expressions' properties should point to the same objects, however the expressions themselves might not be the same object
            if (first.CodeNodeType != second.CodeNodeType)
            {
                return false;
            }
            return first.Equals(second);
        }
    }
}