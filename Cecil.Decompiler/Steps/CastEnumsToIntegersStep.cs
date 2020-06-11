using System;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	class CastEnumsToIntegersStep : BaseCodeVisitor, IDecompilationStep
	{
		private TypeSystem typeSystem;
		private TypeReference decompiledMethodReturnType;

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			this.decompiledMethodReturnType = context.MethodContext.Method.ReturnType;
			Visit(body);
			return body;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			string oldLeftTypeName = GetExpressionTypeName(node.Left);
			string oldRightTypeName = GetExpressionTypeName(node.Right);

			base.VisitBinaryExpression(node);

			bool shouldUpdateType = GetExpressionTypeName(node.Left) != oldLeftTypeName || GetExpressionTypeName(node.Right) != oldRightTypeName;

			if (node.IsAssignmentExpression || (node.IsSelfAssign && !node.IsEventHandlerAddOrRemove))
			{
				/// Add cast to integer type, if the left side of an assignment is of integer type and
				/// the right side is of enum type.
				Expression leftSide = node.Left;
				Expression rightSide = node.Right;
				if (rightSide.HasType && leftSide.HasType)
				{
					TypeReference leftType = leftSide.ExpressionType;
					TypeDefinition rightType = rightSide.ExpressionType.Resolve();
					if (ShouldAddCast(rightType, leftType))
					{
						node.Right = new ExplicitCastExpression(rightSide, leftType, null);
						shouldUpdateType = true;
					}

					TypeDefinition supposedLeftTypeEnum = leftType.Resolve();
					if (supposedLeftTypeEnum != null && rightType != null && supposedLeftTypeEnum.IsEnum && rightType.IsValueType && supposedLeftTypeEnum.FullName != rightType.FullName)
					{
						node.Right = new ExplicitCastExpression(rightSide, leftType, null);
						shouldUpdateType = true;
					}
				}
			}
			else if (IsArithmeticOperator(node.Operator))
			{
				/// Add cast to enum's underlying type when taking part in an arithmetic operation

				if (node.Left.HasType)
				{
					TypeDefinition leftType = node.Left.ExpressionType.Resolve();
					if (leftType != null && leftType.IsEnum)
					{
						node.Left = new ExplicitCastExpression(node.Left, GetEnumUnderlyingType(leftType), null);
						shouldUpdateType = true;
					}
				}

				if (node.Right.HasType)
				{
					TypeDefinition rightType = node.Right.ExpressionType.Resolve();
					if (rightType != null && rightType.IsEnum)
					{
						node.Right = new ExplicitCastExpression(node.Right, GetEnumUnderlyingType(rightType), null);
						shouldUpdateType = true;
					}
				}
			}
			else if (IsBitwiseOperator(node.Operator) || node.IsComparisonExpression)
			{
				/// Adds cast to integer type when exactly one side of the binary expression
				/// that is a bitwise operation is an enum

				Expression leftSide = node.Left;
				Expression rightSide = node.Right;
				if (rightSide.HasType && leftSide.HasType)
				{
					TypeReference leftTypeInteger = leftSide.ExpressionType;
					TypeDefinition rightTypeEnum = rightSide.ExpressionType.Resolve();
					if (ShouldAddCast(rightTypeEnum, leftTypeInteger))
					{
						node.Right = new ExplicitCastExpression(rightSide, leftTypeInteger, null);
						shouldUpdateType = true;
					}

					TypeReference rightTypeInteger = rightSide.ExpressionType;
					TypeDefinition leftTypeEnum = leftSide.ExpressionType.Resolve();
					if (ShouldAddCast(leftTypeEnum, rightTypeInteger))
					{
						node.Left = new ExplicitCastExpression(leftSide, rightTypeInteger, null);
						shouldUpdateType = true;
					}

					if (leftTypeEnum != null && leftTypeEnum.IsEnum && rightTypeEnum != null && rightTypeEnum.IsEnum && leftTypeEnum.FullName != rightTypeEnum.FullName)
					{
						node.Left = new ExplicitCastExpression(leftSide, GetEnumUnderlyingType(leftTypeEnum), null);
						node.Right = new ExplicitCastExpression(rightSide, GetEnumUnderlyingType(leftTypeEnum), null);
						shouldUpdateType = true;
					}
				}
			}
			else if (IsShiftOperator(node.Operator))
			{
				if (node.Left.HasType)
				{
					TypeDefinition leftSupposedEnum = node.Left.ExpressionType.Resolve();
					if (leftSupposedEnum != null && leftSupposedEnum.IsEnum)
					{
						node.Left = new ExplicitCastExpression(node.Left, GetEnumUnderlyingType(leftSupposedEnum), null);
						shouldUpdateType = true;
					}
				}

				if (node.Right.HasType)
				{
					TypeDefinition rightSupposedEnum = node.Right.ExpressionType.Resolve();
					if (rightSupposedEnum != null && rightSupposedEnum.IsEnum)
					{
						node.Right = new ExplicitCastExpression(node.Right, GetEnumUnderlyingType(rightSupposedEnum), null);
						shouldUpdateType = true;
					}
				}
			}

			if (shouldUpdateType)
			{
				node.UpdateType();
			}
		}

		private string GetExpressionTypeName(Expression expression)
		{
			return expression.HasType ? expression.ExpressionType.FullName : "";
		}

		private bool IsArithmeticOperator(BinaryOperator binaryOperator)
		{
			return binaryOperator == BinaryOperator.Add || binaryOperator == BinaryOperator.Subtract || binaryOperator == BinaryOperator.Divide ||
						binaryOperator == BinaryOperator.Multiply || binaryOperator == BinaryOperator.Modulo;
		}

		private bool IsBitwiseOperator(BinaryOperator binaryOperator)
		{
			return binaryOperator == BinaryOperator.BitwiseAnd || binaryOperator == BinaryOperator.BitwiseOr || binaryOperator == BinaryOperator.BitwiseXor;
		}

		private bool IsShiftOperator(BinaryOperator binaryOperator)
		{
			return binaryOperator == BinaryOperator.LeftShift || binaryOperator == BinaryOperator.RightShift;
		}

		private TypeReference GetEnumUnderlyingType(TypeDefinition enumType)
		{
			return enumType.Fields[0].FieldType;
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			base.VisitUnaryExpression(node);

			if (node.Operator == UnaryOperator.Negate || node.Operator == UnaryOperator.UnaryPlus)
			{
				if (node.Operand.HasType)
				{
					TypeDefinition supposedEnum = node.Operand.ExpressionType.Resolve();
					if (supposedEnum != null && supposedEnum.IsEnum)
					{
						node.Operand = new ExplicitCastExpression(node.Operand, GetEnumUnderlyingType(supposedEnum), null);
						node.DecideExpressionType();
					}
				}
			}
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			/// Add cast when an argument needs to be of integer type, but the pushed value is of enum type
			base.VisitMethodInvocationExpression(node);

			CheckArguments(node.MethodExpression.Method.Parameters, node.Arguments);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);

			if (node.Constructor != null)
			{
				CheckArguments(node.Constructor.Parameters, node.Arguments);
			}

			// node.Initializer contains BinaryExpressions
		}

		public override void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			/// Add cast when an argument needs to be of integer type, but the pushed value is of enum type
			base.VisitDelegateInvokeExpression(node);

			CheckArguments(node.InvokeMethodReference.Parameters, node.Arguments);
		}

		private void CheckArguments(Mono.Collections.Generic.Collection<ParameterDefinition> parameters, ExpressionCollection arguments)
		{
			for (int i = 0; i < arguments.Count; i++)
			{
				Expression argument = arguments[i];
				if (argument.HasType)
				{
					TypeDefinition argumentType = argument.ExpressionType.Resolve();
					TypeReference parameterType = parameters[i].ParameterType;
					if (ShouldAddCast(argumentType, parameterType))
					{
						arguments[i] = new ExplicitCastExpression(argument, parameterType, null);
					}
				}
			}
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			/// Add cast when the returned value is of enum type, but the signature of the method declares integer type.
			base.VisitReturnExpression(node);

			if (node.Value != null && node.Value.HasType)
			{
				TypeDefinition returnedType = node.Value.ExpressionType.Resolve();
				if (ShouldAddCast(returnedType, decompiledMethodReturnType))
				{
					node.Value = new ExplicitCastExpression(node.Value, decompiledMethodReturnType, null);
				}
			}
		}

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			base.VisitArrayIndexerExpression(node);

			for (int i = 0; i < node.Indices.Count; i++)
			{
				Expression index = node.Indices[i];
				if (index.HasType)
				{
					TypeDefinition indexType = index.ExpressionType.Resolve();
					if (ShouldAddCast(indexType, typeSystem.Int32))
					{
						node.Indices[i] = new ExplicitCastExpression(index, typeSystem.Int32, null);
					}
				}
			}
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			base.VisitArrayCreationExpression(node);

			if (node.Initializer != null)
			{
				TypeDefinition arrayType = node.ExpressionType.Resolve();
				AddArrayInitializerCasts(arrayType, node.Initializer.Expression);
			}
		}

		private void AddArrayInitializerCasts(TypeDefinition arrayType, BlockExpression blockExpression)
		{
			for (int i = 0; i < blockExpression.Expressions.Count; i++)
			{
				Expression element = blockExpression.Expressions[i];

				// Multidimensional array
				if (element.CodeNodeType == CodeNodeType.BlockExpression)
				{
					AddArrayInitializerCasts(arrayType, element as BlockExpression);
				}
				// Array elements
				else if (ShouldAddCast(arrayType, element.ExpressionType))
				{
					blockExpression.Expressions[i] = new ExplicitCastExpression(element, arrayType, null);
				}
			}
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			base.VisitPropertyReferenceExpression(node);
			if (node.IsIndexer)
			{
				CheckArguments(node.Property.Parameters, node.Arguments);
			}
		}

		public bool ShouldAddCast(TypeDefinition supposedEnum, TypeReference expectedType)
		{
			if (supposedEnum == null || expectedType == null)
			{
				return false;
			}
			if (supposedEnum != null && supposedEnum.IsEnum)
			{
				if (IsIntegerType(expectedType))
				{
					return true;
				}
			}
			return false;
		}

		private bool IsIntegerType(TypeReference type)
		{
			if (type == null)
			{
				return false;
			}
			string typeName = type.FullName;
			if (typeName == typeSystem.Byte.FullName ||
				typeName == typeSystem.SByte.FullName ||
				typeName == typeSystem.Int16.FullName ||
				typeName == typeSystem.UInt16.FullName ||
				typeName == typeSystem.Int32.FullName ||
				typeName == typeSystem.UInt32.FullName ||
				typeName == typeSystem.Int64.FullName ||
				typeName == typeSystem.UInt64.FullName)
			{
				return true;
			}
			return false;
		}
	}
}
