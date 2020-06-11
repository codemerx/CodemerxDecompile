using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	public interface IFileGeneratedInfo
	{
		string FullPath { get; }

		bool HasErrors { get; }
	}
}
