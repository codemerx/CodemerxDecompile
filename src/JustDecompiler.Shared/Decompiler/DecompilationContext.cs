/*
    Copyright CodeMerx 2020
    This file is part of CodemerxDecompile.

    CodemerxDecompile is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    CodemerxDecompile is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.
*/

using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompilationContext
	{
		public MethodSpecificContext MethodContext { get; set; }

		public TypeSpecificContext TypeContext { get; set; }

		public ModuleSpecificContext ModuleContext { get; set; }

		public AssemblySpecificContext AssemblyContext { get; set; }

        public ILanguage Language{ get; set; }

        public bool IsStopped { get; private set; }

        public DecompilationContext(MethodSpecificContext methodContext, TypeSpecificContext typeContext, ILanguage language)
			: this(methodContext, typeContext, new ModuleSpecificContext(), new AssemblySpecificContext(), language)
		{
		}

		public DecompilationContext(MethodSpecificContext methodContext, TypeSpecificContext typeContext, ModuleSpecificContext moduleContext, AssemblySpecificContext assemblyContext, ILanguage language)
		{
			this.MethodContext = methodContext;
			this.TypeContext = typeContext;
			this.ModuleContext = moduleContext;
			this.AssemblyContext = assemblyContext;
            this.Language = language;
            this.IsStopped = false;
		}

		public DecompilationContext() { }

        public void StopPipeline()
        {
            this.IsStopped = true;
        }
	}
}