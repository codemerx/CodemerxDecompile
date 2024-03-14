using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	public class ExceptionThrownNotifier : IExceptionThrownNotifier
	{
		public ExceptionThrownNotifier()
		{
		}

		protected void OnExceptionThrown(Exception ex)
		{
			this.OnExceptionThrown(this, ex);
		}

		protected void OnExceptionThrown(object sender, Exception ex)
		{
			EventHandler<Exception> eventHandler = this.ExceptionThrown;
			if (eventHandler != null)
			{
				eventHandler(sender, ex);
			}
		}

		public event EventHandler<Exception> ExceptionThrown;
	}
}