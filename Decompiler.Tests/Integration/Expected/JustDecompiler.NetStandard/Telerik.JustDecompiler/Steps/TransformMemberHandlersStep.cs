using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class TransformMemberHandlersStep : BaseCodeTransformer, IDecompilationStep
	{
		private DecompilationContext context;

		private TypeSystem typeSystem;

		private TypeDefinition cachedSystemTypeTypeDefinition;

		public TransformMemberHandlersStep()
		{
		}

		private Expression GetFieldHandleExpression(FieldReference fieldReference, IEnumerable<Instruction> instructions)
		{
			TypeDefinition systemTypeTypeDefinition = this.GetSystemTypeTypeDefinition();
			MethodReference systemTypeMethodReference = this.GetSystemTypeMethodReference(systemTypeTypeDefinition, "GetField", new String[] { "System.String" });
			MethodInvocationExpression methodInvocationExpression = new MethodInvocationExpression(new MethodReferenceExpression(new TypeOfExpression(fieldReference.DeclaringType, null), systemTypeMethodReference, null), null);
			methodInvocationExpression.Arguments.Add(new LiteralExpression(fieldReference.Name, this.typeSystem, null));
			MethodReference handlePropertyGetterReference = this.GetHandlePropertyGetterReference(typeof(FieldInfo), "get_FieldHandle");
			return new PropertyReferenceExpression(new MethodInvocationExpression(new MethodReferenceExpression(methodInvocationExpression, handlePropertyGetterReference, null), instructions), null);
		}

		private MethodReference GetHandlePropertyGetterReference(Type type, string getterName)
		{
			MethodReference methodReference = null;
			foreach (MethodDefinition method in Utilities.GetCorlibTypeReference(type, this.context.TypeContext.CurrentType.Module).Resolve().Methods)
			{
				if (!(method.Name == getterName) || !method.IsGetter)
				{
					continue;
				}
				methodReference = method;
				return methodReference;
			}
			return methodReference;
		}

		private Expression GetMethodHandleExpression(MethodReference methodReference, IEnumerable<Instruction> instructions)
		{
			TypeDefinition systemTypeTypeDefinition = this.GetSystemTypeTypeDefinition();
			MethodReference systemTypeMethodReference = this.GetSystemTypeMethodReference(systemTypeTypeDefinition, "GetMethod", (methodReference.HasParameters ? new String[] { "System.String", "System.Type[]" } : new String[] { "System.String" }));
			MethodReference handlePropertyGetterReference = this.GetHandlePropertyGetterReference(typeof(MethodBase), "get_MethodHandle");
			MethodInvocationExpression methodInvocationExpression = new MethodInvocationExpression(new MethodReferenceExpression(new TypeOfExpression(methodReference.DeclaringType, null), systemTypeMethodReference, null), null);
			LiteralExpression literalExpression = new LiteralExpression(methodReference.Name, this.typeSystem, null);
			methodInvocationExpression.Arguments.Add(literalExpression);
			if (methodReference.HasParameters)
			{
				BlockExpression blockExpression = new BlockExpression(null);
				foreach (ParameterDefinition parameter in methodReference.Parameters)
				{
					blockExpression.Expressions.Add(new TypeOfExpression(parameter.ParameterType, null));
				}
				ArrayCreationExpression arrayCreationExpression = new ArrayCreationExpression(systemTypeTypeDefinition, new InitializerExpression(blockExpression, InitializerType.ArrayInitializer), null);
				arrayCreationExpression.Dimensions.Add(new LiteralExpression((object)blockExpression.Expressions.Count, this.typeSystem, null));
				methodInvocationExpression.Arguments.Add(arrayCreationExpression);
			}
			return new PropertyReferenceExpression(new MethodInvocationExpression(new MethodReferenceExpression(methodInvocationExpression, handlePropertyGetterReference, null), instructions), null);
		}

		private MethodReference GetSystemTypeMethodReference(TypeDefinition corlibTypeTypeDefinition, string methodName, string[] parametersNames)
		{
			MethodReference methodReference = null;
			foreach (MethodDefinition method in corlibTypeTypeDefinition.Methods)
			{
				if (!(method.Name == methodName) || method.Parameters.Count != (int)parametersNames.Length)
				{
					continue;
				}
				bool flag = true;
				int num = 0;
				while (num < method.Parameters.Count)
				{
					if (method.Parameters[num].ParameterType.FullName == parametersNames[num])
					{
						num++;
					}
					else
					{
						flag = false;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				methodReference = method;
				return methodReference;
			}
			return methodReference;
		}

		private TypeDefinition GetSystemTypeTypeDefinition()
		{
			TypeDefinition typeDefinition = this.cachedSystemTypeTypeDefinition;
			if (typeDefinition == null)
			{
				TypeDefinition typeDefinition1 = Utilities.GetCorlibTypeReference(typeof(Type), this.context.TypeContext.CurrentType.Module).Resolve();
				TypeDefinition typeDefinition2 = typeDefinition1;
				this.cachedSystemTypeTypeDefinition = typeDefinition1;
				typeDefinition = typeDefinition2;
			}
			return typeDefinition;
		}

		private Expression GetTypeHandleExpression(TypeReference typeReference, IEnumerable<Instruction> instructions)
		{
			TypeOfExpression typeOfExpression = new TypeOfExpression(typeReference, null);
			MethodReference handlePropertyGetterReference = this.GetHandlePropertyGetterReference(typeof(Type), "get_TypeHandle");
			return new PropertyReferenceExpression(new MethodInvocationExpression(new MethodReferenceExpression(typeOfExpression, handlePropertyGetterReference, null), instructions), null);
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.typeSystem = context.TypeContext.CurrentType.Module.TypeSystem;
			return (BlockStatement)this.Visit(body);
		}

		public override ICodeNode VisitMemberHandleExpression(MemberHandleExpression node)
		{
			MethodReference memberReference = node.MemberReference as MethodReference;
			if (memberReference != null)
			{
				return this.GetMethodHandleExpression(memberReference, node.MappedInstructions);
			}
			TypeReference typeReference = node.MemberReference as TypeReference;
			if (typeReference != null)
			{
				return this.GetTypeHandleExpression(typeReference, node.MappedInstructions);
			}
			FieldReference fieldReference = node.MemberReference as FieldReference;
			if (fieldReference == null)
			{
				throw new NotSupportedException();
			}
			return this.GetFieldHandleExpression(fieldReference, node.MappedInstructions);
		}
	}
}