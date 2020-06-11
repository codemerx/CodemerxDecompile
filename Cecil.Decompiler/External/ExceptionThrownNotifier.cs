using System;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
    public class ExceptionThrownNotifier : IExceptionThrownNotifier
    {
        public event EventHandler<Exception> ExceptionThrown;

        protected void OnExceptionThrown(Exception ex)
        {
            OnExceptionThrown(this, ex);
        }

        protected void OnExceptionThrown(object sender, Exception ex)
        {
            EventHandler<Exception> exceptionThrown = this.ExceptionThrown;
            if (exceptionThrown != null)
            {
                exceptionThrown(sender, ex);
            }
        }
    }
}
