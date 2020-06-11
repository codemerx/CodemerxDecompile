#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompilationPipeline
    {
        private readonly List<IDecompilationStep> steps;

        public DecompilationContext Context { get; protected set; }

        public BlockStatement Body { get; protected set; }

		public DecompilationPipeline(params IDecompilationStep [] steps)
		: this(steps as IEnumerable<IDecompilationStep>)
		{
		}

        public DecompilationPipeline(IEnumerable<IDecompilationStep> steps)
            :this(steps,null)
        { }

        public DecompilationPipeline(IEnumerable<IDecompilationStep> steps, DecompilationContext context)
        {
            this.Context = context;
            this.steps = new List<IDecompilationStep>(steps);
        }

		public void AddSteps(IEnumerable<IDecompilationStep> steps)
		{
			this.steps.AddRange(steps);
		}

        public DecompilationContext Run(MethodBody body, ILanguage language)
        {
            if (Context == null)
            {
                this.Context = GetNewContext(body, language);
            }

            Body = RunInternal(body, new BlockStatement(), language);

            return Context;
        }

        protected BlockStatement RunInternal(MethodBody body, BlockStatement block, ILanguage language)
        {
            try
            {
                if (body.Instructions.Count != 0 || body.Method.IsJustDecompileGenerated)
                {
                    foreach (IDecompilationStep step in steps)
                    {
                        if (this.Context.IsStopped)
                        {
                            break;
                        }

                        block = step.Process(Context, block);
                    }
                }
            }
            finally
            {
                if (Context.MethodContext.IsMethodBodyChanged)
                {
                    body.Method.RefreshBody();
                }
            }

            return block;
        }

        private DecompilationContext GetNewContext(MethodBody body, ILanguage language)
        {
            MethodSpecificContext methodSpecificContext = new MethodSpecificContext(body);
            TypeSpecificContext typeSpecificContext = new TypeSpecificContext(body.Method.DeclaringType);
            return new DecompilationContext(methodSpecificContext, typeSpecificContext, language);
        }

		public IEnumerable<IDecompilationStep> Steps
		{
			get { return steps; }
		}
	}
}