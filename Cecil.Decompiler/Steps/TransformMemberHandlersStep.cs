using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
    class TransformMemberHandlersStep : BaseCodeTransformer, IDecompilationStep
    {
        private DecompilationContext context;
        private TypeSystem typeSystem;
        private TypeDefinition cachedSystemTypeTypeDefinition;

        public Ast.Statements.BlockStatement Process(Decompiler.DecompilationContext context, Ast.Statements.BlockStatement body)
        {
            this.context = context;
            this.typeSystem = context.TypeContext.CurrentType.Module.TypeSystem;
            return (Telerik.JustDecompiler.Ast.Statements.BlockStatement)Visit(body);
        }

        public override ICodeNode VisitMemberHandleExpression(Ast.Expressions.MemberHandleExpression node)
        {
            MethodReference methodRef = node.MemberReference as MethodReference;
            if (methodRef != null)
            {
                return GetMethodHandleExpression(methodRef, node.MappedInstructions);
            }

            TypeReference typeRef = node.MemberReference as TypeReference;
            if (typeRef != null)
            {
                return GetTypeHandleExpression(typeRef, node.MappedInstructions);
            }

            FieldReference fieldRef = node.MemberReference as FieldReference;
            if (fieldRef != null)
            {
                return GetFieldHandleExpression(fieldRef, node.MappedInstructions);
            }

            throw new NotSupportedException();
        }

        private TypeDefinition GetSystemTypeTypeDefinition()
        {
            return cachedSystemTypeTypeDefinition ?? (cachedSystemTypeTypeDefinition = Utilities.GetCorlibTypeReference(typeof(Type), this.context.TypeContext.CurrentType.Module).Resolve());
        }

        private Expression GetMethodHandleExpression(MethodReference methodReference, IEnumerable<Instruction> instructions)
        {
            TypeDefinition corlibTypeTypeDefinition = GetSystemTypeTypeDefinition();

            string[] parametersNames = methodReference.HasParameters ? new string[] { "System.String", "System.Type[]" } : new string[] { "System.String" };
            MethodReference getMethodReference = GetSystemTypeMethodReference(corlibTypeTypeDefinition, "GetMethod", parametersNames);

            MethodReference getMethodHandleReference = GetHandlePropertyGetterReference(typeof(System.Reflection.MethodBase), "get_MethodHandle");

            TypeOfExpression typeOfExpression = new TypeOfExpression(methodReference.DeclaringType, null);
            MethodReferenceExpression getMethodMethodReferenceExpression = new MethodReferenceExpression(typeOfExpression, getMethodReference, null);
            MethodInvocationExpression getMethodMethodInvocationExpression = new MethodInvocationExpression(getMethodMethodReferenceExpression, null);
            LiteralExpression argument = new LiteralExpression(methodReference.Name, this.typeSystem, null);
            getMethodMethodInvocationExpression.Arguments.Add(argument);

            if (methodReference.HasParameters)
            {
                BlockExpression blockExpression = new BlockExpression(null);
                foreach (ParameterDefinition parameter in methodReference.Parameters)
                {
                    blockExpression.Expressions.Add(new TypeOfExpression(parameter.ParameterType, null));
                }

				InitializerExpression initializer = new InitializerExpression(blockExpression, InitializerType.ArrayInitializer);
				ArrayCreationExpression getMethodTypeParametersArray = new ArrayCreationExpression(corlibTypeTypeDefinition, initializer, null);
                getMethodTypeParametersArray.Dimensions.Add(new LiteralExpression(blockExpression.Expressions.Count, this.typeSystem, null));

                getMethodMethodInvocationExpression.Arguments.Add(getMethodTypeParametersArray);
            }

            MethodReferenceExpression getMethodHandleMethodReferenceExpression = new MethodReferenceExpression(getMethodMethodInvocationExpression, getMethodHandleReference, null);
            MethodInvocationExpression getMethodHandleMethodInvocationExpression = new MethodInvocationExpression(getMethodHandleMethodReferenceExpression, instructions);
            PropertyReferenceExpression methodHandlePropertyReferenceExpression = new PropertyReferenceExpression(getMethodHandleMethodInvocationExpression, null);

            return methodHandlePropertyReferenceExpression;
        }

        private MethodReference GetSystemTypeMethodReference(TypeDefinition corlibTypeTypeDefinition, string methodName, string[] parametersNames)
        {
            MethodReference getMethodReference = null;

            foreach (MethodDefinition method in corlibTypeTypeDefinition.Methods)
            {
                if (method.Name == methodName && method.Parameters.Count == parametersNames.Length)
                {
                    bool isFound = true;
                    for (int i = 0; i < method.Parameters.Count; i++)
                    {
                        if (method.Parameters[i].ParameterType.FullName != parametersNames[i])
                        {
                            isFound = false;
                            break;
                        }
                    }

                    if (isFound)
                    {
                        getMethodReference = method;
                        break;
                    }
                }
            }

            return getMethodReference;
        }

        private MethodReference GetHandlePropertyGetterReference(Type type, string getterName)
        {
            TypeDefinition corlibTypeDefinition = Utilities.GetCorlibTypeReference(type, this.context.TypeContext.CurrentType.Module).Resolve();

            MethodReference getMethodHandleReference = null;

            foreach (MethodDefinition method in corlibTypeDefinition.Methods)
            {
                if (method.Name == getterName && method.IsGetter)
                {
                    getMethodHandleReference = method;
                    break;
                }
            }

            return getMethodHandleReference;
        }

        private Expression GetTypeHandleExpression(TypeReference typeReference, IEnumerable<Instruction> instructions)
        {
            TypeOfExpression typeOfExpression = new TypeOfExpression(typeReference, null);
            MethodReference getHandleReference = GetHandlePropertyGetterReference(typeof(Type), "get_TypeHandle");
            MethodReferenceExpression getHandleReferenceExpression = new MethodReferenceExpression(typeOfExpression, getHandleReference, null);
            MethodInvocationExpression methodInvoke = new MethodInvocationExpression(getHandleReferenceExpression, instructions);

            return new PropertyReferenceExpression(methodInvoke, null);
        }

        private Expression GetFieldHandleExpression(FieldReference fieldReference, IEnumerable<Instruction> instructions)
        {
            TypeDefinition corlibTypeTypeDefinition = GetSystemTypeTypeDefinition();
            MethodReference getFieldReference = GetSystemTypeMethodReference(corlibTypeTypeDefinition, "GetField", new string[] { "System.String" });

            TypeOfExpression typeOfExpression = new TypeOfExpression(fieldReference.DeclaringType, null);
            MethodReferenceExpression getFieldMethodReferenceExpression = new MethodReferenceExpression(typeOfExpression, getFieldReference, null);

            MethodInvocationExpression getFieldInfoInvocation = new MethodInvocationExpression(getFieldMethodReferenceExpression, null);
            getFieldInfoInvocation.Arguments.Add(new LiteralExpression(fieldReference.Name, this.typeSystem, null));

            MethodReference getFieldHandleReference = GetHandlePropertyGetterReference(typeof(System.Reflection.FieldInfo), "get_FieldHandle");
            MethodInvocationExpression getFieldHandleInvoke = new MethodInvocationExpression(new MethodReferenceExpression(getFieldInfoInvocation, getFieldHandleReference, null), instructions);

            return new PropertyReferenceExpression(getFieldHandleInvoke, null);
        }
    }
}
