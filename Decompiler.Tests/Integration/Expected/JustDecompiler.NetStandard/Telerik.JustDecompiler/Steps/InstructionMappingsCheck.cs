using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Text;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class InstructionMappingsCheck : IDecompilationStep
	{
		public InstructionMappingsCheck()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			V_0 = new HashSet<Instruction>(body.get_UnderlyingSameMethodInstructions());
			if (context.get_MethodContext().get_IsMethodBodyChanged())
			{
				context.get_MethodContext().get_Method().RefreshBody();
				context.get_MethodContext().set_IsMethodBodyChanged(false);
			}
			V_1 = new List<Instruction>();
			V_2 = context.get_MethodContext().get_Method().get_Body().get_Instructions().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (V_0.Contains(V_3))
					{
						continue;
					}
					V_1.Add(V_3);
				}
			}
			finally
			{
				V_2.Dispose();
			}
			if (V_1.get_Count() > 0)
			{
				V_4 = new StringBuilder("Found unmapped instructions.\n");
				V_5 = V_1.GetEnumerator();
				try
				{
					while (V_5.MoveNext())
					{
						V_6 = V_5.get_Current();
						dummyVar0 = V_4.AppendLine(V_6.ToString());
					}
				}
				finally
				{
					((IDisposable)V_5).Dispose();
				}
				throw new Exception(V_4.ToString());
			}
			return body;
		}
	}
}