using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace JustDecompile.External.JustAssembly
{
	public interface ICodeViewerResults
	{
		IEnumerable<Tuple<int, ITokenProvider>> LineToMemberTokenMap { get; }

		string NewLine { get; }
	}
}
