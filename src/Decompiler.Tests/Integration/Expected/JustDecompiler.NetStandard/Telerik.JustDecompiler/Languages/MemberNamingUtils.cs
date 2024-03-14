using System;
using System.IO;

namespace Telerik.JustDecompiler.Languages
{
	public static class MemberNamingUtils
	{
		public static string GetMemberDeclartionForLanguage(object memberReference, ILanguage language, bool renameInvalidMembers)
		{
			PlainTextFormatter plainTextFormatter = new PlainTextFormatter(new StringWriter());
			language.GetWriter(plainTextFormatter, SimpleExceptionFormatter.Instance, MemberNamingUtils.GetWriterSettings(renameInvalidMembers)).WriteMemberNavigationName(memberReference);
			return plainTextFormatter.ToString();
		}

		public static string GetMemberEscapedOnlyNameForLanguage(object memberReference, ILanguage language)
		{
			PlainTextFormatter plainTextFormatter = new PlainTextFormatter(new StringWriter());
			language.GetWriter(plainTextFormatter, SimpleExceptionFormatter.Instance, MemberNamingUtils.GetWriterSettings(false)).WriteMemberEscapedOnlyName(memberReference);
			return plainTextFormatter.ToString();
		}

		public static string GetMemberNavigationPathName(object memberReference, ILanguage language)
		{
			PlainTextFormatter plainTextFormatter = new PlainTextFormatter(new StringWriter());
			language.GetWriter(plainTextFormatter, SimpleExceptionFormatter.Instance, MemberNamingUtils.GetWriterSettings(false)).WriteMemberNavigationPathFullName(memberReference);
			return plainTextFormatter.ToString();
		}

		public static string GetNamespaceForLanguage(string @namespace, ILanguage language, bool renameInvalidMembers)
		{
			PlainTextFormatter plainTextFormatter = new PlainTextFormatter(new StringWriter());
			language.GetWriter(plainTextFormatter, SimpleExceptionFormatter.Instance, MemberNamingUtils.GetWriterSettings(renameInvalidMembers)).WriteNamespaceNavigationName(@namespace);
			return plainTextFormatter.ToString();
		}

		private static IWriterSettings GetWriterSettings(bool renameInvalidMembers = false)
		{
			return new WriterSettings(false, false, renameInvalidMembers, false, false, false, true, false);
		}
	}
}