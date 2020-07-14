using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.ObjectModel;
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
		}

		private void CastMethodArguments(MethodReference method, ExpressionCollection arguments)
		{
			for (int i = 0; i < arguments.Count; i++)
			{
				Expression item = arguments[i];
				TypeReference typeReference = method.get_Parameters().get_Item(i).ResolveParameterType(method);
				if (item.HasType && typeReference != null && !(item is LiteralExpression))
				{
					TypeReference expressionType = item.ExpressionType;
					if (this.IsUnsignedIntegerType(expressionType) && this.IsSignedIntegerType(typeReference) || this.IsSignedIntegerType(expressionType) && this.IsUnsignedIntegerType(typeReference))
					{
						Expression expression = item;
						if (item is ExplicitCastExpression)
						{
							ExplicitCastExpression explicitCastExpression = item as ExplicitCastExpression;
							if (this.IsIntegerType(explicitCastExpression.TargetType))
							{
								expression = explicitCastExpression.Expression;
							}
						}
						arguments[i] = new ExplicitCastExpression(expression, typeReference, null);
					}
				}
			}
		}

		private int GetIntegerTypeBytes(TypeReference type)
		{
			string fullName = type.get_FullName();
			if (fullName != null)
			{
				if (fullName == "System.SByte" || fullName == "System.Byte")
				{
					return 1;
				}
				if (fullName == "System.Int16" || fullName == "System.UInt16")
				{
					return 2;
				}
				if (fullName == "System.Int32" || fullName == "System.UInt32")
				{
					return 4;
				}
				if (fullName == "System.Int64" || fullName == "System.UInt64")
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
			string fullName = type.get_FullName();
			if (fullName == this.typeSystem.get_SByte().get_FullName() || fullName == this.typeSystem.get_Int16().get_FullName() || fullName == this.typeSystem.get_Int32().get_FullName())
			{
				return true;
			}
			return fullName == this.typeSystem.get_Int64().get_FullName();
		}

		private bool IsUnsignedIntegerType(TypeReference type)
		{
			string fullName = type.get_FullName();
			if (fullName == this.typeSystem.get_Byte().get_FullName() || fullName == this.typeSystem.get_UInt16().get_FullName() || fullName == this.typeSystem.get_UInt32().get_FullName())
			{
				return true;
			}
			return fullName == this.typeSystem.get_UInt64().get_FullName();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.MethodContext.Method.get_Module().get_TypeSystem();
			this.decompiledMethodReturnType = context.MethodContext.Method.get_ReturnType();
			this.Visit(body);
			if (context.MethodContext.CtorInvokeExpression != null)
			{
				this.Visit(context.MethodContext.CtorInvokeExpression);
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
			base.VisitBaseCtorExpression(node);
			this.CastMethodArguments(node.MethodExpression.Method, node.Arguments);
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			base.VisitBinaryExpression(node);
			if (node.IsAssignmentExpression && node.Left.HasType && !(node.Right is LiteralExpression) && node.Right.HasType && this.ShouldAddCastToAssignment(node.Left.ExpressionType, node.Right.ExpressionType))
			{
				Expression right = node.Right;
				if (node.Right is ExplicitCastExpression)
				{
					ExplicitCastExpression explicitCastExpression = node.Right as ExplicitCastExpression;
					if (this.IsIntegerType(explicitCastExpression.TargetType))
					{
						right = explicitCastExpression.Expression;
					}
				}
				node.Right = new ExplicitCastExpression(right, node.Left.ExpressionType, null);
			}
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.VisitMethodInvocationExpression(node);
			this.CastMethodArguments(node.MethodExpression.Method, node.Arguments);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);
			this.CastMethodArguments(node.Constructor, node.Arguments);
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			base.VisitReturnExpression(node);
			if (this.decompiledMethodReturnType != null && node.Value != null && node.Value.HasType && this.ShouldAddCastToAssignment(this.decompiledMethodReturnType, node.Value.ExpressionType))
			{
				Expression value = node.Value;
				if (node.Value is ExplicitCastExpression)
				{
					ExplicitCastExpression explicitCastExpression = node.Value as ExplicitCastExpression;
					if (this.IsIntegerType(explicitCastExpression.TargetType))
					{
						value = explicitCastExpression.Expression;
					}
				}
				node.Value = new ExplicitCastExpression(value, this.decompiledMethodReturnType, null);
			}
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			base.VisitThisCtorExpression(node);
			this.CastMethodArguments(node.MethodExpression.Method, node.Arguments);
		}
	}
}