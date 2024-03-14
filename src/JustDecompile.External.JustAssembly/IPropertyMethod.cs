using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	public interface IPropertyMethod
	{
		PropertyMethodType PropertyMethodType { get; }
		uint PropertyMethodToken { get; }
	}
}
