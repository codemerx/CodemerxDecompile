using System;

namespace Telerik.JustDecompiler.Languages
{
	public static class MemberNamingUtils
	{
		public static string GetMemberDeclartionForLanguage(object memberReference, ILanguage language, bool renameInvalidMembers)
		{
			V_0 = new PlainTextFormatter(new StringWriter());
			language.GetWriter(V_0, SimpleExceptionFormatter.get_Instance(), MemberNamingUtils.GetWriterSettings(renameInvalidMembers)).WriteMemberNavigationName(memberReference);
			return V_0.ToString();
		}

		public static string GetMemberEscapedOnlyNameForLanguage(object memberReference, ILanguage language)
		{
			V_0 = new PlainTextFormatter(new StringWriter());
			language.GetWriter(V_0, SimpleExceptionFormatter.get_Instance(), MemberNamingUtils.GetWriterSettings(false)).WriteMemberEscapedOnlyName(memberReference);
			return V_0.ToString();
		}

		public static string GetMemberNavigationPathName(object memberReference, ILanguage language)
		{
			V_0 = new PlainTextFormatter(new StringWriter());
			language.GetWriter(V_0, SimpleExceptionFormatter.get_Instance(), MemberNamingUtils.GetWriterSettings(false)).WriteMemberNavigationPathFullName(memberReference);
			return V_0.ToString();
		}

		public static string GetNamespaceForLanguage(string namespace, ILanguage language, bool renameInvalidMembers)
		{
			V_0 = new PlainTextFormatter(new StringWriter());
			language.GetWriter(V_0, SimpleExceptionFormatter.get_Instance(), MemberNamingUtils.GetWriterSettings(renameInvalidMembers)).WriteNamespaceNavigationName(namespace);
			return V_0.ToString();
		}

		private static IWriterSettings GetWriterSettings(bool renameInvalidMembers = false)
		{
			return new WriterSettings(false, false, renameInvalidMembers, false, false, false, true, false);
		}
	}
}