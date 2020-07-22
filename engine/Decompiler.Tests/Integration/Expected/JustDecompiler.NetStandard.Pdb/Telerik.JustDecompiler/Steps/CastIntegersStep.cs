using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	public class CastIntegersStep : BaseCodeVisitor, IDecompilationStep
	{
		private TypeSystem typeSystem;

		private TypeReference decompiledMethodReturnType;

		public CastIntegersStep()
		{
			base();
			return;
		}

		private void CastMethodArguments(MethodReference method, ExpressionCollection arguments)
		{
			V_0 = 0;
			while (V_0 < arguments.get_Count())
			{
				V_1 = arguments.get_Item(V_0);
				V_2 = method.get_Parameters().get_Item(V_0).ResolveParameterType(method);
				if (V_1.get_HasType() && V_2 != null && V_1 as LiteralExpression == null)
				{
					V_3 = V_1.get_ExpressionType();
					if (this.IsUnsignedIntegerType(V_3) && this.IsSignedIntegerType(V_2) || this.IsSignedIntegerType(V_3) && this.IsUnsignedIntegerType(V_2))
					{
						V_4 = V_1;
						if (V_1 as ExplicitCastExpression != null)
						{
							V_5 = V_1 as ExplicitCastExpression;
							if (this.IsIntegerType(V_5.get_TargetType()))
							{
								V_4 = V_5.get_Expression();
							}
						}
						arguments.set_Item(V_0, new ExplicitCastExpression(V_4, V_2, null));
					}
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private int GetIntegerTypeBytes(TypeReference type)
		{
			V_0 = type.get_FullName();
			if (V_0 != null)
			{
				if (String.op_Equality(V_0, "System.SByte") || String.op_Equality(V_0, "System.Byte"))
				{
					return 1;
				}
				if (String.op_Equality(V_0, "System.Int16") || String.op_Equality(V_0, "System.UInt16"))
				{
					return 2;
				}
				if (String.op_Equality(V_0, "System.Int32") || String.op_Equality(V_0, "System.UInt32"))
				{
					return 4;
				}
				if (String.op_Equality(V_0, "System.Int64") || String.op_Equality(V_0, "System.UInt64"))
				{
					return 8;
				}
			}
			throw new NotSupportedException(String.Format("Not supported type {0}.", type.get_FullName()));
		}

		private bool IsIntegerType(TypeReference type)
		{
			if (this.IsSignedIntegerType(type))
			{
				return true;
			}
			return this.IsUnsignedIntegerType(type);
		}

		private bool IsSignedIntegerType(TypeReference type)
		{
			V_0 = type.get_FullName();
			if (String.op_Equality(V_0, this.typeSystem.get_SByte().get_FullName()) || String.op_Equality(V_0, this.typeSystem.get_Int16().get_FullName()) || String.op_Equality(V_0, this.typeSystem.get_Int32().get_FullName()))
			{
				return true;
			}
			return String.op_Equality(V_0, this.typeSystem.get_Int64().get_FullName());
		}

		private bool IsUnsignedIntegerType(TypeReference type)
		{
			V_0 = type.get_FullName();
			if (String.op_Equality(V_0, this.typeSystem.get_Byte().get_FullName()) || String.op_Equality(V_0, this.typeSystem.get_UInt16().get_FullName()) || String.op_Equality(V_0, this.typeSystem.get_UInt32().get_FullName()))
			{
				return true;
			}
			return String.op_Equality(V_0, this.typeSystem.get_UInt64().get_FullName());
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.decompiledMethodReturnType = context.get_MethodContext().get_Method().get_ReturnType();
			this.Visit(body);
			if (context.get_MethodContext().get_CtorInvokeExpression() != null)
			{
				this.Visit(context.get_MethodContext().get_CtorInvokeExpression());
			}
			return body;
		}

		private bool ShouldAddCastToAssignment(TypeReference assignToType, TypeReference assignFromType)
		{
			if (this.IsSignedIntegerType(assignToType) && this.IsUnsignedIntegerType(assignFromType) && this.GetIntegerTypeBytes(assignToType) > this.GetIntegerTypeBytes(assignFromType))
			{
				return false;
			}
			if (this.IsUnsignedIntegerType(assignToType) && this.IsSignedIntegerType(assignFromType))
			{
				return true;
			}
			if (!this.IsSignedIntegerType(assignToType))
			{
				return false;
			}
			return this.IsUnsignedIntegerType(assignFromType);
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.VisitBaseCtorExpression(node);
			this.CastMethodArguments(node.get_MethodExpression().get_Method(), node.get_Arguments());
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			this.VisitBinaryExpression(node);
			if (node.get_IsAssignmentExpression() && node.get_Left().get_HasType() && node.get_Right() as LiteralExpression == null && node.get_Right().get_HasType() && this.ShouldAddCastToAssignment(node.get_Left().get_ExpressionType(), node.get_Right().get_ExpressionType()))
			{
				V_0 = node.get_Right();
				if (node.get_Right() as ExplicitCastExpression != null)
				{
					V_1 = node.get_Right() as ExplicitCastExpression;
					if (this.IsIntegerType(V_1.get_TargetType()))
					{
						V_0 = V_1.get_Expression();
					}
				}
				node.set_Right(new ExplicitCastExpression(V_0, node.get_Left().get_ExpressionType(), null));
			}
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.VisitMethodInvocationExpression(node);
			this.CastMethodArguments(node.get_MethodExpression().get_Method(), node.get_Arguments());
			return;
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			this.VisitObjectCreationExpression(node);
			this.CastMethodArguments(node.get_Constructor(), node.get_Arguments());
			return;
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			this.VisitReturnExpression(node);
			if (this.decompiledMethodReturnType != null && node.get_Value() != null && node.get_Value().get_HasType() && this.ShouldAddCastToAssignment(this.decompiledMethodReturnType, node.get_Value().get_ExpressionType()))
			{
				V_0 = node.get_Value();
				if (node.get_Value() as ExplicitCastExpression != null)
				{
					V_1 = node.get_Value() as ExplicitCastExpression;
					if (this.IsIntegerType(V_1.get_TargetType()))
					{
						V_0 = V_1.get_Expression();
					}
				}
				node.set_Value(new ExplicitCastExpression(V_0, this.decompiledMethodReturnType, null));
			}
			return;
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.VisitThisCtorExpression(node);
			this.CastMethodArguments(node.get_MethodExpression().get_Method(), node.get_Arguments());
			return;
		}
	}
}