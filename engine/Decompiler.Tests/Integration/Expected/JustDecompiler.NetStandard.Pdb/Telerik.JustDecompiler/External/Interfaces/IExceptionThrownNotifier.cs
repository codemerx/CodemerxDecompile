using System;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.External.Interfaces
{
	public interface IExceptionThrownNotifier
	{
		event EventHandler<Exception> ExceptionThrown;
	}
}