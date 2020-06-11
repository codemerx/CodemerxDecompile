using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	public enum MemberType : byte
	{
		Field = 0,
		Property,
		Method,
		Event,
		Type
	}
}
