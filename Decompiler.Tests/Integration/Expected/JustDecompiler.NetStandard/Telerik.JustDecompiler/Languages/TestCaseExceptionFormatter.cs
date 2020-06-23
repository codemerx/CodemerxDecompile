using System;
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
				if (TestCaseExceptionFormatter.instance == null)
				{
					TestCaseExceptionFormatter.instance = new TestCaseExceptionFormatter();
				}
				return TestCaseExceptionFormatter.instance;
			}
		}

		public TestCaseExceptionFormatter()
		{
		}

		public string[] Format(Exception exception, string memberName, string filePath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (memberName != null)
			{
				stringBuilder.Append(Environment.NewLine).Append("Current member / type: ").Append(memberName).Append(Environment.NewLine);
			}
			stringBuilder.Append(exception.Message).Append(Environment.NewLine);
			if (exception.InnerException != null)
			{
				stringBuilder.Append(exception.InnerException.Message).Append(Environment.NewLine);
			}
			return stringBuilder.ToString().Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
		}
	}
}