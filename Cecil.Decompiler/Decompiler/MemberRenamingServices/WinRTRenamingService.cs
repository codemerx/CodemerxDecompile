using System;
using System.Linq;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler.MemberRenamingServices
{
    public class WinRTRenamingService : DefaultMemberRenamingService
    {
        private const string CLRPrefix = "<CLR>";

        public WinRTRenamingService(ILanguage language, bool renameInvalidMembers)
            : base(language, renameInvalidMembers)
        {
        }

        protected override string GetActualTypeName(string typeName)
        {
            return base.GetActualTypeName(typeName.StartsWith(CLRPrefix) ? typeName.Substring(CLRPrefix.Length) : typeName);
        }
    }
}
