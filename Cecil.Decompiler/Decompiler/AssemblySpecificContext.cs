using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler
{
    public class AssemblySpecificContext
    {
		public ICollection<string> AssemblyNamespaceUsings { get; private set; }

		public AssemblySpecificContext()
		{
			this.AssemblyNamespaceUsings = new HashSet<string>();
		}

		public AssemblySpecificContext(ICollection<string> assemblyNamespaceUsings)
		{
			this.AssemblyNamespaceUsings = assemblyNamespaceUsings;
		}
    }
}
