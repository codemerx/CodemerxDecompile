using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	public class ExceptionThrownNotifier : IExceptionThrownNotifier
	{
		public ExceptionThrownNotifier()
		{
			base();
			return;
		}

		protected void OnExceptionThrown(Exception ex)
		{
			this.OnExceptionThrown(this, ex);
			return;
		}

		protected void OnExceptionThrown(object sender, Exception ex)
		{
			V_0 = this.ExceptionThrown;
			if (V_0 != null)
			{
				V_0.Invoke(sender, ex);
			}
			return;
		}

		public event EventHandler<Exception> ExceptionThrown;
	}
}