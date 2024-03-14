using Mono.Cecil;
using Mono.Cecil.Extensions;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildAnonymousTypesInitializersStep
	{
		private readonly BaseCodeTransformer transformer;

		private readonly TypeSystem typeSystem;

		private BlockExpression initializerExpressions;

		public RebuildAnonymousTypesInitializersStep(BaseCodeTransformer transformer, TypeSystem typeSystem)
		{
			this.transformer = transformer;
			this.typeSystem = typeSystem;
		}

		private PropertyDefinition FindPropertyOfType(TypeDefinition typeDefinition, TypeReference parameterType)
		{
			PropertyDefinition propertyDefinition;
			Mono.Collections.Generic.Collection<PropertyDefinition>.Enumerator enumerator = typeDefinition.get_Properties().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					PropertyDefinition current = enumerator.get_Current();
					if ((object)current.get_PropertyType() != (object)parameterType)
					{
						continue;
					}
					propertyDefinition = current;
					return propertyDefinition;
				}
				return null;
			}
			finally
			{
				enumerator.Dispose();
			}
			return propertyDefinition;
		}

		private int GetParameterIndexWithType(MethodDefinition methodDef, TypeReference typeReference)
		{
			int num;
			int num1 = 0;
			Mono.Collections.Generic.Collection<ParameterDefinition>.Enumerator enumerator = methodDef.get_Parameters().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if ((object)enumerator.get_Current().get_ParameterType() != (object)typeReference)
					{
						num1++;
					}
					else
					{
						num = num1;
						return num;
					}
				}
				return -1;
			}
			finally
			{
				enumerator.Dispose();
			}
			return num;
		}

		private void ProcessAnonymousType(TypeDefinition anonymousTypeDefinition, GenericInstanceType anonymousInstanceType, MethodDefinition constructorDefinition, ExpressionCollection constructorArguments)
		{
			for (int i = 0; i < constructorDefinition.get_Parameters().get_Count(); i++)
			{
				ParameterDefinition item = constructorDefinition.get_Parameters().get_Item(i);
				int num = anonymousTypeDefinition.get_GenericParameters().IndexOf(item.get_ParameterType() as GenericParameter);
				PropertyDefinition propertyDefinition = this.FindPropertyOfType(anonymousTypeDefinition, item.get_ParameterType());
				TypeReference typeReference = anonymousInstanceType.get_GenericArguments().get_Item(num);
				if (anonymousInstanceType.get_PostionToArgument().ContainsKey(num))
				{
					typeReference = anonymousInstanceType.get_PostionToArgument()[num];
				}
				Expression anonymousPropertyInitializerExpression = new AnonymousPropertyInitializerExpression(propertyDefinition, typeReference);
				int parameterIndexWithType = this.GetParameterIndexWithType(constructorDefinition, item.get_ParameterType());
				Expression expression = this.transformer.Visit(constructorArguments[parameterIndexWithType].Clone()) as Expression;
				this.initializerExpressions.Expressions.Add(new BinaryExpression(BinaryOperator.Assign, anonymousPropertyInitializerExpression, expression, this.typeSystem, null, false));
			}
		}

		public ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			if (node.Type == null || node.Constructor == null || !node.Type.get_IsGenericInstance())
			{
				return null;
			}
			TypeDefinition typeDefinition = node.Type.Resolve();
			if (!typeDefinition.IsAnonymous())
			{
				return null;
			}
			this.initializerExpressions = new BlockExpression(null);
			this.ProcessAnonymousType(typeDefinition, node.Type as GenericInstanceType, node.Constructor.Resolve(), node.Arguments);
			InitializerExpression initializerExpression = new InitializerExpression(this.initializerExpressions, InitializerType.AnonymousInitializer);
			return new AnonymousObjectCreationExpression(node.Constructor, typeDefinition, initializerExpression, node.MappedInstructions);
		}
	}
}