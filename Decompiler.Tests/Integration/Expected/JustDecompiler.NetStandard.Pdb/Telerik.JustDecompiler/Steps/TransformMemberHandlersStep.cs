using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
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
			base();
			return;
		}

		private Expression GetFieldHandleExpression(FieldReference fieldReference, IEnumerable<Instruction> instructions)
		{
			V_0 = this.GetSystemTypeTypeDefinition();
			stackVariable6 = new String[1];
			stackVariable6[0] = "System.String";
			V_1 = this.GetSystemTypeMethodReference(V_0, "GetField", stackVariable6);
			stackVariable18 = new MethodInvocationExpression(new MethodReferenceExpression(new TypeOfExpression(fieldReference.get_DeclaringType(), null), V_1, null), null);
			stackVariable18.get_Arguments().Add(new LiteralExpression(fieldReference.get_Name(), this.typeSystem, null));
			V_2 = this.GetHandlePropertyGetterReference(Type.GetTypeFromHandle(// 
			// Current member / type: Telerik.JustDecompiler.Ast.Expressions.Expression Telerik.JustDecompiler.Steps.TransformMemberHandlersStep::GetFieldHandleExpression(Mono.Cecil.FieldReference,System.Collections.Generic.IEnumerable`1<Mono.Cecil.Cil.Instruction>)
			// Exception in: Telerik.JustDecompiler.Ast.Expressions.Expression GetFieldHandleExpression(Mono.Cecil.FieldReference,System.Collections.Generic.IEnumerable<Mono.Cecil.Cil.Instruction>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private MethodReference GetHandlePropertyGetterReference(Type type, string getterName)
		{
			V_0 = null;
			V_1 = Utilities.GetCorlibTypeReference(type, this.context.get_TypeContext().get_CurrentType().get_Module()).Resolve().get_Methods().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!String.op_Equality(V_2.get_Name(), getterName) || !V_2.get_IsGetter())
					{
						continue;
					}
					V_0 = V_2;
					goto Label0;
				}
			}
			finally
			{
				V_1.Dispose();
			}
		Label0:
			return V_0;
		}

		private Expression GetMethodHandleExpression(MethodReference methodReference, IEnumerable<Instruction> instructions)
		{
			V_0 = this.GetSystemTypeTypeDefinition();
			if (methodReference.get_HasParameters())
			{
				stackVariable5 = new String[2];
				stackVariable5[0] = "System.String";
				stackVariable5[1] = "System.Type[]";
			}
			else
			{
				stackVariable5 = new String[1];
				stackVariable5[0] = "System.String";
			}
			V_2 = this.GetSystemTypeMethodReference(V_0, "GetMethod", stackVariable5);
			V_3 = this.GetHandlePropertyGetterReference(Type.GetTypeFromHandle(// 
			// Current member / type: Telerik.JustDecompiler.Ast.Expressions.Expression Telerik.JustDecompiler.Steps.TransformMemberHandlersStep::GetMethodHandleExpression(Mono.Cecil.MethodReference,System.Collections.Generic.IEnumerable`1<Mono.Cecil.Cil.Instruction>)
			// Exception in: Telerik.JustDecompiler.Ast.Expressions.Expression GetMethodHandleExpression(Mono.Cecil.MethodReference,System.Collections.Generic.IEnumerable<Mono.Cecil.Cil.Instruction>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private MethodReference GetSystemTypeMethodReference(TypeDefinition corlibTypeTypeDefinition, string methodName, string[] parametersNames)
		{
			V_0 = null;
			V_1 = corlibTypeTypeDefinition.get_Methods().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!String.op_Equality(V_2.get_Name(), methodName) || V_2.get_Parameters().get_Count() != (int)parametersNames.Length)
					{
						continue;
					}
					V_3 = true;
					V_4 = 0;
					while (V_4 < V_2.get_Parameters().get_Count())
					{
						if (!String.op_Inequality(V_2.get_Parameters().get_Item(V_4).get_ParameterType().get_FullName(), parametersNames[V_4]))
						{
							V_4 = V_4 + 1;
						}
						else
						{
							V_3 = false;
							break;
						}
					}
					if (!V_3)
					{
						continue;
					}
					V_0 = V_2;
					goto Label0;
				}
			}
			finally
			{
				V_1.Dispose();
			}
		Label0:
			return V_0;
		}

		private TypeDefinition GetSystemTypeTypeDefinition()
		{
			stackVariable1 = this.cachedSystemTypeTypeDefinition;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable11 = Utilities.GetCorlibTypeReference(Type.GetTypeFromHandle(// 
				// Current member / type: Mono.Cecil.TypeDefinition Telerik.JustDecompiler.Steps.TransformMemberHandlersStep::GetSystemTypeTypeDefinition()
				// Exception in: Mono.Cecil.TypeDefinition GetSystemTypeTypeDefinition()
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		private Expression GetTypeHandleExpression(TypeReference typeReference, IEnumerable<Instruction> instructions)
		{
			stackVariable2 = new TypeOfExpression(typeReference, null);
			V_0 = this.GetHandlePropertyGetterReference(Type.GetTypeFromHandle(// 
			// Current member / type: Telerik.JustDecompiler.Ast.Expressions.Expression Telerik.JustDecompiler.Steps.TransformMemberHandlersStep::GetTypeHandleExpression(Mono.Cecil.TypeReference,System.Collections.Generic.IEnumerable`1<Mono.Cecil.Cil.Instruction>)
			// Exception in: Telerik.JustDecompiler.Ast.Expressions.Expression GetTypeHandleExpression(Mono.Cecil.TypeReference,System.Collections.Generic.IEnumerable<Mono.Cecil.Cil.Instruction>)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.typeSystem = context.get_TypeContext().get_CurrentType().get_Module().get_TypeSystem();
			return (BlockStatement)this.Visit(body);
		}

		public override ICodeNode VisitMemberHandleExpression(MemberHandleExpression node)
		{
			V_0 = node.get_MemberReference() as MethodReference;
			if (V_0 != null)
			{
				return this.GetMethodHandleExpression(V_0, node.get_MappedInstructions());
			}
			V_1 = node.get_MemberReference() as TypeReference;
			if (V_1 != null)
			{
				return this.GetTypeHandleExpression(V_1, node.get_MappedInstructions());
			}
			V_2 = node.get_MemberReference() as FieldReference;
			if (V_2 == null)
			{
				throw new NotSupportedException();
			}
			return this.GetFieldHandleExpression(V_2, node.get_MappedInstructions());
		}
	}
}