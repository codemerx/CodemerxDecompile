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
			for (int i = 0; i < parameters.get_Count(); i++)
			{
				TypeDefinition typeDefinition = parameters.get_Item(i).get_ParameterType().Resolve();
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
				if ((item.CodeNodeType != CodeNodeType.LiteralExpression || ((LiteralExpression)item).Value != null || typeDefinition.get_IsValueType()) && typeDefinition.get_FullName() != typeDefinition1.get_FullName() && !this.IsTypeDescendantOf(typeDefinition1, typeDefinition))
				{
					return false;
				}
			}
			return true;
		}

		private void FixArguments(MethodReference method, ExpressionCollection arguments)
		{
			TypeDefinition typeDefinition = method.get_DeclaringType().Resolve();
			if (typeDefinition == null)
			{
				return;
			}
			List<MethodDefinition> sameNameMethods = this.GetSameNameMethods(typeDefinition, method, arguments);
			if (sameNameMethods.Count > 0)
			{
				for (int i = 0; i < arguments.Count; i++)
				{
					TypeReference typeReference = method.get_Parameters().get_Item(i).ResolveParameterType(method);
					if (arguments[i].HasType && !(arguments[i].ExpressionType.get_FullName() == typeReference.get_FullName()) && this.ShouldAddCast(arguments[i], sameNameMethods, i, typeReference))
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
			foreach (MethodDefinition methodDefinition1 in declaringTypeDefinition.get_Methods())
			{
				if (methodDefinition1.get_Name() != method.get_Name() || methodDefinition1.get_HasParameters() != method.get_HasParameters() || methodDefinition1.get_Parameters().get_Count() != method.get_Parameters().get_Count() || (object)methodDefinition1 == (object)methodDefinition || methodDefinition1.get_HasGenericParameters() != methodDefinition.get_HasGenericParameters() || !this.ArgumentsMatchParameters(methodDefinition1.get_Parameters(), arguments))
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
			if (descendant.get_IsGenericParameter())
			{
				return true;
			}
			TypeDefinition typeDefinition1 = descendant.Resolve();
			if (typeDefinition1 == null)
			{
				return false;
			}
			if (descendant.get_IsArray() == ancestor.get_IsArray())
			{
				while (typeDefinition1 != null)
				{
					if (typeDefinition1.get_BaseType() != null && typeDefinition1.get_BaseType().get_FullName() == ancestor.get_FullName())
					{
						return true;
					}
					if (typeDefinition1.get_HasInterfaces())
					{
						foreach (TypeReference @interface in typeDefinition1.get_Interfaces())
						{
							if (@interface.get_FullName() != ancestor.get_FullName())
							{
								continue;
							}
							flag = true;
							return flag;
						}
					}
					if (typeDefinition1.get_BaseType() == null)
					{
						typeDefinition = null;
					}
					else
					{
						typeDefinition = typeDefinition1.get_BaseType().Resolve();
					}
					typeDefinition1 = typeDefinition;
				}
				return false;
			}
			else
			{
				foreach (TypeReference typeReference in typeDefinition1.get_Interfaces())
				{
					if (typeReference.get_FullName() != ancestor.get_FullName())
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
					TypeReference parameterType = enumerator.Current.get_Parameters().get_Item(argumentIndex).get_ParameterType();
					if (this.IsTypeDescendantOf(calledMethodParamType, parameterType) || calledMethodParamType.get_FullName() == parameterType.get_FullName())
					{
						continue;
					}
					if (this.IsTypeDescendantOf(expressionType, parameterType) || expressionType.get_FullName() == parameterType.get_FullName())
					{
						flag = true;
						return flag;
					}
					else
					{
						if (argument.CodeNodeType != CodeNodeType.LiteralExpression || ((LiteralExpression)argument).Value != null || parameterType.get_IsValueType())
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