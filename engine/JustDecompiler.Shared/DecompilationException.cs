using System;

namespace Telerik.JustDecompiler
{
    class DecompilationException : Exception
    {
        public DecompilationException():base()
        {
        }

        public DecompilationException(string message) : base(message)
        {
        }
    }
}
