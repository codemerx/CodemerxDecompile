using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler
{
	public class BlockDecompilationPipeline : DecompilationPipeline
	{
		public BlockDecompilationPipeline(params IDecompilationStep[] steps)
		{
			base(steps);
			return;
		}

		public BlockDecompilationPipeline(IEnumerable<IDecompilationStep> steps, DecompilationContext context)
		{
			base(steps);
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			this.set_Context(context);
			return;
		}

		public DecompilationContext Run(MethodBody body, BlockStatement block, ILanguage language = null)
		{
			this.set_Body(this.RunInternal(body, block, language));
			return this.get_Context();
		}
	}
}