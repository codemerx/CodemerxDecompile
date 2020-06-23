using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class HandleVirtualMethodInvocations
	{
		private MethodDefinition method;

		public HandleVirtualMethodInvocations(MethodDefinition method)
		{
			this.method = method;
		}

		private List<TypeDefinition> GetInheritanceChain(TypeDefinition targetType, string definingTypeFullName)
		{
			List<TypeDefinition> typeDefinitions = new List<TypeDefinition>();
			List<int> nums = new List<int>();
			typeDefinitions.Add(targetType);
			nums.Add(-1);
			int item = -1;
			int num = 0;
			while (num < typeDefinitions.Count)
			{
				TypeDefinition typeDefinition = typeDefinitions[num];
				if (typeDefinition == null || typeDefinition.FullName == definingTypeFullName)
				{
					item = num;
					break;
				}
				else
				{
					TypeReference baseType = typeDefinition.BaseType;
					if (baseType != null)
					{
						TypeDefinition typeDefinition1 = baseType.Resolve();
						if (typeDefinition1 != null)
						{
							typeDefinitions.Add(typeDefinition1);
							nums.Add(num);
						}
					}
					foreach (TypeReference @interface in typeDefinition.Interfaces)
					{
						if (@interface == null)
						{
							continue;
						}
						TypeDefinition typeDefinition2 = @interface.Resolve();
						if (typeDefinition2 == null)
						{
							continue;
						}
						typeDefinitions.Add(typeDefinition2);
						nums.Add(num);
					}
					num++;
				}
			}
			List<TypeDefinition> typeDefinitions1 = new List<TypeDefinition>();
			while (item != -1)
			{
				typeDefinitions1.Add(typeDefinitions[item]);
				item = nums[item];
			}
			return typeDefinitions1;
		}

		public void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			Func<MethodDefinition, bool> func = null;
			if (!node.VirtualCall)
			{
				MethodReferenceExpression methodExpression = node.MethodExpression;
				if (methodExpression == null)
				{
					return;
				}
				if (methodExpression.Target is ThisReferenceExpression)
				{
					TypeDefinition typeDefinition = this.method.DeclaringType.Resolve();
					if (typeDefinition != null && typeDefinition != methodExpression.Method.DeclaringType.Resolve())
					{
						TypeReference baseType = this.method.DeclaringType.BaseType;
						if (baseType == null || baseType.FullName == typeof(Object).FullName)
						{
							return;
						}
						methodExpression.Target = new BaseReferenceExpression(methodExpression.Method.DeclaringType, (methodExpression.Target as ThisReferenceExpression).MappedInstructions);
					}
				}
				return;
			}
			MethodReferenceExpression methodReferenceExpression = node.MethodExpression;
			if (!methodReferenceExpression.Target.HasType)
			{
				return;
			}
			if (methodReferenceExpression.Target.ExpressionType.FullName == methodReferenceExpression.Method.DeclaringType.FullName)
			{
				return;
			}
			MethodReference method = methodReferenceExpression.Method;
			TypeDefinition typeDefinition1 = methodReferenceExpression.Target.ExpressionType.Resolve();
			if (typeDefinition1 == null)
			{
				return;
			}
			foreach (TypeDefinition inheritanceChain in this.GetInheritanceChain(typeDefinition1, methodReferenceExpression.Method.DeclaringType.FullName))
			{
				Collection<MethodDefinition> methods = inheritanceChain.Methods;
				Func<MethodDefinition, bool> func1 = func;
				if (func1 == null)
				{
					Func<MethodDefinition, bool> name = (MethodDefinition x) => {
						if (x.Name != methodReferenceExpression.Method.Name)
						{
							return false;
						}
						return x.HasSameSignatureWith(methodReferenceExpression.Method);
					};
					Func<MethodDefinition, bool> func2 = name;
					func = name;
					func1 = func2;
				}
				MethodDefinition methodDefinition = methods.FirstOrDefault<MethodDefinition>(func1);
				if (methodDefinition == null)
				{
					continue;
				}
				method = methodDefinition;
			}
			node.MethodExpression = new MethodReferenceExpression(node.MethodExpression.Target, method, node.MethodExpression.MappedInstructions);
		}
	}
}