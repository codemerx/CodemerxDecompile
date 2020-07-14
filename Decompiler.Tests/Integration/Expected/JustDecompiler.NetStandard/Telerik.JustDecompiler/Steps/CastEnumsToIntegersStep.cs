using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class CastEnumsToIntegersStep : BaseCodeVisitor, IDecompilationStep
	{
		private TypeSystem typeSystem;

		private TypeReference decompiledMethodReturnType;

		public CastEnumsToIntegersStep()
		{
		}

		private void AddArrayInitializerCasts(TypeDefinition arrayType, BlockExpression blockExpression)
		{
			for (int i = 0; i < blockExpression.Expressions.Count; i++)
			{
				Expression item = blockExpression.Expressions[i];
				if (item.CodeNodeType == CodeNodeType.BlockExpression)
				{
					this.AddArrayInitializerCasts(arrayType, item as BlockExpression);
				}
				else if (this.ShouldAddCast(arrayType, item.ExpressionType))
				{
					blockExpression.Expressions[i] = new ExplicitCastExpression(item, arrayType, null);
				}
			}
		}

		private void CheckArguments(Mono.Collections.Generic.Collection<ParameterDefinition> parameters, ExpressionCollection arguments)
		{
			for (int i = 0; i < arguments.Count; i++)
			{
				Expression item = arguments[i];
				if (item.HasType)
				{
					TypeDefinition typeDefinition = item.ExpressionType.Resolve();
					TypeReference parameterType = parameters.get_Item(i).get_ParameterType();
					if (this.ShouldAddCast(typeDefinition, parameterType))
					{
						arguments[i] = new ExplicitCastExpression(item, parameterType, null);
					}
				}
			}
		}

		private TypeReference GetEnumUnderlyingType(TypeDefinition enumType)
		{
			return enumType.get_Fields().get_Item(0).get_FieldType();
		}

		private string GetExpressionTypeName(Expression expression)
		{
			if (!expression.HasType)
			{
				return "";
			}
			return expression.ExpressionType.get_FullName();
		}

		private bool IsArithmeticOperator(BinaryOperator binaryOperator)
		{
			if (binaryOperator == BinaryOperator.Add || binaryOperator == BinaryOperator.Subtract || binaryOperator == BinaryOperator.Divide || binaryOperator == BinaryOperator.Multiply)
			{
				return true;
			}
			return binaryOperator == BinaryOperator.Modulo;
		}

		private bool IsBitwiseOperator(BinaryOperator binaryOperator)
		{
			if (binaryOperator == BinaryOperator.BitwiseAnd || binaryOperator == BinaryOperator.BitwiseOr)
			{
				return true;
			}
			return binaryOperator == BinaryOperator.BitwiseXor;
		}

		private bool IsIntegerType(TypeReference type)
		{
			if (type == null)
			{
				return false;
			}
			string fullName = type.get_FullName();
			if (!(fullName == this.typeSystem.get_Byte().get_FullName()) && !(fullName == this.typeSystem.get_SByte().get_FullName()) && !(fullName == this.typeSystem.get_Int16().get_FullName()) && !(fullName == this.typeSystem.get_UInt16().get_FullName()) && !(fullName == this.typeSystem.get_Int32().get_FullName()) && !(fullName == this.typeSystem.get_UInt32().get_FullName()) && !(fullName == this.typeSystem.get_Int64().get_FullName()) && !(fullName == this.typeSystem.get_UInt64().get_FullName()))
			{
				return false;
			}
			return true;
		}

		private bool IsShiftOperator(BinaryOperator binaryOperator)
		{
			if (binaryOperator == BinaryOperator.LeftShift)
			{
				return true;
			}
			return binaryOperator == BinaryOperator.RightShift;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.MethodContext.Method.get_Module().get_TypeSystem();
			this.decompiledMethodReturnType = context.MethodContext.Method.get_ReturnType();
			this.Visit(body);
			return body;
		}

		public bool ShouldAddCast(TypeDefinition supposedEnum, TypeReference expectedType)
		{
			if (supposedEnum == null || expectedType == null)
			{
				return false;
			}
			if (supposedEnum != null && supposedEnum.get_IsEnum() && this.IsIntegerType(expectedType))
			{
				return true;
			}
			return false;
		}

		public override void VisitArrayCreationExpression(ArrayCreationExpression node)
		{
			base.VisitArrayCreationExpression(node);
			if (node.Initializer != null)
			{
				TypeDefinition typeDefinition = node.ExpressionType.Resolve();
				this.AddArrayInitializerCasts(typeDefinition, node.Initializer.Expression);
			}
		}

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			base.VisitArrayIndexerExpression(node);
			for (int i = 0; i < node.Indices.Count; i++)
			{
				Expression item = node.Indices[i];
				if (item.HasType && this.ShouldAddCast(item.ExpressionType.Resolve(), this.typeSystem.get_Int32()))
				{
					node.Indices[i] = new ExplicitCastExpression(item, this.typeSystem.get_Int32(), null);
				}
			}
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			string expressionTypeName = this.GetExpressionTypeName(node.Left);
			string str = this.GetExpressionTypeName(node.Right);
			base.VisitBinaryExpression(node);
			bool flag = (this.GetExpressionTypeName(node.Left) != expressionTypeName ? true : this.GetExpressionTypeName(node.Right) != str);
			if (node.IsAssignmentExpression || node.IsSelfAssign && !node.IsEventHandlerAddOrRemove)
			{
				Expression left = node.Left;
				Expression right = node.Right;
				if (right.HasType && left.HasType)
				{
					TypeReference expressionType = left.ExpressionType;
					TypeDefinition typeDefinition = right.ExpressionType.Resolve();
					if (this.ShouldAddCast(typeDefinition, expressionType))
					{
						node.Right = new ExplicitCastExpression(right, expressionType, null);
						flag = true;
					}
					TypeDefinition typeDefinition1 = expressionType.Resolve();
					if (typeDefinition1 != null && typeDefinition != null && typeDefinition1.get_IsEnum() && typeDefinition.get_IsValueType() && typeDefinition1.get_FullName() != typeDefinition.get_FullName())
					{
						node.Right = new ExplicitCastExpression(right, expressionType, null);
						flag = true;
					}
				}
			}
			else if (this.IsArithmeticOperator(node.Operator))
			{
				if (node.Left.HasType)
				{
					TypeDefinition typeDefinition2 = node.Left.ExpressionType.Resolve();
					if (typeDefinition2 != null && typeDefinition2.get_IsEnum())
					{
						node.Left = new ExplicitCastExpression(node.Left, this.GetEnumUnderlyingType(typeDefinition2), null);
						flag = true;
					}
				}
				if (node.Right.HasType)
				{
					TypeDefinition typeDefinition3 = node.Right.ExpressionType.Resolve();
					if (typeDefinition3 != null && typeDefinition3.get_IsEnum())
					{
						node.Right = new ExplicitCastExpression(node.Right, this.GetEnumUnderlyingType(typeDefinition3), null);
						flag = true;
					}
				}
			}
			else if (this.IsBitwiseOperator(node.Operator) || node.IsComparisonExpression)
			{
				Expression expression = node.Left;
				Expression right1 = node.Right;
				if (right1.HasType && expression.HasType)
				{
					TypeReference typeReference = expression.ExpressionType;
					TypeDefinition typeDefinition4 = right1.ExpressionType.Resolve();
					if (this.ShouldAddCast(typeDefinition4, typeReference))
					{
						node.Right = new ExplicitCastExpression(right1, typeReference, null);
						flag = true;
					}
					TypeReference expressionType1 = right1.ExpressionType;
					TypeDefinition typeDefinition5 = expression.ExpressionType.Resolve();
					if (this.ShouldAddCast(typeDefinition5, expressionType1))
					{
						node.Left = new ExplicitCastExpression(expression, expressionType1, null);
						flag = true;
					}
					if (typeDefinition5 != null && typeDefinition5.get_IsEnum() && typeDefinition4 != null && typeDefinition4.get_IsEnum() && typeDefinition5.get_FullName() != typeDefinition4.get_FullName())
					{
						node.Left = new ExplicitCastExpression(expression, this.GetEnumUnderlyingType(typeDefinition5), null);
						node.Right = new ExplicitCastExpression(right1, this.GetEnumUnderlyingType(typeDefinition5), null);
						flag = true;
					}
				}
			}
			else if (this.IsShiftOperator(node.Operator))
			{
				if (node.Left.HasType)
				{
					TypeDefinition typeDefinition6 = node.Left.ExpressionType.Resolve();
					if (typeDefinition6 != null && typeDefinition6.get_IsEnum())
					{
						node.Left = new ExplicitCastExpression(node.Left, this.GetEnumUnderlyingType(typeDefinition6), null);
						flag = true;
					}
				}
				if (node.Right.HasType)
				{
					TypeDefinition typeDefinition7 = node.Right.ExpressionType.Resolve();
					if (typeDefinition7 != null && typeDefinition7.get_IsEnum())
					{
						node.Right = new ExplicitCastExpression(node.Right, this.GetEnumUnderlyingType(typeDefinition7), null);
						flag = true;
					}
				}
			}
			if (flag)
			{
				node.UpdateType();
			}
		}

		public override void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			base.VisitDelegateInvokeExpression(node);
			this.CheckArguments(node.InvokeMethodReference.get_Parameters(), node.Arguments);
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.VisitMethodInvocationExpression(node);
			this.CheckArguments(node.MethodExpression.Method.get_Parameters(), node.Arguments);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);
			if (node.Constructor != null)
			{
				this.CheckArguments(node.Constructor.get_Parameters(), node.Arguments);
			}
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			base.VisitPropertyReferenceExpression(node);
			if (node.IsIndexer)
			{
				this.CheckArguments(node.Property.get_Parameters(), node.Arguments);
			}
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			base.VisitReturnExpression(node);
			if (node.Value != null && node.Value.HasType && this.ShouldAddCast(node.Value.ExpressionType.Resolve(), this.decompiledMethodReturnType))
			{
				node.Value = new ExplicitCastExpression(node.Value, this.decompiledMethodReturnType, null);
			}
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			base.VisitUnaryExpression(node);
			if ((node.Operator == UnaryOperator.Negate || node.Operator == UnaryOperator.UnaryPlus) && node.Operand.HasType)
			{
				TypeDefinition typeDefinition = node.Operand.ExpressionType.Resolve();
				if (typeDefinition != null && typeDefinition.get_IsEnum())
				{
					node.Operand = new ExplicitCastExpression(node.Operand, this.GetEnumUnderlyingType(typeDefinition), null);
					node.DecideExpressionType();
				}
			}
		}
	}
}