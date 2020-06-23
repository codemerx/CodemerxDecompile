using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mono.Cecil.Extensions
{
	public static class MethodDefinitionExtensions
	{
		public static IEnumerable<Instruction> GetMethodInstructions(this MethodDefinition method)
		{
			IEnumerable<Instruction> instructions;
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
				instructions = method.Body.Instructions;
			}
			catch
			{
				instructions = Enumerable.Empty<Instruction>();
			}
			return instructions;
		}

		public static MethodDefinition GetMoreVisibleMethod(this MethodDefinition method, MethodDefinition other)
		{
			if (method == null)
			{
				return other;
			}
			if (other == null)
			{
				return method;
			}
			if (method.IsPrivate)
			{
				return other;
			}
			if (method.IsFamily || method.IsAssembly)
			{
				if (!other.IsPublic && !other.IsFamilyOrAssembly)
				{
					return method;
				}
				return other;
			}
			if (!method.IsFamilyOrAssembly)
			{
				return method;
			}
			if (other.IsPublic)
			{
				return other;
			}
			return method;
		}

		public static bool HasAsyncAttributes(this MethodDefinition self)
		{
			bool flag;
			bool flag1 = false;
			bool flag2 = false;
			Collection<CustomAttribute>.Enumerator enumerator = self.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					CustomAttribute current = enumerator.Current;
					if (!(flag1 & flag2))
					{
						if (!flag1 && MethodDefinitionExtensions.IsAsyncStateMachineAttribute(current))
						{
							flag1 = true;
						}
						if (flag2 || !MethodDefinitionExtensions.IsDebuggerStepThroughAttribute(current))
						{
							continue;
						}
						flag2 = true;
					}
					else
					{
						flag = true;
						return flag;
					}
				}
				return flag1 & flag2;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static bool HasAsyncStateMachineVariable(this MethodDefinition self)
		{
			bool flag;
			TypeDefinition typeDefinition;
			Collection<VariableDefinition>.Enumerator enumerator = self.Body.Variables.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					VariableDefinition current = enumerator.Current;
					if (current.VariableType == null)
					{
						typeDefinition = null;
					}
					else
					{
						typeDefinition = current.VariableType.Resolve();
					}
					TypeDefinition typeDefinition1 = typeDefinition;
					if (typeDefinition1 == null || !typeDefinition1.IsAsyncStateMachine())
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public static bool IsAsync(this MethodDefinition self)
		{
			TypeDefinition typeDefinition;
			if (self.IsAsync(out typeDefinition))
			{
				return true;
			}
			if (!self.HasAsyncAttributes())
			{
				return false;
			}
			return self.HasAsyncStateMachineVariable();
		}

		public static bool IsAsync(this MethodDefinition self, out TypeDefinition asyncStateMachineType)
		{
			bool flag;
			asyncStateMachineType = null;
			Collection<CustomAttribute>.Enumerator enumerator = self.CustomAttributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (!MethodDefinitionExtensions.IsAsyncAttribute(enumerator.Current, self, self.DeclaringType, out asyncStateMachineType))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
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
			TypeReference value = customAttribute.ConstructorArguments[0].Value as TypeReference;
			if (value == null)
			{
				return false;
			}
			TypeDefinition typeDefinition = value.Resolve();
			if (typeDefinition == null || typeDefinition.DeclaringType != declaringType || !typeDefinition.IsAsyncStateMachine())
			{
				return false;
			}
			stateMachineType = typeDefinition;
			return true;
		}

		private static bool IsAsyncStateMachineAttribute(CustomAttribute customAttribute)
		{
			return customAttribute.AttributeType.FullName == "System.Runtime.CompilerServices.AsyncStateMachineAttribute";
		}

		private static bool IsDebuggerStepThroughAttribute(CustomAttribute customAttribute)
		{
			return customAttribute.AttributeType.FullName == "System.Diagnostics.DebuggerStepThroughAttribute";
		}

		public static bool IsExtern(this MethodDefinition method)
		{
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

		public static bool IsFunction(this MethodDefinition self)
		{
			if (self.FixedReturnType == null)
			{
				return false;
			}
			return self.FixedReturnType.FullName != "System.Void";
		}

		public static bool IsQueryableMethod(this MethodDefinition self)
		{
			if (self == null || !self.IsStatic)
			{
				return false;
			}
			return self.DeclaringType.FullName == "System.Linq.Queryable";
		}

		public static bool IsQueryMethod(this MethodDefinition self)
		{
			// 
			// Current member / type: System.Boolean Mono.Cecil.Extensions.MethodDefinitionExtensions::IsQueryMethod(Mono.Cecil.MethodDefinition)
			// File path: C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\Decompiler.Tests\bin\Release\netcoreapp2.1\JustDecompiler.NetStandard.dll
			// 
			// Product version: 0.0.0.0
			// Exception in: System.Boolean IsQueryMethod(Mono.Cecil.MethodDefinition)
			// 
			// Object reference not set to an instance of an object.
			//    at Telerik.JustDecompiler.Decompiler.LogicFlow.DTree.BaseDominatorTreeBuilder.ComputeDominanceFrontiers() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\DTree\BaseDominatorTreeBuilder.cs:line 107
			//    at Telerik.JustDecompiler.Decompiler.LogicFlow.DTree.BaseDominatorTreeBuilder.BuildTreeInternal(BaseDominatorTreeBuilder theBuilder) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\DTree\BaseDominatorTreeBuilder.cs:line 26
			//    at Telerik.JustDecompiler.Decompiler.LogicFlow.DominatorTreeDependentStep.GetDominatorTreeFromContext(ILogicalConstruct construct) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\DominatorTreeDependentStep.cs:line 20
			//    at Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopBuilder.ProcessLogicalConstruct(ILogicalConstruct construct) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\Loops\LoopBuilder.cs:line 68
			//    at Telerik.JustDecompiler.Decompiler.LogicFlow.Loops.LoopBuilder.BuildLoops(ILogicalConstruct block) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\Loops\LoopBuilder.cs:line 59
			//    at Telerik.JustDecompiler.Decompiler.LogicFlow.LogicalFlowBuilderStep.BuildLogicalConstructTree() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 128
			//    at Telerik.JustDecompiler.Decompiler.LogicFlow.LogicalFlowBuilderStep.Process(DecompilationContext context, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\LogicFlow\LogicalFlowBuilderStep.cs:line 51
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.RunInternal(MethodBody body, BlockStatement block, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 81
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.Run(MethodBody body, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.Decompile(MethodBody body, ILanguage language, DecompilationContext& context, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\Extensions.cs:line 61
			//    at Telerik.JustDecompiler.Decompiler.WriterContextServices.BaseWriterContextService.DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}