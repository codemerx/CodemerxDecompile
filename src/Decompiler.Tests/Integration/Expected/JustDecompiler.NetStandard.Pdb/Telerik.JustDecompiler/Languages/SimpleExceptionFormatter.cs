using System;
using System.Reflection;
using System.Text;

namespace Telerik.JustDecompiler.Languages
{
	public class SimpleExceptionFormatter : IExceptionFormatter
	{
		private static SimpleExceptionFormatter instance;

		public static IExceptionFormatter Instance
		{
			get
			{
				if (SimpleExceptionFormatter.instance == null)
				{
					SimpleExceptionFormatter.instance = new SimpleExceptionFormatter();
				}
				return SimpleExceptionFormatter.instance;
			}
		}

		public SimpleExceptionFormatter()
		{
		}

		public string[] Format(Exception exception, string memberName, string filePath)
		{
			StringBuilder stringBuilder = new StringBuilder();
			if (memberName != null)
			{
				stringBuilder.Append(Environment.NewLine).Append("Current member / type: ").Append(memberName).Append(Environment.NewLine);
			}
			if (!String.IsNullOrEmpty(filePath))
			{
				stringBuilder.Append("File path: ").Append(filePath).Append(Environment.NewLine);
			}
			stringBuilder.Append(Environment.NewLine).Append(String.Concat("Product version: ", Assembly.GetExecutingAssembly().GetName().Version.ToString())).Append(Environment.NewLine);
			stringBuilder.Append(exception.Message).Append(Environment.NewLine).Append(exception.StackTrace).Append(Environment.NewLine);
			if (exception.InnerException != null)
			{
				stringBuilder.Append(exception.InnerException.Message).Append(Environment.NewLine).Append(exception.InnerException.StackTrace).Append(Environment.NewLine);
			}
			return stringBuilder.ToString().Split(new String[] { Environment.NewLine }, StringSplitOptions.None);
		}
	}
}