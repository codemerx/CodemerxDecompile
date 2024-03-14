using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompilationPipeline
	{
		private readonly List<IDecompilationStep> steps;

		public BlockStatement Body
		{
			get;
			protected set;
		}

		public DecompilationContext Context
		{
			get;
			protected set;
		}

		public IEnumerable<IDecompilationStep> Steps
		{
			get
			{
				return this.steps;
			}
		}

		public DecompilationPipeline(params IDecompilationStep[] steps) : this((IEnumerable<IDecompilationStep>)steps)
		{
		}

		public DecompilationPipeline(IEnumerable<IDecompilationStep> steps) : this(steps, null)
		{
		}

		public DecompilationPipeline(IEnumerable<IDecompilationStep> steps, DecompilationContext context)
		{
			this.Context = context;
			this.steps = new List<IDecompilationStep>(steps);
		}

		public void AddSteps(IEnumerable<IDecompilationStep> steps)
		{
			this.steps.AddRange(steps);
		}

		private DecompilationContext GetNewContext(MethodBody body, ILanguage language)
		{
			MethodSpecificContext methodSpecificContext = new MethodSpecificContext(body);
			TypeSpecificContext typeSpecificContext = new TypeSpecificContext(body.get_Method().get_DeclaringType());
			return new DecompilationContext(methodSpecificContext, typeSpecificContext, language);
		}

		public DecompilationContext Run(MethodBody body, ILanguage language)
		{
			if (this.Context == null)
			{
				this.Context = this.GetNewContext(body, language);
			}
			this.Body = this.RunInternal(body, new BlockStatement(), language);
			return this.Context;
		}

		protected BlockStatement RunInternal(MethodBody body, BlockStatement block, ILanguage language)
		{
			try
			{
				if (body.get_Instructions().get_Count() != 0 || body.get_Method().get_IsJustDecompileGenerated())
				{
					foreach (IDecompilationStep step in this.steps)
					{
						if (!this.Context.IsStopped)
						{
							block = step.Process(this.Context, block);
						}
						else
						{
							return block;
						}
					}
				}
			}
			finally
			{
				if (this.Context.MethodContext.IsMethodBodyChanged)
				{
					body.get_Method().RefreshBody();
				}
			}
			return block;
		}
	}
}