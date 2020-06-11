using System;
using System.Linq;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class CastIntegersStep : BaseCodeVisitor, IDecompilationStep
	{
		private TypeSystem typeSystem;
		private TypeReference decompiledMethodReturnType;

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.MethodContext.Method.Module.TypeSystem;
			this.decompiledMethodReturnType = context.MethodContext.Method.ReturnType;
			Visit(body);

			if (context.MethodContext.CtorInvokeExpression != null)
			{
				Visit(context.MethodContext.CtorInvokeExpression);
			}

			return body;
		}

		public override void VisitBinaryExpression(Ast.Expressions.BinaryExpression node)
		{
			base.VisitBinaryExpression(node);

			if (node.IsAssignmentExpression && node.Left.HasType && !(node.Right is LiteralExpression) && node.Right.HasType 
				&& ShouldAddCastToAssignment(node.Left.ExpressionType, node.Right.ExpressionType))
			{
				Expression toCast = node.Right;

				if (node.Right is ExplicitCastExpression)
				{
					ExplicitCastExpression rightCast = node.Right as ExplicitCastExpression;
					if (IsIntegerType(rightCast.TargetType))
					{
						toCast = rightCast.Expression;
					}
				} 

				node.Right = new ExplicitCastExpression(toCast, node.Left.ExpressionType, null);
			}
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			base.VisitReturnExpression(node);

			if (decompiledMethodReturnType != null && node.Value != null && node.Value.HasType && ShouldAddCastToAssignment(decompiledMethodReturnType, node.Value.ExpressionType))
			{
				Expression toCast = node.Value;

				if (node.Value is ExplicitCastExpression)
				{
					ExplicitCastExpression valueCast = node.Value as ExplicitCastExpression;
					if (IsIntegerType(valueCast.TargetType))
					{
						toCast = valueCast.Expression;
					}
				} 

				node.Value = new ExplicitCastExpression(toCast, decompiledMethodReturnType, null);
			}
		}

		private void CastMethodArguments(MethodReference method, ExpressionCollection arguments)
		{
			for (int i = 0; i < arguments.Count; i++)
			{
				Expression argument = arguments[i];

				TypeReference parameterType = method.Parameters[i].ResolveParameterType(method);

				if (argument.HasType && parameterType != null && !(argument is LiteralExpression))
				{
					TypeReference argumentType = argument.ExpressionType;
					if ((IsUnsignedIntegerType(argumentType) && IsSignedIntegerType(parameterType)) || (IsSignedIntegerType(argumentType) && IsUnsignedIntegerType(parameterType)))
					{
						Expression toCast = argument;

						if (argument is ExplicitCastExpression)
						{
							ExplicitCastExpression argumentCast = argument as ExplicitCastExpression;
							if (IsIntegerType(argumentCast.TargetType))
							{
								toCast = argumentCast.Expression;
							}
						}

						arguments[i] = new ExplicitCastExpression(toCast, parameterType, null);
					}
				}
			}
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.VisitMethodInvocationExpression(node);

			CastMethodArguments(node.MethodExpression.Method, node.Arguments);
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			base.VisitBaseCtorExpression(node);

			CastMethodArguments(node.MethodExpression.Method, node.Arguments);
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			base.VisitThisCtorExpression(node);

			CastMethodArguments(node.MethodExpression.Method, node.Arguments);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);

			CastMethodArguments(node.Constructor, node.Arguments);
		}		private bool ShouldAddCastToAssignment(TypeReference assignToType, TypeReference assignFromType)
		{
			if (IsSignedIntegerType(assignToType) && IsUnsignedIntegerType(assignFromType))
			{
				if (GetIntegerTypeBytes(assignToType) > GetIntegerTypeBytes(assignFromType))
				{
					return false;
				}
			}

			return ((IsUnsignedIntegerType(assignToType) && IsSignedIntegerType(assignFromType)) || (IsSignedIntegerType(assignToType) && IsUnsignedIntegerType(assignFromType)));
		}		private bool IsIntegerType(TypeReference type)
		{
			return IsSignedIntegerType(type) || IsUnsignedIntegerType(type);
		}		private bool IsUnsignedIntegerType(TypeReference type)
		{
			string typeName = type.FullName;

			return typeName == typeSystem.Byte.FullName || typeName == typeSystem.UInt16.FullName || 
				typeName == typeSystem.UInt32.FullName || typeName == typeSystem.UInt64.FullName;
		}		private bool IsSignedIntegerType(TypeReference type)
		{
			string typeName = type.FullName;

			return typeName == typeSystem.SByte.FullName || typeName == typeSystem.Int16.FullName || 
				typeName == typeSystem.Int32.FullName || typeName == typeSystem.Int64.FullName;
		}		private int GetIntegerTypeBytes(TypeReference type)
		{
			switch (type.FullName)
			{
				case "System.SByte":
				case "System.Byte":
					return 1;
				case "System.Int16":
				case "System.UInt16":
					return 2;
				case "System.Int32":
				case "System.UInt32":
					return 4;
				case "System.Int64":
				case "System.UInt64":
					return 8;
				default:
					throw new NotSupportedException(string.Format("Not supported type {0}.", type.FullName));
			}
		}
	}
}
