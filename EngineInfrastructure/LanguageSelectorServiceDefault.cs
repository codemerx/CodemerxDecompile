using System.IO;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;

namespace JustDecompile.EngineInfrastructure
{
    public abstract class LanguageSelectorServiceDefault : ILanguageSelectorService
    {
        public static ILanguageSelectorService Instance { get; protected set; }

        public abstract ILanguageSelector GetLanguageSelector();

        public abstract ILanguage GetLanguageFromString(string language);

        public ILanguage GetLanguage()
        {
            ILanguageSelector languageSelector = GetLanguageSelector();
            if (languageSelector == null)
            {
                return LanguageFactory.GetLanguage(CSharpVersion.V7);
            }
            else
            {
                return languageSelector.GetLanguage();
            }
        }

        public string GetNamespaceForSelectedLanguage(string @namespace, bool renameInvalidMembers)
        {
            ILanguage currentLanguage = GetLanguage();

			return MemberNamingUtils.GetNamespaceForLanguage(@namespace, currentLanguage, renameInvalidMembers);
        }

        public string GetMemberDeclartionForSelectedLanguage(object memberReference, bool renameInvalidMembers)
        {
            ILanguage currentLanguage = GetLanguage();

			return MemberNamingUtils.GetMemberDeclartionForLanguage(memberReference, currentLanguage, renameInvalidMembers);
        }

        public string GetMemberEscapedOnlyNameForSelectedLanguage(object memberReference)
        {
            ILanguage currentLanguage = GetLanguage();

			return MemberNamingUtils.GetMemberEscapedOnlyNameForLanguage(memberReference, currentLanguage);
        }

		public string GetMemberNavigationPathName(object memberReference)
		{
			ILanguage currentLanguage = GetLanguage();

			return MemberNamingUtils.GetMemberNavigationPathName(memberReference, currentLanguage);
		}
    }
}