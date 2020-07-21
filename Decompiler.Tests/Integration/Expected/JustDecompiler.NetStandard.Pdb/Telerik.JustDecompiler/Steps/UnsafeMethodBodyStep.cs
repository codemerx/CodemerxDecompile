using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class UnsafeMethodBodyStep : IDecompilationStep
	{
		public UnsafeMethodBodyStep()
		{
			base();
			return;
		}

		private bool IsUnsafe(IEnumerable<Instruction> methodBody)
		{
			V_0 = methodBody.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (!OpCode.op_Equality(V_1.get_OpCode(), OpCodes.Conv_U) && !OpCode.op_Equality(V_1.get_OpCode(), OpCodes.Conv_I))
					{
						continue;
					}
					V_2 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label1:
			return V_2;
		Label0:
			return false;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			V_0 = context.get_MethodContext().get_Method();
			if (!V_0.get_IsUnsafe() && V_0.get_HasBody() && this.IsUnsafe(V_0.get_Body().get_Instructions()))
			{
				V_1 = new UnsafeBlockStatement(body.get_Statements());
				body.set_Statements(new StatementCollection());
				body.AddStatement(V_1);
			}
			return body;
		}
	}
}