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
			base();
			return;
		}

		public string[] Format(Exception exception, string memberName, string filePath)
		{
			V_0 = new StringBuilder();
			if (memberName != null)
			{
				dummyVar0 = V_0.Append(Environment.get_NewLine()).Append("Current member / type: ").Append(memberName).Append(Environment.get_NewLine());
			}
			dummyVar1 = V_0.Append(exception.get_Message()).Append(Environment.get_NewLine());
			if (exception.get_InnerException() != null)
			{
				dummyVar2 = V_0.Append(exception.get_InnerException().get_Message()).Append(Environment.get_NewLine());
			}
			stackVariable11 = V_0.ToString();
			stackVariable13 = new String[1];
			stackVariable13[0] = Environment.get_NewLine();
			return stackVariable11.Split(stackVariable13, 0);
		}
	}
}