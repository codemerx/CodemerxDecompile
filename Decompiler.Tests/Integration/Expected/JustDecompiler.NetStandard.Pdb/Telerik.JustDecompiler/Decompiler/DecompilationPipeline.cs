using Mono.Cecil.Cil;
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

		public DecompilationPipeline(params IDecompilationStep[] steps)
		{
			this(steps);
			return;
		}

		public DecompilationPipeline(IEnumerable<IDecompilationStep> steps)
		{
			this(steps, null);
			return;
		}

		public DecompilationPipeline(IEnumerable<IDecompilationStep> steps, DecompilationContext context)
		{
			base();
			this.set_Context(context);
			this.steps = new List<IDecompilationStep>(steps);
			return;
		}

		public void AddSteps(IEnumerable<IDecompilationStep> steps)
		{
			this.steps.AddRange(steps);
			return;
		}

		private DecompilationContext GetNewContext(MethodBody body, ILanguage language)
		{
			stackVariable1 = new MethodSpecificContext(body);
			V_0 = new TypeSpecificContext(body.get_Method().get_DeclaringType());
			return new DecompilationContext(stackVariable1, V_0, language);
		}

		public DecompilationContext Run(MethodBody body, ILanguage language)
		{
			if (this.get_Context() == null)
			{
				this.set_Context(this.GetNewContext(body, language));
			}
			this.set_Body(this.RunInternal(body, new BlockStatement(), language));
			return this.get_Context();
		}

		protected BlockStatement RunInternal(MethodBody body, BlockStatement block, ILanguage language)
		{
			try
			{
				if (body.get_Instructions().get_Count() != 0 || body.get_Method().get_IsJustDecompileGenerated())
				{
					V_0 = this.steps.GetEnumerator();
					try
					{
						while (V_0.MoveNext())
						{
							V_1 = V_0.get_Current();
							if (!this.get_Context().get_IsStopped())
							{
								block = V_1.Process(this.get_Context(), block);
							}
							else
							{
								goto Label0;
							}
						}
					}
					finally
					{
						((IDisposable)V_0).Dispose();
					}
				}
			}
			finally
			{
				if (this.get_Context().get_MethodContext().get_IsMethodBodyChanged())
				{
					body.get_Method().RefreshBody();
				}
			}
		Label0:
			return block;
		}
	}
}