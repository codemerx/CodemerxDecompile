using Mono.Cecil;
using Mono.Collections.Generic;
using System;
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
			base();
			return;
		}

		private void AddArrayInitializerCasts(TypeDefinition arrayType, BlockExpression blockExpression)
		{
			V_0 = 0;
			while (V_0 < blockExpression.get_Expressions().get_Count())
			{
				V_1 = blockExpression.get_Expressions().get_Item(V_0);
				if (V_1.get_CodeNodeType() != 18)
				{
					if (this.ShouldAddCast(arrayType, V_1.get_ExpressionType()))
					{
						blockExpression.get_Expressions().set_Item(V_0, new ExplicitCastExpression(V_1, arrayType, null));
					}
				}
				else
				{
					this.AddArrayInitializerCasts(arrayType, V_1 as BlockExpression);
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private void CheckArguments(Collection<ParameterDefinition> parameters, ExpressionCollection arguments)
		{
			V_0 = 0;
			while (V_0 < arguments.get_Count())
			{
				V_1 = arguments.get_Item(V_0);
				if (V_1.get_HasType())
				{
					V_2 = V_1.get_ExpressionType().Resolve();
					V_3 = parameters.get_Item(V_0).get_ParameterType();
					if (this.ShouldAddCast(V_2, V_3))
					{
						arguments.set_Item(V_0, new ExplicitCastExpression(V_1, V_3, null));
					}
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private TypeReference GetEnumUnderlyingType(TypeDefinition enumType)
		{
			return enumType.get_Fields().get_Item(0).get_FieldType();
		}

		private string GetExpressionTypeName(Expression expression)
		{
			if (!expression.get_HasType())
			{
				return "";
			}
			return expression.get_ExpressionType().get_FullName();
		}

		private bool IsArithmeticOperator(BinaryOperator binaryOperator)
		{
			if (binaryOperator == 1 || binaryOperator == 3 || binaryOperator == 7 || binaryOperator == 5)
			{
				return true;
			}
			return binaryOperator == 24;
		}

		private bool IsBitwiseOperator(BinaryOperator binaryOperator)
		{
			if (binaryOperator == 22 || binaryOperator == 21)
			{
				return true;
			}
			return binaryOperator == 23;
		}

		private bool IsIntegerType(TypeReference type)
		{
			if (type == null)
			{
				return false;
			}
			V_0 = type.get_FullName();
			if (!String.op_Equality(V_0, this.typeSystem.get_Byte().get_FullName()) && !String.op_Equality(V_0, this.typeSystem.get_SByte().get_FullName()) && !String.op_Equality(V_0, this.typeSystem.get_Int16().get_FullName()) && !String.op_Equality(V_0, this.typeSystem.get_UInt16().get_FullName()) && !String.op_Equality(V_0, this.typeSystem.get_Int32().get_FullName()) && !String.op_Equality(V_0, this.typeSystem.get_UInt32().get_FullName()) && !String.op_Equality(V_0, this.typeSystem.get_Int64().get_FullName()) && !String.op_Equality(V_0, this.typeSystem.get_UInt64().get_FullName()))
			{
				return false;
			}
			return true;
		}

		private bool IsShiftOperator(BinaryOperator binaryOperator)
		{
			if (binaryOperator == 17)
			{
				return true;
			}
			return binaryOperator == 19;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.decompiledMethodReturnType = context.get_MethodContext().get_Method().get_ReturnType();
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
			this.VisitArrayCreationExpression(node);
			if (node.get_Initializer() != null)
			{
				V_0 = node.get_ExpressionType().Resolve();
				this.AddArrayInitializerCasts(V_0, node.get_Initializer().get_Expression());
			}
			return;
		}

		public override void VisitArrayIndexerExpression(ArrayIndexerExpression node)
		{
			this.VisitArrayIndexerExpression(node);
			V_0 = 0;
			while (V_0 < node.get_Indices().get_Count())
			{
				V_1 = node.get_Indices().get_Item(V_0);
				if (V_1.get_HasType() && this.ShouldAddCast(V_1.get_ExpressionType().Resolve(), this.typeSystem.get_Int32()))
				{
					node.get_Indices().set_Item(V_0, new ExplicitCastExpression(V_1, this.typeSystem.get_Int32(), null));
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			V_0 = this.GetExpressionTypeName(node.get_Left());
			V_1 = this.GetExpressionTypeName(node.get_Right());
			this.VisitBinaryExpression(node);
			if (String.op_Inequality(this.GetExpressionTypeName(node.get_Left()), V_0))
			{
				stackVariable16 = true;
			}
			else
			{
				stackVariable16 = String.op_Inequality(this.GetExpressionTypeName(node.get_Right()), V_1);
			}
			V_2 = stackVariable16;
			if (node.get_IsAssignmentExpression() || node.get_IsSelfAssign() && !node.get_IsEventHandlerAddOrRemove())
			{
				V_3 = node.get_Left();
				V_4 = node.get_Right();
				if (V_4.get_HasType() && V_3.get_HasType())
				{
					V_5 = V_3.get_ExpressionType();
					V_6 = V_4.get_ExpressionType().Resolve();
					if (this.ShouldAddCast(V_6, V_5))
					{
						node.set_Right(new ExplicitCastExpression(V_4, V_5, null));
						V_2 = true;
					}
					V_7 = V_5.Resolve();
					if (V_7 != null && V_6 != null && V_7.get_IsEnum() && V_6.get_IsValueType() && String.op_Inequality(V_7.get_FullName(), V_6.get_FullName()))
					{
						node.set_Right(new ExplicitCastExpression(V_4, V_5, null));
						V_2 = true;
					}
				}
			}
			else
			{
				if (!this.IsArithmeticOperator(node.get_Operator()))
				{
					if (this.IsBitwiseOperator(node.get_Operator()) || node.get_IsComparisonExpression())
					{
						V_10 = node.get_Left();
						V_11 = node.get_Right();
						if (V_11.get_HasType() && V_10.get_HasType())
						{
							V_12 = V_10.get_ExpressionType();
							V_13 = V_11.get_ExpressionType().Resolve();
							if (this.ShouldAddCast(V_13, V_12))
							{
								node.set_Right(new ExplicitCastExpression(V_11, V_12, null));
								V_2 = true;
							}
							V_14 = V_11.get_ExpressionType();
							V_15 = V_10.get_ExpressionType().Resolve();
							if (this.ShouldAddCast(V_15, V_14))
							{
								node.set_Left(new ExplicitCastExpression(V_10, V_14, null));
								V_2 = true;
							}
							if (V_15 != null && V_15.get_IsEnum() && V_13 != null && V_13.get_IsEnum() && String.op_Inequality(V_15.get_FullName(), V_13.get_FullName()))
							{
								node.set_Left(new ExplicitCastExpression(V_10, this.GetEnumUnderlyingType(V_15), null));
								node.set_Right(new ExplicitCastExpression(V_11, this.GetEnumUnderlyingType(V_15), null));
								V_2 = true;
							}
						}
					}
					else
					{
						if (this.IsShiftOperator(node.get_Operator()))
						{
							if (node.get_Left().get_HasType())
							{
								V_16 = node.get_Left().get_ExpressionType().Resolve();
								if (V_16 != null && V_16.get_IsEnum())
								{
									node.set_Left(new ExplicitCastExpression(node.get_Left(), this.GetEnumUnderlyingType(V_16), null));
									V_2 = true;
								}
							}
							if (node.get_Right().get_HasType())
							{
								V_17 = node.get_Right().get_ExpressionType().Resolve();
								if (V_17 != null && V_17.get_IsEnum())
								{
									node.set_Right(new ExplicitCastExpression(node.get_Right(), this.GetEnumUnderlyingType(V_17), null));
									V_2 = true;
								}
							}
						}
					}
				}
				else
				{
					if (node.get_Left().get_HasType())
					{
						V_8 = node.get_Left().get_ExpressionType().Resolve();
						if (V_8 != null && V_8.get_IsEnum())
						{
							node.set_Left(new ExplicitCastExpression(node.get_Left(), this.GetEnumUnderlyingType(V_8), null));
							V_2 = true;
						}
					}
					if (node.get_Right().get_HasType())
					{
						V_9 = node.get_Right().get_ExpressionType().Resolve();
						if (V_9 != null && V_9.get_IsEnum())
						{
							node.set_Right(new ExplicitCastExpression(node.get_Right(), this.GetEnumUnderlyingType(V_9), null));
							V_2 = true;
						}
					}
				}
			}
			if (V_2)
			{
				node.UpdateType();
			}
			return;
		}

		public override void VisitDelegateInvokeExpression(DelegateInvokeExpression node)
		{
			this.VisitDelegateInvokeExpression(node);
			this.CheckArguments(node.get_InvokeMethodReference().get_Parameters(), node.get_Arguments());
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.VisitMethodInvocationExpression(node);
			this.CheckArguments(node.get_MethodExpression().get_Method().get_Parameters(), node.get_Arguments());
			return;
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			this.VisitObjectCreationExpression(node);
			if (node.get_Constructor() != null)
			{
				this.CheckArguments(node.get_Constructor().get_Parameters(), node.get_Arguments());
			}
			return;
		}

		public override void VisitPropertyReferenceExpression(PropertyReferenceExpression node)
		{
			this.VisitPropertyReferenceExpression(node);
			if (node.get_IsIndexer())
			{
				this.CheckArguments(node.get_Property().get_Parameters(), node.get_Arguments());
			}
			return;
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			this.VisitReturnExpression(node);
			if (node.get_Value() != null && node.get_Value().get_HasType() && this.ShouldAddCast(node.get_Value().get_ExpressionType().Resolve(), this.decompiledMethodReturnType))
			{
				node.set_Value(new ExplicitCastExpression(node.get_Value(), this.decompiledMethodReturnType, null));
			}
			return;
		}

		public override void VisitUnaryExpression(UnaryExpression node)
		{
			this.VisitUnaryExpression(node);
			if (node.get_Operator() == UnaryOperator.Negate || node.get_Operator() == 10 && node.get_Operand().get_HasType())
			{
				V_0 = node.get_Operand().get_ExpressionType().Resolve();
				if (V_0 != null && V_0.get_IsEnum())
				{
					node.set_Operand(new ExplicitCastExpression(node.get_Operand(), this.GetEnumUnderlyingType(V_0), null));
					node.DecideExpressionType();
				}
			}
			return;
		}
	}
}