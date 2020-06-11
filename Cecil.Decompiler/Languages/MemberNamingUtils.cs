using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Telerik.JustDecompiler.Languages
{
	public static class MemberNamingUtils
	{
		public static string GetNamespaceForLanguage(string @namespace, ILanguage language, bool renameInvalidMembers)
		{
			var formatter = new PlainTextFormatter(new StringWriter());

			ILanguageWriter writer = language.GetWriter(formatter, SimpleExceptionFormatter.Instance, GetWriterSettings(renameInvalidMembers));

			writer.WriteNamespaceNavigationName(@namespace);

			return formatter.ToString();
		}

		public static string GetMemberEscapedOnlyNameForLanguage(object memberReference, ILanguage language)
		{
			var formatter = new PlainTextFormatter(new StringWriter());

			ILanguageWriter writer = language.GetWriter(formatter, SimpleExceptionFormatter.Instance, GetWriterSettings());

			writer.WriteMemberEscapedOnlyName(memberReference);

			return formatter.ToString();
		}

		public static string GetMemberNavigationPathName(object memberReference, ILanguage language)
		{
			var formatter = new PlainTextFormatter(new StringWriter());

			ILanguageWriter writer = language.GetWriter(formatter, SimpleExceptionFormatter.Instance, GetWriterSettings());

			writer.WriteMemberNavigationPathFullName(memberReference);

			return formatter.ToString();
		}

		public static string GetMemberDeclartionForLanguage(object memberReference, ILanguage language, bool renameInvalidMembers)
		{
			var formatter = new PlainTextFormatter(new StringWriter());

			ILanguageWriter writer = language.GetWriter(formatter, SimpleExceptionFormatter.Instance, GetWriterSettings(renameInvalidMembers));

			writer.WriteMemberNavigationName(memberReference);

			return formatter.ToString();
		}

        private static IWriterSettings GetWriterSettings(bool renameInvalidMembers = false)
        {
            return new WriterSettings(renameInvalidMembers: renameInvalidMembers);
        }
	}
}
