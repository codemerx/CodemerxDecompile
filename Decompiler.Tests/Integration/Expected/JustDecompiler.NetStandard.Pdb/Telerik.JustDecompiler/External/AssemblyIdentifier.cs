using JustDecompile.SmartAssembly.Attributes;
using System;

namespace Telerik.JustDecompiler.External
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public struct AssemblyIdentifier
	{
		private string path;

		public string AssemblyPath
		{
			get
			{
				return this.path;
			}
			private set
			{
				this.path = value;
				return;
			}
		}

		public AssemblyIdentifier(string path)
		{
			this.path = path;
			return;
		}
	}
}