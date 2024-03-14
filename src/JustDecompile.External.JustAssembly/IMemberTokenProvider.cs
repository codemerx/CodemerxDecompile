using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	public interface IMemberTokenProvider : ITokenProvider
	{
		uint DeclaringTypeToken { get; }
		uint MemberToken { get; }
	}
}
