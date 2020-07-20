using System;
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
			if (!String.IsNullOrEmpty(filePath))
			{
				dummyVar1 = V_0.Append("File path: ").Append(filePath).Append(Environment.get_NewLine());
			}
			dummyVar2 = V_0.Append(Environment.get_NewLine()).Append(String.Concat("Product version: ", Assembly.GetExecutingAssembly().GetName().get_Version().ToString())).Append(Environment.get_NewLine());
			dummyVar3 = V_0.Append(exception.get_Message()).Append(Environment.get_NewLine()).Append(exception.get_StackTrace()).Append(Environment.get_NewLine());
			if (exception.get_InnerException() != null)
			{
				dummyVar4 = V_0.Append(exception.get_InnerException().get_Message()).Append(Environment.get_NewLine()).Append(exception.get_InnerException().get_StackTrace()).Append(Environment.get_NewLine());
			}
			stackVariable30 = V_0.ToString();
			stackVariable32 = new String[1];
			stackVariable32[0] = Environment.get_NewLine();
			return stackVariable30.Split(stackVariable32, 0);
		}
	}
}