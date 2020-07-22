using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	public interface IEventMethod
	{
		EventMethodType EventMethodType { get; }

		uint EventMethodToken { get; }
	}
}
