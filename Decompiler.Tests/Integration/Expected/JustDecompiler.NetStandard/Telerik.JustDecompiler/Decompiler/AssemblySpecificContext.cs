using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Decompiler
{
	public class AssemblySpecificContext
	{
		public ICollection<string> AssemblyNamespaceUsings
		{
			get;
			private set;
		}

		public AssemblySpecificContext()
		{
			base();
			this.set_AssemblyNamespaceUsings(new HashSet<string>());
			return;
		}

		public AssemblySpecificContext(ICollection<string> assemblyNamespaceUsings)
		{
			base();
			this.set_AssemblyNamespaceUsings(assemblyNamespaceUsings);
			return;
		}
	}
}