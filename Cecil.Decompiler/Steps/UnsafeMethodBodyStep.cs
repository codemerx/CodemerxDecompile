using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Steps
{
	class UnsafeMethodBodyStep : IDecompilationStep
	{
		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			MethodDefinition method = context.MethodContext.Method;

			if (!method.IsUnsafe && method.HasBody && IsUnsafe(method.Body.Instructions))
			{
				UnsafeBlockStatement unsafeStatement = new UnsafeBlockStatement(body.Statements);
				body.Statements = new StatementCollection();
				body.AddStatement(unsafeStatement);
			}

			return body;
		}

		private bool IsUnsafe(IEnumerable<Instruction> methodBody)
		{
			foreach (Instruction instruction in methodBody)
			{
				if (instruction.OpCode == OpCodes.Conv_U || instruction.OpCode == OpCodes.Conv_I)
				{
					return true;
				}
			}
			return false;
		}
	}
}
