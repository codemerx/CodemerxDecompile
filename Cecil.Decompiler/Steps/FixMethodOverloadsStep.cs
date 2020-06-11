using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil.Extensions;

namespace Telerik.JustDecompiler.Steps
{
	class FixMethodOverloadsStep : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			VisitBlockStatement(body);
			return body;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.VisitMethodInvocationExpression(node);
			if (node.MethodExpression.CodeNodeType == CodeNodeType.MethodReferenceExpression)
			{
				FixArguments(node.MethodExpression.Method, node.Arguments);
			}
		}

		private void FixArguments(MethodReference method, ExpressionCollection arguments)
		{
			TypeDefinition declaringTypeDefinition = method.DeclaringType.Resolve();
			if (declaringTypeDefinition == null)
			{
				return;
			}
			List<MethodDefinition> sameNameMethods = GetSameNameMethods(declaringTypeDefinition, method, arguments);
			if (sameNameMethods.Count > 0)
			{
				for (int i = 0; i < arguments.Count; i++)
				{
					TypeReference paramType = method.Parameters[i].ResolveParameterType(method);
					if (!arguments[i].HasType)
					{
						continue;
					}
					if (arguments[i].ExpressionType.FullName == paramType.FullName)
					{
						continue;
					}
					if (ShouldAddCast(arguments[i], sameNameMethods, i, paramType))
					{
						arguments[i] = new ExplicitCastExpression(arguments[i], paramType, null);
					}
				}
			}
		}

		private bool ShouldAddCast(Expression argument, List<MethodDefinition> sameNameMethods, int argumentIndex, TypeReference calledMethodParamType)
		{
			if (!argument.HasType)
			{
				return true;
			}
			TypeReference expressionType = argument.ExpressionType;
			//if (argument.ExpressionType.IsGenericParameter)
			//{
			//	return false;
			//}
			foreach (MethodDefinition method in sameNameMethods)
			{
				TypeReference parameterType = method.Parameters[argumentIndex].ParameterType;
				if (IsTypeDescendantOf(calledMethodParamType, parameterType) || calledMethodParamType.FullName == parameterType.FullName)
				{
					/// Either the called method has more specific type,
					/// or the types match.
					continue;
				}
				if (IsTypeDescendantOf(expressionType, parameterType) || expressionType.FullName == parameterType.FullName)
				{
					return true;
				}
				if (argument.CodeNodeType == CodeNodeType.LiteralExpression && ((LiteralExpression)argument).Value == null)
				{
					if (!parameterType.IsValueType)
					{
						return true;
					}
				}
			}
			return false;
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);
			MethodReference calledMethod = node.Constructor;
			if (calledMethod != null)
			{
				FixArguments(calledMethod, node.Arguments);
			}
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			base.VisitThisCtorExpression(node);
			var method = node.MethodExpression.Method;
			if (node.Arguments.Count > 0)
			{
				FixArguments(method, node.Arguments);
			}
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			base.VisitBaseCtorExpression(node);
			var method = node.MethodExpression.Method;
			if (node.Arguments.Count > 0)
			{
				FixArguments(method, node.Arguments);
			}
		}

		private List<MethodDefinition> GetSameNameMethods(TypeDefinition declaringTypeDefinition, MethodReference method, ExpressionCollection arguments)
		{
			//TODO: Get only the methods, for which the arguments might match
			List<MethodDefinition> result = new List<MethodDefinition>();
			MethodDefinition methodResolved = method.Resolve();
			if (methodResolved == null)
			{
				return result;
			}
			foreach (MethodDefinition typeMethod in declaringTypeDefinition.Methods)
			{
				if (typeMethod.Name != method.Name)
				{
					continue;
				}
				if (typeMethod.HasParameters != method.HasParameters)
				{
					continue;
				}
				if (typeMethod.Parameters.Count != method.Parameters.Count)
				{
					continue;
				}
				if (typeMethod == methodResolved)
				{
					continue;
				}
				if (typeMethod.HasGenericParameters != methodResolved.HasGenericParameters)
				{
					continue;
				}
				if (!ArgumentsMatchParameters(typeMethod.Parameters, arguments))
				{
					continue;
				}
				result.Add(typeMethod);
			}
			return result;
		}
  
		private bool ArgumentsMatchParameters(Collection<ParameterDefinition> parameters, ExpressionCollection arguments)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				// generics?
				TypeDefinition parameterType = parameters[i].ParameterType.Resolve();
				Expression currentArgument = arguments[i];
				if (!currentArgument.HasType)
				{
					return true;
				}
				TypeDefinition argumentType = currentArgument.ExpressionType.Resolve();
				if (parameterType == null || argumentType == null)
				{
					return true;
				}
				if (currentArgument.CodeNodeType == CodeNodeType.LiteralExpression && ((LiteralExpression)currentArgument).Value == null &&
					!parameterType.IsValueType)
				{
					continue;
				}
				if (parameterType.FullName != argumentType.FullName)
				{
					if (!IsTypeDescendantOf(argumentType, parameterType))
					{
						return false;
					}
				}
			}
			return true;
		}

		private bool IsTypeDescendantOf(TypeReference descendant, TypeReference ancestor)
		{
			if (descendant.IsGenericParameter)
			{
				return true;
			}
			TypeDefinition currentType = descendant.Resolve();

			if (currentType == null)
			{
				return false;
			}

			if (descendant.IsArray != ancestor.IsArray)
			{
				foreach (TypeReference currentTypeInterface in currentType.Interfaces)
				{
					if (currentTypeInterface.FullName == ancestor.FullName)
					{
						return true;
					}
				}
				return false;
			}

			while (currentType != null)
			{
				if (currentType.BaseType != null && currentType.BaseType.FullName == ancestor.FullName)
				{
					return true;
				}

				if (currentType.HasInterfaces)
				{
					foreach (TypeReference currentTypeInterface in currentType.Interfaces)
					{
						if (currentTypeInterface.FullName == ancestor.FullName)
						{
							return true;
						}
					}
				}

				currentType = currentType.BaseType == null ? null : currentType.BaseType.Resolve();
			}

			return false;
		}
	}
}
