using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil.Extensions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Steps
{
    class RebuildAnonymousTypesInitializersStep
    {
        private readonly BaseCodeTransformer transformer;
        private readonly TypeSystem typeSystem;
        private BlockExpression initializerExpressions;

        public RebuildAnonymousTypesInitializersStep(BaseCodeTransformer transformer, TypeSystem typeSystem)
        {
            this.transformer = transformer;
            this.typeSystem = typeSystem;
        }

		public ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
            if(node.Type == null || node.Constructor == null || !node.Type.IsGenericInstance)
            {
                return null;
            }

            TypeDefinition typeDef = node.Type.Resolve();
            if (!typeDef.IsAnonymous())
            {
                return null;
            }

            initializerExpressions = new BlockExpression(null);
            ProcessAnonymousType(typeDef, node.Type as GenericInstanceType, node.Constructor.Resolve(), node.Arguments);

			InitializerExpression initializer = new InitializerExpression(initializerExpressions, InitializerType.AnonymousInitializer);
			return new AnonymousObjectCreationExpression(node.Constructor, typeDef, initializer, node.MappedInstructions);
		}

        private void ProcessAnonymousType(TypeDefinition anonymousTypeDefinition, GenericInstanceType anonymousInstanceType,
            MethodDefinition constructorDefinition, ExpressionCollection constructorArguments)
        {
            for (int i = 0; i < constructorDefinition.Parameters.Count; i++)
            {
                ParameterDefinition currentParameter = constructorDefinition.Parameters[i];
                int genericParameterIndex = anonymousTypeDefinition.GenericParameters.IndexOf(currentParameter.ParameterType as GenericParameter);
                PropertyDefinition property = FindPropertyOfType(anonymousTypeDefinition, currentParameter.ParameterType);
				TypeReference propertyType = anonymousInstanceType.GenericArguments[genericParameterIndex];
				if (anonymousInstanceType.PostionToArgument.ContainsKey(genericParameterIndex))
				{
					propertyType = anonymousInstanceType.PostionToArgument[genericParameterIndex];
				}
                Expression parameterExpression =
                    new AnonymousPropertyInitializerExpression(property, propertyType);
                int argumentIndex = GetParameterIndexWithType(constructorDefinition, currentParameter.ParameterType);
                Expression argumentExpression = transformer.Visit(constructorArguments[argumentIndex].Clone()) as Expression;
                initializerExpressions.Expressions.Add(
                    new BinaryExpression(BinaryOperator.Assign, parameterExpression, argumentExpression, this.typeSystem, null));
            }
        }

        private PropertyDefinition FindPropertyOfType(TypeDefinition typeDefinition, TypeReference parameterType)
        {
            foreach (PropertyDefinition property in typeDefinition.Properties)
            {
                if (property.PropertyType == parameterType)
                {
                    return property;
                }
            }
            return null;
        }

        private int GetParameterIndexWithType(MethodDefinition methodDef, TypeReference typeReference)
        {
            int i = 0;
            foreach (ParameterDefinition parameter in methodDef.Parameters)
            {
                if (parameter.ParameterType == typeReference)
                {
                    return i;
                }
                i++;
            }

            return -1;
        }
    }
}
