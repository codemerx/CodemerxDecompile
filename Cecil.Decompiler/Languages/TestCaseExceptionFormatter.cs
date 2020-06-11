using System;
using System.Linq;
using System.Text;

namespace Telerik.JustDecompiler.Languages
{
	public class TestCaseExceptionFormatter : IExceptionFormatter
	{
		private static IExceptionFormatter instance;

		public static IExceptionFormatter Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new TestCaseExceptionFormatter();
				}

				return instance;
			}
		}

		public string[] Format(Exception exception, string memberName, string filePath)
		{
			var exceptionLines = new StringBuilder();

			if (memberName != null)
			{
				exceptionLines.Append(Environment.NewLine)
						  .Append("Current member / type: ")
						  .Append(memberName)
						  .Append(Environment.NewLine);
			}

			exceptionLines.Append(exception.Message)
					  .Append(Environment.NewLine);

			if (exception.InnerException != null)
			{
				exceptionLines.Append(exception.InnerException.Message)
					  .Append(Environment.NewLine);
			}

			return exceptionLines.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		}
	}
}
