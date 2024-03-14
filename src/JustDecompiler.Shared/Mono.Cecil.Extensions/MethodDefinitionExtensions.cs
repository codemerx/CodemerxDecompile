using Mono.Cecil.Cil;
using System.Collections.Generic;
using System;
using System.Linq;
using Telerik.JustDecompiler;

namespace Mono.Cecil.Extensions
{
	public static class MethodDefinitionExtensions
	{
        public static IEnumerable<Instruction> GetMethodInstructions(this MethodDefinition method)
        {
            if (method == null)
            {
                throw new ArgumentNullException("MethodDefinition");
            }
            if (!method.HasBody)
            {
                return Enumerable.Empty<Instruction>();
            }
            try
            {
                MethodBody body = method.Body;
                return body.Instructions;
            }
            catch 
            {
                return Enumerable.Empty<Instruction>(); 
            }
        }

		public static MethodDefinition GetMoreVisibleMethod(this MethodDefinition method, MethodDefinition other)
		{
			if (method == null)
				return other;

			if (other == null)
				return method;

			if (method.IsPrivate)
			{
				return other;
			}
			if (method.IsFamily || method.IsAssembly)
			{
				if (other.IsPublic || other.IsFamilyOrAssembly)
				{
					return other;
				}
				return method;
			}
			if (method.IsFamilyOrAssembly)
			{
				if (other.IsPublic)
				{
					return other;
				}
				return method;
			}
			return method;
		}

        public static bool IsExtern(this MethodDefinition method)
        {
            // TODO: Handle the DLLImports and other possible cases
            if (method.IsInternalCall)
            {
                return true;
            }
            if (method.IsPInvokeImpl)
            {
                return true;
            }
            if (method.IsRuntime)
            {
                return true;
            }
            return false;
        }

        public static bool IsAsync(this MethodDefinition self)
        {
            TypeDefinition typeDef;
            return self.IsAsync(out typeDef) || (self.HasAsyncAttributes() && HasAsyncStateMachineVariable(self));
        }

		public static bool HasAsyncStateMachineVariable(this MethodDefinition self)
		{
			foreach (VariableDefinition variable in self.Body.Variables)
			{
				TypeDefinition typeDef = variable.VariableType == null ? null : variable.VariableType.Resolve();

				if (typeDef != null && typeDef.IsAsyncStateMachine())
				{
					return true;
				}
			}

			return false;
		}

        /// <summary>
        /// Determines if the MethodDefinition is async method.
        /// </summary>
        /// <remarks>
        /// Since C# 6.0 the DebuggerStepThrough attribute is no longer nessesary to exists, if method is async.
        /// </remarks>
		public static bool IsAsync(this MethodDefinition self, out TypeDefinition asyncStateMachineType)
		{
			asyncStateMachineType = null;
			foreach (CustomAttribute attribute in self.CustomAttributes)
			{
				if (IsAsyncAttribute(attribute, self, self.DeclaringType, out asyncStateMachineType))
				{
                    return true;
				}
			}

            return false;
		}

		public static bool IsFunction(this MethodDefinition self)
		{
			return (self.FixedReturnType != null) && (self.FixedReturnType.FullName != Constants.Void);
		}

		public static bool HasAsyncAttributes(this MethodDefinition self)
		{
			bool hasAsyncStateMachineAttribute = false;
			bool hasDebuggerStepThroughAttribute = false;
			foreach (CustomAttribute attribute in self.CustomAttributes)
			{
				if (hasAsyncStateMachineAttribute && hasDebuggerStepThroughAttribute)
				{
					return true;
				}

				if (!hasAsyncStateMachineAttribute && IsAsyncStateMachineAttribute(attribute))
				{
					hasAsyncStateMachineAttribute = true;
				}

				if (!hasDebuggerStepThroughAttribute && IsDebuggerStepThroughAttribute(attribute))
				{
					hasDebuggerStepThroughAttribute = true;
				}
			}

			return hasAsyncStateMachineAttribute && hasDebuggerStepThroughAttribute;
		}

        private static bool IsAsyncAttribute(CustomAttribute customAttribute, MethodDefinition method, TypeDefinition declaringType, out TypeDefinition stateMachineType)
        {
            stateMachineType = null;

            if (customAttribute.AttributeType.FullName != "System.Runtime.CompilerServices.AsyncStateMachineAttribute")
            {
                return false;
            }

            customAttribute.Resolve();
            if (customAttribute.ConstructorArguments.Count != 1 || customAttribute.ConstructorArguments[0].Type.FullName != "System.Type")
            {
                return false;
            }

            TypeReference typeReference = customAttribute.ConstructorArguments[0].Value as TypeReference;
            if (typeReference == null)
            {
                return false;
            }

            TypeDefinition typeDef = typeReference.Resolve();
            if (typeDef == null || typeDef.DeclaringType != declaringType || !typeDef.IsAsyncStateMachine())
            {
                return false;
            }

            stateMachineType = typeDef;
            return true;
        }

        private static bool IsDebuggerStepThroughAttribute(CustomAttribute customAttribute)
        {
            return customAttribute.AttributeType.FullName == "System.Diagnostics.DebuggerStepThroughAttribute";
        }

		private static bool IsAsyncStateMachineAttribute(CustomAttribute customAttribute)
		{
			return customAttribute.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
		}

        public static bool IsQueryMethod(this MethodDefinition self)
        {
            if (self == null || !self.IsStatic || self.DeclaringType.FullName != "System.Linq.Enumerable" && self.DeclaringType.FullName != "System.Linq.Queryable")
            {
                return false;
            }

            switch (self.Name)
            {
                case "Select":
                case "SelectMany":
                case "Where":
                case "Join":
                case "GroupJoin":
                case "OrderBy":
                case "ThenBy":
                case "OrderByDescending":
                case "ThenByDescending":
                case "GroupBy":
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsQueryableMethod(this MethodDefinition self)
        {
            return self != null && self.IsStatic && self.DeclaringType.FullName == "System.Linq.Queryable";
        }
	}
}
