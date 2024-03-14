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
		public BlockDecompilationPipeline(params IDecompilationStep[] steps) : base(steps)
		{
		}

		public BlockDecompilationPipeline(IEnumerable<IDecompilationStep> steps, DecompilationContext context) : base(steps)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			base.Context = context;
		}

		public DecompilationContext Run(MethodBody body, BlockStatement block, ILanguage language = null)
		{
			base.Body = base.RunInternal(body, block, language);
			return base.Context;
		}
	}
}