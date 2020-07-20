using System;

namespace Decompiler.Tests.Helpers
{
    internal class ContentAssertException : Exception
    {
        public ContentAssertException(string message, Exception innerException) : base(message, innerException) { }
    }
}
