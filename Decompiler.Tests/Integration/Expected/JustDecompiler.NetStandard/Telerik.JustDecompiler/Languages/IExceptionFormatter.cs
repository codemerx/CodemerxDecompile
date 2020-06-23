using System;

namespace Telerik.JustDecompiler.Languages
{
	public interface IExceptionFormatter
	{
		string[] Format(Exception exception, string memberName, string filePath);
	}
}