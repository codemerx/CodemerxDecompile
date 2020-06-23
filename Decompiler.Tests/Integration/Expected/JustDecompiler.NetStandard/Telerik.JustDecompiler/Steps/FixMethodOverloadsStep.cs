using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class FixMethodOverloadsStep : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		public FixMethodOverloadsStep()
		{
		}

		private bool ArgumentsMatchParameters(Mono.Collections.Generic.Collection<ParameterDefinition> parameters, ExpressionCollection arguments)
		{
			for (int i = 0; i < parameters.Count; i++)
			{
				TypeDefinition typeDefinition = parameters[i].ParameterType.Resolve();
				Expression item = arguments[i];
				if (!item.HasType)
				{
					return true;
				}
				TypeDefinition typeDefinition1 = item.ExpressionType.Resolve();
				if (typeDefinition == null || typeDefinition1 == null)
				{
					return true;
				}
				if ((item.CodeNodeType != CodeNodeType.LiteralExpression || ((LiteralExpression)item).Value != null || typeDefinition.IsValueType) && typeDefinition.FullName != typeDefinition1.FullName && !this.IsTypeDescendantOf(typeDefinition1, typeDefinition))
				{
					return false;
				}
			}
			return true;
		}

		private void FixArguments(MethodReference method, ExpressionCollection arguments)
		{
			TypeDefinition typeDefinition = method.DeclaringType.Resolve();
			if (typeDefinition == null)
			{
				return;
			}
			List<MethodDefinition> sameNameMethods = this.GetSameNameMethods(typeDefinition, method, arguments);
			if (sameNameMethods.Count > 0)
			{
				for (int i = 0; i < arguments.Count; i++)
				{
					TypeReference typeReference = method.Parameters[i].ResolveParameterType(method);
					if (arguments[i].HasType && !(arguments[i].ExpressionType.FullName == typeReference.FullName) && this.ShouldAddCast(arguments[i], sameNameMethods, i, typeReference))
					{
						arguments[i] = new ExplicitCastExpression(arguments[i], typeReference, null);
					}
				}
			}
		}

		private List<MethodDefinition> GetSameNameMethods(TypeDefinition declaringTypeDefinition, MethodReference method, ExpressionCollection arguments)
		{
			List<MethodDefinition> methodDefinitions = new List<MethodDefinition>();
			MethodDefinition methodDefinition = method.Resolve();
			if (methodDefinition == null)
			{
				return methodDefinitions;
			}
			foreach (MethodDefinition methodDefinition1 in declaringTypeDefinition.Methods)
			{
				if (methodDefinition1.Name != method.Name || methodDefinition1.HasParameters != method.HasParameters || methodDefinition1.Parameters.Count != method.Parameters.Count || methodDefinition1 == methodDefinition || methodDefinition1.HasGenericParameters != methodDefinition.HasGenericParameters || !this.ArgumentsMatchParameters(methodDefinition1.Parameters, arguments))
				{
					continue;
				}
				methodDefinitions.Add(methodDefinition1);
			}
			return methodDefinitions;
		}

		private bool IsTypeDescendantOf(TypeReference descendant, TypeReference ancestor)
		{
			bool flag;
			TypeDefinition typeDefinition;
			if (descendant.IsGenericParameter)
			{
				return true;
			}
			TypeDefinition typeDefinition1 = descendant.Resolve();
			if (typeDefinition1 == null)
			{
				return false;
			}
			if (descendant.IsArray == ancestor.IsArray)
			{
				while (typeDefinition1 != null)
				{
					if (typeDefinition1.BaseType != null && typeDefinition1.BaseType.FullName == ancestor.FullName)
					{
						return true;
					}
					if (typeDefinition1.HasInterfaces)
					{
						foreach (TypeReference @interface in typeDefinition1.Interfaces)
						{
							if (@interface.FullName != ancestor.FullName)
							{
								continue;
							}
							flag = true;
							return flag;
						}
					}
					if (typeDefinition1.BaseType == null)
					{
						typeDefinition = null;
					}
					else
					{
						typeDefinition = typeDefinition1.BaseType.Resolve();
					}
					typeDefinition1 = typeDefinition;
				}
				return false;
			}
			else
			{
				foreach (TypeReference typeReference in typeDefinition1.Interfaces)
				{
					if (typeReference.FullName != ancestor.FullName)
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.VisitBlockStatement(body);
			return body;
		}

		private bool ShouldAddCast(Expression argument, List<MethodDefinition> sameNameMethods, int argumentIndex, TypeReference calledMethodParamType)
		{
			bool flag;
			if (!argument.HasType)
			{
				return true;
			}
			TypeReference expressionType = argument.ExpressionType;
			List<MethodDefinition>.Enumerator enumerator = sameNameMethods.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					TypeReference parameterType = enumerator.Current.Parameters[argumentIndex].ParameterType;
					if (this.IsTypeDescendantOf(calledMethodParamType, parameterType) || calledMethodParamType.FullName == parameterType.FullName)
					{
						continue;
					}
					if (this.IsTypeDescendantOf(expressionType, parameterType) || expressionType.FullName == parameterType.FullName)
					{
						flag = true;
						return flag;
					}
					else
					{
						if (argument.CodeNodeType != CodeNodeType.LiteralExpression || ((LiteralExpression)argument).Value != null || parameterType.IsValueType)
						{
							continue;
						}
						flag = true;
						return flag;
					}
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			base.VisitBaseCtorExpression(node);
			MethodReference method = node.MethodExpression.Method;
			if (node.Arguments.Count > 0)
			{
				this.FixArguments(method, node.Arguments);
			}
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.VisitMethodInvocationExpression(node);
			if (node.MethodExpression.CodeNodeType == CodeNodeType.MethodReferenceExpression)
			{
				this.FixArguments(node.MethodExpression.Method, node.Arguments);
			}
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);
			MethodReference constructor = node.Constructor;
			if (constructor != null)
			{
				this.FixArguments(constructor, node.Arguments);
			}
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			base.VisitThisCtorExpression(node);
			MethodReference method = node.MethodExpression.Method;
			if (node.Arguments.Count > 0)
			{
				this.FixArguments(method, node.Arguments);
			}
		}
	}
}