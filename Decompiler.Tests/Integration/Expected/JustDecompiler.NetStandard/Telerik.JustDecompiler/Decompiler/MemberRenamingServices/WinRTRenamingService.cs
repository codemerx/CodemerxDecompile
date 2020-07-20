using System;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.MemberRenamingServices
{
	public class WinRTRenamingService : DefaultMemberRenamingService
	{
		private const string CLRPrefix = "<CLR>";

		public WinRTRenamingService(ILanguage language, bool renameInvalidMembers)
		{
			base(language, renameInvalidMembers);
			return;
		}

		protected override string GetActualTypeName(string typeName)
		{
			if (typeName.StartsWith("<CLR>"))
			{
				stackVariable7 = typeName.Substring("<CLR>".get_Length());
			}
			else
			{
				stackVariable7 = typeName;
			}
			return this.GetActualTypeName(stackVariable7);
		}
	}
}