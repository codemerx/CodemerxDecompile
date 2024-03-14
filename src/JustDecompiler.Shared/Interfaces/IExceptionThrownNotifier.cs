using System;

namespace Telerik.JustDecompiler.External.Interfaces
{
    public interface IExceptionThrownNotifier
    {
        event EventHandler<Exception> ExceptionThrown;
    }
}
