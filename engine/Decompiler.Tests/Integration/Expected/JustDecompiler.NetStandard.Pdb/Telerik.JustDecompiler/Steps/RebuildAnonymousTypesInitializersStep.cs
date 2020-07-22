using Mono.Cecil;
using Mono.Collections.Generic;
using System;
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
			base();
			this.transformer = transformer;
			this.typeSystem = typeSystem;
			return;
		}

		private PropertyDefinition FindPropertyOfType(TypeDefinition typeDefinition, TypeReference parameterType)
		{
			V_0 = typeDefinition.get_Properties().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if ((object)V_1.get_PropertyType() != (object)parameterType)
					{
						continue;
					}
					V_2 = V_1;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				V_0.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return null;
		}

		private int GetParameterIndexWithType(MethodDefinition methodDef, TypeReference typeReference)
		{
			V_0 = 0;
			V_1 = methodDef.get_Parameters().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					if ((object)V_1.get_Current().get_ParameterType() != (object)typeReference)
					{
						V_0 = V_0 + 1;
					}
					else
					{
						V_2 = V_0;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				V_1.Dispose();
			}
		Label1:
			return V_2;
		Label0:
			return -1;
		}

		private void ProcessAnonymousType(TypeDefinition anonymousTypeDefinition, GenericInstanceType anonymousInstanceType, MethodDefinition constructorDefinition, ExpressionCollection constructorArguments)
		{
			V_0 = 0;
			while (V_0 < constructorDefinition.get_Parameters().get_Count())
			{
				V_1 = constructorDefinition.get_Parameters().get_Item(V_0);
				V_2 = anonymousTypeDefinition.get_GenericParameters().IndexOf(V_1.get_ParameterType() as GenericParameter);
				stackVariable19 = this.FindPropertyOfType(anonymousTypeDefinition, V_1.get_ParameterType());
				V_3 = anonymousInstanceType.get_GenericArguments().get_Item(V_2);
				if (anonymousInstanceType.get_PostionToArgument().ContainsKey(V_2))
				{
					V_3 = anonymousInstanceType.get_PostionToArgument().get_Item(V_2);
				}
				V_4 = new AnonymousPropertyInitializerExpression(stackVariable19, V_3);
				V_5 = this.GetParameterIndexWithType(constructorDefinition, V_1.get_ParameterType());
				V_6 = this.transformer.Visit(constructorArguments.get_Item(V_5).Clone()) as Expression;
				this.initializerExpressions.get_Expressions().Add(new BinaryExpression(26, V_4, V_6, this.typeSystem, null, false));
				V_0 = V_0 + 1;
			}
			return;
		}

		public ICodeNode VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			if (node.get_Type() == null || node.get_Constructor() == null || !node.get_Type().get_IsGenericInstance())
			{
				return null;
			}
			V_0 = node.get_Type().Resolve();
			if (!V_0.IsAnonymous())
			{
				return null;
			}
			this.initializerExpressions = new BlockExpression(null);
			this.ProcessAnonymousType(V_0, node.get_Type() as GenericInstanceType, node.get_Constructor().Resolve(), node.get_Arguments());
			V_1 = new InitializerExpression(this.initializerExpressions, 3);
			return new AnonymousObjectCreationExpression(node.get_Constructor(), V_0, V_1, node.get_MappedInstructions());
		}
	}
}