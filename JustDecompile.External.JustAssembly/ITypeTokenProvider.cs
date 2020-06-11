using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	public interface ITypeTokenProvider : ITokenProvider
	{
		uint TypeToken { get; }
	}
}
