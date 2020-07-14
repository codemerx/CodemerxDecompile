using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Cil;

namespace Telerik.JustDecompiler.Decompiler.StateMachines
{
	internal static class StateMachineUtilities
	{
		public static void FixInstructionConnections(InstructionBlock[] orderedBlocks)
		{
			orderedBlocks[0].First.set_Previous(null);
			for (int i = 0; i < (int)orderedBlocks.Length - 1; i++)
			{
				orderedBlocks[i].Last.set_Next(orderedBlocks[i + 1].First);
				orderedBlocks[i + 1].First.set_Previous(orderedBlocks[i].Last);
			}
			orderedBlocks[(int)orderedBlocks.Length - 1].Last.set_Next(null);
		}

		public static bool IsUnconditionalBranch(Instruction instruction)
		{
			Code code = instruction.get_OpCode().get_Code();
			if (code == 55 || code == 42 || code == 187)
			{
				return true;
			}
			return code == 188;
		}

		public static bool TryGetOperandOfLdc(Instruction instruction, out int operand)
		{
			switch (instruction.get_OpCode().get_Code())
			{
				case 21:
				{
					operand = -1;
					break;
				}
				case 22:
				{
					operand = 0;
					break;
				}
				case 23:
				{
					operand = 1;
					break;
				}
				case 24:
				{
					operand = 2;
					break;
				}
				case 25:
				{
					operand = 3;
					break;
				}
				case 26:
				{
					operand = 4;
					break;
				}
				case 27:
				{
					operand = 5;
					break;
				}
				case 28:
				{
					operand = 6;
					break;
				}
				case 29:
				{
					operand = 7;
					break;
				}
				case 30:
				{
					operand = 8;
					break;
				}
				case 31:
				{
					operand = (SByte)instruction.get_Operand();
					break;
				}
				case 32:
				{
					operand = (Int32)instruction.get_Operand();
					break;
				}
				default:
				{
					operand = 0;
					return false;
				}
			}
			return true;
		}

		public static bool TryGetVariableFromInstruction(Instruction instruction, IList<VariableDefinition> variableCollection, out VariableReference varReference)
		{
			// 
			// Current member / type: System.Boolean Telerik.JustDecompiler.Decompiler.StateMachines.StateMachineUtilities::TryGetVariableFromInstruction(Mono.Cecil.Cil.Instruction,System.Collections.Generic.IList`1<Mono.Cecil.Cil.VariableDefinition>,Mono.Cecil.Cil.VariableReference&)
			// File path: C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\Decompiler.Tests\bin\Release\netcoreapp2.1\Integration\Actual\JustDecompiler.NetStandard.dll
			// 
			// Product version: 0.0.0.0
			// Exception in: System.Boolean TryGetVariableFromInstruction(Mono.Cecil.Cil.Instruction,System.Collections.Generic.IList<Mono.Cecil.Cil.VariableDefinition>,Mono.Cecil.Cil.VariableReference&)
			// 
			// Object reference not set to an instance of an object.
			//    at Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.FindLowestCommonAncestor(ICollection`1 typeNodes) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\TypeInference\TypeInferer.cs:line 510
			//    at Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.MergeWithLowestCommonAncestor() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\TypeInference\TypeInferer.cs:line 445
			//    at Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.ProcessSingleConstraints() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\TypeInference\TypeInferer.cs:line 363
			//    at Telerik.JustDecompiler.Decompiler.TypeInference.TypeInferer.InferTypes() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\TypeInference\TypeInferer.cs:line 307
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.Process(DecompilationContext theContext, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\ExpressionDecompilerStep.cs:line 86
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.RunInternal(MethodBody body, BlockStatement block, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 81
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.Run(MethodBody body, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.Decompile(MethodBody body, ILanguage language, DecompilationContext& context, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\Extensions.cs:line 61
			//    at Telerik.JustDecompiler.Decompiler.WriterContextServices.BaseWriterContextService.DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}
	}
}