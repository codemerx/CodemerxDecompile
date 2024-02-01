using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class UnsafeMethodBodyStep : IDecompilationStep
	{
		public UnsafeMethodBodyStep()
		{
		}

		private bool IsUnsafe(IEnumerable<Instruction> methodBody)
		{
			bool flag;
			using (IEnumerator<Instruction> enumerator = methodBody.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					Instruction current = enumerator.Current;
					if (!(current.get_OpCode() == OpCodes.Conv_U) && !(current.get_OpCode() == OpCodes.Conv_I))
					{
						continue;
					}
					flag = true;
					return flag;
				}
				return false;
			}
			return flag;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			MethodDefinition method = context.MethodContext.Method;
			if (!method.get_IsUnsafe() && method.get_HasBody() && this.IsUnsafe(method.get_Body().get_Instructions()))
			{
				UnsafeBlockStatement unsafeBlockStatement = new UnsafeBlockStatement(body.Statements);
				body.Statements = new StatementCollection();
				body.AddStatement(unsafeBlockStatement);
			}
			return body;
		}
	}
}