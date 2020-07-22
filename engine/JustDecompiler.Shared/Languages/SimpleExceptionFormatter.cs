using System;
using System.Linq;
using System.Text;
#if !ENGINEONLYBUILD && !JUSTASSEMBLY
using JustDecompile.Settings;
#endif

namespace Telerik.JustDecompiler.Languages
{
	public class SimpleExceptionFormatter : IExceptionFormatter
	{
		private static SimpleExceptionFormatter instance;

		public static IExceptionFormatter Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SimpleExceptionFormatter();
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

			if (!string.IsNullOrEmpty(filePath))
			{
				exceptionLines.Append("File path: ")
						  .Append(filePath)
						  .Append(Environment.NewLine);
			}

			exceptionLines.Append(Environment.NewLine)
					  .Append("Product version: " +
#if !ENGINEONLYBUILD && !JUSTASSEMBLY
                      VersionChecker.CurrentVersion)
#else
                      System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString())
#endif
					  .Append(Environment.NewLine);


			exceptionLines.Append(exception.Message)
					  .Append(Environment.NewLine)
					  .Append(exception.StackTrace)
					  .Append(Environment.NewLine);

			if (exception.InnerException != null)
			{
				exceptionLines.Append(exception.InnerException.Message)
					  .Append(Environment.NewLine)
					  .Append(exception.InnerException.StackTrace)
					  .Append(Environment.NewLine);
			}

			return exceptionLines.ToString().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
		}
	}
}
